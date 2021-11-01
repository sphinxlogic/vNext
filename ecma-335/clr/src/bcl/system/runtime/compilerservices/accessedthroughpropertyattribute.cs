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
//------------------------------------------------------------------------------
//------------------------------------------------------------------------------
namespace System.Runtime.CompilerServices 
{
	using System;

	[AttributeUsage(AttributeTargets.Field)]
[System.Runtime.InteropServices.ComVisible(true)]
	public sealed class AccessedThroughPropertyAttribute : Attribute
	{
		private readonly string propertyName;

		public AccessedThroughPropertyAttribute(string propertyName)
		{
			this.propertyName = propertyName;
		}

		public string PropertyName 
		{
			get 
			{
				return propertyName;
			}
		}
	}
}

