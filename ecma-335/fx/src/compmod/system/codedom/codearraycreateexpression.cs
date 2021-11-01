//------------------------------------------------------------------------------
// <copyright file="CodeArrayCreateExpression.cs" company="Microsoft">
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
    ///    <para> Represents
    ///       an expression that creates an array.</para>
    /// </devdoc>
    [
        ClassInterface(ClassInterfaceType.AutoDispatch),
        ComVisible(true),
        Serializable,
    ]
    public class CodeArrayCreateExpression : CodeExpression {
        private CodeTypeReference createType;
        private CodeExpressionCollection initializers = new CodeExpressionCollection();
        private CodeExpression sizeExpression;
        private int size = 0;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeArrayCreateExpression'/>.
        ///    </para>
        /// </devdoc>
        public CodeArrayCreateExpression() {
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeArrayCreateExpression'/> with the specified
        ///       array type and initializers.
        ///    </para>
        /// </devdoc>
        public CodeArrayCreateExpression(CodeTypeReference createType, params CodeExpression[] initializers) {
            this.createType = createType;
            this.initializers.AddRange(initializers);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArrayCreateExpression(string createType, params CodeExpression[] initializers) {
            this.createType = new CodeTypeReference(createType);
            this.initializers.AddRange(initializers);
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArrayCreateExpression(Type createType, params CodeExpression[] initializers) {
            this.createType = new CodeTypeReference(createType);
            this.initializers.AddRange(initializers);
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeArrayCreateExpression'/>. with the specified array
        ///       type and size.
        ///    </para>
        /// </devdoc>
        public CodeArrayCreateExpression(CodeTypeReference createType, int size) {
            this.createType = createType;
            this.size = size;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArrayCreateExpression(string createType, int size) {
            this.createType = new CodeTypeReference(createType);
            this.size = size;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArrayCreateExpression(Type createType, int size) {
            this.createType = new CodeTypeReference(createType);
            this.size = size;
        }

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of <see cref='System.CodeDom.CodeArrayCreateExpression'/>. with the specified array
        ///       type and size.
        ///    </para>
        /// </devdoc>
        public CodeArrayCreateExpression(CodeTypeReference createType, CodeExpression size) {
            this.createType = createType;
            this.sizeExpression = size;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArrayCreateExpression(string createType, CodeExpression size) {
            this.createType = new CodeTypeReference(createType);
            this.sizeExpression = size;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public CodeArrayCreateExpression(Type createType, CodeExpression size) {
            this.createType = new CodeTypeReference(createType);
            this.sizeExpression = size;
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the type of the array to create.
        ///    </para>
        /// </devdoc>
        public CodeTypeReference CreateType {
            get {
                if (createType == null) {
                    createType = new CodeTypeReference("");
                }
                return createType;
            }
            set {
                createType = value;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the initializers to initialize the array with.
        ///    </para>
        /// </devdoc>
        public CodeExpressionCollection Initializers {
            get {
                return initializers;
            }
        }

        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the size of the array.
        ///    </para>
        /// </devdoc>
        public int Size {
            get {
                return size;
            }
            set {
                size = value;
            }
        }

        /// <devdoc>
        ///    <para>Gets or sets the size of the array.</para>
        /// </devdoc>
        public CodeExpression SizeExpression {
            get {
                return sizeExpression;
            }
            set {
                sizeExpression = value;
            }
        }
    }
}
