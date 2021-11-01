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
** Class: IFormatProvider
**
**
** Purpose: Notes a class which knows how to return formatting information
**
**
============================================================*/
namespace System {
    
	using System;
[System.Runtime.InteropServices.ComVisible(true)]
    public interface IFormatProvider
    {
	// Interface does not need to be marked with the serializable attribute
        Object GetFormat(Type formatType);
    }
}
