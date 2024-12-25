using System;

namespace FA.Automation.MessageBus
{
    public class Constant
    {
        public struct FuncType
        {
            public const string Input_SD = "5";
            public const string Input_Monitor = "3";
            public const string EnterInputFunc = "20";
            public const string Input_Prod = "1";
            public const string Input_ED = "7";
            public const string Output_SD = "6";
            public const string Output_ED = "8";
            public const string Output_Monitor = "4";
            public const string Output_Prod = "2";
            public const string EnterOutputFunc = "21";
        }
        //public struct MessageCategory 
        //{
        //    public const string NORMAL = "NORMAL";      // 一般信息提示
        //    public const string OPERATIONTIPS = "INFO";          // 操作提示
        //    public const string WARNING = "WARNING";    // 业务上的错误提示
        //    public const string ALARM = "ALARM";        // 机台上的错误提示
        //    public const string ERROR = "ERROR";        // 发生程序异常或代码逻辑错误
        //}

        public enum MessageCategory
        {
            NORMAL, // 一般信息提示
            INFO, // 操作提示
            WARNING, // 业务上的错误提示
            ALARM, // 机台上的错误提示
            ERROR // 发生程序异常或代码逻辑错误
        }
        public struct Common
        {
            public const string TimeTransFormat = "yyyy-MM-dd HH:mm:ss";
            public const string TimeSendFormat = "yyyyMMddHHmmss";
        }
        public struct DriverStatus
        {
            public const string DisConnected = "DisConnected";
            public const string Connected = "Connected";
            public const string Initok = "Initok";
        }
        public struct JobStatus
        {
            public const string JobSent = "JobSent";
            public const string JobCreated = "JobCreated";
            public const string JobEnd = "JobEnd";
        }
        public struct ErrMsgType
        {
            public const string EQPErr = "EQPErr:";
            public const string EAPErr = "EAPErr:";
            public const string MESErr = "MESErr:";
            public const string RMSErr = "RMSErr:";
        }
        public struct DataType
        {
            public const string EAPDATA = "EAPDATA";
            public const string ALARMDATA = "ALARMDATA";
            public const string ALARMLISTDATA = "ALARMLISTDATA";
            public const string LOTDATA = "LOTDATA";
            public const string LOTREGISTE = "LOTREGISTE";
        }
        public struct StateStatus
        {
            public const string Waiting = "Waiting"; //in UI: 3, white
            public const string Running = "Running"; //in UI: 0, green
            public const string Over = "Over"; //in UI: 1, blue
            public const string Alarm = "Alarm"; //in UI: 2, red
        }
        public struct ChannelStatus
        {
            public const string Full = "Full";
            public const string Empty = "Empty";
        }
        public struct EQStatus
        {
            public const string IDLE = "IDLE";
            public const string RUNNING = "RUNNING";
            //Furnace
            public const string INACTIVE = "INACTIVE";
            public const string INITIALIZING = "INITIALIZING";

            public const string PAUSED = "PAUSED";
            public const string EXCEPTION = "EXCEPTION";
            public const string OFFLINE = "OFFLINE";
            public const string ONLINE = "ONLINE";
            public const string UNDEFINED = "UNDEFINED";
        }
        public struct CleanerEQStatus
        {
            public const string Idle = "Idle";
            public const string Running = "Running";
            public const string Complete = "Complete";
        }
        public struct JobCommand
        {
            public const string STARTPROCESS = "STARTPROCESS";
            public const string STOP = "STOP";
            public const string PAUSE = "PAUSE";
            public const string ABORT = "ABORT";
            public const string RESUME = "RESUME";
            public const string DELETE = "DELETE";
            public const string CANCEL = "CANCEL";
        }

        public enum EQPControlState
        {
            ONLINEREMOTE,
            ONLINELOCAL,
            OFFLINE
        }

        public enum EQPCategory
        {
            MAIN,
            ACC
        }


        public struct EQPControlStateNumber
        {
            public const string Offline_EqptOffline = "0";
            public const string Offline_AttemptOnline = "0";
            public const string Offline_HostOffline = "0";
            public const string Online_Local = "4";
            public const string Online_Remote = "5";
        }

        public struct EQPControlStateNumberForLH8004
        {
            public const string OFFLINE = "0";
            public const string Online_Local = "1";
            public const string Online_Remote = "2";
        }

        public struct Rorze_RR8221ControlStateNumber
        {
            public const string OFFLINE = "0";
            public const string Online_Local = "1";
            public const string Online_Remote = "2";
        }

