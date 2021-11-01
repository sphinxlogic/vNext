//------------------------------------------------------------------------------
// <copyright file="TimeSpanValidatorAttribute.cs" company="Microsoft">
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

using System;
using System.Configuration.Internal;
using System.Collections;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using System.Xml;
using System.Globalization;
using System.ComponentModel;
using System.Security;
using System.Text;

namespace System.Configuration {

    [AttributeUsage(AttributeTargets.Property)]
    public sealed class TimeSpanValidatorAttribute : ConfigurationValidatorAttribute {

        private TimeSpan _min = TimeSpan.MinValue;
        private TimeSpan _max = TimeSpan.MaxValue;
        private bool _excludeRange = false;

        public const string TimeSpanMinValue = "-10675199.02:48:05.4775808";
        public const string TimeSpanMaxValue = "10675199.02:48:05.4775807";

        public TimeSpanValidatorAttribute() {
        }

        public override ConfigurationValidatorBase ValidatorInstance {
            get {
                return new TimeSpanValidator(_min, _max, _excludeRange);
            }
        }

        public TimeSpan MinValue {
            get {
                return _min;
            }
        }

        public TimeSpan MaxValue {
            get {
                return _max;
            }
        }

        public string MinValueString {
            get {
                return _min.ToString();
            }
            set {
                TimeSpan timeValue = TimeSpan.Parse(value);

                if (_max < timeValue) {
                    throw new ArgumentOutOfRangeException("value", SR.GetString(SR.Validator_min_greater_than_max));
                }

                _min = timeValue;
            }
        }

        public string MaxValueString {
            get {
                return _max.ToString();
            }
            set {
                TimeSpan timeValue = TimeSpan.Parse(value);

                if (_min > timeValue) {
                    throw new ArgumentOutOfRangeException("value", SR.GetString(SR.Validator_min_greater_than_max));
                }

                _max = timeValue;
            }
        }

        public bool ExcludeRange {
            get {
                return _excludeRange;
            }
            set {
                _excludeRange = value;
            }
        }
    }
}
