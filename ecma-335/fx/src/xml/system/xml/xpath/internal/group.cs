//------------------------------------------------------------------------------
// <copyright file="Group.cs" company="Microsoft">
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

namespace MS.Internal.Xml.XPath {
    using System;
    using System.Xml;
    using System.Xml.XPath;
    using System.Diagnostics;
    using System.Globalization;
    internal class Group : AstNode {
        private AstNode groupNode;

        public Group(AstNode groupNode) {
            this.groupNode = groupNode;
        }
        public override AstType         Type       { get { return AstType.Group;           } }
        public override XPathResultType ReturnType { get { return XPathResultType.NodeSet; } }

        public AstNode GroupNode { get { return groupNode; } }
    }
}