        public struct LHRorze_RR8221ControlStateNumber
        {
            public const string OFFLINE = "0";
            public const string Online_Local = "1";
            public const string Online_Remote = "2";
        }

        public struct EQPComMode
        {
            public const string HOSTCONTROL = "HOSTCONTROL";
            public const string DUALCONTROL = "DUALCONTROL";
            public const string HOSTMONITORING = "HOSTMONITORING";
        }

        //904 Robot State
        public struct RobotState
        {
            public const string IDLE = "ROBOTIDLE";
            public const string ERROR = "ROBOTERROR";
            public const string BUSY = "ROBOTBUSY";
        }

        public struct CassetteStatus
        {
            public const string EMPTY = "EMPTY";
            public const string LOT = "LOT";
        }
        public struct LotStatus
        {
            public const string RUNNING = "RUNNING";
            public const string COMPLETED = "COMPLETED";
            public const string WAITING = "WAITING";
            public const string OPENDOOR = "OPENDOOR";
            public const string PPSELECEED = "PPSELECEED";
            //Canon Running;仅用于Inline机台
            public const string CANONRUNNING = "CANONRUNNING";
            public const string STOP = "STOP";
            public const string WRITETAG = "WRITETAG";
            public const string TAGWRITECOMPLETE = "TAGWRITECOMPLETE";
        }
        /// <summary>
        /// SINGLE:单LOT RUN; MULTI:多个Lot组成一个batch一起run
        /// </summary>
        public struct BatchModel
        {
            public const string SINGLE_CASSETTE = "SINGLE";
            public const string MULTI_CASSETTE = "MULTI";
        }

        public struct SendedSECSMessage
        {
            public const string S1F3_CHECK_CONTROL_MODE = "S1F3_CHECK_CONTROL_MODE";
            public const string S7F19_QUERY_RECIPE = "S7F19_QUERY_RECIPE";
            public const string S2F41_PPSELECT = "S2F41_PPSELECT";
            public const string S2F41_START = "S2F41_START";
            public const string SEND_CHECK_LOGINSTATUS = "SEND_CHECK_LOGINSTATUS";
            public const string SEND_CHECK_BATCHSIZE = "SEND_CHECK_BATCHSIZE";
            public const string SEND_CHECK_EQPSTATUS = "SEND_CHECK_EQPSTATUS";
            public const string S2F41_NOCASSETTES = "S2F41_NOCASSETTES";
        }
        public struct CarrierStatus
        {
            public const string LOADW = "Load Wait";
            public const string LOAD_ACTIVE = "Loading";
            public const string RECEIVED = "Cassette Received";
            public const string TAKEIN_READY = "TakeIn Ready";
            public const string TAKEIN_INIT = "TakeIn Init";
            public const string TAKEIN_IP = "TakeIn InProgress";
            public const string TAKEIN_COMP = "TakeIn Complete";
            public const string PROCESS_START = "Process Start";
            public const string PROCESS_END = "Process End";
            public const string UNLOAD_READY = "Unload Ready";
            public const string TAKEOUT_WAIT = "TakeOut Wait";
            public const string TAKEOUT_IP = "TakeOut InProgress";
            public const string TAKEOUT_COMP = "TakeOut Complete";
            public const string UNLOADED = "Cassette Unloaded";

        }

        public struct BatchProcessStatus
        {
            public const string PROCESSW = "Process Wait";
            public const string PROCESS_START = "Process Start";
            public const string PROCESS_END = "Process End";
        }

        public struct BatchType
        {
            public const string PM = "Product Monitor";
            public const string P = "Product";
            public const string M = "Monitor";
            public const string D = "Dummy";
            public const string NA = "NA";
        }

        public struct EDCErrType
        {
            public const string SizeErr = "Size Error";
            [Obsolete] public const string RecipeSettingErr = "RecipeSetting Error";
            [Obsolete] public const string RecipeAndSizeErr = "RecipeSetting And Size Error";
            public const string NotConfigErr = "para not configurate in EAP";
            public const string NotMatchErr = "Slot not match with MES";
        }

