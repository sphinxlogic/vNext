
//

//

//

//

//

//

//

//
//--  

//

//
#define PHY_INITIATE_BUS_RESET              0x40        

///////////////////////////////////////////////////////
//
typedef union _VERSION_REGISTER {
    struct {
        UINT32       Revision:8;             // bits 0-7
        UINT32       Reserved:8;             // bits 8-15
        UINT32       Version:8;              // bits 16-23
        UINT32       GUID_ROM:1;             // bit  24
        UINT32       Reserved1:7;            // bits 25-31
    };
    UINT32 all;
} VERSION_REGISTER;
C_ASSERT(sizeof(VERSION_REGISTER) == 4);

typedef union _VENDOR_ID_REGISTER {
    struct {
        UINT32       VendorCompanyId:24;     // bits 0-23
        UINT32       VendorUnique:8;         // bits 24-31
    };
    UINT32 all;
} VENDOR_ID_REGISTER;
C_ASSERT(sizeof(VENDOR_ID_REGISTER) == 4);

typedef union _GUID_ROM_REGISTER {
    struct {
        UINT32       Reserved0:16;           // bits 0-15
        UINT32       RdData:8;               // bits 16-23
        UINT32       Reserved1:1;            // bit  24
        UINT32       RdStart:1;              // bit  25
        UINT32       Reserved2:5;            // bits 26-30
        UINT32       AddrReset:1;            // bits 31
    };
    UINT32 all;
} GUID_ROM_REGISTER;
C_ASSERT(sizeof(GUID_ROM_REGISTER) == 4);

typedef union _AT_RETRIES_REGISTER {
    struct {
        UINT32       MaxATReqRetries:4;      // bits 0-3
        UINT32       MaxATRespRetries:4;     // bits 4-7
        UINT32       MaxPhysRespRetries:4;   // bits 8-11
        UINT32       Reserved:4;             // bits 12-15
        UINT32       CycleLimit:13;          // bits 16-28
        UINT32       SecondLimit:3;          // bits 29-31
    };
    UINT32 all;
} AT_RETRIES_REGISTER;
C_ASSERT(sizeof(AT_RETRIES_REGISTER) == 4);

typedef union _CSR_CONTROL_REGISTER {
    struct {
        UINT32       CsrSel:2;               // bits 0-1
        UINT32       Reserved:29;            // bits 2-30
        UINT32       CsrDone:1;              // bit  31
    };
    UINT32 all;
} CSR_CONTROL_REGISTER;
C_ASSERT(sizeof(CSR_CONTROL_REGISTER) == 4);

typedef union _CONFIG_ROM_HEADER_REGISTER {
    struct {
        UINT32       Rom_crc_value:16;       // bits 0-15
        UINT32       Crc_length:8;           // bits 16-23
        UINT32       Info_length:8;          // bits 24-31
    };
    UINT32 all;
} CONFIG_ROM_HEADER_REGISTER;
C_ASSERT(sizeof(CONFIG_ROM_HEADER_REGISTER) == 4);

typedef union _BUS_OPTIONS_REGISTER {
    struct {
        UINT32       Link_spd:3;             // bits 0-2
        UINT32       Reserved0:3;            // bits 3-5
        UINT32       g:2;                    // bits 6-7
        UINT32       Reserved1:4;            // bits 8-11
        UINT32       Max_rec:4;              // bits 12-15
        UINT32       Cyc_clk_acc:8;          // bits 16-23
        UINT32       Reserved2:3;            // bits 24-26
        UINT32       Pmc:1;                  // bit  27
        UINT32       Bmc:1;                  // bit  28
        UINT32       Isc:1;                  // bit  29
        UINT32       Cmc:1;                  // bit  30
        UINT32       Irmc:1;                 // bit  31
    };
    UINT32 all;
} BUS_OPTIONS_REGISTER;
C_ASSERT(sizeof(BUS_OPTIONS_REGISTER) == 4);

