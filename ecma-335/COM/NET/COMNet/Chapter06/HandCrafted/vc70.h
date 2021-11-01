
#pragma warning( disable: 4049 )  /* more than 64k source lines */

/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 6.00.0347 */
/* at Mon Oct 28 11:49:11 2002
 */
/* Compiler settings for vc70.idl:
    Os, W1, Zp8, env=Win32 (32b run)
    protocol : dce , ms_ext, c_ext
    error checks: allocation ref bounds_check enum stub_data 
    VC __declspec() decoration level: 
         __declspec(uuid()), __declspec(selectany), __declspec(novtable)
         DECLSPEC_UUID(), MIDL_INTERFACE()
*/
//@@MIDL_FILE_HEADING(  )


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 440
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __vc70_h__
#define __vc70_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __ISimplest_FWD_DEFINED__
#define __ISimplest_FWD_DEFINED__
typedef interface ISimplest ISimplest;
#endif 	/* __ISimplest_FWD_DEFINED__ */


#ifndef __CObject1_FWD_DEFINED__
#define __CObject1_FWD_DEFINED__

#ifdef __cplusplus
typedef class CObject1 CObject1;
#else
typedef struct CObject1 CObject1;
#endif /* __cplusplus */

#endif 	/* __CObject1_FWD_DEFINED__ */


/* header files for imported files */
#include "prsht.h"
#include "mshtml.h"
#include "mshtmhst.h"
#include "exdisp.h"
#include "objsafe.h"

#ifdef __cplusplus
extern "C"{
#endif 

void * __RPC_USER MIDL_user_allocate(size_t);
void __RPC_USER MIDL_user_free( void * ); 

#ifndef __ISimplest_INTERFACE_DEFINED__
#define __ISimplest_INTERFACE_DEFINED__

/* interface ISimplest */
/* [unique][helpstring][dual][uuid][object] */ 


EXTERN_C const IID IID_ISimplest;

#if defined(__cplusplus) && !defined(CINTERFACE)
    
    MIDL_INTERFACE("103FF9D9-8BC9-4ea8-8CD4-C1E627D04358")
    ISimplest : public IDispatch
    {
    public:
        virtual HRESULT STDMETHODCALLTYPE Square( 
            /* [in] */ short val,
            /* [retval][out] */ long *pResult) = 0;
        
    };
    
#else 	/* C style interface */

    typedef struct ISimplestVtbl
    {
        BEGIN_INTERFACE
        
        HRESULT ( STDMETHODCALLTYPE *QueryInterface )( 
            ISimplest * This,
            /* [in] */ REFIID riid,
            /* [iid_is][out] */ void **ppvObject);
        
        ULONG ( STDMETHODCALLTYPE *AddRef )( 
            ISimplest * This);
        
        ULONG ( STDMETHODCALLTYPE *Release )( 
            ISimplest * This);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfoCount )( 
            ISimplest * This,
            /* [out] */ UINT *pctinfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetTypeInfo )( 
            ISimplest * This,
            /* [in] */ UINT iTInfo,
            /* [in] */ LCID lcid,
            /* [out] */ ITypeInfo **ppTInfo);
        
        HRESULT ( STDMETHODCALLTYPE *GetIDsOfNames )( 
            ISimplest * This,
            /* [in] */ REFIID riid,
            /* [size_is][in] */ LPOLESTR *rgszNames,
            /* [in] */ UINT cNames,
            /* [in] */ LCID lcid,
            /* [size_is][out] */ DISPID *rgDispId);
        
        /* [local] */ HRESULT ( STDMETHODCALLTYPE *Invoke )( 
            ISimplest * This,
            /* [in] */ DISPID dispIdMember,
            /* [in] */ REFIID riid,
            /* [in] */ LCID lcid,
            /* [in] */ WORD wFlags,
            /* [out][in] */ DISPPARAMS *pDispParams,
            /* [out] */ VARIANT *pVarResult,
            /* [out] */ EXCEPINFO *pExcepInfo,
            /* [out] */ UINT *puArgErr);
        
        HRESULT ( STDMETHODCALLTYPE *Square )( 
            ISimplest * This,
            /* [in] */ short val,
            /* [retval][out] */ long *pResult);
        
        END_INTERFACE
    } ISimplestVtbl;

    interface ISimplest
    {
        CONST_VTBL struct ISimplestVtbl *lpVtbl;
    };

    

#ifdef COBJMACROS


#define ISimplest_QueryInterface(This,riid,ppvObject)	\
    (This)->lpVtbl -> QueryInterface(This,riid,ppvObject)

#define ISimplest_AddRef(This)	\
    (This)->lpVtbl -> AddRef(This)

#define ISimplest_Release(This)	\
    (This)->lpVtbl -> Release(This)


#define ISimplest_GetTypeInfoCount(This,pctinfo)	\
    (This)->lpVtbl -> GetTypeInfoCount(This,pctinfo)

#define ISimplest_GetTypeInfo(This,iTInfo,lcid,ppTInfo)	\
    (This)->lpVtbl -> GetTypeInfo(This,iTInfo,lcid,ppTInfo)

#define ISimplest_GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)	\
    (This)->lpVtbl -> GetIDsOfNames(This,riid,rgszNames,cNames,lcid,rgDispId)

#define ISimplest_Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)	\
    (This)->lpVtbl -> Invoke(This,dispIdMember,riid,lcid,wFlags,pDispParams,pVarResult,pExcepInfo,puArgErr)


#define ISimplest_Square(This,val,pResult)	\
    (This)->lpVtbl -> Square(This,val,pResult)

#endif /* COBJMACROS */


#endif 	/* C style interface */



HRESULT STDMETHODCALLTYPE ISimplest_Square_Proxy( 
    ISimplest * This,
    /* [in] */ short val,
    /* [retval][out] */ long *pResult);


void __RPC_STUB ISimplest_Square_Stub(
    IRpcStubBuffer *This,
    IRpcChannelBuffer *_pRpcChannelBuffer,
    PRPC_MESSAGE _pRpcMessage,
    DWORD *_pdwStubPhase);



#endif 	/* __ISimplest_INTERFACE_DEFINED__ */



#ifndef __MyServer_LIBRARY_DEFINED__
#define __MyServer_LIBRARY_DEFINED__

/* library MyServer */
/* [uuid][helpstring][version] */ 


EXTERN_C const IID LIBID_MyServer;

EXTERN_C const CLSID CLSID_CObject1;

#ifdef __cplusplus

class DECLSPEC_UUID("15615078-523C-43A0-BE6F-651E78A89213")
CObject1;
#endif
#endif /* __MyServer_LIBRARY_DEFINED__ */

/* Additional Prototypes for ALL interfaces */

/* end of Additional Prototypes */

#ifdef __cplusplus
}
#endif

#endif


