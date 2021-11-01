﻿/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Apache License, Version 2.0, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 *
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Scripting.Runtime;

namespace IronPython.Runtime.Exceptions {
    #region Generated AttributeErrorException

    // *** BEGIN GENERATED CODE ***
    // generated by function: gen_one_exception_specialized from: generate_exceptions.py


    [Serializable]
    public class AttributeErrorException : MissingMemberException, IPythonAwareException {
        private object _pyExceptionObject;
        private List<DynamicStackFrame> _frames;
        private TraceBack _traceback;

        public AttributeErrorException() : base() { }
        public AttributeErrorException(string msg) : base(msg) { }
        public AttributeErrorException(string message, Exception innerException)
            : base(message, innerException) {
        }
#if !SILVERLIGHT // SerializationInfo
        protected AttributeErrorException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase")]
        public override void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("frames", _frames);
            info.AddValue("traceback", _traceback);
            base.GetObjectData(info, context);
        }
#endif

        object IPythonAwareException.PythonException {
            get { return _pyExceptionObject; }
            set { _pyExceptionObject = value; }
        }

        List<DynamicStackFrame> IPythonAwareException.Frames {
            get { return _frames; }
            set { _frames = value; }
        }

        TraceBack IPythonAwareException.TraceBack {
            get { return _traceback; }
            set { _traceback = value; }
        }
    }


    // *** END GENERATED CODE ***

    #endregion
}
