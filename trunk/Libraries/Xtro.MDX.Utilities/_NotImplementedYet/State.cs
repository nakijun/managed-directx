using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;
using Device = Xtro.MDX.Direct3D10.Device;

namespace Xtro.MDX.Utilities
{
    sealed class State
    {
        static readonly object Lock = new object(); // Lock Object

        DeviceSettings FCurrentDeviceSettings;
        public DeviceSettings CurrentDeviceSettings { get { lock (Lock) return FCurrentDeviceSettings; } set { lock (Lock) FCurrentDeviceSettings = value; } }

        RasterizerState FRasterizerState;
        public RasterizerState RasterizerState { get { lock (Lock) return FRasterizerState; } set { lock (Lock) FRasterizerState = value; } }

        // ReSharper disable InconsistentNaming
        bool FDirect3D_Available;
        public bool Direct3D_Available { get { lock (Lock) return FDirect3D_Available; } set { lock (Lock) FDirect3D_Available = value; } }
        // ReSharper restore InconsistentNaming
        Factory FFactory;
        public Factory Factory { get { lock (Lock) return FFactory; } set { lock (Lock) FFactory = value; } }
        Adapter FAdapter;
        public Adapter Adapter { get { lock (Lock) return FAdapter; } set { lock (Lock) FAdapter = value; } }
        List<Output> FOutputs;
        public List<Output> Outputs { get { lock (Lock) return FOutputs; } set { lock (Lock) FOutputs = value; } }
        Device FDevice;
        public Device Device { get { lock (Lock) return FDevice; } set { lock (Lock) FDevice = value; } }
        SwapChain FSwapChain;
        public SwapChain SwapChain { get { lock (Lock) return FSwapChain; } set { lock (Lock) FSwapChain = value; } }
        Texture2D FDepthStencil;
        public Texture2D DepthStencil { get { lock (Lock) return FDepthStencil; } set { lock (Lock) FDepthStencil = value; } }
        DepthStencilView FDepthStencilView;
        public DepthStencilView DepthStencilView { get { lock (Lock) return FDepthStencilView; } set { lock (Lock) FDepthStencilView = value; } }
        RenderTargetView FRenderTargetView;
        public RenderTargetView RenderTargetView { get { lock (Lock) return FRenderTargetView; } set { lock (Lock) FRenderTargetView = value; } }
        RasterizerState FDefaultRasterizerState;
        public RasterizerState DefaultRasterizerState { get { lock (Lock) return FDefaultRasterizerState; } set { lock (Lock) FDefaultRasterizerState = value; } }
        SurfaceDescription FBackBufferSurfaceDescription;
        public SurfaceDescription BackBufferSurfaceDescription { get { lock (Lock) return FBackBufferSurfaceDescription; } set { lock (Lock) FBackBufferSurfaceDescription = value; } }
        bool FRenderingOccluded;
        public bool RenderingOccluded { get { lock (Lock) return FRenderingOccluded; } set { lock (Lock) FRenderingOccluded = value; } }
        bool FDoNotStoreBufferSize;
        public bool DoNotStoreBufferSize { get { lock (Lock) return FDoNotStoreBufferSize; } set { lock (Lock) FDoNotStoreBufferSize = value; } }
        // ReSharper disable InconsistentNaming
        Counter FCounterGPU_Idle;
        public Counter CounterGPU_Idle { get { lock (Lock) return FCounterGPU_Idle; } set { lock (Lock) FCounterGPU_Idle = value; } }
        // ReSharper restore InconsistentNaming
        Counter FCounterVertexProcessing;
        public Counter CounterVertexProcessing { get { lock (Lock) return FCounterVertexProcessing; } set { lock (Lock) FCounterVertexProcessing = value; } }
        Counter FCounterGeometryProcessing;
        public Counter CounterGeometryProcessing { get { lock (Lock) return FCounterGeometryProcessing; } set { lock (Lock) FCounterGeometryProcessing = value; } }
        Counter FCounterPixelProcessing;
        public Counter CounterPixelProcessing { get { lock (Lock) return FCounterPixelProcessing; } set { lock (Lock) FCounterPixelProcessing = value; } }
        // ReSharper disable InconsistentNaming
        Counter FCounterOtherGPU_Processing;
        public Counter CounterOtherGPU_Processing { get { lock (Lock) return FCounterOtherGPU_Processing; } set { lock (Lock) FCounterOtherGPU_Processing = value; } }
        // ReSharper restore InconsistentNaming
        Counter FCounterHostAdapterBandwidthUtilization;
        public Counter CounterHostAdapterBandwidthUtilization { get { lock (Lock) return FCounterHostAdapterBandwidthUtilization; } set { lock (Lock) FCounterHostAdapterBandwidthUtilization = value; } }
        Counter FCounterLocalVidmemBandwidthUtilization;
        public Counter CounterLocalVidmemBandwidthUtilization { get { lock (Lock) return FCounterLocalVidmemBandwidthUtilization; } set { lock (Lock) FCounterLocalVidmemBandwidthUtilization = value; } }
        Counter FCounterVertexThroughputUtilization;
        public Counter CounterVertexThroughputUtilization { get { lock (Lock) return FCounterVertexThroughputUtilization; } set { lock (Lock) FCounterVertexThroughputUtilization = value; } }
        Counter FCounterTriangleSetupThroughputUtilization;
        public Counter CounterTriangleSetupThroughputUtilization { get { lock (Lock) return FCounterTriangleSetupThroughputUtilization; } set { lock (Lock) FCounterTriangleSetupThroughputUtilization = value; } }
        Counter FCounterFillrateThrougputUtilization;
        public Counter CounterFillrateThrougputUtilization { get { lock (Lock) return FCounterFillrateThrougputUtilization; } set { lock (Lock) FCounterFillrateThrougputUtilization = value; } }
        Counter FCounterVSMemoryLimited;
        public Counter CounterVSMemoryLimited { get { lock (Lock) return FCounterVSMemoryLimited; } set { lock (Lock) FCounterVSMemoryLimited = value; } }
        Counter FCounterVSComputationLimited;
        public Counter CounterVSComputationLimited { get { lock (Lock) return FCounterVSComputationLimited; } set { lock (Lock) FCounterVSComputationLimited = value; } }
        Counter FCounterGSMemoryLimited;
        public Counter CounterGSMemoryLimited { get { lock (Lock) return FCounterGSMemoryLimited; } set { lock (Lock) FCounterGSMemoryLimited = value; } }
        Counter FCounterGSComputationLimited;
        public Counter CounterGSComputationLimited { get { lock (Lock) return FCounterGSComputationLimited; } set { lock (Lock) FCounterGSComputationLimited = value; } }
        Counter FCounterPSMemoryLimited;
        public Counter CounterPSMemoryLimited { get { lock (Lock) return FCounterPSMemoryLimited; } set { lock (Lock) FCounterPSMemoryLimited = value; } }
        Counter FCounterPSComputationLimited;
        public Counter CounterPSComputationLimited { get { lock (Lock) return FCounterPSComputationLimited; } set { lock (Lock) FCounterPSComputationLimited = value; } }
        Counter FCounterPostTransformCacheHitRate;
        public Counter CounterPostTransformCacheHitRate { get { lock (Lock) return FCounterPostTransformCacheHitRate; } set { lock (Lock) FCounterPostTransformCacheHitRate = value; } }
        Counter FCounterTextureCacheHitRate;
        public Counter CounterTextureCacheHitRate { get { lock (Lock) return FCounterTextureCacheHitRate; } set { lock (Lock) FCounterTextureCacheHitRate = value; } }
        Counters FCounterData = new Counters();
        public Counters CounterData { get { lock (Lock) return FCounterData; } set { lock (Lock) FCounterData = value; } }

