// ==++==
// 
//   
//    Copyright (c) 2006 Microsoft Corporation.  All rights reserved.
//   
//    The use and distribution terms for this software are contained in the file
//    named license.txt, which can be found in the root of this distribution.
//    By using this software in any fashion, you are agreeing to be bound by the
//    terms of this license.
//   
//    You must not remove this notice, or any other, from this software.
//   
// 
// ==--==
// SecurityPermission.cs
//

namespace System.Security.Permissions
{
    using System;
    using System.IO;
    using System.Security.Util;
    using System.Text;
    using System.Threading;
    using System.Runtime.Remoting;
    using System.Security;
    using System.Runtime.Serialization;
    using System.Reflection;
    using System.Globalization;

    [Flags, Serializable]
[System.Runtime.InteropServices.ComVisible(true)]
    public enum SecurityPermissionFlag
    {
        NoFlags = 0x00,
        /* The following enum value is used in the EE (ASSERT_PERMISSION in security.cpp)
         * Should this value change, make corresponding changes there
         */ 
        Assertion = 0x01,
        UnmanagedCode = 0x02,       // Update vm\Security.h if you change this !
        SkipVerification = 0x04,    // Update vm\Security.h if you change this !
        Execution = 0x08,
        ControlThread = 0x10,
        ControlEvidence = 0x20,
        ControlPolicy = 0x40,
        SerializationFormatter = 0x80,
        ControlDomainPolicy = 0x100,
        ControlPrincipal = 0x200,
        ControlAppDomain = 0x400,
        RemotingConfiguration = 0x800,
        Infrastructure = 0x1000,
        BindingRedirects = 0x2000,
        AllFlags = 0x3fff,
    }

[System.Runtime.InteropServices.ComVisible(true)]
    [Serializable()] sealed public class SecurityPermission 
           : CodeAccessPermission, IUnrestrictedPermission, IBuiltInPermission
    {
        private SecurityPermissionFlag m_flags;
        
        //
        // Public Constructors
        //
    
        public SecurityPermission(PermissionState state)
        {
            if (state == PermissionState.Unrestricted)
            {
                SetUnrestricted( true );
            }
            else if (state == PermissionState.None)
            {
                SetUnrestricted( false );
                Reset();
            }
            else
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_InvalidPermissionState"));
            }
        }
        
        
        // SecurityPermission
        //
        public SecurityPermission(SecurityPermissionFlag flag)
        {
            VerifyAccess(flag);
            
            SetUnrestricted(false);
            m_flags = flag;
        }
    
    
        //------------------------------------------------------
        //
        // PRIVATE AND PROTECTED MODIFIERS 
        //
        //------------------------------------------------------
        
        
        private void SetUnrestricted(bool unrestricted)
        {
            if (unrestricted)
            {
                m_flags = SecurityPermissionFlag.AllFlags;
            }
        }
    
        private void Reset()
        {
            m_flags = SecurityPermissionFlag.NoFlags;
        }
        
        
        public SecurityPermissionFlag Flags
        {
            set
            {
                VerifyAccess(value);
            
                m_flags = value;
            }
            
            get
            {
                return m_flags;
            }
        }
        
        //
        // CodeAccessPermission methods
        // 
        
       /*
         * IPermission interface implementation
         */
         
        public override bool IsSubsetOf(IPermission target)
        {
            if (target == null)
            {
                return m_flags == 0;
            }
        
            SecurityPermission operand = target as SecurityPermission;
            if (operand != null)
            {
                return (((int)this.m_flags) & ~((int)operand.m_flags)) == 0;
            }
            else
            {
                throw new 
                    ArgumentException(
                                    String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_WrongType"), this.GetType().FullName)
                                     );
            }

        }
        
        public override IPermission Union(IPermission target) {
            if (target == null) return(this.Copy());
            if (!VerifyType(target)) {
                throw new 
                    ArgumentException(
                                    String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_WrongType"), this.GetType().FullName)
                                     );
            }
            SecurityPermission sp_target = (SecurityPermission) target;
            if (sp_target.IsUnrestricted() || IsUnrestricted()) {
                return(new SecurityPermission(PermissionState.Unrestricted));
            }
            SecurityPermissionFlag flag_union = (SecurityPermissionFlag)(m_flags | sp_target.m_flags);
            return(new SecurityPermission(flag_union));
        }
    
