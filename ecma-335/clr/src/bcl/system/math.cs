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
/*============================================================
**
** Class:  Math
**
**
** Purpose: Some floating-point math operations
**
** 
===========================================================*/
namespace System {
    
    //This class contains only static members and doesn't require serialization.
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.ConstrainedExecution;

    public static class Math {
    
      private static double doubleRoundLimit = 1e16d;

      private const int maxRoundingDigits = 15;
      
      // This table is required for the Round function which can specify the number of digits to round to
      private static double[] roundPower10Double = new double[] { 
          1E0, 1E1, 1E2, 1E3, 1E4, 1E5, 1E6, 1E7, 1E8,
          1E9, 1E10, 1E11, 1E12, 1E13, 1E14, 1E15
      };          

      public const double PI = 3.14159265358979323846;
      public const double E  = 2.7182818284590452354;
    
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Acos(double d);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Asin(double d);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Atan(double d);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Atan2(double y,double x);
      
      public static Decimal Ceiling(Decimal d) {
        return Decimal.Ceiling(d);
      }

      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Ceiling(double a);      
      
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Cos (double d);

      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Cosh(double value);
      
      public static Decimal Floor(Decimal d) {
        return Decimal.Floor(d);
      }

      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Floor(double d);
      
      private static unsafe double InternalRound(double value, int digits, MidpointRounding mode) {
        if (Abs(value) < doubleRoundLimit) {
            Double power10 = roundPower10Double[digits];
            value *= power10;
            if (mode == MidpointRounding.AwayFromZero) {                
                double fraction = SplitFractionDouble(&value); 
                if (Abs(fraction) >= 0.5d) {
                    value += Sign(fraction);
                }
            }
            else {
                // On X86 this can be inlined to just a few instructions
                value = Round(value);
            }
            value /= power10;
        }
        return value;
      }           
      
      private unsafe static double InternalTruncate(double d) {
        SplitFractionDouble(&d); 
        return d;                
      }            
      
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Sin(double a);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Tan(double a);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Sinh(double value);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Tanh(double value);

      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Round(double a);

      public static double Round(double value, int digits)
      {
           if ((digits < 0) || (digits > maxRoundingDigits))
               throw new ArgumentOutOfRangeException("digits", Environment.GetResourceString("ArgumentOutOfRange_RoundingDigits"));
           return InternalRound(value, digits, MidpointRounding.ToEven);                     
      }

      public static double Round(double value, MidpointRounding mode) {
         return Round(value, 0, mode);
      }
      
      public static double Round(double value, int digits, MidpointRounding mode) {
          if ((digits < 0) || (digits > maxRoundingDigits))
              throw new ArgumentOutOfRangeException("digits", Environment.GetResourceString("ArgumentOutOfRange_RoundingDigits"));
          if (mode < MidpointRounding.ToEven || mode > MidpointRounding.AwayFromZero) {            
              throw new ArgumentException(Environment.GetResourceString("Argument_InvalidEnumValue", mode, "MidpointRounding"), "mode");
          }
          return InternalRound(value, digits, mode);                           
      }
                                                
      public static Decimal Round(Decimal d) {
        return Decimal.Round(d,0);
      }

      public static Decimal Round(Decimal d, int decimals) {
        return Decimal.Round(d,decimals);
      }
      
      public static Decimal Round(Decimal d, MidpointRounding mode) {
        return Decimal.Round(d, 0, mode);
      }
      
      public static Decimal Round(Decimal d, int decimals, MidpointRounding mode) {
        return Decimal.Round(d, decimals, mode);
      }

      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      private static unsafe extern double SplitFractionDouble(double* value);
      
      public static Decimal Truncate(Decimal d) {
        return Decimal.Truncate(d);
      }
      
      public static double Truncate(double d) {
        return InternalTruncate(d);
      }
            
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Sqrt(double d);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Log (double d);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Log10(double d);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Exp(double d);
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      public static extern double Pow(double x, double y);
            