        // 从PLC读取的变量名
        public struct PLCReadDataVarName
        {
            public const string AllPLCInfo = "AllPLCInfo";
            public const string ProductQRcode = "ProductQRcode";
            public const string EQPStarted = "EQPStarted";
            public const string RecipeReceived = "RecipeReceived";
            public const string SPMFull = "SPMFull";
            public const string SPMSetTemperature = "SPMSetTemperature";
            public const string SPMCurTemperature = "SPMCurTemperature";
            public const string SPMSetTime = "SPMSetTime";
            public const string SPMRemainTime = "SPMRemainTime";
            public const string HQDRFull = "HQDRFull";
            public const string HQDRSetTemperature = "HQDRSetTemperature";
            public const string HQDRCurTemperature = "HQDRCurTemperature";
            public const string HQDRSetTime = "HQDRSetTime";
            public const string HQDRRemainTime = "HQDRRemainTime";
            public const string DHFFull = "DHFFull";
            public const string DHFSetTime = "DHFSetTime";
            public const string DHFRemainTime = "DHFRemainTime";
            public const string QDR1Full = "QDR1Full";
            public const string QDR1SetTime = "QDR1SetTime";
            public const string QDR1RemainTime = "QDR1RemainTime";
            public const string SC1Full = "SC1Full";
            public const string SC1SetTemperature = "SC1SetTemperature";
            public const string SC1CurTemperature = "SC1CurTemperature";
            public const string SC1SetTime = "SC1SetTime";
            public const string SC1RemainTime = "SC1RemainTime";
            public const string QDR2Full = "QDR2Full";
            public const string QDR2SetTime = "QDR2SetTime";
            public const string QDR2RemainTime = "QDR2RemainTime";
            public const string SC2Full = "SC2Full";
            public const string SC2SetTemperature = "SC2SetTemperature";
            public const string SC2CurTemperature = "SC2CurTemperature";
            public const string SC2SetTime = "SC2SetTime";
            public const string SC2RemainTime = "SC2RemainTime";
            public const string SaosenFull = "SaosenFull";
            public const string SaosenSetTime = "SaosenSetTime";
            public const string SaosenRemainTime = "SaosenRemainTime";
            public const string IPAFull = "IPAFull";
            public const string IPASetTemperature = "IPASetTemperature";
            public const string IPACurTemperature = "IPACurTemperature";
            public const string IPASetTime = "IPASetTime";
            public const string IPARemainTime = "IPARemainTime";
            public const string EQPEnded = "EQPEnded";

        }

        // 向PLC写入的变量名
        public struct PLCWriteDataVarName
        {
            public const string MESOver = "MESOver";
            public const string Recipe = "Recipe";
        }

        public struct SlotState
        {
            public const char Empty = '0';
            public const char Full = '1';
            public const char Selected = '2'; //Full and Selected
        }


        public struct CassetteOperation
        {
            public const string SplitLot = "SplitLot";
            public const string MergeLot = "MergeLot";
            public const string TransCst = "TransCst";
        }

        public struct ErrorCode
        {
            public const string checkmore = "2";
            public const string success = "1";
            public const string fail = "0";

        }

        public struct GetDictionaryType
        {
            public const string key = "Key";
            public const string value = "Value";
        }

        public struct Message
        {
            public const string noRecipe = "No recipe to RMS check";
            public const string noCheckFlag = "当前recipe在RMS中Check Flag为false,请检查RMS的配置";
            public const string noSubRecipe = "No sub recipe to RMS check";
            public const string noRecipeSetting = "RMS not having recipe format setting";
            public const string noRecipeVer = "RMS中没有任何可供RMS比对的recipe 版本";
            public const string noActive = "RMS中没有Active 版本";
            public const string compareFaid = "参数值与RMS中设定不一致";
            public const string compareSuccess = "参数值与RMS中设定完全一致";
            public const string noHistory = "没有历史记录或查询条件错误";
            public const string needMoreCheck = "继续check recipe body";
            public const string noData = "机台或者数据中的数据一方出现没有数据的情况";
            public const string checkCountToLitter = "需要比对的数据行少于3条";
        }

        public struct XmlNodeType
        {
            public const string ElementNode = "Element";
            public const string AttributeNode = "Attribute";
            public const string NormalNode = "Normal";
            public const string ElementAttributeNode = "Element,Attribute";
        }

        public struct RecipeType
        {
            public const string Recipe = "Recipe";
            public const string SubRecipe = "SubRecipe";
            public const string Unit = "Unit";
        }

        public struct CurrectRMSCheckState
        {
            public const string RecipeCheck = "RecipeCheck";
            public const string SubRecipeCheck = "SubRecipeCheck";
            public const string UnitRecipeCheck = "UnitRecipeCheck";
        }
        public struct RecipeBodyParaType
        {
            public const string STRING = "STRING";
            public const string INT = "INT";
            public const string DOUBLE = "DOUBLE";
        }

