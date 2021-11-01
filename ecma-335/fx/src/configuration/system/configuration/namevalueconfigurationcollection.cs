//------------------------------------------------------------------------------
// <copyright file="NameValueConfigurationCollection.cs" company="Microsoft">
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

/*
 * ImmutableCollections
 *
 * Copyright (c) 2004 Microsoft Corporation
 */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Net;
using System.Configuration;

//
// This file contains configuration collections that are used by multiple sections
//
namespace System.Configuration
{

    [ConfigurationCollection(typeof(NameValueConfigurationElement))]
    public sealed class NameValueConfigurationCollection : ConfigurationElementCollection
    {
        private static ConfigurationPropertyCollection _properties;

        static NameValueConfigurationCollection() {
            // Property initialization
            _properties = new ConfigurationPropertyCollection();
        }

        protected internal override ConfigurationPropertyCollection Properties {
            get {
                return _properties;
            }
        }
        //
        // Constructor
        //
        public NameValueConfigurationCollection() 
        {
        }

        //
        // Accessors
        //

        public new NameValueConfigurationElement this[string name]
        {
            get
            {
                return (NameValueConfigurationElement) BaseGet(name);
            }
            set
            {
                int index = -1; // append by default
                NameValueConfigurationElement tempElement = (NameValueConfigurationElement) BaseGet(name);
                if (tempElement != null)
                {
                    index = BaseIndexOf(tempElement);
                    BaseRemoveAt(index);
                }
                BaseAdd(index, value);
            }
        }

        public String[] AllKeys
        {
            get
            {
                return StringUtil.ObjectArrayToStringArray(BaseGetAllKeys());
            }
        }

        //
        // Methods
        //

        public void Add(NameValueConfigurationElement nameValue) 
        {
            BaseAdd(nameValue);
        }
        public void Remove(NameValueConfigurationElement nameValue) 
        {
            if (BaseIndexOf(nameValue) >= 0)
                BaseRemove(nameValue.Name);
        }
        public void Remove(string name)
        {
            BaseRemove(name);
        }
        public void Clear()
        {
            BaseClear();
        }
        protected override ConfigurationElement CreateNewElement()
        {
            return new NameValueConfigurationElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((NameValueConfigurationElement)element).Name;
        }
    }
}