        Form FForm;
        public Form Form { get { lock (Lock) return FForm; } set { lock (Lock) FForm = value; } }
        Screen FAdapterMonitor;
        public Screen AdapterMonitor { get { lock (Lock) return FAdapterMonitor; } set { lock (Lock) FAdapterMonitor = value; } }

        uint FFullScreenBackBufferWidthAtModeChange;
        public uint FullScreenBackBufferWidthAtModeChange { get { lock (Lock) return FFullScreenBackBufferWidthAtModeChange; } set { lock (Lock) FFullScreenBackBufferWidthAtModeChange = value; } }
        uint FFullScreenBackBufferHeightAtModeChange;
        public uint FullScreenBackBufferHeightAtModeChange { get { lock (Lock) return FFullScreenBackBufferHeightAtModeChange; } set { lock (Lock) FFullScreenBackBufferHeightAtModeChange = value; } }
        uint FWindowBackBufferWidthAtModeChange;
        public uint WindowBackBufferWidthAtModeChange { get { lock (Lock) return FWindowBackBufferWidthAtModeChange; } set { lock (Lock) FWindowBackBufferWidthAtModeChange = value; } }
        uint FWindowBackBufferHeightAtModeChange;
        public uint WindowBackBufferHeightAtModeChange { get { lock (Lock) return FWindowBackBufferHeightAtModeChange; } set { lock (Lock) FWindowBackBufferHeightAtModeChange = value; } }
        Windows.WindowPlacement FWindowedPlacement;
        public Windows.WindowPlacement WindowedPlacement { get { lock (Lock) return FWindowedPlacement; } set { lock (Lock) FWindowedPlacement = value; } }
        bool FTopmostWhileWindowed;
        public bool TopmostWhileWindowed { get { lock (Lock) return FTopmostWhileWindowed; } set { lock (Lock) FTopmostWhileWindowed = value; } }
        bool FMinimized;
        public bool Minimized { get { lock (Lock) return FMinimized; } set { lock (Lock) FMinimized = value; } }
        bool FMaximized;
        public bool Maximized { get { lock (Lock) return FMaximized; } set { lock (Lock) FMaximized = value; } }
        bool FMinimizedWhileFullscreen;
        public bool MinimizedWhileFullscreen { get { lock (Lock) return FMinimizedWhileFullscreen; } set { lock (Lock) FMinimizedWhileFullscreen = value; } }
        bool FIgnoreSizeChange;
        public bool IgnoreSizeChange { get { lock (Lock) return FIgnoreSizeChange; } set { lock (Lock) FIgnoreSizeChange = value; } }

