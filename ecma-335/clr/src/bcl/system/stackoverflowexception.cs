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
/*=============================================================================
**
** Class: StackOverflowException
**
**
** Purpose: The exception class for stack overflow.
**
**
=============================================================================*/

namespace System {
    
	using System;
	using System.Runtime.Serialization;
[System.Runtime.InteropServices.ComVisible(true)]
    [Serializable()] public sealed class StackOverflowException : SystemException {
        public StackOverflowException() 
            : base(Environment.GetResourceString("Arg_StackOverflowException")) {
    		SetErrorCode(__HResults.COR_E_STACKOVERFLOW);
        }
    
        public StackOverflowException(String message) 
            : base(message) {
    		SetErrorCode(__HResults.COR_E_STACKOVERFLOW);
        }
    	
        public StackOverflowException(String message, Exception innerException) 
            : base(message, innerException) {
    		SetErrorCode(__HResults.COR_E_STACKOVERFLOW);
        }

        internal StackOverflowException(SerializationInfo info, StreamingContext context) : base (info, context) {
        }
		
    }
}
