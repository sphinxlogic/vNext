//------------------------------------------------------------------------------
// <copyright file="CodeDelegateCreateExpression.cs" company="Microsoft">
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
    using System.Runtime.InteropServices;

    /// <devdoc>
    ///    <para>
    ///       Represents a delegate creation expression.
    ///    </para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
        Serializable,
    ]
    public class CodeDelegateCreateExpression : CodeExpression {
        private CodeTypeReference delegateType;
        private CodeExpression targetObject;
        private string methodName;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeDelegateCreateExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeDelegateCreateExpression() {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeDelegateCreateExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeDelegateCreateExpression(CodeTypeReference delegateType, CodeExpression targetObject, string methodName) {
            this.delegateType = delegateType;
            this.targetObject = targetObject;
            this.methodName = methodName;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the delegate type.
        ///    </para>
        /// </devdoc>
        public CodeTypeReference DelegateType {
            get {
                if (delegateType == null) {
                    delegateType = new CodeTypeReference("");
                }
                return delegateType;
            }
            set {
                delegateType = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the target object.
        ///    </para>
        /// </devdoc>
        public CodeExpression TargetObject {
            get {
                return targetObject;
            }
            set {
                targetObject = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets the method name.
        ///    </para>
        /// </devdoc>
        public string MethodName {
            get {
                return (methodName == null) ? string.Empty : methodName;
            }
            set {
                methodName = value;
            }
        }
    }
}