        double FTime;
        public double Time { get { lock (Lock) return FTime; } set { lock (Lock) FTime = value; } }
        double FAbsoluteTime;
        public double AbsoluteTime { get { lock (Lock) return FAbsoluteTime; } set { lock (Lock) FAbsoluteTime = value; } }
        float FElapsedTime;
        public float ElapsedTime { get { lock (Lock) return FElapsedTime; } set { lock (Lock) FElapsedTime = value; } }

        double FLastStatsUpdateTime;
        public double LastStatsUpdateTime { get { lock (Lock) return FLastStatsUpdateTime; } set { lock (Lock) FLastStatsUpdateTime = value; } }
        uint FLastStatsUpdateFrames;
        public uint LastStatsUpdateFrames { get { lock (Lock) return FLastStatsUpdateFrames; } set { lock (Lock) FLastStatsUpdateFrames = value; } }
        float FFPS;
        public float FPS { get { lock (Lock) return FFPS; } set { lock (Lock) FFPS = value; } }
        int FCurrentFrameNumber;
        public int CurrentFrameNumber { get { lock (Lock) return FCurrentFrameNumber; } set { lock (Lock) FCurrentFrameNumber = value; } }

        bool FHandleEscape=true;
        public bool HandleEscape { get { lock (Lock) return FHandleEscape; } set { lock (Lock) FHandleEscape = value; } }
        bool FHandleAltEnter=true;
        public bool HandleAltEnter { get { lock (Lock) return FHandleAltEnter; } set { lock (Lock) FHandleAltEnter = value; } }
        bool FHandlePause=true;
        public bool HandlePause { get { lock (Lock) return FHandlePause; } set { lock (Lock) FHandlePause = value; } }
        bool FShowMessageBoxOnError=true;
        public bool ShowMessageBoxOnError { get { lock (Lock) return FShowMessageBoxOnError; } set { lock (Lock) FShowMessageBoxOnError = value; } }
        bool FNoStats;
        public bool NoStats { get { lock (Lock) return FNoStats; } set { lock (Lock) FNoStats = value; } }
        bool FClipCursorWhenFullScreen;
        public bool ClipCursorWhenFullScreen { get { lock (Lock) return FClipCursorWhenFullScreen; } set { lock (Lock) FClipCursorWhenFullScreen = value; } }
        bool FShowCursorWhenFullScreen;
        public bool ShowCursorWhenFullScreen { get { lock (Lock) return FShowCursorWhenFullScreen; } set { lock (Lock) FShowCursorWhenFullScreen = value; } }
        bool FConstantFrameTime;
        public bool ConstantFrameTime { get { lock (Lock) return FConstantFrameTime; } set { lock (Lock) FConstantFrameTime = value; } }
        float FTimePerFrame;
        public float TimePerFrame { get { lock (Lock) return FTimePerFrame; } set { lock (Lock) FTimePerFrame = value; } }
        bool FWireframeMode;
        public bool WireframeMode { get { lock (Lock) return FWireframeMode; } set { lock (Lock) FWireframeMode = value; } }
        bool FAutoChangeAdapter=true;
        public bool AutoChangeAdapter { get { lock (Lock) return FAutoChangeAdapter; } set { lock (Lock) FAutoChangeAdapter = value; } }
        bool FWindowCreatedWithDefaultPositions;
        public bool WindowCreatedWithDefaultPositions { get { lock (Lock) return FWindowCreatedWithDefaultPositions; } set { lock (Lock) FWindowCreatedWithDefaultPositions = value; } }
        int FExitCode;
        public int ExitCode { get { lock (Lock) return FExitCode; } set { lock (Lock) FExitCode = value; } }