        public static double IEEERemainder(double x, double y) {
            double regularMod = x % y;
            if (Double.IsNaN(regularMod)) {
                return Double.NaN;
            }
            if (regularMod == 0) {
                if (Double.IsNegative(x)) {
                    return Double.NegativeZero;
                }            
            }
            double alternativeResult;
            alternativeResult = regularMod - (Math.Abs(y) * Math.Sign(x));
            if (Math.Abs(alternativeResult) == Math.Abs(regularMod)) {
                double divisionResult = x/y;
                double roundedResult = Math.Round(divisionResult);
                if (Math.Abs(roundedResult) > Math.Abs(divisionResult)) {
                    return alternativeResult;
                }
                else { 
                    return regularMod;
                }
            }
            if (Math.Abs(alternativeResult) < Math.Abs(regularMod)) {
                return alternativeResult;
            }
            else {
                return regularMod;
            }
        }
          
      /*================================Abs=========================================
      **Returns the absolute value of it's argument.
      ============================================================================*/
      [CLSCompliant(false)]
      public static sbyte Abs(sbyte value) {
        if (value >= 0)
            return value;
        else
            return AbsHelper(value);        
      }

      private static sbyte AbsHelper(sbyte value)
      {
          BCLDebug.Assert(value < 0, "AbsHelper should only be called for negative values! (hack for JIT inlining)");
          if (value == SByte.MinValue)
              throw new OverflowException(Environment.GetResourceString("Overflow_NegateTwosCompNum"));
          return ((sbyte)(-value));
      }
 
           
      public static short Abs(short value) {
          if (value >= 0)
              return value;
          else
              return AbsHelper(value);        
      }
        
      private static short AbsHelper(short value) {
          BCLDebug.Assert(value < 0, "AbsHelper should only be called for negative values! (hack for JIT inlining)");
          if (value == Int16.MinValue)
              throw new OverflowException(Environment.GetResourceString("Overflow_NegateTwosCompNum"));
          return (short) -value;
      }
    
      public static int Abs(int value) {
          if (value >= 0)
              return value;
          else
              return AbsHelper(value);        
      }
        
      private static int AbsHelper(int value) {
          BCLDebug.Assert(value < 0, "AbsHelper should only be called for negative values! (hack for JIT inlining)");
          if (value == Int32.MinValue)
              throw new OverflowException(Environment.GetResourceString("Overflow_NegateTwosCompNum"));
          return -value;
      }
    
      public static long Abs(long value) {
          if (value >= 0)
              return value;
          else
              return AbsHelper(value);        
      }
        
      private static long AbsHelper(long value) {
          BCLDebug.Assert(value < 0, "AbsHelper should only be called for negative values! (hack for JIT inlining)");
          if (value == Int64.MinValue)
              throw new OverflowException(Environment.GetResourceString("Overflow_NegateTwosCompNum"));
          return -value;
      }
    
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      extern public static float Abs(float value);
        // This is special code to handle NaN (We need to make sure NaN's aren't 
        // negated).  In CSharp, the else clause here should always be taken if 
        // value is NaN, since the normal case is taken if and only if value < 0.
        // To illustrate this completely, a compiler has translated this into:
        // "load value; load 0; bge; ret -value ; ret value".  
        // The bge command branches for comparisons with the unordered NaN.  So 
        // it runs the else case, which returns +value instead of negating it. 
        //  return (value < 0) ? -value : value;
    
      [MethodImplAttribute(MethodImplOptions.InternalCall)]
      extern public static double Abs(double value);
        // This is special code to handle NaN (We need to make sure NaN's aren't 
        // negated).  In CSharp, the else clause here should always be taken if 
        // value is NaN, since the normal case is taken if and only if value < 0.
        // To illustrate this completely, a compiler has translated this into:
        // "load value; load 0; bge; ret -value ; ret value".  
        // The bge command branches for comparisons with the unordered NaN.  So 
        // it runs the else case, which returns +value instead of negating it. 
        // return (value < 0) ? -value : value;

      public static Decimal Abs(Decimal value)
      {
          return Decimal.Abs(value);
      }
    
