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
////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics.SymbolStore;
using System.Runtime.Remoting;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Reflection.Emit;
using System.Collections;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.IO;
using System.Globalization;
using System.Runtime.Versioning;

namespace System.Reflection 
{

    [Serializable, Flags] 
[System.Runtime.InteropServices.ComVisible(true)]
    public enum PortableExecutableKinds 
    {
        NotAPortableExecutableImage = 0x0,

        ILOnly                      = 0x1,
        
        Required32Bit               = 0x2,

        PE32Plus                    = 0x4,
        
        Unmanaged32Bit              = 0x8,
    }
    
    [Serializable] 
[System.Runtime.InteropServices.ComVisible(true)]
    public enum ImageFileMachine 
    {
        I386    = 0x014c,
            
        IA64    = 0x0200,
        
        AMD64   = 0x8664,
    }


    [Serializable()]
    [ClassInterface(ClassInterfaceType.None)]
    [ComDefaultInterface(typeof(_Module))]
[System.Runtime.InteropServices.ComVisible(true)]
    public class Module : _Module, ISerializable, ICustomAttributeProvider
    {   
        #region FCalls
        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern Type GetTypeInternal(String className, bool ignoreCase, bool throwOnError);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern IntPtr GetHINSTANCE();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern String InternalGetName();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern String InternalGetFullyQualifiedName();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern Type[] GetTypesInternal(ref StackCrawlMark stackMark);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern Assembly GetAssemblyInternal();

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern int InternalGetTypeToken(String strFullName, Module refedModule, String strRefedModuleFileName, int tkResolution);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern Type InternalLoadInMemoryTypeByName(String className);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern int InternalGetMemberRef(Module refedModule, int tr, int defToken);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern int InternalGetMemberRefFromSignature(int tr, String methodName, byte[] signature, int length);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern int _InternalGetMemberRefOfMethodInfo(int tr, IntPtr method);
        internal int InternalGetMemberRefOfMethodInfo(int tr, RuntimeMethodHandle method)
        {
            return _InternalGetMemberRefOfMethodInfo(tr, method.Value);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern int _InternalGetMemberRefOfFieldInfo(int tkType, IntPtr interfaceHandle, int tkField);
        internal int InternalGetMemberRefOfFieldInfo(int tkType, RuntimeTypeHandle declaringType, int tkField)
        {
            return _InternalGetMemberRefOfFieldInfo(tkType, declaringType.Value, tkField);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern int InternalGetTypeSpecTokenWithBytes(byte[] signature, int length);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern int nativeGetArrayMethodToken(int tkTypeSpec, String methodName, byte[] signature, int sigLength, int baseToken);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern void InternalSetFieldRVAContent(int fdToken, byte[] data, int length);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern int InternalGetStringConstant(String str);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern void InternalPreSavePEFile(int portableExecutableKind, int imageFileMachine);  

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        private extern void _InternalSavePEFile(String fileName, int entryPoint, int isExe, bool isManifestFile);
        internal void InternalSavePEFile(String fileName, MethodToken entryPoint, int isExe, bool isManifestFile)
        {
            _InternalSavePEFile(fileName, entryPoint.Token, isExe, isManifestFile);
        }

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern void InternalSetResourceCounts(int resCount);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern void InternalAddResource(
            String strName, byte[] resBytes, 
            int resByteCount, int tkFile, int attribute, 
            int portableExecutableKind, int imageFileMachine);

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern void InternalSetModuleProps(String strModuleName); 

        [MethodImplAttribute(MethodImplOptions.InternalCall)]
        internal extern bool IsResourceInternal();

        #endregion

        #region Static Constructor
        static Module()
        {
            __Filters _fltObj;
            _fltObj = new __Filters();
            FilterTypeName = new TypeFilter(_fltObj.FilterTypeName);
            FilterTypeNameIgnoreCase = new TypeFilter(_fltObj.FilterTypeNameIgnoreCase);
        }        
        #endregion

        #region Public Statics
        public static readonly TypeFilter FilterTypeName;
        
        public static readonly TypeFilter FilterTypeNameIgnoreCase;

        public MethodBase ResolveMethod(int metadataToken)
        {
            return ResolveMethod(metadataToken, null, null);
        }

        private static RuntimeTypeHandle[] ConvertToTypeHandleArray(Type[] genericArguments)
        {
            if (genericArguments == null) 
                return null;

            int size = genericArguments.Length;
            RuntimeTypeHandle[] typeHandleArgs = new RuntimeTypeHandle[size];
            for (int i = 0; i < size; i++) 
            {
                Type typeArg = genericArguments[i];
                if (typeArg == null) 
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidGenericInstArray"));
                typeArg = typeArg.UnderlyingSystemType;
                if (typeArg == null) 
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidGenericInstArray"));
                if (!(typeArg is RuntimeType)) 
                    throw new ArgumentException(Environment.GetResourceString("Argument_InvalidGenericInstArray"));
                typeHandleArgs[i] = typeArg.GetTypeHandleInternal();
            }
            return typeHandleArgs;
        }

        public byte[] ResolveSignature(int metadataToken)
        {
            MetadataToken tk = new MetadataToken(metadataToken);

            if (!MetadataImport.IsValidToken(tk))
                throw new ArgumentOutOfRangeException("metadataToken",
                    String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", tk, this)));

            if (!tk.IsMemberRef && !tk.IsMethodDef && !tk.IsTypeSpec && !tk.IsSignature && !tk.IsFieldDef)
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", tk, this)),
                                            "metadataToken");

            ConstArray signature;
            if (tk.IsMemberRef)
                signature = MetadataImport.GetMemberRefProps(metadataToken);
            else
                signature = MetadataImport.GetSignatureFromToken(metadataToken);

            byte[] sig = new byte[signature.Length];

            for (int i = 0; i < signature.Length; i++)
                sig[i] = signature[i];

            return sig;
        }

        public MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            MetadataToken tk = new MetadataToken(metadataToken);

            if (!MetadataImport.IsValidToken(tk))
                throw new ArgumentOutOfRangeException("metadataToken",
                    String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", tk, this)));

            RuntimeTypeHandle[] typeArgs = ConvertToTypeHandleArray(genericTypeArguments);
            RuntimeTypeHandle[] methodArgs = ConvertToTypeHandleArray(genericMethodArguments);

            try 
            {
                if (!tk.IsMethodDef && !tk.IsMethodSpec)
                {
                    if (!tk.IsMemberRef)
                        throw new ArgumentException("metadataToken",
                            String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethod", tk, this)));

                    unsafe
                    {
                        ConstArray sig = MetadataImport.GetMemberRefProps(tk);
                        
                        if (*(CorCallingConvention*)sig.Signature.ToPointer() == CorCallingConvention.Field)
                            throw new ArgumentException("metadataToken", 
                                String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMethod"), tk, this));
                    }
                }

                RuntimeMethodHandle methodHandle = GetModuleHandle().ResolveMethodHandle(tk, typeArgs, methodArgs);
                Type declaringType = methodHandle.GetDeclaringType().GetRuntimeType();
    
                if (declaringType.IsGenericType || declaringType.IsArray)
                {
                    MetadataToken tkDeclaringType = new MetadataToken(MetadataImport.GetParentToken(tk));
                
                    if (tk.IsMethodSpec)
                        tkDeclaringType = new MetadataToken(MetadataImport.GetParentToken(tkDeclaringType));
                
                    declaringType = ResolveType(tkDeclaringType, genericTypeArguments, genericMethodArguments);
                }

                return System.RuntimeType.GetMethodBase(declaringType.GetTypeHandleInternal(), methodHandle);    
            } 
            catch (BadImageFormatException e) 
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_BadImageFormatExceptionResolve"), e);
            }
        }

        internal FieldInfo ResolveLiteralField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            MetadataToken tk = new MetadataToken(metadataToken);

            if (!MetadataImport.IsValidToken(tk))
                throw new ArgumentOutOfRangeException("metadataToken",
                    String.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Argument_InvalidToken", tk, this)));

            int tkDeclaringType;
            string fieldName;

            fieldName = MetadataImport.GetName(tk).ToString();
            tkDeclaringType = MetadataImport.GetParentToken(tk);

            Type declaringType = ResolveType(tkDeclaringType, genericTypeArguments, genericMethodArguments);

            declaringType.GetFields();

            try
            {
                return declaringType.GetField(fieldName, 
                    BindingFlags.Static | BindingFlags.Instance | 
                    BindingFlags.Public | BindingFlags.NonPublic | 
                    BindingFlags.DeclaredOnly);
            }
            catch
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveField"), tk, this), "metadataToken");
            }
        }

        public FieldInfo ResolveField(int metadataToken)
        {
            return ResolveField(metadataToken, null, null);
        }

        public FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            MetadataToken tk = new MetadataToken(metadataToken);

            if (!MetadataImport.IsValidToken(tk))
                throw new ArgumentOutOfRangeException("metadataToken",
                    String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", tk, this)));

            RuntimeTypeHandle[] typeArgs = ConvertToTypeHandleArray(genericTypeArguments);
            RuntimeTypeHandle[] methodArgs = ConvertToTypeHandleArray(genericMethodArguments);

            try 
            {
                RuntimeFieldHandle fieldHandle = new RuntimeFieldHandle();
            
                if (!tk.IsFieldDef)
                {
                    if (!tk.IsMemberRef)
                        throw new ArgumentException("metadataToken",
                            String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveField"), tk, this));

                    unsafe 
                    {
                        ConstArray sig = MetadataImport.GetMemberRefProps(tk);
                        
                        if (*(CorCallingConvention*)sig.Signature.ToPointer() != CorCallingConvention.Field)
                            throw new ArgumentException("metadataToken",
                                String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveField"), tk, this));                            
                    }

                    fieldHandle = GetModuleHandle().ResolveFieldHandle(tk, typeArgs, methodArgs);
                }

                fieldHandle = GetModuleHandle().ResolveFieldHandle(metadataToken, typeArgs, methodArgs);
                Type declaringType = fieldHandle.GetApproxDeclaringType().GetRuntimeType();
                
                if (declaringType.IsGenericType || declaringType.IsArray)
                {
                    int tkDeclaringType = GetModuleHandle().GetMetadataImport().GetParentToken(metadataToken);
                    declaringType = ResolveType(tkDeclaringType, genericTypeArguments, genericMethodArguments);
                }

                return System.RuntimeType.GetFieldInfo(declaringType.GetTypeHandleInternal(), fieldHandle);
            }
            catch(MissingFieldException)
            {
                return ResolveLiteralField(tk, genericTypeArguments, genericMethodArguments);
            }
            catch (BadImageFormatException e) 
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_BadImageFormatExceptionResolve"), e);
            }           
        }

        public Type ResolveType(int metadataToken)
        {
            return ResolveType(metadataToken, null, null);
        }

        public Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            MetadataToken tk = new MetadataToken(metadataToken);

            if (tk.IsGlobalTypeDefToken)
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveModuleType"), tk), "metadataToken");
            
            if (!MetadataImport.IsValidToken(tk))
                throw new ArgumentOutOfRangeException("metadataToken",
                    String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", tk, this)));

            if (!tk.IsTypeDef && !tk.IsTypeSpec && !tk.IsTypeRef)
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveType"), tk, this), "metadataToken");

            RuntimeTypeHandle[] typeArgs = ConvertToTypeHandleArray(genericTypeArguments);
            RuntimeTypeHandle[] methodArgs = ConvertToTypeHandleArray(genericMethodArguments);

            try 
            {
                Type t = GetModuleHandle().ResolveTypeHandle(metadataToken, typeArgs, methodArgs).GetRuntimeType();
                    
                if (t == null)
                    throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveType"), tk, this), "metadataToken");

                return t;
            } 
            catch (BadImageFormatException e) 
            {
                throw new ArgumentException(Environment.GetResourceString("Argument_BadImageFormatExceptionResolve"), e);
            }
        }

        public MemberInfo ResolveMember(int metadataToken)
        {
            return ResolveMember(metadataToken, null, null);
        }

        public MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            MetadataToken tk = new MetadataToken(metadataToken);

            if (tk.IsProperty)
                throw new ArgumentException(Environment.GetResourceString("InvalidOperation_PropertyInfoNotAvailable"));

            if (tk.IsEvent)
                throw new ArgumentException(Environment.GetResourceString("InvalidOperation_EventInfoNotAvailable"));

            if (tk.IsMethodSpec || tk.IsMethodDef)
                return ResolveMethod(metadataToken, genericTypeArguments, genericMethodArguments);

            if (tk.IsFieldDef)
                return ResolveField(metadataToken, genericTypeArguments, genericMethodArguments);

            if (tk.IsTypeRef || tk.IsTypeDef || tk.IsTypeSpec)
                return ResolveType(metadataToken, genericTypeArguments, genericMethodArguments);

            if (tk.IsMemberRef)
            {
                if (!MetadataImport.IsValidToken(tk))
                    throw new ArgumentOutOfRangeException("metadataToken",
                        String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_InvalidToken", tk, this)));

                ConstArray sig = MetadataImport.GetMemberRefProps(tk);

                unsafe 
                {
                    if (*(CorCallingConvention*)sig.Signature.ToPointer() == CorCallingConvention.Field)
                    {
                        return ResolveField(tk, genericTypeArguments, genericMethodArguments);
                    }
                    else
                    {
                        return ResolveMethod(tk, genericTypeArguments, genericMethodArguments);
                    }
                }                
            }

            throw new ArgumentException("metadataToken",
                String.Format(CultureInfo.CurrentCulture, Environment.GetResourceString("Argument_ResolveMember", tk, this)));
        }

        public string ResolveString(int metadataToken)
        {
            MetadataToken tk = new MetadataToken(metadataToken);
            if (!tk.IsString)
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Argument_ResolveString"), metadataToken, ToString()));

            if (!MetadataImport.IsValidToken(tk))
                throw new ArgumentOutOfRangeException("metadataToken",
                    String.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Argument_InvalidToken", tk, this)));

            string str = MetadataImport.GetUserString(metadataToken);
            
            if (str == null)                
                throw new ArgumentException(
                    String.Format(CultureInfo.CurrentUICulture, Environment.GetResourceString("Argument_ResolveString"), metadataToken, ToString()));

            return str;
        }
        
        public void GetPEKind(out PortableExecutableKinds peKind, out ImageFileMachine machine)
        {
            GetModuleHandle().GetPEKind(out peKind, out machine);
        }

	public int MDStreamVersion
	{
	    get {  return GetModuleHandle().MDStreamVersion; }
	}

        #endregion

        #region Literals
        private const BindingFlags DefaultLookup = BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public;
        #endregion

        #region Data Members
        #pragma warning disable 169
        // If you add any data members, you need to update the native declaration ReflectModuleBaseObject.
        internal ArrayList          m_TypeBuilderList;
        internal ISymbolWriter      m_iSymWriter;
        internal ModuleBuilderData  m_moduleData;
        private RuntimeType         m_runtimeType;
        private IntPtr              m_pRefClass;
        internal IntPtr             m_pData;
        internal IntPtr             m_pInternalSymWriter;
        private IntPtr              m_pGlobals;
        private IntPtr              m_pFields;
        internal MethodToken        m_EntryPoint;
        #pragma warning restore 169
        #endregion

        #region Constructor
        internal Module() 
        {
            // Construct a new module.  This returns the default dynamic module.
            // 0 is defined as being a module without an entry point (ie a DLL);
            // This must throw because this dies in ToString() when constructed here...
            throw new NotSupportedException(Environment.GetResourceString(ResId.NotSupported_Constructor));
            //m_EntryPoint=new MethodToken(0);
        }
        #endregion

        #region Private Members
        private FieldInfo InternalGetField(String name, BindingFlags bindingAttr)
        {
            if (RuntimeType == null)
                return null;

            return RuntimeType.GetField(name, bindingAttr);
        }
        #endregion

        #region Internal Members
        internal virtual bool IsDynamic()
        { 
            if (this is ModuleBuilder)
                return true;

            return false; 
        }
    
        internal RuntimeType RuntimeType
        {
            get
            {
                unsafe
                {
                    if (m_runtimeType == null)
                        m_runtimeType = GetModuleHandle().GetModuleTypeHandle().GetRuntimeType() as RuntimeType;

                    return m_runtimeType;
                }
            }
        }
        #endregion

        #region Protected Virtuals
        protected virtual MethodInfo GetMethodImpl(String name,BindingFlags bindingAttr,Binder binder,
            CallingConventions callConvention, Type[] types,ParameterModifier[] modifiers)
        {
            if (RuntimeType == null)
                return null;

            if (types == null)
            {
                return RuntimeType.GetMethod(name, bindingAttr);
            }
            else
            {
                return RuntimeType.GetMethod(name, bindingAttr, binder, callConvention, types, modifiers);
            }
        }
        
        internal MetadataImport MetadataImport
        {
            get
            {
                unsafe
                {
                    return ModuleHandle.GetMetadataImport();
                }
            }
        }
        #endregion

        #region ICustomAttributeProvider Members
        public virtual Object[] GetCustomAttributes(bool inherit)
        {
            return CustomAttribute.GetCustomAttributes(this, typeof(object) as RuntimeType);
        }

        public virtual Object[] GetCustomAttributes(Type attributeType, bool inherit)
        {
            if (attributeType == null)
                throw new ArgumentNullException("attributeType");

            RuntimeType attributeRuntimeType = attributeType.UnderlyingSystemType as RuntimeType;

            if (attributeRuntimeType == null) 
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"),"attributeType");

            return CustomAttribute.GetCustomAttributes(this, attributeRuntimeType);
        }
    
        public virtual bool IsDefined (Type attributeType, bool inherit)
        {
            if (attributeType == null)
                throw new ArgumentNullException("attributeType");

            RuntimeType attributeRuntimeType = attributeType.UnderlyingSystemType as RuntimeType;

            if (attributeRuntimeType == null) 
                throw new ArgumentException(Environment.GetResourceString("Arg_MustBeType"),"caType");

            return CustomAttribute.IsDefined(this, attributeRuntimeType);
        }
        
        #endregion

        #region Public Virtuals
        [SecurityPermissionAttribute(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.SerializationFormatter)]
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }
            UnitySerializationHolder.GetUnitySerializationInfo(info, UnitySerializationHolder.ModuleUnity, this.ScopeName, GetAssemblyInternal());
        }

[System.Runtime.InteropServices.ComVisible(true)]
        public virtual Type GetType(String className, bool ignoreCase)
        {
            return GetType(className, false, ignoreCase);
        }

[System.Runtime.InteropServices.ComVisible(true)]
        public virtual Type GetType(String className) {
            return GetType(className, false, false);
        }

[System.Runtime.InteropServices.ComVisible(true)]
        public virtual Type GetType(String className, bool throwOnError, bool ignoreCase)
        {
            return GetTypeInternal(className, throwOnError, ignoreCase);
        }

        public virtual String FullyQualifiedName 
        {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get
            {
                String fullyQualifiedName = InternalGetFullyQualifiedName();
                
                if (fullyQualifiedName != null) {
                    bool checkPermission = true;
                    try {
                        Path.GetFullPathInternal(fullyQualifiedName);
                    }
                    catch(ArgumentException) {
                        checkPermission = false;
                    }
                    if (checkPermission) {
                        new FileIOPermission( FileIOPermissionAccess.PathDiscovery, fullyQualifiedName ).Demand();
                    }
                }

                return fullyQualifiedName;
            }
        }

        public virtual Type[] FindTypes(TypeFilter filter,Object filterCriteria)
        {
            StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
            Type[] c = GetTypesInternal(ref stackMark);
            int cnt = 0;
            for (int i = 0;i<c.Length;i++) {
                if (filter!=null && !filter(c[i],filterCriteria))
                    c[i] = null;
                else
                    cnt++;
            }
            if (cnt == c.Length)
                return c;
            
            Type[] ret = new Type[cnt];
            cnt=0;
            for (int i=0;i<c.Length;i++) {
                if (c[i] != null)
                    ret[cnt++] = c[i];
            }
            return ret;
        }   
        
        public virtual Type[] GetTypes()
        {
            StackCrawlMark stackMark = StackCrawlMark.LookForMyCaller;
            return GetTypesInternal(ref stackMark);
        }
        
        #endregion

        #region Public Members

        public Guid ModuleVersionId
        {
            get
            {
                unsafe 
                {
                    Guid mvid;
                    MetadataImport.GetScopeProps(out mvid);
                    return mvid;
                }
            }
        }
        
        public int MetadataToken
        {
            get
            {
                return GetModuleHandle().GetToken();
            }
        }

        public bool IsResource()
        {
            return IsResourceInternal();
        }

        public FieldInfo[] GetFields()
        {
            if (RuntimeType == null)
                return new FieldInfo[0];

            return RuntimeType.GetFields();
        }

        public FieldInfo[] GetFields(BindingFlags bindingFlags)
        {
            if (RuntimeType == null)
                return new FieldInfo[0];

            return RuntimeType.GetFields(bindingFlags);
        }

        public FieldInfo GetField(String name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return GetField(name,Module.DefaultLookup);
        }

        public FieldInfo GetField(String name, BindingFlags bindingAttr)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            return InternalGetField(name, bindingAttr);
        }

        public MethodInfo[] GetMethods()
        {
            if (RuntimeType == null)
                return new MethodInfo[0];

            return RuntimeType.GetMethods();           
        }
        
        public MethodInfo[] GetMethods(BindingFlags bindingFlags)
        {
            if (RuntimeType == null)
                return new MethodInfo[0];
        
            return RuntimeType.GetMethods(bindingFlags);           
        }
        
        public MethodInfo GetMethod(
            String name, BindingFlags bindingAttr, Binder binder, CallingConventions callConvention, Type[] types, ParameterModifier[] modifiers)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (types == null)
                throw new ArgumentNullException("types");

            for (int i =0;i < types.Length;i++)
            {
                if (types[i] == null)
                    throw new ArgumentNullException("types");
            }

            return GetMethodImpl(name,bindingAttr,binder,callConvention,types,modifiers);
            
        }
        
