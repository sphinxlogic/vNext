﻿//--
// <copyright file="BoogieAsmLinkVerb.cs" company="Microsoft Corporation">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
//--

namespace NuBuild
{
    using System;
    using System.Collections.Generic;

    internal class BoogieAsmLinkVerb
            : BoogieAsmWorkerBase, IAsmProducer
    {
        public BoogieAsmLinkVerb(IContextGeneratingVerb context, BuildObject input)
            : base(context, input)
        {
        }

        public BuildObject getAsmFile()
        {
            return basmInput.makeOutputObject(MasmVerb.MASM_EXTN);
        }

        public override BuildObject outputFile()
        {
            return this.getAsmFile();
        }

        public override IEnumerable<IVerb> getVerbs()
        {
            return getVerifyishVerbs();
        }

        protected override int getVersion()
        {
            return 23;
        }

        protected override string getAction()
        {
            return "-link";
        }

        protected override bool outFlagWorks()
        {
            return false;
        }

        protected override bool includeAllImps()
        {
            return true;
        }
    }
}