        public override IPermission Intersect(IPermission target)
        {
            if (target == null)
                return null;
            else if (!VerifyType(target))
            {
                throw new 
                    ArgumentException(
                                    String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_WrongType"), this.GetType().FullName)
                                     );
            }
            
            SecurityPermission operand = (SecurityPermission)target;
            SecurityPermissionFlag isectFlags = SecurityPermissionFlag.NoFlags;
           
            if (operand.IsUnrestricted())
            {
                if (this.IsUnrestricted())
                    return new SecurityPermission(PermissionState.Unrestricted);
                else
                    isectFlags = (SecurityPermissionFlag)this.m_flags;
            }
            else if (this.IsUnrestricted())
            {
                isectFlags = (SecurityPermissionFlag)operand.m_flags;
            }
            else
            {
                isectFlags = (SecurityPermissionFlag)m_flags & (SecurityPermissionFlag)operand.m_flags;
            }
            
            if (isectFlags == 0)
                return null;
            else
                return new SecurityPermission(isectFlags);
        }
    
        public override IPermission Copy()
        {
            if (IsUnrestricted())
                return new SecurityPermission(PermissionState.Unrestricted);
            else
                return new SecurityPermission((SecurityPermissionFlag)m_flags);
        }
    
        public bool IsUnrestricted()
        {
            return m_flags == SecurityPermissionFlag.AllFlags;
        }
        
        private
        void VerifyAccess(SecurityPermissionFlag type)
        {
            if ((type & ~SecurityPermissionFlag.AllFlags) != 0)
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Arg_EnumIllegalVal"), (int)type));
        }
        
    
        //------------------------------------------------------
        //
        // PUBLIC ENCODING METHODS
        //
        //------------------------------------------------------
        
        private const String _strHeaderAssertion  = "Assertion";
        private const String _strHeaderUnmanagedCode = "UnmanagedCode";
        private const String _strHeaderExecution = "Execution";
        private const String _strHeaderSkipVerification = "SkipVerification";
        private const String _strHeaderControlThread = "ControlThread";
        private const String _strHeaderControlEvidence = "ControlEvidence";
        private const String _strHeaderControlPolicy = "ControlPolicy";
        private const String _strHeaderSerializationFormatter = "SerializationFormatter";
        private const String _strHeaderControlDomainPolicy = "ControlDomainPolicy";
        private const String _strHeaderControlPrincipal = "ControlPrincipal";
        private const String _strHeaderControlAppDomain = "ControlAppDomain";
    
        public override SecurityElement ToXml()
        {
            SecurityElement esd = CodeAccessPermission.CreatePermissionElement( this, "System.Security.Permissions.SecurityPermission" );
            if (!IsUnrestricted())
            {
                esd.AddAttribute( "Flags", XMLUtil.BitFieldEnumToString( typeof( SecurityPermissionFlag ), m_flags ) );
            }
            else
            {
                esd.AddAttribute( "Unrestricted", "true" );
            }
            return esd;
        }
    
        public override void FromXml(SecurityElement esd)
        {
            CodeAccessPermission.ValidateElement( esd, this );
            if (XMLUtil.IsUnrestricted( esd ))
            {
                m_flags = SecurityPermissionFlag.AllFlags;
                return;
            }
           
            Reset () ;
            SetUnrestricted (false) ;
    
            String flags = esd.Attribute( "Flags" );
    
            if (flags != null)
                m_flags = (SecurityPermissionFlag)Enum.Parse( typeof( SecurityPermissionFlag ), flags );
        }
       
        //
        // Object Overrides
        //
        

        /// <internalonly/>
        int IBuiltInPermission.GetTokenIndex()
        {
            return SecurityPermission.GetTokenIndex();
        }

        internal static int GetTokenIndex()
        {
            return BuiltInPermissionIndex.SecurityPermissionIndex;
        }

        // This can be used as a place-holder for SkipVerification permission
        [SecurityPermission(SecurityAction.LinkDemand, SkipVerification = true)]
        static internal void MethodWithSkipVerificationLinkDemand() {}
    }


}
