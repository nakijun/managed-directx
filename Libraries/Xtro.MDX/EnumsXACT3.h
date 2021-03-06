public enum class CueState
{
	Created = XACT_CUESTATE_CREATED,  
	Preparing = XACT_CUESTATE_PREPARING,
	Prepared = XACT_CUESTATE_PREPARED, 
	Playing = XACT_CUESTATE_PLAYING,  
	Stopping = XACT_CUESTATE_STOPPING, 
	Stopped = XACT_CUESTATE_STOPPED,  
	Paused = XACT_CUESTATE_PAUSED   
};

[Flags]
public enum class CueStopFlag
{
	Release = XACT_FLAG_CUE_STOP_RELEASE,
	Immediate = XACT_FLAG_CUE_STOP_IMMEDIATE
};

public enum class Error
{
	OutOfMemory = XACTENGINE_E_OUTOFMEMORY,               
	InvalidArgument = XACTENGINE_E_INVALIDARG,                
	NotImplemented = XACTENGINE_E_NOTIMPL,                   
	Fail = XACTENGINE_E_FAIL,                      
	AlreadyInitialized = XACTENGINE_E_ALREADYINITIALIZED,        
	NotInitialized = XACTENGINE_E_NOTINITIALIZED,            
	Expired = XACTENGINE_E_EXPIRED,                   
	NoNotificationCallback = XACTENGINE_E_NONOTIFICATIONCALLBACK,    
	NotificationRegistered = XACTENGINE_E_NOTIFICATIONREGISTERED,    
	InvalidUsage = XACTENGINE_E_INVALIDUSAGE,              
	InvalidData = XACTENGINE_E_INVALIDDATA,               
	InstanceLimitFailToPlay = XACTENGINE_E_INSTANCELIMITFAILTOPLAY,   
	NoGlobalSettings = XACTENGINE_E_NOGLOBALSETTINGS,          
	InvalidVariableIndex = XACTENGINE_E_INVALIDVARIABLEINDEX,      
	InvalidCategory = XACTENGINE_E_INVALIDCATEGORY,           
	InvalidCueIndex = XACTENGINE_E_INVALIDCUEINDEX,           
	InvalidWaveIndex = XACTENGINE_E_INVALIDWAVEINDEX,          
	InvalidTrackIndex = XACTENGINE_E_INVALIDTRACKINDEX,         
	InvalidSoundOffsetOrIndex = XACTENGINE_E_INVALIDSOUNDOFFSETORINDEX, 
	ReadFile = XACTENGINE_E_READFILE,                  
	UnknownEvent = XACTENGINE_E_UNKNOWNEVENT,              
	InCallback = XACTENGINE_E_INCALLBACK,                
	NoWaveBank = XACTENGINE_E_NOWAVEBANK,                
	SelectVariation = XACTENGINE_E_SELECTVARIATION,           
	MultipleAuditionEngines = XACTENGINE_E_MULTIPLEAUDITIONENGINES,   
	WaveBankNotPrepared = XACTENGINE_E_WAVEBANKNOTPREPARED,       
	NoRenderer = XACTENGINE_E_NORENDERER,            
	InvalidEntryCount = XACTENGINE_E_INVALIDENTRYCOUNT,         
	SeekTimeBeyondCueEnd = XACTENGINE_E_SEEKTIMEBEYONDCUEEND,      
	SeekTimeBeyondWaveEnd = XACTENGINE_E_SEEKTIMEBEYONDWAVEEND,     
	NoFriendlyName = XACTENGINE_E_NOFRIENDLYNAMES,           
	AuditionWriteFile = XACTENGINE_E_AUDITION_WRITEFILE,        
	AuditionNoSoundBank = XACTENGINE_E_AUDITION_NOSOUNDBANK,      
	AuditionInvalidRpcIndex = XACTENGINE_E_AUDITION_INVALIDRPCINDEX,  
	AuditionMissingData = XACTENGINE_E_AUDITION_MISSINGDATA,      
	AuditionUnknownCommand = XACTENGINE_E_AUDITION_UNKNOWNCOMMAND,   
	AuditionInvalidDspIndex = XACTENGINE_E_AUDITION_INVALIDDSPINDEX,  
	AuditionMissingWave = XACTENGINE_E_AUDITION_MISSINGWAVE,      
	AuditionCreateDirectoryFailed = XACTENGINE_E_AUDITION_CREATEDIRECTORYFAILED,
	AuditionInvalidSession = XACTENGINE_E_AUDITION_INVALIDSESSION   
};

public enum class WaveBankSegmentId
{
    BankData = WAVEBANK_SEGIDX_BANKDATA,
    EntryMetaData = WAVEBANK_SEGIDX_ENTRYMETADATA,
    SeekTables = WAVEBANK_SEGIDX_SEEKTABLES,   
    EntryNames = WAVEBANK_SEGIDX_ENTRYNAMES,   
    EntryWaveData = WAVEBANK_SEGIDX_ENTRYWAVEDATA,
    Count = WAVEBANK_SEGIDX_COUNT
};