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
** File:    ContextBoundObject.cs
**       
**
** Purpose: Defines the root type for all context bound types
**          
**
===========================================================*/
namespace System {   
    
    using System;
    using System.Security.Permissions;
    [Serializable()]
[System.Runtime.InteropServices.ComVisible(true)]
    public abstract class ContextBoundObject : MarshalByRefObject
    {
    }
}
