import sys, unittest, struct, math
from binascii import hexlify

from ctypes import *

def bin(s):
    return hexlify(memoryview(s)).upper()

# Each *simple* type that supports different byte orders has an
# __ctype_be__ attribute that specifies the same type in BIG ENDIAN
# byte order, and a __ctype_le__ attribute that is the same type in
# LITTLE ENDIAN byte order.
#
# For Structures and Unions, these types are created on demand.

class Test(unittest.TestCase):
    def X_test(self):
        print >> sys.stderr,  sys.byteorder
        for i in range(32):
            bits = BITS()
            setattr(bits, "i%s" % i, 1)
            dump(bits)

    def test_endian_short(self):
        if sys.byteorder == "little":
            self.assertTrue(c_short.__ctype_le__ is c_short)
            self.assertTrue(c_short.__ctype_be__.__ctype_le__ is c_short)
        else:
            self.assertTrue(c_short.__ctype_be__ is c_short)
            self.assertTrue(c_short.__ctype_le__.__ctype_be__ is c_short)
        s = c_short.__ctype_be__(0x1234)
        self.assertEqual(bin(struct.pack(">h", 0x1234)), "1234")
        self.assertEqual(bin(s), "1234")
        self.assertEqual(s.value, 0x1234)

        s = c_short.__ctype_le__(0x1234)
        self.assertEqual(bin(struct.pack("<h", 0x1234)), "3412")
        self.assertEqual(bin(s), "3412")
        self.assertEqual(s.value, 0x1234)

        s = c_ushort.__ctype_be__(0x1234)
        self.assertEqual(bin(struct.pack(">h", 0x1234)), "1234")
        self.assertEqual(bin(s), "1234")
        self.assertEqual(s.value, 0x1234)

        s = c_ushort.__ctype_le__(0x1234)
        self.assertEqual(bin(struct.pack("<h", 0x1234)), "3412")
        self.assertEqual(bin(s), "3412")
        self.assertEqual(s.value, 0x1234)

    def test_endian_int(self):
        if sys.byteorder == "little":
            self.assertTrue(c_int.__ctype_le__ is c_int)
            self.assertTrue(c_int.__ctype_be__.__ctype_le__ is c_int)
        else:
            self.assertTrue(c_int.__ctype_be__ is c_int)
            self.assertTrue(c_int.__ctype_le__.__ctype_be__ is c_int)

        s = c_int.__ctype_be__(0x12345678)
        self.assertEqual(bin(struct.pack(">i", 0x12345678)), "12345678")
        self.assertEqual(bin(s), "12345678")
        self.assertEqual(s.value, 0x12345678)

        s = c_int.__ctype_le__(0x12345678)
        self.assertEqual(bin(struct.pack("<i", 0x12345678)), "78563412")
        self.assertEqual(bin(s), "78563412")
        self.assertEqual(s.value, 0x12345678)

        s = c_uint.__ctype_be__(0x12345678)
        self.assertEqual(bin(struct.pack(">I", 0x12345678)), "12345678")
        self.assertEqual(bin(s), "12345678")
        self.assertEqual(s.value, 0x12345678)

        s = c_uint.__ctype_le__(0x12345678)
        self.assertEqual(bin(struct.pack("<I", 0x12345678)), "78563412")
        self.assertEqual(bin(s), "78563412")
        self.assertEqual(s.value, 0x12345678)

    def test_endian_longlong(self):
        if sys.byteorder == "little":
            self.assertTrue(c_longlong.__ctype_le__ is c_longlong)
            self.assertTrue(c_longlong.__ctype_be__.__ctype_le__ is c_longlong)
        else:
            self.assertTrue(c_longlong.__ctype_be__ is c_longlong)
            self.assertTrue(c_longlong.__ctype_le__.__ctype_be__ is c_longlong)

        s = c_longlong.__ctype_be__(0x1234567890ABCDEF)
        self.assertEqual(bin(struct.pack(">q", 0x1234567890ABCDEF)), "1234567890ABCDEF")
        self.assertEqual(bin(s), "1234567890ABCDEF")
        self.assertEqual(s.value, 0x1234567890ABCDEF)

        s = c_longlong.__ctype_le__(0x1234567890ABCDEF)
        self.assertEqual(bin(struct.pack("<q", 0x1234567890ABCDEF)), "EFCDAB9078563412")
        self.assertEqual(bin(s), "EFCDAB9078563412")
        self.assertEqual(s.value, 0x1234567890ABCDEF)

        s = c_ulonglong.__ctype_be__(0x1234567890ABCDEF)
        self.assertEqual(bin(struct.pack(">Q", 0x1234567890ABCDEF)), "1234567890ABCDEF")
        self.assertEqual(bin(s), "1234567890ABCDEF")
        self.assertEqual(s.value, 0x1234567890ABCDEF)

        s = c_ulonglong.__ctype_le__(0x1234567890ABCDEF)
        self.assertEqual(bin(struct.pack("<Q", 0x1234567890ABCDEF)), "EFCDAB9078563412")
        self.assertEqual(bin(s), "EFCDAB9078563412")
        self.assertEqual(s.value, 0x1234567890ABCDEF)

    def test_endian_float(self):
        if sys.byteorder == "little":
            self.assertTrue(c_float.__ctype_le__ is c_float)
            self.assertTrue(c_float.__ctype_be__.__ctype_le__ is c_float)
        else:
            self.assertTrue(c_float.__ctype_be__ is c_float)
            self.assertTrue(c_float.__ctype_le__.__ctype_be__ is c_float)
        s = c_float(math.pi)
        self.assertEqual(bin(struct.pack("f", math.pi)), bin(s))
        # Hm, what's the precision of a float compared to a double?
        self.assertAlmostEqual(s.value, math.pi, 6)
        s = c_float.__ctype_le__(math.pi)
        self.assertAlmostEqual(s.value, math.pi, 6)
        self.assertEqual(bin(struct.pack("<f", math.pi)), bin(s))
        s = c_float.__ctype_be__(math.pi)
        self.assertAlmostEqual(s.value, math.pi, 6)
        self.assertEqual(bin(struct.pack(">f", math.pi)), bin(s))

    def test_endian_double(self):
        if sys.byteorder == "little":
            self.assertTrue(c_double.__ctype_le__ is c_double)
            self.assertTrue(c_double.__ctype_be__.__ctype_le__ is c_double)
        else:
            self.assertTrue(c_double.__ctype_be__ is c_double)
            self.assertTrue(c_double.__ctype_le__.__ctype_be__ is c_double)
        s = c_double(math.pi)
        self.assertEqual(s.value, math.pi)
        self.assertEqual(bin(struct.pack("d", math.pi)), bin(s))
        s = c_double.__ctype_le__(math.pi)
        self.assertEqual(s.value, math.pi)
        self.assertEqual(bin(struct.pack("<d", math.pi)), bin(s))
        s = c_double.__ctype_be__(math.pi)
        self.assertEqual(s.value, math.pi)
        self.assertEqual(bin(struct.pack(">d", math.pi)), bin(s))

    def test_endian_other(self):
        self.assertTrue(c_byte.__ctype_le__ is c_byte)
        self.assertTrue(c_byte.__ctype_be__ is c_byte)

        self.assertTrue(c_ubyte.__ctype_le__ is c_ubyte)
        self.assertTrue(c_ubyte.__ctype_be__ is c_ubyte)

        self.assertTrue(c_char.__ctype_le__ is c_char)
        self.assertTrue(c_char.__ctype_be__ is c_char)

    def test_struct_fields_1(self):
        if sys.byteorder == "little":
            base = BigEndianStructure
        else:
            base = LittleEndianStructure

        class T(base):
            pass
        _fields_ = [("a", c_ubyte),
                    ("b", c_byte),
                    ("c", c_short),
                    ("d", c_ushort),
                    ("e", c_int),
                    ("f", c_uint),
                    ("g", c_long),
                    ("h", c_ulong),
                    ("i", c_longlong),
                    ("k", c_ulonglong),
                    ("l", c_float),
                    ("m", c_double),
                    ("n", c_char),

                    ("b1", c_byte, 3),
                    ("b2", c_byte, 3),
                    ("b3", c_byte, 2),
                    ("a", c_int * 3 * 3 * 3)]
        T._fields_ = _fields_

        # these fields do not support different byte order:
        for typ in c_wchar, c_void_p, POINTER(c_int):
            _fields_.append(("x", typ))
            class T(base):
                pass
            self.assertRaises(TypeError, setattr, T, "_fields_", [("x", typ)])

    def test_struct_struct(self):
        # Nested structures with different byte order not (yet) supported
        if sys.byteorder == "little":
            base = BigEndianStructure
        else:
            base = LittleEndianStructure

        class T(Structure):
            _fields_ = [("a", c_int),
                        ("b", c_int)]
        class S(base):
            pass
        self.assertRaises(TypeError, setattr, S, "_fields_", [("s", T)])

    def test_struct_fields_2(self):
        # standard packing in struct uses no alignment.
        # So, we have to align using pad bytes.
        #
        # Unaligned accesses will crash Python (on those platforms that
        # don't allow it, like sparc solaris).
        if sys.byteorder == "little":
            base = BigEndianStructure
            fmt = ">bxhid"
        else:
            base = LittleEndianStructure
            fmt = "<bxhid"

        class S(base):
            _fields_ = [("b", c_byte),
                        ("h", c_short),
                        ("i", c_int),
                        ("d", c_double)]

        s1 = S(0x12, 0x1234, 0x12345678, 3.14)
        s2 = struct.pack(fmt, 0x12, 0x1234, 0x12345678, 3.14)
        self.assertEqual(bin(s1), bin(s2))

    def test_unaligned_nonnative_struct_fields(self):
        if sys.byteorder == "little":
            base = BigEndianStructure
            fmt = ">b h xi xd"
        else:
            base = LittleEndianStructure
            fmt = "<b h xi xd"

        class S(base):
            _pack_ = 1
            _fields_ = [("b", c_byte),

                        ("h", c_short),

                        ("_1", c_byte),
                        ("i", c_int),

                        ("_2", c_byte),
                        ("d", c_double)]

        s1 = S()
        s1.b = 0x12
        s1.h = 0x1234
        s1.i = 0x12345678
        s1.d = 3.14
        s2 = struct.pack(fmt, 0x12, 0x1234, 0x12345678, 3.14)
        self.assertEqual(bin(s1), bin(s2))

    def test_unaligned_native_struct_fields(self):
        if sys.byteorder == "little":
            fmt = "<b h xi xd"
        else:
            base = LittleEndianStructure
            fmt = ">b h xi xd"

        class S(Structure):
            _pack_ = 1
            _fields_ = [("b", c_byte),

                        ("h", c_short),

                        ("_1", c_byte),
                        ("i", c_int),

                        ("_2", c_byte),
                        ("d", c_double)]

        s1 = S()
        s1.b = 0x12
        s1.h = 0x1234
        s1.i = 0x12345678
        s1.d = 3.14
        s2 = bytes(struct.pack(fmt, 0x12, 0x1234, 0x12345678, 3.14))
        self.assertEqual(bin(s1), bin(s2))

if __name__ == "__main__":
    unittest.main()
