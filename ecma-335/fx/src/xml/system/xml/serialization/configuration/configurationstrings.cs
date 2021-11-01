//------------------------------------------------------------------------------
// <copyright file="ConfigurationStrings.cs" company="Microsoft Corporation">
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
// <owner current="true" primary="true">ElenaK</owner>                                                                
//------------------------------------------------------------------------------
namespace System.Xml.Serialization.Configuration
{
    using System;
    using System.Configuration;
    using System.Globalization;

    internal static class ConfigurationStrings
    {
        static string GetSectionPath(string sectionName)
        {
            return string.Format(CultureInfo.InvariantCulture, @"{0}/{1}", ConfigurationStrings.SectionGroupName, sectionName);
        }

        static internal string SchemaImporterExtensionsSectionPath
        {
            get { return ConfigurationStrings.GetSectionPath(ConfigurationStrings.SchemaImporterExtensionsSectionName); }
        }

        static internal string DateTimeSerializationSectionPath 
        {
            get { return ConfigurationStrings.GetSectionPath(ConfigurationStrings.DateTimeSerializationSectionName); }
        }

        static internal string XmlSerializerSectionPath 
        {
            get { return ConfigurationStrings.GetSectionPath(ConfigurationStrings.XmlSerializerSectionName); }
        }

        internal const string Name = "name";
        internal const string SchemaImporterExtensionsSectionName = "schemaImporterExtensions";
        internal const string DateTimeSerializationSectionName = "dateTimeSerialization";
        internal const string XmlSerializerSectionName = "xmlSerializer";
        internal const string SectionGroupName = "system.xml.serialization";
        internal const string SqlTypesSchemaImporterChar = "SqlTypesSchemaImporterChar";
        internal const string SqlTypesSchemaImporterNChar = "SqlTypesSchemaImporterNChar";
        internal const string SqlTypesSchemaImporterVarChar = "SqlTypesSchemaImporterVarChar";
        internal const string SqlTypesSchemaImporterNVarChar = "SqlTypesSchemaImporterNVarChar";
        internal const string SqlTypesSchemaImporterText = "SqlTypesSchemaImporterText";
        internal const string SqlTypesSchemaImporterNText = "SqlTypesSchemaImporterNText";
        internal const string SqlTypesSchemaImporterVarBinary = "SqlTypesSchemaImporterVarBinary";
        internal const string SqlTypesSchemaImporterBinary = "SqlTypesSchemaImporterBinary";
        internal const string SqlTypesSchemaImporterImage = "SqlTypesSchemaImporterImage";
        internal const string SqlTypesSchemaImporterDecimal = "SqlTypesSchemaImporterDecimal";
        internal const string SqlTypesSchemaImporterNumeric = "SqlTypesSchemaImporterNumeric";
        internal const string SqlTypesSchemaImporterBigInt = "SqlTypesSchemaImporterBigInt";
        internal const string SqlTypesSchemaImporterInt = "SqlTypesSchemaImporterInt";
        internal const string SqlTypesSchemaImporterSmallInt = "SqlTypesSchemaImporterSmallInt";
        internal const string SqlTypesSchemaImporterTinyInt = "SqlTypesSchemaImporterTinyInt";
        internal const string SqlTypesSchemaImporterBit = "SqlTypesSchemaImporterBit";
        internal const string SqlTypesSchemaImporterFloat = "SqlTypesSchemaImporterFloat";
        internal const string SqlTypesSchemaImporterReal = "SqlTypesSchemaImporterReal";
        internal const string SqlTypesSchemaImporterDateTime = "SqlTypesSchemaImporterDateTime";
        internal const string SqlTypesSchemaImporterSmallDateTime = "SqlTypesSchemaImporterSmallDateTime";
        internal const string SqlTypesSchemaImporterMoney = "SqlTypesSchemaImporterMoney";
        internal const string SqlTypesSchemaImporterSmallMoney = "SqlTypesSchemaImporterSmallMoney";
        internal const string SqlTypesSchemaImporterUniqueIdentifier = "SqlTypesSchemaImporterUniqueIdentifier";
        internal const string Type = "type";
        internal const string Mode = "mode";
        internal const string CheckDeserializeAdvances = "checkDeserializeAdvances";
    }
}