        bool FInitialized;
        public bool Initialized { get { lock (Lock) return FInitialized; } set { lock (Lock) FInitialized = value; } }
        bool FDeviceCreated;
        public bool DeviceCreated { get { lock (Lock) return FDeviceCreated; } set { lock (Lock) FDeviceCreated = value; } }
        bool FInitializeCalled;
        public bool InitializeCalled { get { lock (Lock) return FInitializeCalled; } set { lock (Lock) FInitializeCalled = value; } }
        bool FDeviceCreateCalled;
        public bool DeviceCreateCalled { get { lock (Lock) return FDeviceCreateCalled; } set { lock (Lock) FDeviceCreateCalled = value; } }
        bool FInsideDeviceCallback;
        public bool InsideDeviceCallback { get { lock (Lock) return FInsideDeviceCallback; } set { lock (Lock) FInsideDeviceCallback = value; } }
        bool FDeviceObjectsCreated;
        public bool DeviceObjectsCreated { get { lock (Lock) return FDeviceObjectsCreated; } set { lock (Lock) FDeviceObjectsCreated = value; } }
        bool FDeviceObjectsReset;
        public bool DeviceObjectsReset { get { lock (Lock) return FDeviceObjectsReset; } set { lock (Lock) FDeviceObjectsReset = value; } }
        bool FActive=true;
        public bool Active { get { lock (Lock) return FActive; } set { lock (Lock) FActive = value; } }
        bool FRenderingPaused;
        public bool RenderingPaused { get { lock (Lock) return FRenderingPaused; } set { lock (Lock) FRenderingPaused = value; } }
        bool FTimePaused;
        public bool TimePaused { get { lock (Lock) return FTimePaused; } set { lock (Lock) FTimePaused = value; } }
        int FPauseRenderingCount;
        public int PauseRenderingCount { get { lock (Lock) return FPauseRenderingCount; } set { lock (Lock) FPauseRenderingCount = value; } }
        int FPauseTimeCount;
        public int PauseTimeCount { get { lock (Lock) return FPauseTimeCount; } set { lock (Lock) FPauseTimeCount = value; } }
        bool FDeviceLost;
        public bool DeviceLost { get { lock (Lock) return FDeviceLost; } set { lock (Lock) FDeviceLost = value; } }
        bool FNotifyOnMouseMove;
        public bool NotifyOnMouseMove { get { lock (Lock) return FNotifyOnMouseMove; } set { lock (Lock) FNotifyOnMouseMove = value; } }
        bool FAutomation;
        public bool Automation { get { lock (Lock) return FAutomation; } set { lock (Lock) FAutomation = value; } }
        bool FInSizeMove;
        public bool InSizeMove { get { lock (Lock) return FInSizeMove; } set { lock (Lock) FInSizeMove = value; } }
        uint FTimerLastID;
        public uint TimerLastID { get { lock (Lock) return FTimerLastID; } set { lock (Lock) FTimerLastID = value; } }
        bool FAppCalledWasKeyPressed;
        public bool AppCalledWasKeyPressed { get { lock (Lock) return FAppCalledWasKeyPressed; } set { lock (Lock) FAppCalledWasKeyPressed = value; } }