        public MethodInfo GetMethod(String name,Type[] types)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (types == null)
                throw new ArgumentNullException("types");

            for (int i =0;i < types.Length;i++)
            {
                if (types[i] == null)
                    throw new ArgumentNullException("types");
            }

            return GetMethodImpl(name, Module.DefaultLookup, null, CallingConventions.Any, types, null);
        }
        
        public MethodInfo GetMethod(String name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            return GetMethodImpl(name,Module.DefaultLookup,null,CallingConventions.Any,
                null,null);
        }
            
        public String ScopeName 
        { 
            get 
            { 
                return InternalGetName(); 
            } 
        }

        public String Name 
        {
            [ResourceExposure(ResourceScope.None)]
            [ResourceConsumption(ResourceScope.Machine, ResourceScope.Machine)]
            get 
            {
                String s = InternalGetFullyQualifiedName();
                int i = s.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                if (i == -1)
                    return s;

                return new String(s.ToCharArray(),i+1,s.Length-i-1);
            }
        }
    
        public override String ToString()
        {
            return ScopeName;
        }
    
        public Assembly Assembly 
        { 
            get 
            { 
                return GetAssemblyInternal(); 
            } 
        }
        public unsafe ModuleHandle ModuleHandle 
        {
            get 
            {
                //TODO: Add this back when the GetPEKind is removed from ModuleHandle
                //if (Assembly.ReflectionOnly)
                //    throw new InvalidOperationException(Environment.GetResourceString("InvalidOperation_NotAllowedInReflectionOnly"));
                return new ModuleHandle((void*)m_pData);
            }
        }

        internal unsafe ModuleHandle GetModuleHandle()
        {
            return new ModuleHandle((void*)m_pData);
        }
        #endregion

        void _Module.GetTypeInfoCount(out uint pcTInfo)
        {
            throw new NotImplementedException();
        }

        void _Module.GetTypeInfo(uint iTInfo, uint lcid, IntPtr ppTInfo)
        {
            throw new NotImplementedException();
        }

        void _Module.GetIDsOfNames([In] ref Guid riid, IntPtr rgszNames, uint cNames, uint lcid, IntPtr rgDispId)
        {
            throw new NotImplementedException();
        }

        void _Module.Invoke(uint dispIdMember, [In] ref Guid riid, uint lcid, short wFlags, IntPtr pDispParams, IntPtr pVarResult, IntPtr pExcepInfo, IntPtr puArgErr)
        {
            throw new NotImplementedException();
        }
   }
}