typedef union _HC_CONTROL_REGISTER {
    struct {
        UINT32       Reserved:16;            // bits 0-15
        UINT32       SoftReset:1;            // bit  16
        UINT32       LinkEnable:1;           // bit  17
        UINT32       PostedWriteEnable:1;    // bit  18
        UINT32       Lps:1;                  // bit  19
        UINT32       Reserved2:2;            // bits 20-21
        UINT32       APhyEnhanceEnable:1;    // bit  22
        UINT32       ProgramPhyEnable:1;     // bit  23
        UINT32       Reserved3:6;            // bits 24-29
        UINT32       NoByteSwapData:1;       // bit  30
        UINT32       Reserved4:1;            // bit  31
    };
    UINT32 all;
} HC_CONTROL_REGISTER;
C_ASSERT(sizeof(HC_CONTROL_REGISTER) == 4);

typedef union _FAIRNESS_CONTROL_REGISTER {
    struct {
        UINT32       Pri_req:8;              // bits 0-7
        UINT32       Reserved0:24;           // bits 8-31
    };
    UINT32 all;
} FAIRNESS_CONTROL_REGISTER;
C_ASSERT(sizeof(FAIRNESS_CONTROL_REGISTER) == 4);

typedef union _LINK_CONTROL_REGISTER {
    struct {
        UINT32       Reserved0:4;            // bits 0-3
        UINT32       CycleSyncLReqEnable:1;  // bit  4
        UINT32       Reserved1:4;            // bits 5-8
        UINT32       RcvSelfId:1;            // bit  9
        UINT32       RcvPhyPkt:1;            // bit  10
        UINT32       Reserved2:9;            // bits 11-19
        UINT32       CycleTimerEnable:1;     // bit  20
        UINT32       CycleMaster:1;          // bit  21
        UINT32       CycleSource:1;          // bit  22
        UINT32       Reserved3:9;            // bits 23-31
    };
    UINT32 all;
} LINK_CONTROL_REGISTER;
C_ASSERT(sizeof(LINK_CONTROL_REGISTER) == 4);

typedef union _NODE_ID_REGISTER {
    struct {
        UINT32       NodeId:6;               // bits 0-5
        UINT32       BusId:10;               // bits 6-15
        UINT32       Reserved1:11;           // bits 16-26
        UINT32       Cps:1;                  // bit  27
        UINT32       Reserved2:2;            // bits 28-29
        UINT32       Root:1;                 // bit  30
        UINT32       IdValid:1;              // bit  31
    };
    UINT32 all;
} NODE_ID_REGISTER;
C_ASSERT(sizeof(NODE_ID_REGISTER) == 4);

typedef union _SELF_ID_BUFFER_REGISTER {
    UINT32 SelfIdBufferPointer;
    struct {
        UINT32       Reserved0:11;           // bits 0-10
        UINT32       SelfIdBuffer:21;        // bits 11-32
    };
    UINT32 all;
} SELF_ID_BUFFER_REGISTER;
C_ASSERT(sizeof(SELF_ID_BUFFER_REGISTER) == 4);

typedef union _SELF_ID_COUNT_REGISTER {
    struct {
        UINT32       Reserved0:2;            // bits 0-1
        UINT32       SelfIdSize:11;          // bits 2-12
        UINT32       Reserved1:3;            // bits 13-15
        UINT32       SelfIdGeneration:8;     // bits 16-23
        UINT32       Reserved2:7;            // bits 24-30
        UINT32       SelfIdError:1;          // bit  31
    };
    UINT32 all;
} SELF_ID_COUNT_REGISTER;
C_ASSERT(sizeof(SELF_ID_COUNT_REGISTER) == 4);

typedef union _PHY_CONTROL_REGISTER {
    struct {
        UINT32       WrData:8;               // bits 0-7
        UINT32       RegAddr:4;              // bits 8-11
        UINT32       Reserved0:2;            // bits 12-13
        UINT32       WrReg:1;                // bit  14
        UINT32       RdReg:1;                // bit  15
        UINT32       RdData:8;               // bits 16-23
        UINT32       RdAddr:4;               // bits 24-27
        UINT32       Reserved1:3;            // bits 28-30
        UINT32       RdDone:1;               // bit  31
    };
    UINT32 all;
} PHY_CONTROL_REGISTER;
C_ASSERT(sizeof(PHY_CONTROL_REGISTER) == 4);

