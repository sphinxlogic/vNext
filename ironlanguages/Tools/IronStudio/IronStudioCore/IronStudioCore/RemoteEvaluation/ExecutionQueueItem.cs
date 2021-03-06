/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
 *
 * This source code is subject to terms and conditions of the Apache License, Version 2.0. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the Apache License, Version 2.0, please send an email to 
 * ironpy@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Apache License, Version 2.0.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Microsoft.IronStudio.RemoteEvaluation {
    abstract class ExecutionQueueItem {
        private readonly AutoResetEvent _event = new AutoResetEvent(false);

        public abstract void Process();

        public void Wait() {
            _event.WaitOne();
        }

        internal void Complete() {
            _event.Set();
        }
    }
}
