// ==++==
// 
//   
//    Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//   
//    The use and distribution terms for this software are contained in the file
//    named license.txt, which can be found in the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by the
//    terms of this license.
//   
//    You must not remove this notice, or any other, from this software.
//   
// 
// ==--==

using System;
using System.Collections;
using System.Collections.Generic;

namespace System.Collections.Generic
{
    using System.Globalization;
    using System.Runtime.CompilerServices;
    
    [Serializable()]
    [TypeDependencyAttribute("System.Collections.Generic.GenericEqualityComparer`1")]
    public abstract class EqualityComparer<T> : IEqualityComparer, IEqualityComparer<T>
    {
        static EqualityComparer<T> defaultComparer;

        public static EqualityComparer<T> Default {
            get {
                EqualityComparer<T> comparer = defaultComparer;
                if (comparer == null) {
                    comparer = CreateComparer();
                    defaultComparer = comparer;
                }
                return comparer;
            }
        }
        
        private static EqualityComparer<T> CreateComparer() {
            Type t = typeof(T);
            // Specialize type byte for performance reasons
            if (t == typeof(byte)) {
                return (EqualityComparer<T>)(object)(new ByteEqualityComparer());
            }
            // If T implements IEquatable<T> return a GenericEqualityComparer<T>
            if (typeof(IEquatable<T>).IsAssignableFrom(t)) {
                //return (EqualityComparer<T>)Activator.CreateInstance(typeof(GenericEqualityComparer<>).MakeGenericType(t));
                return (EqualityComparer<T>)(typeof(GenericEqualityComparer<int>).TypeHandle.CreateInstanceForAnotherGenericParameter(t));
            }
            // If T is a Nullable<U> where U implements IEquatable<U> return a NullableEqualityComparer<U>
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                Type u = t.GetGenericArguments()[0];
                if (typeof(IEquatable<>).MakeGenericType(u).IsAssignableFrom(u)) {
                    //return (EqualityComparer<T>)Activator.CreateInstance(typeof(NullableEqualityComparer<>).MakeGenericType(u));
                    return (EqualityComparer<T>)(typeof(NullableEqualityComparer<int>).TypeHandle.CreateInstanceForAnotherGenericParameter(u));
                }
            }
            // Otherwise return an ObjectEqualityComparer<T>
            return new ObjectEqualityComparer<T>();
        }

        public abstract bool Equals(T x, T y);
        public abstract int GetHashCode(T obj);

        internal virtual int IndexOf(T[] array, T value, int startIndex, int count) {
            int endIndex = startIndex + count;
            for (int i = startIndex; i < endIndex; i++) {
                if (Equals(array[i], value)) return i;
            }
            return -1;
        }

        internal virtual int LastIndexOf(T[] array, T value, int startIndex, int count) {
            int endIndex = startIndex - count + 1;
            for (int i = startIndex; i >= endIndex; i--) {
                if (Equals(array[i], value)) return i;
            }
            return -1;
        }

        int IEqualityComparer.GetHashCode(object obj) {
            if (obj == null) return 0;
            if (obj is T) return GetHashCode((T)obj);
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return 0;            
        }                        