typedef union _ISOCH_CYCLE_TIMER_REGISTER {
    struct {
        UINT32       CycleOffset:12;         // bits 0-11
        UINT32       CycleCount:13;          // bits 12-24
        UINT32       CycleSeconds:7;         // bits 25-31
    };
    UINT32 all;
} ISOCH_CYCLE_TIMER_REGISTER;
C_ASSERT(sizeof(ISOCH_CYCLE_TIMER_REGISTER) == 4);

typedef union _INT_EVENT_MASK_REGISTER {
    struct {
        UINT32       ReqTxComplete:1;        // bit  0
        UINT32       RspTxComplete:1;        // bit  1
        UINT32       ARRQ:1;                 // bit  2
        UINT32       ARRS:1;                 // bit  3
        UINT32       RQPkt:1;                // bit  4
        UINT32       RSPPkt:1;               // bit  5
        UINT32       IsochTx:1;              // bit  6
        UINT32       IsochRx:1;              // bit  7
        UINT32       PostedWriteErr:1;       // bit  8
        UINT32       LockRespErr:1;          // bit  9
        UINT32       Reserved0:6;            // bits 10-15
        UINT32       SelfIdComplete:1;       // bit  16
        UINT32       BusReset:1;             // bit  17
        UINT32       Reserved1:1;            // bit  18
        UINT32       Phy:1;                  // bit  19
        UINT32       CycleSynch:1;           // bit  20
        UINT32       Cycle64Secs:1;          // bit  21
        UINT32       CycleLost:1;            // bit  22
        UINT32       CycleInconsistent:1;    // bit  23
        UINT32       UnrecoverableError:1;   // bit  24
        UINT32       CycleTooLong:1;         // bit  25
        UINT32       PhyRegRcvd:1;           // bit  26
        UINT32       Reserved2:3;            // bits 27-29
        UINT32       VendorSpecific:1;       // bit  30
        UINT32       MasterIntEnable:1;      // bit  31
    };
    UINT32 all;
} INT_EVENT_MASK_REGISTER;
C_ASSERT(sizeof(INT_EVENT_MASK_REGISTER) == 4);

typedef union _COMMAND_POINTER_REGISTER {
    struct {
        UINT32       Z:4;                    // bits 0-3
        UINT32       DescriptorAddr:28;      // bits 4-31
    };
    UINT32 all;
} COMMAND_POINTER_REGISTER;
C_ASSERT(sizeof(COMMAND_POINTER_REGISTER) == 4);

typedef union _CONTEXT_CONTROL_REGISTER {
    struct {
        UINT32       EventCode:5;            // bits 0-4
        UINT32       Spd:3;                  // bits 5-7
        UINT32       Reserved0:2;            // bits 8-9
        UINT32       Active:1;               // bit  10
        UINT32       Dead:1;                 // bit  11
        UINT32       Wake:1;                 // bit  12
        UINT32       Reserved1:2;            // bits 13-14
        UINT32       Run:1;                  // bit  15
        UINT32       Reserved2:16;           // bits 16-31
    };
    UINT32 all;
} CONTEXT_CONTROL_REGISTER;
C_ASSERT(sizeof(CONTEXT_CONTROL_REGISTER) == 4);

typedef union _IT_CONTEXT_CONTROL_REGISTER {
    struct {
        UINT32       EventCode:5;            // bits 0-4
        UINT32       Spd:3;                  // bits 5-7
        UINT32       Reserved0:2;            // bits 8-9
        UINT32       Active:1;               // bit  10
        UINT32       Dead:1;                 // bit  11
        UINT32       Wake:1;                 // bit  12
        UINT32       Reserved1:2;            // bits 13-14
        UINT32       Run:1;                  // bit  15
        UINT32       CycleMatch:15;          // bits 16-30
        UINT32       CycleMatchEnable:1;     // bit  31
    };
    UINT32 all;
} IT_CONTEXT_CONTROL_REGISTER;
C_ASSERT(sizeof(IT_CONTEXT_CONTROL_REGISTER) == 4);

