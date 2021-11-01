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
** File:    IMessage.cs
**
**
** Purpose: Defines the message object interface
**
**
===========================================================*/
namespace System.Runtime.Remoting.Messaging {
    using System;
    using IDictionary = System.Collections.IDictionary;
    using System.Security.Permissions;
    
[System.Runtime.InteropServices.ComVisible(true)]
    public interface IMessage
    {
        IDictionary Properties     
	{
	    [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.Infrastructure)] 		
	    get;
	}
    }

}