        public struct EAPType
        {
            public const string STAND = "0";
            public const string METROLOGY = "1";
            public const string Rorze_RR8221 = "2";
            public const string INLINE = "3";
            public const string BATCH = "4";
        }
        public struct SlotMapStatus
        {
            public const string NotRead = "0";
            public const string WaitingForHost = "1";
            public const string VerificationOK = "2";
            public const string VerificationFailed = "3";
        }

        public enum charmberType
        {
            A,
            B,
            C,
            D,
            E,
            F,
            CH1,
            CH2,
            CH3,
            CH4,
            Null
        }

        public enum processFlag
        {
            processStart,
            processEnd
        }

        public struct TransferType
        {
            public const string TRANSELF = "tranself";
            public const string TRANSFER = "transfer";
            public const string BATCH = "Batch";
            public const string INTERLOT = "InterLot";
        }
        public struct Rorze_RR8221Port
        {
            public const string PORT1 = "A";
            public const string PORT2 = "B";
            public const string PORT3 = "C";
        }
        public struct FURNANCEPROCESSMODE
        {
            public const string TWIN = "TWIN";
            public const string SINGLE = "SINGLE";
        }

        public struct HoldLotType
        {
            public const string ALARM = "ALARM"; //ALS返回结果需要HOLD LOT时，是否HOLD LOT
            public const string MAPPING = "MAPPING"; //MAPPING结果不正确时，是否HOLD LOT
            public const string COMMAND = "COMMAND"; //PPSELECT/START等命令失败时，是否HOLD LOT
            public const string WRITETAG = "WRITETAG"; //WriteTag失败
            public const string TRACKOUT = "TRACKOUT"; //TrackOut失败
            public const string EDC = "EDC"; //EDC参数收集有误
            public const string APC = "APC"; //APC要求HOLD
            public const string PROCESSED_WAFER = "PROCESSED_WAFER"; //CheckProcessedWafer失败时，HOLD LOT
            public const string AlwaysHold = "AlwaysHold";
        }

        public struct WETPortName
        {
            public const string IN_PORT1 = "1";
            public const string IN_PORT2 = "2";
            public const string OUT_PORT1 = "3";
            public const string OUT_PORT2 = "4";
        }
        //Inline货架使用
        public struct PortType
        {
            public const string Normal = "NormalPort";
            public const string In = "InPort";
            public const string Out = "OutPort";
            public const string Storage = "StoragePort";
        }

        public struct LayerNum
        {
            public const string First = "First";
            public const string Second = "Second";
        }

        public struct OfflineCollectionState
        {
            public const string WAITING = "WAITING";
            public const string COLLECTING = "COLLECTING";
            public const string COMPLETE = "COMPLETE";
        }

        public struct CommandType_EQP
        {
            public const string LOCK = "LOCK";
            public const string UNLOCK = "UNLOCK";
            public const string LOAD = "LOAD";
            public const string UNLOAD = "UNLOAD";
            public const string PPSELECT = "PPSELECT";
            public const string START = "START";
            public const string PPCANCEL = "PPCANCEL";
            public const string QUERY_CONTROLMODE = "QUERY_CONTROLMODE";
            public const string QUERY_RECIPE_LIST = "QUERY_RECIPE_LIST";
            public const string QUERY_RECIPE_BODY = "QUERY_RECIPE_BODY";
            public const string QUERY_PJOB_SPACE = "QUERY_PJOB_SPACE";
            public const string PROCEED_WITH_CARRIER_1ST = "PROCEED_WITH_CARRIER_1ST";
            public const string PROCEED_WITH_CARRIER_2ND = "PROCEED_WITH_CARRIER_2ND";
            public const string CJOB_CREATE = "CJOB_CREATE";
            public const string PJOB_CREATE = "PJOB_CREATE";
        }
        public struct CommandType_SMIF
        {
            public const string LOCAL = "LOCAL";
            public const string REMOTE = "REMOTE";
            public const string ENABLELOAD = "ENABLELOAD";
            public const string ENABLEUNLOAD = "ENABLEUNLOAD";
            public const string LOAD = "LOAD";
            public const string UNLOAD = "UNLOAD";
            public const string LOCK = "LOCK";
            public const string UNLOCK = "UNLOCK";
            public const string READ_LOTID = "READ_LOTID";
            public const string READ_LCDFILE = "READ_LCDFILE";
            public const string WRITE_LCDFILE = "WRITE_LCDFILE";
            public const string QUERY_SLOTMAP = "QUERY_SLOTMAP";
        }

        public enum PjobStateType
        {
            WaitingStart,
            Processing,
            ProcessEnd,
            Warning,
            Error 
        }