typedef union _IR_CONTEXT_CONTROL_REGISTER {
    struct {
        UINT32       EventCode:5;            // bits 0-4
        UINT32       Spd:3;                  // bits 5-7
        UINT32       Reserved0:2;            // bits 8-9
        UINT32       Active:1;               // bit  10
        UINT32       Dead:1;                 // bit  11
        UINT32       Wake:1;                 // bit  12
        UINT32       Reserved1:2;            // bits 13-14
        UINT32       Run:1;                  // bit  15
        UINT32       CycleMatch:12;          // bits 16-27
        UINT32       MultiChanMode:1;        // bit  28
        UINT32       CycleMatchEnable:1;     // bit  29
        UINT32       IsochHeader:1;          // bit  30
        UINT32       BufferFill:1;           // bit  31
    };
    UINT32 all;
} IR_CONTEXT_CONTROL_REGISTER;
C_ASSERT(sizeof(IR_CONTEXT_CONTROL_REGISTER) == 4);

typedef union _CONTEXT_MATCH_REGISTER {
    struct {
        UINT32       ChannelNumber:6;        // bits 0-5
        UINT32       Reserved:1;             // bit  6
        UINT32       Tag1SyncFilter:1;       // bit  7
        UINT32       Sync:4;                 // bits 8-11
        UINT32       CycleMatch:13;          // bits 12-24
        UINT32       Reserved1:3;            // bits 25-27
        UINT32       Tag:4;                  // bit  28-31
    };
    UINT32 all;
} CONTEXT_MATCH_REGISTER;
C_ASSERT(sizeof(CONTEXT_MATCH_REGISTER) == 4);

/////////////////////////////////////////////////////////////
//
typedef struct _DMA_CONTEXT_REGISTERS {
    CONTEXT_CONTROL_REGISTER    ContextControlSet;
    CONTEXT_CONTROL_REGISTER    ContextControlClear;
    UINT32                       Reserved0[1];
    COMMAND_POINTER_REGISTER    CommandPtr;
    UINT32                       Reserved1[4];
} DMA_CONTEXT_REGISTERS;

typedef struct _DMA_ISOCH_RCV_CONTEXT_REGISTERS {
    IR_CONTEXT_CONTROL_REGISTER ContextControlSet;
    IR_CONTEXT_CONTROL_REGISTER ContextControlClear;
    UINT32                       Reserved0[1];
    COMMAND_POINTER_REGISTER    CommandPtr;
    CONTEXT_MATCH_REGISTER      ContextMatch;
    UINT32                       Reserved1[3];
} DMA_ISOCH_RCV_CONTEXT_REGISTERS;

typedef struct _DMA_ISOCH_XMIT_CONTEXT_REGISTERS {
    IT_CONTEXT_CONTROL_REGISTER ContextControlSet;
    IT_CONTEXT_CONTROL_REGISTER ContextControlClear;
    UINT32                       Reserved0[1];
    COMMAND_POINTER_REGISTER    CommandPtr;
} DMA_ISOCH_XMIT_CONTEXT_REGISTERS;

