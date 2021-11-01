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
// DecoderReplacementFallback.cs
//
namespace System.Text
{
    using System;

    [Serializable]
    public sealed class DecoderReplacementFallback : DecoderFallback
    {
        // Our variables
        private String strDefault;

        // Construction.  Default replacement fallback uses no best fit and ? replacement string
        public DecoderReplacementFallback() : this("?")
        {
        }

        public DecoderReplacementFallback(String replacement)
        {
            if (replacement == null)
                throw new ArgumentNullException("replacement");

            // Make sure it doesn't have bad surrogate pairs
            bool bFoundHigh=false;
            for (int i = 0; i < replacement.Length; i++)
            {
                // Found a surrogate?
                if (Char.IsSurrogate(replacement,i))
                {
                    // High or Low?
                    if (Char.IsHighSurrogate(replacement, i))
                    {
                        // if already had a high one, stop
                        if (bFoundHigh)
                            break;  // break & throw at the bFoundHIgh below
                        bFoundHigh = true;
                    }
                    else
                    {
                        // Low, did we have a high?
                        if (!bFoundHigh)
                        {
                            // Didn't have one, make if fail when we stop
                            bFoundHigh = true;
                            break;
                        }

                        // Clear flag
                        bFoundHigh = false;
                    }
                }
                // If last was high we're in trouble (not surrogate so not low surrogate, so break)
                else if (bFoundHigh)
                    break;
            }
            if (bFoundHigh)
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidCharSequenceNoIndex", "replacement"));

            strDefault = replacement;
        }

        public String DefaultString
        {
             get
             {
                return strDefault;
             }
        }

        public override DecoderFallbackBuffer CreateFallbackBuffer()
        {
            return new DecoderReplacementFallbackBuffer(this);
        }

        // Maximum number of characters that this instance of this fallback could return
        public override int MaxCharCount
        {
            get
            {
                return strDefault.Length;
            }
        }

        public override bool Equals(Object value)
        {
            DecoderReplacementFallback that = value as DecoderReplacementFallback;
            if (that != null)
            {
                return (this.strDefault == that.strDefault);
            }
            return (false);
        }

        public override int GetHashCode()
        {
            return strDefault.GetHashCode();
        }
    }



    public sealed class DecoderReplacementFallbackBuffer : DecoderFallbackBuffer
    {
        // Store our default string
        private String strDefault;
        int fallbackCount = -1;
        int fallbackIndex = -1;

        // Construction
        public DecoderReplacementFallbackBuffer(DecoderReplacementFallback fallback)
        {
            this.strDefault = fallback.DefaultString;
        }

        // Fallback Methods
        public override bool Fallback(byte[] bytesUnknown, int index)
        {
            // We expect no previous fallback in our buffer
            BCLDebug.Assert(fallbackCount < 1, "[DecoderReplacementFallbackBuffer.Fallback] Calling fallback without a previously empty buffer");

            // We can't call recursively but others might (note, we don't test on last char!!!)
            if (fallbackCount >= 1)
            {
                ThrowLastBytesRecursive(bytesUnknown);
            }

            // Go ahead and get our fallback
            if (strDefault.Length == 0)
                return false;

            fallbackCount = strDefault.Length;
            fallbackIndex = -1;

            return true;
        }

        public override char GetNextChar()
        {
            // We want it to get < 0 because == 0 means that the current/last character is a fallback
            // and we need to detect recursion.  We could have a flag but we already have this counter.
            fallbackCount--;
            fallbackIndex++;

            // Do we have anything left? 0 is now last fallback char, negative is nothing left
            if (fallbackCount < 0)
                return (char)0;

            // Need to get it out of the buffer.
            BCLDebug.Assert(fallbackIndex < strDefault.Length && fallbackIndex >= 0,
                            "Index exceeds buffer range");
            return strDefault[fallbackIndex];
        }

        public override bool MovePrevious()
        {
            // Back up one, only if we just processed the last character (or earlier)
            if (fallbackCount >= -1 && fallbackIndex >= 0)
            {
                fallbackIndex--;
                fallbackCount++;
                return true;
            }

            // Return false 'cause we couldn't do it.
            return false;
        }

        // How many characters left to output?
        public override int Remaining
        {
            get
            {
                // Our count is 0 for 1 character left.
                return (fallbackCount < 0) ? 0 : fallbackCount;
            }
        }

        // Clear the buffer
        public override unsafe void Reset()
        {
            fallbackCount = -1;
            fallbackIndex = -1;
            byteStart = null;
        }

        // This version just counts the fallback and doesn't actually copy anything.
        internal unsafe override int InternalFallback(byte[] bytes, byte* pBytes)
        {
            // return our replacement string Length
            return strDefault.Length;
        }
    }
}