        bool FReleasingSwapChain;
        public bool ReleasingSwapChain { get { lock (Lock) return FReleasingSwapChain; } set { lock (Lock) FReleasingSwapChain = value; } }
        bool FIsInGammaCorrectMode=true;
        public bool IsInGammaCorrectMode { get { lock (Lock) return FIsInGammaCorrectMode; } set { lock (Lock) FIsInGammaCorrectMode = value; } }

        Callbacks.ModifyDeviceSettings FModifyDeviceSettingsFunction;
        public Callbacks.ModifyDeviceSettings ModifyDeviceSettingsFunction { get { lock (Lock) return FModifyDeviceSettingsFunction; } set { lock (Lock) FModifyDeviceSettingsFunction = value; } }
        Callbacks.DeviceRemoved FDeviceRemovedFunction;
        public Callbacks.DeviceRemoved DeviceRemovedFunction { get { lock (Lock) return FDeviceRemovedFunction; } set { lock (Lock) FDeviceRemovedFunction = value; } }
        Callbacks.FrameMove FFrameMoveFunction;
        public Callbacks.FrameMove FrameMoveFunction { get { lock (Lock) return FFrameMoveFunction; } set { lock (Lock) FFrameMoveFunction = value; } }

        Callbacks.IsDeviceAcceptable FIsDeviceAcceptableFunction;
        public Callbacks.IsDeviceAcceptable IsDeviceAcceptableFunction { get { lock (Lock) return FIsDeviceAcceptableFunction; } set { lock (Lock) FIsDeviceAcceptableFunction = value; } }
        Callbacks.DeviceCreated FDeviceCreatedFunction;
        public Callbacks.DeviceCreated DeviceCreatedFunction { get { lock (Lock) return FDeviceCreatedFunction; } set { lock (Lock) FDeviceCreatedFunction = value; } }
        Callbacks.SwapChainResized FSwapChainResizedFunction;
        public Callbacks.SwapChainResized SwapChainResizedFunction { get { lock (Lock) return FSwapChainResizedFunction; } set { lock (Lock) FSwapChainResizedFunction = value; } }
        Callbacks.SwapChainReleasing FSwapChainReleasingFunction;
        public Callbacks.SwapChainReleasing SwapChainReleasingFunction { get { lock (Lock) return FSwapChainReleasingFunction; } set { lock (Lock) FSwapChainReleasingFunction = value; } }
        Callbacks.DeviceDestroyed FDeviceDestroyedFunction;
        public Callbacks.DeviceDestroyed DeviceDestroyedFunction { get { lock (Lock) return FDeviceDestroyedFunction; } set { lock (Lock) FDeviceDestroyedFunction = value; } }
        Callbacks.FrameRender FFrameRenderFunction;
        public Callbacks.FrameRender FrameRenderFunction { get { lock (Lock) return FFrameRenderFunction; } set { lock (Lock) FFrameRenderFunction = value; } }

        object FModifyDeviceSettingsFunctionUserContext;
        public object ModifyDeviceSettingsFunctionUserContext { get { lock (Lock) return FModifyDeviceSettingsFunctionUserContext; } set { lock (Lock) FModifyDeviceSettingsFunctionUserContext = value; } }
        object FDeviceRemovedFunctionUserContext;
        public object DeviceRemovedFunctionUserContext { get { lock (Lock) return FDeviceRemovedFunctionUserContext; } set { lock (Lock) FDeviceRemovedFunctionUserContext = value; } }
        object FFrameMoveFunctionUserContext;
        public object FrameMoveFunctionUserContext { get { lock (Lock) return FFrameMoveFunctionUserContext; } set { lock (Lock) FFrameMoveFunctionUserContext = value; } }

