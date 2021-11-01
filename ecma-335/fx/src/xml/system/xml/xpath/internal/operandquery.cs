//------------------------------------------------------------------------------
// <copyright file="OperandQuery.cs" company="Microsoft">
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
    using System.Collections;

    internal sealed class OperandQuery : ValueQuery {
        internal object val;

        public OperandQuery(object val) {
            this.val = val;
        }

        public override object Evaluate(XPathNodeIterator nodeIterator) {
            return val;
        }
        public override XPathResultType StaticType { get { return GetXPathType(val); } }
        public override XPathNodeIterator Clone() { return this; }

        public override void PrintQuery(XmlWriter w) {
            w.WriteStartElement(this.GetType().Name);
            w.WriteAttributeString("value", val.ToString());
            w.WriteEndElement();
        }
    }
}