typedef struct _OHCI_REGISTER_MAP {
    VERSION_REGISTER            Version;                
    GUID_ROM_REGISTER           GUID_ROM;               
    AT_RETRIES_REGISTER         ATRetries;              
    UINT32                       CsrData;                
    UINT32                       CsrCompare;             
    CSR_CONTROL_REGISTER        CsrControl;             
    CONFIG_ROM_HEADER_REGISTER  ConfigRomHeader;        
    UINT32                       BusId;                  
    BUS_OPTIONS_REGISTER        BusOptions;             
    UINT32                       GuidHi;                 
    UINT32                       GuidLo;                 
    UINT32                       Reserved0[2];           
    UINT32                       ConfigRomMap;           

    UINT32                       PostedWriteAddressLo;   
    UINT32                       PostedWriteAddressHi;   

    VENDOR_ID_REGISTER          VendorId;               
    UINT32                       Reserved1[3];           

    HC_CONTROL_REGISTER         HCControlSet;           
    HC_CONTROL_REGISTER         HCControlClear;         

    UINT32                       Reserved2[3];           

    SELF_ID_BUFFER_REGISTER     SelfIdBufferPtr;        
    SELF_ID_COUNT_REGISTER      SelfIdCount;            

    UINT32                       Reserved3[1];           

    UINT32                       IRChannelMaskHiSet;     
    UINT32                       IRChannelMaskHiClear;   
    UINT32                       IRChannelMaskLoSet;     
    UINT32                       IRChannelMaskLoClear;   

    INT_EVENT_MASK_REGISTER     IntEventSet;            
    INT_EVENT_MASK_REGISTER     IntEventClear;          

    INT_EVENT_MASK_REGISTER     IntMaskSet;             
    INT_EVENT_MASK_REGISTER     IntMaskClear;           

    UINT32                       IsoXmitIntEventSet;     
    UINT32                       IsoXmitIntEventClear;   

    UINT32                       IsoXmitIntMaskSet;      
    UINT32                       IsoXmitIntMaskClear;    

    UINT32                       IsoRecvIntEventSet;     
    UINT32                       IsoRecvIntEventClear;   

    UINT32                       IsoRecvIntMaskSet;      
    UINT32                       IsoRecvIntMaskClear;    

    UINT32                       Reserved4[11];          

    FAIRNESS_CONTROL_REGISTER   FairnessControl;        

    LINK_CONTROL_REGISTER       LinkControlSet;         
    LINK_CONTROL_REGISTER       LinkControlClear;       

    NODE_ID_REGISTER            NodeId;                 
    PHY_CONTROL_REGISTER        PhyControl;             

    ISOCH_CYCLE_TIMER_REGISTER  IsochCycleTimer;        

    UINT32                       Reserved5[3];           

    UINT32                       AsynchReqFilterHiSet;   
    UINT32                       AsynchReqFilterHiClear; 

    UINT32                       AsynchReqFilterLoSet;   
    UINT32                       AsynchReqFilterLoClear; 

    UINT32                       PhyReqFilterHiSet;      
    UINT32                       PhyReqFilterHiClear;    

    UINT32                       PhyReqFilterLoSet;      
    UINT32                       PhyReqFilterLoClear;    

    UINT32                       PhysicalUpperBound;     
    UINT32                       Reserved6[23];          

    DMA_CONTEXT_REGISTERS       AsynchContext[4];       
    // ATRsp_Context;   
    // ARReq_Context;   
    // ARRsp_Context;   

    DMA_ISOCH_XMIT_CONTEXT_REGISTERS IT_Context[32];    

    DMA_ISOCH_RCV_CONTEXT_REGISTERS IR_Context[32];     
} OHCI_REGISTER_MAP;
C_ASSERT(sizeof(OHCI_REGISTER_MAP) == 2048);

//

//
typedef union _CONFIG_ROM_INFO {
    struct {
        union {
            UINT16          CRI_CRC_Value:16;
            struct {
                UINT8       CRI_Saved_Info_Length;
                UINT8       CRI_Saved_CRC_Length;
            } Saved;
        };
        UINT8               CRI_CRC_Length;
        UINT8               CRI_Info_Length;
    };
    UINT32   all;
} CONFIG_ROM_INFO;
C_ASSERT(sizeof(CONFIG_ROM_INFO) == 4);

//

//
typedef union _IMMEDIATE_ENTRY {
    struct {
        UINT32               IE_Value:24;
        UINT32               IE_Key:8;
    };
    UINT32   all;
} IMMEDIATE_ENTRY;
C_ASSERT(sizeof(IMMEDIATE_ENTRY) == 4);

//

//
typedef union _DIRECTORY_INFO {
    struct {
        union {
            UINT16          DI_CRC;
            UINT16          DI_Saved_Length;
        };
        UINT16              DI_Length;
    };
    UINT32   all;
} DIRECTORY_INFO;
C_ASSERT(sizeof(DIRECTORY_INFO) == 4);

//
///////////////////////////////////////////////////////////////