        object FIsDeviceAcceptableFunctionUserContext;
        public object IsDeviceAcceptableFunctionUserContext { get { lock (Lock) return FIsDeviceAcceptableFunctionUserContext; } set { lock (Lock) FIsDeviceAcceptableFunctionUserContext = value; } }
        object FDeviceCreatedFunctionUserContext;
        public object DeviceCreatedFunctionUserContext { get { lock (Lock) return FDeviceCreatedFunctionUserContext; } set { lock (Lock) FDeviceCreatedFunctionUserContext = value; } }
        object FDeviceDestroyedFunctionUserContext;
        public object DeviceDestroyedFunctionUserContext { get { lock (Lock) return FDeviceDestroyedFunctionUserContext; } set { lock (Lock) FDeviceDestroyedFunctionUserContext = value; } }
        object FSwapChainResizedFunctionUserContext;
        public object SwapChainResizedFunctionUserContext { get { lock (Lock) return FSwapChainResizedFunctionUserContext; } set { lock (Lock) FSwapChainResizedFunctionUserContext = value; } }
        object FSwapChainReleasingFunctionUserContext;
        public object SwapChainReleasingFunctionUserContext { get { lock (Lock) return FSwapChainReleasingFunctionUserContext; } set { lock (Lock) FSwapChainReleasingFunctionUserContext = value; } }
        object FFrameRenderFunctionUserContext;
        public object FrameRenderFunctionUserContext { get { lock (Lock) return FFrameRenderFunctionUserContext; } set { lock (Lock) FFrameRenderFunctionUserContext = value; } }

        List<Timer> FTimers;
        public List<Timer> Timers { get { lock (Lock) return FTimers; } set { lock (Lock) FTimers = value; } }
        readonly bool[] FKeys = new bool[256];
        public bool[] Keys { get { lock (Lock) return FKeys; } }
        readonly bool[] FLastKeys = new bool[256];
        public bool[] LastKeys { get { lock (Lock) return FLastKeys; } }
        readonly bool[] FMouseButtons = new bool[5];
        public bool[] MouseButtons { get { lock (Lock) return FMouseButtons; } }
        string FStaticFrameStats;
        public string StaticFrameStats { get { lock (Lock) return FStaticFrameStats; } set { lock (Lock) FStaticFrameStats = value; } }
        string FFPSStats;
        public string FPSStats { get { lock (Lock) return FFPSStats; } set { lock (Lock) FFPSStats = value; } }
        string FFrameStats;
        public string FrameStats { get { lock (Lock) return FFrameStats; } set { lock (Lock) FFrameStats = value; } }
        string FDeviceStats;
        public string DeviceStats { get { lock (Lock) return FDeviceStats; } set { lock (Lock) FDeviceStats = value; } }
        string FCounterStats;
        public string CounterStats { get { lock (Lock) return FCounterStats; } set { lock (Lock) FCounterStats = value; } }

        public State()
        {
            FCounterData.GPU_Idle = -1.0f;
            FCounterData.VertexProcessing = -1.0f;
            FCounterData.GeometryProcessing = -1.0f;
            FCounterData.PixelProcessing = -1.0f;
            FCounterData.OtherGPU_Processing = -1.0f;
            FCounterData.HostAdapterBandwidthUtilization = -1.0f;
            FCounterData.LocalVidmemBandwidthUtilization = -1.0f;
            FCounterData.VertexThroughputUtilization = -1.0f;
            FCounterData.TriangleSetupThroughputUtilization = -1.0f;
            FCounterData.FillrateThroughputUtilization = -1.0f;
            FCounterData.VSMemoryLimited = -1.0f;
            FCounterData.VSComputationLimited = -1.0f;
            FCounterData.GSMemoryLimited = -1.0f;
            FCounterData.GSComputationLimited = -1.0f;
            FCounterData.PSMemoryLimited = -1.0f;
            FCounterData.PSComputationLimited = -1.0f;
            FCounterData.PostTransformCacheHitRate = -1.0f;
            FCounterData.TextureCacheHitRate = -1.0f;
        }

        public void Delete()
        {
            Timers = null;
            Functions.Shutdown();
        }

        ~State()
        {
            Delete();
        }
    }
}