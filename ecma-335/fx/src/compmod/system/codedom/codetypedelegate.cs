//------------------------------------------------------------------------------
// <copyright file="CodeTypeDelegate.cs" company="Microsoft">
//     
//      Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//     
//      The use and distribution terms for this software are contained in the file
//      named license.txt, which can be found in the root of this distribution.
//      By using this software in any fashion, you are agreeing to be bound by the
//      terms of this license.
//     
//      You must not remove this notice, or any other, from this software.
//     
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.CodeDom {

    using System.Diagnostics;
    using System;
    using Microsoft.Win32;
    using System.Collections;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    /// <devdoc>
    ///    <para>
    ///       Represents a class or nested class.
    ///    </para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
        Serializable,
    ]
    public class CodeTypeDelegate : CodeTypeDeclaration {
        private CodeParameterDeclarationExpressionCollection parameters = new CodeParameterDeclarationExpressionCollection();
        private CodeTypeReference returnType;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeTypeDelegate'/>.
        ///    </para>
        /// </devdoc>
        public CodeTypeDelegate() {
            TypeAttributes &= ~TypeAttributes.ClassSemanticsMask;
            TypeAttributes |= TypeAttributes.Class;
            BaseTypes.Clear();
            BaseTypes.Add(new CodeTypeReference("System.Delegate"));
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeTypeDelegate'/>.
        ///    </para>
        /// </devdoc>
        public CodeTypeDelegate(string name) : this() {
            Name = name;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the return type of the delegate.
        ///    </para>
        /// </devdoc>
        public CodeTypeReference ReturnType {
            get {
                if (returnType == null) {
                    returnType = new CodeTypeReference("");
                }
                return returnType;
            }
            set {
                returnType = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       The parameters of the delegate.
        ///    </para>
        /// </devdoc>
        public CodeParameterDeclarationExpressionCollection Parameters {
            get {
                return parameters;
            }
        }
    }
}
