# Copyright 2007 Google, Inc. All Rights Reserved.
# Licensed to PSF under a Contributor Agreement.

"""Unit tests for abc.py."""

import unittest
from test import test_support

import abc
from inspect import isabstract


class TestABC(unittest.TestCase):

    def test_abstractmethod_basics(self):
        @abc.abstractmethod
        def foo(self): pass
        self.assertEqual(foo.__isabstractmethod__, True)
        def bar(self): pass
        self.assertEqual(hasattr(bar, "__isabstractmethod__"), False)

    def test_abstractproperty_basics(self):
        @abc.abstractproperty
        def foo(self): pass
        self.assertEqual(foo.__isabstractmethod__, True)
        def bar(self): pass
        self.assertEqual(hasattr(bar, "__isabstractmethod__"), False)

        class C:
            __metaclass__ = abc.ABCMeta
            @abc.abstractproperty
            def foo(self): return 3
        class D(C):
            @property
            def foo(self): return super(D, self).foo
        self.assertEqual(D().foo, 3)

    def test_abstractmethod_integration(self):
        if test_support.due_to_ironpython_bug("http://www.codeplex.com/IronPython/WorkItem/View.aspx?WorkItemId=21116"):
            return
        for abstractthing in [abc.abstractmethod, abc.abstractproperty]:
            class C:
                __metaclass__ = abc.ABCMeta
                @abstractthing
                def foo(self): pass  # abstract
                def bar(self): pass  # concrete
            self.assertEqual(C.__abstractmethods__, set(["foo"]))
            self.assertRaises(TypeError, C)  # because foo is abstract
            self.assertTrue(isabstract(C))
            class D(C):
                def bar(self): pass  # concrete override of concrete
            self.assertEqual(D.__abstractmethods__, set(["foo"]))
            self.assertRaises(TypeError, D)  # because foo is still abstract
            self.assertTrue(isabstract(D))
            class E(D):
                def foo(self): pass
            self.assertEqual(E.__abstractmethods__, set())
            E()  # now foo is concrete, too
            self.assertFalse(isabstract(E))
            class F(E):
                @abstractthing
                def bar(self): pass  # abstract override of concrete
            self.assertEqual(F.__abstractmethods__, set(["bar"]))
            self.assertRaises(TypeError, F)  # because bar is abstract now
            self.assertTrue(isabstract(F))

    def test_subclass_oldstyle_class(self):
        class A:
            __metaclass__ = abc.ABCMeta
        class OldstyleClass:
            pass
        self.assertFalse(issubclass(OldstyleClass, A))
        self.assertFalse(issubclass(A, OldstyleClass))

    def test_isinstance_class(self):
        class A:
            __metaclass__ = abc.ABCMeta
        class OldstyleClass:
            pass
        self.assertFalse(isinstance(OldstyleClass, A))
        self.assertTrue(isinstance(OldstyleClass, type(OldstyleClass)))
        self.assertFalse(isinstance(A, OldstyleClass))
        # This raises a recursion depth error, but is low-priority:
        self.assertTrue(isinstance(A, abc.ABCMeta))

    def test_registration_basics(self):
        class A:
            __metaclass__ = abc.ABCMeta
        class B(object):
            pass
        b = B()
        self.assertEqual(issubclass(B, A), False)
        self.assertEqual(issubclass(B, (A,)), False)
        self.assertNotIsInstance(b, A)
        self.assertNotIsInstance(b, (A,))
        A.register(B)
        self.assertEqual(issubclass(B, A), True)
        self.assertEqual(issubclass(B, (A,)), True)
        self.assertIsInstance(b, A)
        self.assertIsInstance(b, (A,))
        class C(B):
            pass
        c = C()
        self.assertEqual(issubclass(C, A), True)
        self.assertEqual(issubclass(C, (A,)), True)
        self.assertIsInstance(c, A)
        self.assertIsInstance(c, (A,))

    def test_isinstance_invalidation(self):
        class A:
            __metaclass__ = abc.ABCMeta
        class B(object):
            pass
        b = B()
        self.assertEqual(isinstance(b, A), False)
        self.assertEqual(isinstance(b, (A,)), False)
        A.register(B)
        self.assertEqual(isinstance(b, A), True)
        self.assertEqual(isinstance(b, (A,)), True)

    def test_registration_builtins(self):
        class A:
            __metaclass__ = abc.ABCMeta
        A.register(int)
        self.assertIsInstance(42, A)
        self.assertIsInstance(42, (A,))
        self.assertEqual(issubclass(int, A), True)
        self.assertEqual(issubclass(int, (A,)), True)
        class B(A):
            pass
        B.register(basestring)
        self.assertIsInstance("", A)
        self.assertIsInstance("", (A,))
        self.assertEqual(issubclass(str, A), True)
        self.assertEqual(issubclass(str, (A,)), True)

    def test_registration_edge_cases(self):
        class A:
            __metaclass__ = abc.ABCMeta
        A.register(A)  # should pass silently
        class A1(A):
            pass
        self.assertRaises(RuntimeError, A1.register, A)  # cycles not allowed
        class B(object):
            pass
        A1.register(B)  # ok
        A1.register(B)  # should pass silently
        class C(A):
            pass
        A.register(C)  # should pass silently
        self.assertRaises(RuntimeError, C.register, A)  # cycles not allowed
        C.register(B)  # ok

    def test_register_non_class(self):
        class A(object):
            __metaclass__ = abc.ABCMeta
        self.assertRaisesRegexp(TypeError, "Can only register classes",
                                A.register, 4)

    def test_registration_transitiveness(self):
        class A:
            __metaclass__ = abc.ABCMeta
        self.assertTrue(issubclass(A, A))
        self.assertTrue(issubclass(A, (A,)))
        class B:
            __metaclass__ = abc.ABCMeta
        self.assertFalse(issubclass(A, B))
        self.assertFalse(issubclass(A, (B,)))
        self.assertFalse(issubclass(B, A))
        self.assertFalse(issubclass(B, (A,)))
        class C:
            __metaclass__ = abc.ABCMeta
        A.register(B)
        class B1(B):
            pass
        self.assertTrue(issubclass(B1, A))
        self.assertTrue(issubclass(B1, (A,)))
        class C1(C):
            pass
        B1.register(C1)
        self.assertFalse(issubclass(C, B))
        self.assertFalse(issubclass(C, (B,)))
        self.assertFalse(issubclass(C, B1))
        self.assertFalse(issubclass(C, (B1,)))
        self.assertTrue(issubclass(C1, A))
        self.assertTrue(issubclass(C1, (A,)))
        self.assertTrue(issubclass(C1, B))
        self.assertTrue(issubclass(C1, (B,)))
        self.assertTrue(issubclass(C1, B1))
        self.assertTrue(issubclass(C1, (B1,)))
        C1.register(int)
        class MyInt(int):
            pass
        self.assertTrue(issubclass(MyInt, A))
        self.assertTrue(issubclass(MyInt, (A,)))
        self.assertIsInstance(42, A)
        self.assertIsInstance(42, (A,))

    def test_all_new_methods_are_called(self):
        class A:
            __metaclass__ = abc.ABCMeta
        class B(object):
            counter = 0
            def __new__(cls):
                B.counter += 1
                return super(B, cls).__new__(cls)
        class C(A, B):
            pass
        self.assertEqual(B.counter, 0)
        C()
        self.assertEqual(B.counter, 1)


def test_main():
    test_support.run_unittest(TestABC)


if __name__ == "__main__":
    unittest.main()
