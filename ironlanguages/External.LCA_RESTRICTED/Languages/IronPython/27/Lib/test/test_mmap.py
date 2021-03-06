from test.test_support import TESTFN, run_unittest, import_module, is_cli, is_net40, force_gc_collect
import unittest
import os, re, itertools, sys

if is_cli and not is_net40:
    print "mmap unsupported on .NET 2.0. Skipping test."
    sys.exit(0)
mmap = import_module('mmap')

PAGESIZE = mmap.PAGESIZE

class MmapTests(unittest.TestCase):

    def setUp(self):
        if os.path.exists(TESTFN):
            os.unlink(TESTFN)

    def tearDown(self):
        force_gc_collect()
        try:
            os.unlink(TESTFN)
        except OSError:
            pass

    def test_basic(self):
        # Test mmap module on Unix systems and Windows

        # Create a file to be mmap'ed.
        f = open(TESTFN, 'w+')
        try:
            # Write 2 pages worth of data to the file
            f.write('\0'* PAGESIZE)
            f.write('foo')
            f.write('\0'* (PAGESIZE-3) )
            f.flush()
            m = mmap.mmap(f.fileno(), 2 * PAGESIZE)
            f.close()

            # Simple sanity checks

            tp = str(type(m))  # SF bug 128713:  segfaulted on Linux
            self.assertEqual(m.find('foo'), PAGESIZE)

            self.assertEqual(len(m), 2*PAGESIZE)

            self.assertEqual(m[0], '\0')
            self.assertEqual(m[0:3], '\0\0\0')

            # Shouldn't crash on boundary (Issue #5292)
            self.assertRaises(IndexError, m.__getitem__, len(m))
            self.assertRaises(IndexError, m.__setitem__, len(m), '\0')

            # Modify the file's content
            m[0] = '3'
            m[PAGESIZE +3: PAGESIZE +3+3] = 'bar'

            # Check that the modification worked
            self.assertEqual(m[0], '3')
            self.assertEqual(m[0:3], '3\0\0')
            self.assertEqual(m[PAGESIZE-1 : PAGESIZE + 7], '\0foobar\0')

            m.flush()

            # Test doing a regular expression match in an mmap'ed file
            match = re.search('[A-Za-z]+', m)
            if match is None:
                self.fail('regex match on mmap failed!')
            else:
                start, end = match.span(0)
                length = end - start

                self.assertEqual(start, PAGESIZE)
                self.assertEqual(end, PAGESIZE + 6)

            # test seeking around (try to overflow the seek implementation)
            m.seek(0,0)
            self.assertEqual(m.tell(), 0)
            m.seek(42,1)
            self.assertEqual(m.tell(), 42)
            m.seek(0,2)
            self.assertEqual(m.tell(), len(m))

            # Try to seek to negative position...
            self.assertRaises(ValueError, m.seek, -1)

            # Try to seek beyond end of mmap...
            self.assertRaises(ValueError, m.seek, 1, 2)

            # Try to seek to negative position...
            self.assertRaises(ValueError, m.seek, -len(m)-1, 2)

            # Try resizing map
            try:
                m.resize(512)
            except SystemError:
                # resize() not supported
                # No messages are printed, since the output of this test suite
                # would then be different across platforms.
                pass
            else:
                # resize() is supported
                self.assertEqual(len(m), 512)
                # Check that we can no longer seek beyond the new size.
                self.assertRaises(ValueError, m.seek, 513, 0)

                # Check that the underlying file is truncated too
                # (bug #728515)
                f = open(TESTFN)
                f.seek(0, 2)
                self.assertEqual(f.tell(), 512)
                f.close()
                self.assertEqual(m.size(), 512)

            m.close()

        finally:
            try:
                f.close()
            except OSError:
                pass

    def test_access_parameter(self):
        # Test for "access" keyword parameter
        mapsize = 10
        with open(TESTFN, "wb") as f:
            f.write("a"*mapsize)
        f = open(TESTFN, "rb")
        m = mmap.mmap(f.fileno(), mapsize, access=mmap.ACCESS_READ)
        self.assertEqual(m[:], 'a'*mapsize, "Readonly memory map data incorrect.")

        # Ensuring that readonly mmap can't be slice assigned
        try:
            m[:] = 'b'*mapsize
        except TypeError:
            pass
        else:
            self.fail("Able to write to readonly memory map")

        # Ensuring that readonly mmap can't be item assigned
        try:
            m[0] = 'b'
        except TypeError:
            pass
        else:
            self.fail("Able to write to readonly memory map")

        # Ensuring that readonly mmap can't be write() to
        try:
            m.seek(0,0)
            m.write('abc')
        except TypeError:
            pass
        else:
            self.fail("Able to write to readonly memory map")

        # Ensuring that readonly mmap can't be write_byte() to
        try:
            m.seek(0,0)
            m.write_byte('d')
        except TypeError:
            pass
        else:
            self.fail("Able to write to readonly memory map")

        # Ensuring that readonly mmap can't be resized
        try:
            m.resize(2*mapsize)
        except SystemError:   # resize is not universally supported
            pass
        except TypeError:
            pass
        else:
            self.fail("Able to resize readonly memory map")
        f.close()
        del m, f
        with open(TESTFN, "rb") as f:
            self.assertEqual(f.read(), 'a'*mapsize,
               "Readonly memory map data file was modified")

        # Opening mmap with size too big
        f = open(TESTFN, "r+b")
        try:
            m = mmap.mmap(f.fileno(), mapsize+1)
        except ValueError:
            # we do not expect a ValueError on Windows
            # CAUTION:  This also changes the size of the file on disk, and
            # later tests assume that the length hasn't changed.  We need to
            # repair that.
            if hasattr(sys, 'getwindowsversion'):
                self.fail("Opening mmap with size+1 should work on Windows.")
        else:
            # we expect a ValueError on Unix, but not on Windows
            if not hasattr(sys, 'getwindowsversion'):
                self.fail("Opening mmap with size+1 should raise ValueError.")
            m.close()
        f.close()
        if hasattr(sys, 'getwindowsversion'):
            # Repair damage from the resizing test.
            del m, f
            force_gc_collect()
            f = open(TESTFN, 'r+b')
            f.truncate(mapsize)
            f.close()

        # Opening mmap with access=ACCESS_WRITE
        f = open(TESTFN, "r+b")
        m = mmap.mmap(f.fileno(), mapsize, access=mmap.ACCESS_WRITE)
        # Modifying write-through memory map
        m[:] = 'c'*mapsize
        self.assertEqual(m[:], 'c'*mapsize,
               "Write-through memory map memory not updated properly.")
        m.flush()
        m.close()
        f.close()
        f = open(TESTFN, 'rb')
        stuff = f.read()
        f.close()
        self.assertEqual(stuff, 'c'*mapsize,
               "Write-through memory map data file not updated properly.")

        # Opening mmap with access=ACCESS_COPY
        f = open(TESTFN, "r+b")
        m = mmap.mmap(f.fileno(), mapsize, access=mmap.ACCESS_COPY)
        # Modifying copy-on-write memory map
        m[:] = 'd'*mapsize
        self.assertEqual(m[:], 'd' * mapsize,
               "Copy-on-write memory map data not written correctly.")
        m.flush()
        with open(TESTFN, "rb") as f:
            self.assertEqual(f.read(), 'c'*mapsize,
                   "Copy-on-write test data file should not be modified.")
        # Ensuring copy-on-write maps cannot be resized
        self.assertRaises(TypeError, m.resize, 2*mapsize)
        f.close()
        del m, f

        # Ensuring invalid access parameter raises exception
        f = open(TESTFN, "r+b")
        self.assertRaises(ValueError, mmap.mmap, f.fileno(), mapsize, access=4)
        f.close()

        if os.name == "posix":
            # Try incompatible flags, prot and access parameters.
            f = open(TESTFN, "r+b")
            self.assertRaises(ValueError, mmap.mmap, f.fileno(), mapsize,
                              flags=mmap.MAP_PRIVATE,
                              prot=mmap.PROT_READ, access=mmap.ACCESS_WRITE)
            f.close()

    def test_bad_file_desc(self):
        # Try opening a bad file descriptor...
        self.assertRaises(mmap.error, mmap.mmap, -2, 4096)

    def test_tougher_find(self):
        # Do a tougher .find() test.  SF bug 515943 pointed out that, in 2.2,
        # searching for data with embedded \0 bytes didn't work.
        f = open(TESTFN, 'w+')

        data = 'aabaac\x00deef\x00\x00aa\x00'
        n = len(data)
        f.write(data)
        f.flush()
        m = mmap.mmap(f.fileno(), n)
        f.close()

        for start in range(n+1):
            for finish in range(start, n+1):
                slice = data[start : finish]
                self.assertEqual(m.find(slice), data.find(slice))
                self.assertEqual(m.find(slice + 'x'), -1)
        m.close()

    def test_find_end(self):
        # test the new 'end' parameter works as expected
        f = open(TESTFN, 'w+')
        data = 'one two ones'
        n = len(data)
        f.write(data)
        f.flush()
        m = mmap.mmap(f.fileno(), n)
        f.close()

        self.assertEqual(m.find('one'), 0)
        self.assertEqual(m.find('ones'), 8)
        self.assertEqual(m.find('one', 0, -1), 0)
        self.assertEqual(m.find('one', 1), 8)
        self.assertEqual(m.find('one', 1, -1), 8)
        self.assertEqual(m.find('one', 1, -2), -1)


    def test_rfind(self):
        # test the new 'end' parameter works as expected
        f = open(TESTFN, 'w+')
        data = 'one two ones'
        n = len(data)
        f.write(data)
        f.flush()
        m = mmap.mmap(f.fileno(), n)
        f.close()

        self.assertEqual(m.rfind('one'), 8)
        self.assertEqual(m.rfind('one '), 0)
        self.assertEqual(m.rfind('one', 0, -1), 8)
        self.assertEqual(m.rfind('one', 0, -2), 0)
        self.assertEqual(m.rfind('one', 1, -1), 8)
        self.assertEqual(m.rfind('one', 1, -2), -1)


    def test_double_close(self):
        # make sure a double close doesn't crash on Solaris (Bug# 665913)
        f = open(TESTFN, 'w+')

        f.write(2**16 * 'a') # Arbitrary character
        f.close()

        f = open(TESTFN)
        mf = mmap.mmap(f.fileno(), 2**16, access=mmap.ACCESS_READ)
        mf.close()
        mf.close()
        f.close()

    def test_entire_file(self):
        # test mapping of entire file by passing 0 for map length
        if hasattr(os, "stat"):
            f = open(TESTFN, "w+")

            f.write(2**16 * 'm') # Arbitrary character
            f.close()

            f = open(TESTFN, "rb+")
            mf = mmap.mmap(f.fileno(), 0)
            self.assertEqual(len(mf), 2**16, "Map size should equal file size.")
            self.assertEqual(mf.read(2**16), 2**16 * "m")
            mf.close()
            f.close()

    def test_move(self):
        # make move works everywhere (64-bit format problem earlier)
        f = open(TESTFN, 'w+')

        f.write("ABCDEabcde") # Arbitrary character
        f.flush()

        mf = mmap.mmap(f.fileno(), 10)
        mf.move(5, 0, 5)
        self.assertEqual(mf[:], "ABCDEABCDE", "Map move should have duplicated front 5")
        mf.close()
        f.close()

        # more excessive test
        data = "0123456789"
        for dest in range(len(data)):
            for src in range(len(data)):
                for count in range(len(data) - max(dest, src)):
                    expected = data[:dest] + data[src:src+count] + data[dest+count:]
                    m = mmap.mmap(-1, len(data))
                    m[:] = data
                    m.move(dest, src, count)
                    self.assertEqual(m[:], expected)
                    m.close()

        # segfault test (Issue 5387)
        m = mmap.mmap(-1, 100)
        offsets = [-100, -1, 0, 1, 100]
        for source, dest, size in itertools.product(offsets, offsets, offsets):
            try:
                m.move(source, dest, size)
            except ValueError:
                pass

        offsets = [(-1, -1, -1), (-1, -1, 0), (-1, 0, -1), (0, -1, -1),
                   (-1, 0, 0), (0, -1, 0), (0, 0, -1)]
        for source, dest, size in offsets:
            self.assertRaises(ValueError, m.move, source, dest, size)

        m.close()

        m = mmap.mmap(-1, 1) # single byte
        self.assertRaises(ValueError, m.move, 0, 0, 2)
        self.assertRaises(ValueError, m.move, 1, 0, 1)
        self.assertRaises(ValueError, m.move, 0, 1, 1)
        m.move(0, 0, 1)
        m.move(0, 0, 0)


    def test_anonymous(self):
        # anonymous mmap.mmap(-1, PAGE)
        m = mmap.mmap(-1, PAGESIZE)
        for x in xrange(PAGESIZE):
            self.assertEqual(m[x], '\0', "anonymously mmap'ed contents should be zero")

        for x in xrange(PAGESIZE):
            m[x] = ch = chr(x & 255)
            self.assertEqual(m[x], ch)

    def test_extended_getslice(self):
        # Test extended slicing by comparing with list slicing.
        s = "".join(chr(c) for c in reversed(range(256)))
        m = mmap.mmap(-1, len(s))
        m[:] = s
        self.assertEqual(m[:], s)
        indices = (0, None, 1, 3, 19, 300, -1, -2, -31, -300)
        for start in indices:
            for stop in indices:
                # Skip step 0 (invalid)
                for step in indices[1:]:
                    self.assertEqual(m[start:stop:step],
                                     s[start:stop:step])

    def test_extended_set_del_slice(self):
        # Test extended slicing by comparing with list slicing.
        s = "".join(chr(c) for c in reversed(range(256)))
        m = mmap.mmap(-1, len(s))
        indices = (0, None, 1, 3, 19, 300, -1, -2, -31, -300)
        for start in indices:
            for stop in indices:
                # Skip invalid step 0
                for step in indices[1:]:
                    m[:] = s
                    self.assertEqual(m[:], s)
                    L = list(s)
                    # Make sure we have a slice of exactly the right length,
                    # but with different data.
                    data = L[start:stop:step]
                    data = "".join(reversed(data))
                    L[start:stop:step] = data
                    m[start:stop:step] = data
                    self.assertEquals(m[:], "".join(L))

    def make_mmap_file (self, f, halfsize):
        # Write 2 pages worth of data to the file
        f.write ('\0' * halfsize)
        f.write ('foo')
        f.write ('\0' * (halfsize - 3))
        f.flush ()
        return mmap.mmap (f.fileno(), 0)

    def test_offset (self):
        f = open (TESTFN, 'w+b')

        try: # unlink TESTFN no matter what
            halfsize = mmap.ALLOCATIONGRANULARITY
            m = self.make_mmap_file (f, halfsize)
            m.close ()
            f.close ()

            mapsize = halfsize * 2
            # Try invalid offset
            f = open(TESTFN, "r+b")
            for offset in [-2, -1, None]:
                try:
                    m = mmap.mmap(f.fileno(), mapsize, offset=offset)
                    self.assertEqual(0, 1)
                except (ValueError, TypeError, OverflowError):
                    pass
                else:
                    self.assertEqual(0, 0)
            f.close()

            # Try valid offset, hopefully 8192 works on all OSes
            f = open(TESTFN, "r+b")
            m = mmap.mmap(f.fileno(), mapsize - halfsize, offset=halfsize)
            self.assertEqual(m[0:3], 'foo')
            f.close()

            # Try resizing map
            try:
                m.resize(512)
            except SystemError:
                pass
            else:
                # resize() is supported
                self.assertEqual(len(m), 512)
                # Check that we can no longer seek beyond the new size.
                self.assertRaises(ValueError, m.seek, 513, 0)
                # Check that the content is not changed
                self.assertEqual(m[0:3], 'foo')

                # Check that the underlying file is truncated too
                f = open(TESTFN)
                f.seek(0, 2)
                self.assertEqual(f.tell(), halfsize + 512)
                f.close()
                self.assertEqual(m.size(), halfsize + 512)

            m.close()

        finally:
            f.close()
            try:
                os.unlink(TESTFN)
            except OSError:
                pass

    def test_subclass(self):
        class anon_mmap(mmap.mmap):
            def __new__(klass, *args, **kwargs):
                return mmap.mmap.__new__(klass, -1, *args, **kwargs)
        anon_mmap(PAGESIZE)

    def test_prot_readonly(self):
        if not hasattr(mmap, 'PROT_READ'):
            return
        mapsize = 10
        open(TESTFN, "wb").write("a"*mapsize)
        f = open(TESTFN, "rb")
        m = mmap.mmap(f.fileno(), mapsize, prot=mmap.PROT_READ)
        self.assertRaises(TypeError, m.write, "foo")
        f.close()

    def test_error(self):
        self.assertTrue(issubclass(mmap.error, EnvironmentError))
        self.assertIn("mmap.error", str(mmap.error))

    def test_io_methods(self):
        data = "0123456789"
        open(TESTFN, "wb").write("x"*len(data))
        f = open(TESTFN, "r+b")
        m = mmap.mmap(f.fileno(), len(data))
        f.close()
        # Test write_byte()
        for i in xrange(len(data)):
            self.assertEquals(m.tell(), i)
            m.write_byte(data[i])
            self.assertEquals(m.tell(), i+1)
        self.assertRaises(ValueError, m.write_byte, "x")
        self.assertEquals(m[:], data)
        # Test read_byte()
        m.seek(0)
        for i in xrange(len(data)):
            self.assertEquals(m.tell(), i)
            self.assertEquals(m.read_byte(), data[i])
            self.assertEquals(m.tell(), i+1)
        self.assertRaises(ValueError, m.read_byte)
        # Test read()
        m.seek(3)
        self.assertEquals(m.read(3), "345")
        self.assertEquals(m.tell(), 6)
        # Test write()
        m.seek(3)
        m.write("bar")
        self.assertEquals(m.tell(), 6)
        self.assertEquals(m[:], "012bar6789")
        m.seek(8)
        self.assertRaises(ValueError, m.write, "bar")

    if os.name == 'nt':
        def test_tagname(self):
            data1 = "0123456789"
            data2 = "abcdefghij"
            assert len(data1) == len(data2)

            # Test same tag
            m1 = mmap.mmap(-1, len(data1), tagname="foo")
            m1[:] = data1
            m2 = mmap.mmap(-1, len(data2), tagname="foo")
            m2[:] = data2
            self.assertEquals(m1[:], data2)
            self.assertEquals(m2[:], data2)
            m2.close()
            m1.close()

            # Test differnt tag
            m1 = mmap.mmap(-1, len(data1), tagname="foo")
            m1[:] = data1
            m2 = mmap.mmap(-1, len(data2), tagname="boo")
            m2[:] = data2
            self.assertEquals(m1[:], data1)
            self.assertEquals(m2[:], data2)
            m2.close()
            m1.close()

        def test_crasher_on_windows(self):
            # Should not crash (Issue 1733986)
            m = mmap.mmap(-1, 1000, tagname="foo")
            try:
                mmap.mmap(-1, 5000, tagname="foo")[:] # same tagname, but larger size
            except:
                pass
            m.close()

            # Should not crash (Issue 5385)
            with open(TESTFN, "wb") as f:
                f.write("x"*10)
            f = open(TESTFN, "r+b")
            m = mmap.mmap(f.fileno(), 0)
            f.close()
            try:
                m.resize(0) # will raise WindowsError
            except:
                pass
            try:
                m[:]
            except:
                pass
            m.close()


def test_main():
    run_unittest(MmapTests)

if __name__ == '__main__':
    test_main()