        public struct CarrierType
        {
            public const string Monitor = "Monitor";
            public const string Product = "Product";
            public const string FD = "FD";
            public const string SD = "SD";
            public const string ED = "ED";
            public const string Dummy = "Dummy";
            public const string RunCard = "RunCard";
            public const string Season = "Season";

        }

        public enum WaferState
        {
            None,
            Moving,
            Processing,
            Processed,
            Waiting,
            Warning,
            Error
        }
        /// <summary>
        /// 查询PMS SVID时使用的前缀
        /// </summary>
        public const string PMS_QUERY_SVID_PREFIX = "QUERY_PMS";
    }

    public struct Scenario
    {
        public const string InactiveSpoolData = "InactiveSpoolData";
        public const string DisableAllEvents = "DisableAllEvents";
        public const string DeleteLinkReport = "DeleteLinkReport";
        public const string DeleteDefineReport = "DeleteDefineReport";
        public const string DefineReport = "DefineReport";
        public const string LinkReport = "LinkReport";
        public const string EnableAllEvents = "EnableAllEvents";
        public const string EnableAllAlarms = "EnableAllAlarms";
        public const string TimeSet = "TimeSet";
        public const string CommunicationRequest = "CommunicationRequest";
        public const string AreYouThere = "AreYouThere";
        public const string InitControlState = "InitControlState";
        public const string InitEqpConstant = "InitEqpConstant";
    }
    public struct LoadPortControl
    {
        public const string LOCK = "LOCK";
        public const string UNLOCK = "UNLOCK";
        public const string REMOTE = "REMOTE";
        public const string LOCAL = "LOCAL";
        public const string UNLOAD = "UNLOAD";
        public const string START = "START";
        public const string END = "END";
        public const string RFIDREAD = "RFIDREAD";
        public const string CLOSEDOOR = "CLOSEDOOR";
        public const string WRITEEMPTYTAG = "WRITEEMPTYTAG";
        public const string WRITEFULLTAG = "WRITEFULLTAG";
        public const string REFRESHTAG = "REFRESHTAG";

    }
    public struct InitFileNames
    {
        public const string AreYouThere = "AreYouThere";
        public const string CommunicationRequest = "CommunicationRequest";
        public const string InactiveSpoolData = "InactiveSpoolData";
        public const string DisableAllEvents = "DisableAllEvents";
        public const string DeleteLinkReport = "DeleteLinkReport";
        public const string DeleteDefineReport = "DeleteDefineReport";
        public const string DefineReport = "DefineReport";
        public const string LinkReport = "LinkReport";
        public const string EnableAllEvents = "EnableAllEvents";
        public const string EnableAllAlarms = "EnableAllAlarms";
        public const string TimeSet = "TimeSet";
        public const string InitControlState = "InitControlState";
        public const string InitEqpConstant = "InitEqpConstant";
    }
    public struct RMSAction
    {
        public const string RECIPELISTREQUEST = "RECIPELISTREQUEST";
        public const string RECIPEBODYREQUEST = "RECIPEBODYREQUEST";
        public const string RECIPEBODYDOWNLOADREQUEST = "RECIPEBODYDOWNLOADREQUEST";
        public const string RECIPEBODYCHECKREPLY = "RECIPEBODYCHECKREPLY";
        public const string PARAMETERVALUEREQUEST = "PARAMETERVALUEREQUEST";
        public const string NEEDBECHECKPARAMETERLISTREPLY = "NEEDBECHECKPARAMETERLISTREPLY";
        public const string PARAMETERCHECKREPLY = "PARAMETERCHECKREPLY";
    }
    public struct RMSMessageData
    {
        public const string RMSECSVListRequest = "RMSECSVListRequest";
        public const string RMSRecipeListRequest = "RMSRecipeListRequest";
        public const string RMSRecipeBodyRequest = "RMSRecipeBodyRequest";
        public const string EAPRecipeBodyRequest = "EAPRecipeBodyRequest";
        public const string EAPECSVListRequest = "EAPECSVListRequest";
        public const string EAPCheckRecipeRequest = "EAPCheckRecipeRequest";
    }
    public enum FlowStates
    {
        Executed,
        Waiting,
        Executing,
        Pass,
        Alarm
    }

    public struct SorterJobType
    {
        public const string EXCHANGE = "EXCHANGE";
        public const string SPLIT = "SPLIT";
        public const string SORT = "SORT";
        public const string ROTATE = "ROTATE";
        public const string MERGE = "MERGE";
        public const string FLIP = "FLIP";
        public const string WAFERSTART = "WAFERSTART";
    }

}