      /*================================MAX=========================================
      **Returns the larger of val1 and val2
      ============================================================================*/
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static sbyte Max(sbyte val1, sbyte val2) {
        return (val1>=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static byte Max(byte val1, byte val2) {
        return (val1>=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static short Max(short val1, short val2) {
        return (val1>=val2)?val1:val2;
      }
    
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static ushort Max(ushort val1, ushort val2) {
        return (val1>=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static int Max(int val1, int val2) {
        return (val1>=val2)?val1:val2;
      }
    
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static uint Max(uint val1, uint val2) {
        return (val1>=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static long Max(long val1, long val2) {
        return (val1>=val2)?val1:val2;
      }
    
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static ulong Max(ulong val1, ulong val2) {
        return (val1>=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static float Max(float val1, float val2) {
        if (val1 > val2)
            return val1;

        if (Single.IsNaN(val1))
            return val1;

        return val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static double Max(double val1, double val2) {
        if (val1 > val2)
            return val1;

        if (Double.IsNaN(val1))
            return val1;

        return val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static Decimal Max(Decimal val1, Decimal val2) {
        return Decimal.Max(val1,val2);
      }

      /*================================MIN=========================================
      **Returns the smaller of val1 and val2.
      ============================================================================*/
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static sbyte Min(sbyte val1, sbyte val2) {
        return (val1<=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static byte Min(byte val1, byte val2) {
        return (val1<=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static short Min(short val1, short val2) {
        return (val1<=val2)?val1:val2;
      }
    
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static ushort Min(ushort val1, ushort val2) {
        return (val1<=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static int Min(int val1, int val2) {
        return (val1<=val2)?val1:val2;
      }
    
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static uint Min(uint val1, uint val2) {
        return (val1<=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static long Min(long val1, long val2) {
        return (val1<=val2)?val1:val2;
      }
    
      [CLSCompliant(false)]
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static ulong Min(ulong val1, ulong val2) {
        return (val1<=val2)?val1:val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static float Min(float val1, float val2) {
        if (val1 < val2)
            return val1;

        if (Single.IsNaN(val1))
            return val1;

        return val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static double Min(double val1, double val2) {
        if (val1 < val2)
            return val1;

        if (Double.IsNaN(val1))
            return val1;

        return val2;
      }
    
      [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
      public static Decimal Min(Decimal val1, Decimal val2) {
        return Decimal.Min(val1,val2);
      }
    
      /*=====================================Log======================================
      **
      ==============================================================================*/
      public static double Log(double a, double newBase) {
        if (newBase == 1)
            return Double.NaN;
        if (a != 1 && (newBase == 0 || Double.IsPositiveInfinity(newBase)))
            return Double.NaN;

        return (Log(a)/Log(newBase));
      }
    
          
        // Sign function for VB.  Returns -1, 0, or 1 if the sign of the number
        // is negative, 0, or positive.  Throws for floating point NaN's.
        [CLSCompliant(false)]
        public static int Sign(sbyte value)
        {
            if (value < 0)
                return -1;
            else if (value > 0)
                return 1;
            else
                return 0;
        }


        // Sign function for VB.  Returns -1, 0, or 1 if the sign of the number
        // is negative, 0, or positive.  Throws for floating point NaN's.
        public static int Sign(short value)
        {
            if (value < 0)
                return -1;
            else if (value > 0)
                return 1;
            else
                return 0;
        }

        // Sign function for VB.  Returns -1, 0, or 1 if the sign of the number
        // is negative, 0, or positive.  Throws for floating point NaN's.
        public static int Sign(int value)
        {
            if (value < 0)
                return -1;
            else if (value > 0)
                return 1;
            else
                return 0;
        }

        public static int Sign(long value)
        {
            if (value < 0)
                return -1;
            else if (value > 0)
                return 1;
            else
                return 0;
        }

        public static int Sign (float value) 
        {
            if (value < 0)
                return -1;
            else if (value > 0)
                return 1;
            else if (value == 0)
                return 0;
            throw new ArithmeticException(Environment.GetResourceString("Arithmetic_NaN"));
        }

        public static int Sign(double value)
        {
            if (value < 0)
                return -1;
            else if (value > 0)
                return 1;
            else if (value == 0)
                return 0;
            throw new ArithmeticException(Environment.GetResourceString("Arithmetic_NaN"));
        }

        public static int Sign(Decimal value)
        {
            if (value < 0)
                return -1;
            else if (value > 0)
                return 1;
            else
                return 0;
        }

        public static long BigMul(int a, int b) {
            return ((long)a) * b;
        }

        public static int DivRem(int a, int b, out int result) {
            result =  a%b;
            return a/b;
        }

        public static long DivRem(long a, long b, out long result) {
            result =  a%b;
            return a/b;
        }
    }
}