        bool IEqualityComparer.Equals(object x, object y) {
            if (x == y) return true;
            if (x == null || y == null) return false;
            if ((x is T) && (y is T)) return Equals((T)x, (T)y);
            ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArgumentForComparison);
            return false;
        }
    }

    // The methods in this class look identical to the inherited methods, but the calls
    // to Equal bind to IEquatable<T>.Equals(T) instead of Object.Equals(Object)
    [Serializable()]
    internal class GenericEqualityComparer<T>: EqualityComparer<T> where T: IEquatable<T>
    {    
        public override bool Equals(T x, T y) {
            if (x != null) {
                if (y != null) return x.Equals(y);
                return false;
            }
            if (y != null) return false;
            return true;
        }

        public override int GetHashCode(T obj) {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }

        internal override int IndexOf(T[] array, T value, int startIndex, int count) {
            int endIndex = startIndex + count;
            if (value == null) {
                for (int i = startIndex; i < endIndex; i++) {
                    if (array[i] == null) return i;
                }
            }
            else {
                for (int i = startIndex; i < endIndex; i++) {
                    if (array[i] != null && array[i].Equals(value)) return i;
                }
            }
            return -1;
        }

        internal override int LastIndexOf(T[] array, T value, int startIndex, int count) {
            int endIndex = startIndex - count + 1;
            if (value == null) {
                for (int i = startIndex; i >= endIndex; i--) {
                    if (array[i] == null) return i;
                }
            }
            else {
                for (int i = startIndex; i >= endIndex; i--) {
                    if (array[i] != null && array[i].Equals(value)) return i;
                }
            }
            return -1;
        }

        // Equals method for the comparer itself. 
        public override bool Equals(Object obj){
            GenericEqualityComparer<T> comparer = obj as GenericEqualityComparer<T>;
            return comparer != null;
        }

        public override int GetHashCode() {
            return this.GetType().Name.GetHashCode();
        }
    }

    [Serializable()]
    internal class NullableEqualityComparer<T> : EqualityComparer<Nullable<T>> where T : struct, IEquatable<T>
    {
        public override bool Equals(Nullable<T> x, Nullable<T> y) {
            if (x.HasValue) {
                if (y.HasValue) return x.value.Equals(y.value);
                return false;
            }
            if (y.HasValue) return false;
            return true;
        }

        public override int GetHashCode(Nullable<T> obj) {
            return obj.GetHashCode();
        }

        internal override int IndexOf(Nullable<T>[] array, Nullable<T> value, int startIndex, int count) {
            int endIndex = startIndex + count;
            if (!value.HasValue) {
                for (int i = startIndex; i < endIndex; i++) {
                    if (!array[i].HasValue) return i;
                }
            }
            else {
                for (int i = startIndex; i < endIndex; i++) {
                    if (array[i].HasValue && array[i].value.Equals(value.value)) return i;
                }
            }
            return -1;
        }

        internal override int LastIndexOf(Nullable<T>[] array, Nullable<T> value, int startIndex, int count) {
            int endIndex = startIndex - count + 1;
            if (!value.HasValue) {
                for (int i = startIndex; i >= endIndex; i--) {
                    if (!array[i].HasValue) return i;
                }
            }
            else {
                for (int i = startIndex; i >= endIndex; i--) {
                    if (array[i].HasValue && array[i].value.Equals(value.value)) return i;
                }
            }
            return -1;
        }

        // Equals method for the comparer itself. 
        public override bool Equals(Object obj){
            NullableEqualityComparer<T> comparer = obj as NullableEqualityComparer<T>;
            return comparer != null;
        }        

        public override int GetHashCode() {
            return this.GetType().Name.GetHashCode();
        }                                
    }

    [Serializable()]
    internal class ObjectEqualityComparer<T>: EqualityComparer<T>
    {
        public override bool Equals(T x, T y) {
            if (x != null) {
                if (y != null) return x.Equals(y);
                return false;
            }
            if (y != null) return false;
            return true;
        }

        public override int GetHashCode(T obj) {
            if (obj == null) return 0;
            return obj.GetHashCode();
        }

        internal override int IndexOf(T[] array, T value, int startIndex, int count) {
            int endIndex = startIndex + count;
            if (value == null) {
                for (int i = startIndex; i < endIndex; i++) {
                    if (array[i] == null) return i;
                }
            }
            else {
                for (int i = startIndex; i < endIndex; i++) {
                    if (array[i] != null && array[i].Equals(value)) return i;
                }
            }
            return -1;
        }

        internal override int LastIndexOf(T[] array, T value, int startIndex, int count) {
            int endIndex = startIndex - count + 1;
            if (value == null) {
                for (int i = startIndex; i >= endIndex; i--) {
                    if (array[i] == null) return i;
                }
            }
            else {
                for (int i = startIndex; i >= endIndex; i--) {
                    if (array[i] != null && array[i].Equals(value)) return i;
                }
            }
            return -1;
        }

        // Equals method for the comparer itself. 
        public override bool Equals(Object obj){
            ObjectEqualityComparer<T> comparer = obj as ObjectEqualityComparer<T>;
            return comparer != null;
        }        

        public override int GetHashCode() {
            return this.GetType().Name.GetHashCode();
        }                                
    }

    // Performance of IndexOf on byte array is very important for some scenarios.
    // We will call the C runtime function memchr, which is optimized.
    [Serializable()]
    internal class ByteEqualityComparer: EqualityComparer<byte>
    {
        public override bool Equals(byte x, byte y) {
            return x == y;
        }

        public override int GetHashCode(byte b) {
            return b.GetHashCode();
        }

        internal unsafe override int IndexOf(byte[] array, byte value, int startIndex, int count) {
            if (array==null)
                throw new ArgumentNullException("array");
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException("startIndex", Environment.GetResourceString("ArgumentOutOfRange_Index"));
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", Environment.GetResourceString("ArgumentOutOfRange_Count"));
            if (count > array.Length - startIndex)
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidOffLen"));
            if (count == 0) return -1;
            fixed (byte* pbytes = array) {
                return Buffer.IndexOfByte(pbytes, value, startIndex, count);
            }
        }

        internal override int LastIndexOf(byte[] array, byte value, int startIndex, int count) {
            int endIndex = startIndex - count + 1;
            for (int i = startIndex; i >= endIndex; i--) {
                if (array[i] == value) return i;
            }
            return -1;
        }

        // Equals method for the comparer itself. 
        public override bool Equals(Object obj){
            ByteEqualityComparer comparer = obj as ByteEqualityComparer;
            return comparer != null;
        }        

        public override int GetHashCode() {
            return this.GetType().Name.GetHashCode();
        }                                
        
    }
}
