/* ****************************************************************************
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
using System.Linq;
using System.Text;

#if CLR2
using Microsoft.Scripting.Math;
#else
using System.Numerics;
#endif


namespace IronPython.Runtime {
    public interface IBufferProtocol {
        Bytes GetItem(int index);
        void SetItem(int index, object value);
        void SetSlice(Slice index, object value);

        int ItemCount {
            get;
        }

        string Format {
            get;
        }

        BigInteger ItemSize {
            get;
        }

        BigInteger NumberDimensions {
            get;
        }

        bool ReadOnly {
            get;
        }

        IList<BigInteger> GetShape(int start, int ?end);

        PythonTuple Strides {
            get;
        }

        object SubOffsets {
            get;
        }

        Bytes ToBytes(int start, int? end);

        List ToList(int start, int? end);
    }
}
