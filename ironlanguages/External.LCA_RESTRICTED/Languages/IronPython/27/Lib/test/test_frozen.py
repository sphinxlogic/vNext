# Test the frozen module defined in frozen.c.

from test.test_support import captured_stdout, run_unittest
import unittest
import sys

from test import test_support
if  test_support.due_to_ironpython_bug("http://tkbgitvstfat01:8080/WorkItemTracking/WorkItem.aspx?artifactMoniker=316478"):
    sys.exit(0)

class FrozenTests(unittest.TestCase):
    def test_frozen(self):

        with captured_stdout() as stdout:
            try:
                import __hello__
            except ImportError, x:
                self.fail("import __hello__ failed:" + str(x))

            try:
                import __phello__
            except ImportError, x:
                self.fail("import __phello__ failed:" + str(x))

            try:
                import __phello__.spam
            except ImportError, x:
                self.fail("import __phello__.spam failed:" + str(x))

            try:
                import __phello__.foo
            except ImportError:
                pass
            else:
                self.fail("import __phello__.foo should have failed")

        self.assertEquals(stdout.getvalue(),
                          'Hello world...\nHello world...\nHello world...\n')

        del sys.modules['__hello__']
        del sys.modules['__phello__']
        del sys.modules['__phello__.spam']


def test_main():
    run_unittest(FrozenTests)



if __name__ == '__main__':
    test_main()
