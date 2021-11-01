//------------------------------------------------------------------------------
// <copyright file="XmlReservedNS.cs" company="Microsoft">
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

namespace System.Xml {

    /// <summary>
    /// This class defines a set of common XML namespaces for sharing across multiple source files.
    /// </summary>
    internal abstract class XmlReservedNs {
        internal const string NsDataType        = "urn:schemas-microsoft-com:datatypes";
        internal const string NsDataTypeAlias   = "uuid:C2F41010-65B3-11D1-A29F-00AA00C14882";
        internal const string NsDataTypeOld     = "urn:uuid:C2F41010-65B3-11D1-A29F-00AA00C14882/";
        internal const string NsMsxsl           = "urn:schemas-microsoft-com:xslt";
        internal const string NsXdr             = "urn:schemas-microsoft-com:xml-data";
        internal const string NsXslDebug        = "urn:schemas-microsoft-com:xslt-debug";
        internal const string NsXdrAlias        = "uuid:BDC6E3F0-6DA3-11D1-A2A3-00AA00C14882";
        internal const string NsWdXsl           = "http://www.w3.org/TR/WD-xsl";
        internal const string NsXml             = "http://www.w3.org/XML/1998/namespace";
        internal const string NsXmlNs           = "http://www.w3.org/2000/xmlns/";
        internal const string NsXs              = "http://www.w3.org/2001/XMLSchema";
        internal const string NsXsd             = "http://www.w3.org/2001/XMLSchema-datatypes";
        internal const string NsXsi             = "http://www.w3.org/2001/XMLSchema-instance";
        internal const string NsXslt            = "http://www.w3.org/1999/XSL/Transform";
        internal const string NsExsltCommon     = "http://exslt.org/common";
        internal const string NsExsltDates      = "http://exslt.org/dates-and-times";
        internal const string NsExsltMath       = "http://exslt.org/math";
        internal const string NsExsltRegExps    = "http://exslt.org/regular-expressions";
        internal const string NsExsltSets       = "http://exslt.org/sets";
        internal const string NsExsltStrings    = "http://exslt.org/strings";
        internal const string NsXQueryFunc      = "http://www.w3.org/2003/11/xpath-functions";
        internal const string NsXQueryDataType  = "http://www.w3.org/2003/11/xpath-datatypes";
        internal const string NsCollationBase   = "http://collations.microsoft.com";
        internal const string NsCollCodePoint   = "http://www.w3.org/2004/10/xpath-functions/collation/codepoint";
        internal const string NsXsltInternal    = "http://schemas.microsoft.com/framework/2003/xml/xslt/internal";
    };
}
