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
namespace System {
    
    //Only contains static methods.  Does not require serialization
    
    using System;
    using System.Runtime.CompilerServices;
[System.Runtime.InteropServices.ComVisible(true)]
    public static class Buffer
    {
        // Copies from one primitive array to another primitive array without
        // respecting types.  This calls memmove internally.
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void BlockCopy(Array src, int srcOffset,
            Array dst, int dstOffset, int count);

        // A very simple and efficient array copy that assumes all of the
        // parameter validation has already been done.  All counts here are
        // in bytes.
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal static extern void InternalBlockCopy(Array src, int srcOffset,
            Array dst, int dstOffset, int count);

        // This is ported from the optimized CRT assembly in memchr.asm. The JIT generates 
        // pretty good code here and this ends up being within a couple % of the CRT asm.
        // It is however cross platform as the CRT hasn't ported their fast version to 64-bit
        // platforms.
        //
        internal unsafe static int IndexOfByte(byte* src, byte value, int index, int count)
        {
            BCLDebug.Assert(src != null, "src should not be null");

            byte* pByte = src + index;

            // Align up the pointer to sizeof(int).
            while (((int)pByte & 3) != 0)
            {
                if (count == 0)
                    return -1;
                else if (*pByte == value)
                    return (int) (pByte - src);

                count--;
                pByte++;
            }

            // Fill comparer with value byte for comparisons
            //
            // comparer = 0/0/value/value
            uint comparer = (((uint)value << 8) + (uint)value);
            // comparer = value/value/value/value
            comparer = (comparer << 16) + comparer;

            // Run through buffer until we hit a 4-byte section which contains
            // the byte we're looking for or until we exhaust the buffer.
            while (count > 3)
            {
                // Test the buffer for presence of value. comparer contains the byte
                // replicated 4 times.
                uint t1 = *(uint*)pByte;
                t1 = t1 ^ comparer;
                uint t2 = 0x7efefeff + t1;
                t1 = t1 ^ 0xffffffff;
                t1 = t1 ^ t2;
                t1 = t1 & 0x81010100;

                // if t1 is non-zero then these 4-bytes don't contain a match
                if (t1 == 0)
                {
                    count -= 4;
                    pByte += 4;
                    continue;
                }

                // We've found a match for value, figure out which position it's in.
                int foundIndex = (int) (pByte - src);
                if (pByte[0] == value)
                    return foundIndex;
                else if (pByte[1] == value)
                    return foundIndex + 1;
                else if (pByte[2] == value)
                    return foundIndex + 2;
                else if (pByte[3] == value)
                    return foundIndex + 3;
            }

            // Catch any bytes that might be left at the tail of the buffer
            while (count > 0)
            {
                if (*pByte == value)
                    return (int) (pByte - src);

                count--;
                pByte++;
            }

            // If we don't have a match return -1;
            return -1;
        }
		
        // Gets a particular byte out of the array.  The array must be an
        // array of primitives.  
        //
        // This essentially does the following: 
        // return ((byte*)array) + index.
        //
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern byte GetByte(Array array, int index);
    
        // Sets a particular byte in an the array.  The array must be an
        // array of primitives.  
        //
        // This essentially does the following: 
        // *(((byte*)array) + index) = value.
        //
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern void SetByte(Array array, int index, byte value);
    
        // Gets a particular byte out of the array.  The array must be an
        // array of primitives.  
        //
        // This essentially does the following: 
        // return array.length * sizeof(array.UnderlyingElementType).
        //
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        public static extern int ByteLength(Array array);

        internal unsafe static void ZeroMemory(byte* src, long len)
        {
            while(len-- > 0)
                *(src + len) = 0;
        }

        internal unsafe static void memcpy(byte* src, int srcIndex, byte[] dest, int destIndex, int len) {
            BCLDebug.Assert( (srcIndex >= 0) && (destIndex >= 0) && (len >= 0), "Index and length must be non-negative!");
            BCLDebug.Assert(dest.Length - destIndex >= len, "not enough bytes in dest");
            // If dest has 0 elements, the fixed statement will throw an 
            // IndexOutOfRangeException.  Special-case 0-byte copies.
            if (len==0)
                return;
            fixed(byte* pDest = dest) {
                memcpyimpl(src+srcIndex, pDest+destIndex, len);
            }
        }

        internal unsafe static void memcpy(byte[] src, int srcIndex, byte* pDest, int destIndex, int len) {
            BCLDebug.Assert( (srcIndex >= 0) && (destIndex >= 0) && (len >= 0), "Index and length must be non-negative!");        
            BCLDebug.Assert(src.Length - srcIndex >= len, "not enough bytes in src");
            // If dest has 0 elements, the fixed statement will throw an 
            // IndexOutOfRangeException.  Special-case 0-byte copies.
            if (len==0)
                return;
            fixed(byte* pSrc = src) {
                memcpyimpl(pSrc+srcIndex, pDest+destIndex, len);
            }
        }

        internal unsafe static void memcpy(char* pSrc, int srcIndex, char* pDest, int destIndex, int len) {
            BCLDebug.Assert( (srcIndex >= 0) && (destIndex >= 0) && (len >= 0), "Index and length must be non-negative!");
                    
            // No boundary check for buffer overruns - dangerous
            if (len==0)
                return;
            memcpyimpl((byte*)(char*)(pSrc+srcIndex), (byte*)(char*)(pDest+destIndex), len*2);
        }
        
        // Note - using a long instead of an int for the length parameter
        // slows this method down by ~18%.
        internal unsafe static void memcpyimpl(byte* src, byte* dest, int len) {
            BCLDebug.Assert(len >= 0, "Negative length in memcopy!");

            // Portable naive implementation
            while (len-- > 0)
                *dest++ = *src++;
        }
    }    
}
