using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Xtro.MDX.Generic;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.DXGI;
using Device = Xtro.MDX.Direct3D10.Device;
using DXGI_Error = Xtro.MDX.DXGI.Error;
using DXGI_Device = Xtro.MDX.DXGI.Device;
using DXGI_Functions = Xtro.MDX.DXGI.Functions;
using D3D10Functions = Xtro.MDX.Direct3D10.Functions;

namespace Xtro.MDX.Utilities
{
    public static class Functions
    {
        static State State;
        static TimerClass Timer;

        static State GetState()
        {
            CreateState();

            return State;
        }

        static int DelayLoad()
        {
            var Factory = GetState().Factory;
            if (Factory == null)
            {
                DXGI_Functions.CreateFactory(typeof(Factory), out Factory);
                GetState().Factory = Factory;
                if (Factory == null)
                {
                    // If still NULL, then DXGI is not availible
                    GetState().Direct3D_Available = false;
                    return (int)Error.NoDirect3D;
                }

                GetState().Direct3D_Available = true;
            }

            return 0;
        }

        static Functions()
        {
            AppDomain.CurrentDomain.DomainUnload += OnDomainUnload;
        }

        static void OnDomainUnload(object Sender, EventArgs E)
        {
            DestroyState();
        }

        public static void CreateState()
        {
            if (State == null) State = new State();
        }

        public static void DestroyState()
        {
            if (State != null) State.Delete();
            State = null;
        }

        internal static void GetCallbackDeviceAcceptable(out Callbacks.IsDeviceAcceptable Callback, out object UserContext)
        {
            Callback = GetState().IsDeviceAcceptableFunction;
            UserContext = GetState().IsDeviceAcceptableFunctionUserContext;
        }

        public static void SetCallbackDeviceAcceptable(Callbacks.IsDeviceAcceptable Callback, object UserContext)
        {
            GetState().IsDeviceAcceptableFunction = Callback;
            GetState().IsDeviceAcceptableFunctionUserContext = UserContext;
        }

        public static void SetCallbackDeviceCreated(Callbacks.DeviceCreated Callback, object UserContext)
        {
            GetState().DeviceCreatedFunction = Callback;
            GetState().DeviceCreatedFunctionUserContext = UserContext;
        }

        public static void SetCallbackDeviceDestroyed(Callbacks.DeviceDestroyed Callback, object UserContext)
        {
            GetState().DeviceDestroyedFunction = Callback;
            GetState().DeviceDestroyedFunctionUserContext = UserContext;
        }

        public static void SetCallbackSwapChainResized(Callbacks.SwapChainResized Callback, object UserContext)
        {
            GetState().SwapChainResizedFunction = Callback;
            GetState().SwapChainResizedFunctionUserContext = UserContext;
        }

        public static void SetCallbackSwapChainReleasing(Callbacks.SwapChainReleasing Callback, object UserContext)
        {
            GetState().SwapChainReleasingFunction = Callback;
            GetState().SwapChainReleasingFunctionUserContext = UserContext;
        }

        public static void SetCallbackFrameRender(Callbacks.FrameRender Callback, object UserContext)
        {
            GetState().FrameRenderFunction = Callback;
            GetState().FrameRenderFunctionUserContext = UserContext;
        }

        public static void SetCallbackFrameMove(Callbacks.FrameMove Callback, object UserContext)
        {
            GetState().FrameMoveFunction = Callback;
            GetState().FrameMoveFunctionUserContext = UserContext;
        }

        public static void SetCallbackModifyDeviceSettings(Callbacks.ModifyDeviceSettings Callback, object UserContext)
        {
            GetState().ModifyDeviceSettingsFunction = Callback;
            GetState().ModifyDeviceSettingsFunctionUserContext = UserContext;
        }

        public static Device GetDevice()
        {
            return GetState().Device;
        }

        static void DisplayErrorMessage(int Result)
        {
            string Buffer = null;

            int ExitCode;
            var Found = true;
            switch (Result)
            {
            case (int)Error.NoDirect3D:
                ExitCode = 2;
                Buffer = DoesApplicationSupportDirect3D() ? "Could not initialize Direct3D 10. This application requires a Direct3D 10 class\ndevice (hardware or reference rasterizer) running on Windows Vista (or later)." : "Could not initialize Direct3D 9. Check that the latest version of DirectX is correctly installed on your system.  Also make sure that this program was compiled with header files that match the installed DirectX DLLs.";
                break;
            case (int)Error.NoCompatibleDevices:
                ExitCode = 3;
                Buffer = SystemInformation.TerminalServerSession ? "Direct3D does not work over a remote session." : "Could not find any compatible Direct3D devices.";
                break;
            case (int)Error.MediaNotFound:
                ExitCode = 4;
                Buffer = "Could not find required media.";
                break;
            case (int)Error.NonZeroRefCount:
                ExitCode = 5;
                Buffer = "The Direct3D device has a non-zero reference count, meaning some objects were not released.";
                break;
            case (int)Error.CreatingDevice:
                ExitCode = 6;
                Buffer = "Failed creating the Direct3D device.";
                break;
            case (int)Error.ResettingDevice:
                ExitCode = 7;
                Buffer = "Failed resetting the Direct3D device.";
                break;
            case (int)Error.CreatingDeviceObjects:
                ExitCode = 8;
                Buffer = "An error occurred in the device create callback function.";
                break;
            case (int)Error.ResettingDeviceObjects:
                ExitCode = 9;
                Buffer = "An error occurred in the device reset callback function.";
                break;
            // ExitCode 10 means the app exited using a REF device 
            case (int)Error.DeviceRemoved:
                ExitCode = 11;
                Buffer = "The Direct3D device was removed.";
                break;
            default:
                Found = false;
                ExitCode = 1;
                break; // ExitCode 1 means the API was incorrectly called
            }

            GetState().ExitCode = ExitCode;

            var ShowMessageBoxOnError = GetState().ShowMessageBoxOnError;
            var Form = GetForm();
            if (Found && ShowMessageBoxOnError) MessageBox.Show(Buffer, Form.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }



        //--------------------------------------------------------------------------------------
        // Cleans up the 3D environment by:
        //      - Calls the device lost callback 
        //      - Calls the device destroyed callback 
        //      - Releases the D3D device
        //--------------------------------------------------------------------------------------
        static void CleanupEnvironment(bool ReleaseSettings)
        {
            var Device = GetDevice();

            if (Device != null)
            {
                if (GetState().RasterizerState != null) GetState().RasterizerState.Release();
                // Call ClearState to avoid tons of messy debug spew telling us that we're deleting bound objects
                Device.ClearState();

                // Call the app's SwapChain lost callback
                GetState().InsideDeviceCallback = true;
                if (GetState().DeviceObjectsReset)
                {
                    var CallbackSwapChainReleasing = GetState().SwapChainReleasingFunction;
                    if (CallbackSwapChainReleasing != null) CallbackSwapChainReleasing(GetState().SwapChainReleasingFunctionUserContext);
                    GetState().DeviceObjectsReset = false;
                }

                // Release our old depth stencil texture and view 
                var DepthStencil = GetState().DepthStencil;
                if (DepthStencil != null) DepthStencil.Release();
                GetState().DepthStencil = null;
                var DepthStencilView = GetState().DepthStencilView;
                if (DepthStencilView != null) DepthStencilView.Release();
                GetState().DepthStencilView = null;

                // Release our rasterizer state
                var RasterizerState = GetState().DefaultRasterizerState;
                if (RasterizerState != null) RasterizerState.Release();
                GetState().DefaultRasterizerState = null;

                // Cleanup the render target view
                var RenderTargetView = GetState().RenderTargetView;
                if (RenderTargetView != null) RenderTargetView.Release();
                GetState().RenderTargetView = null;

                // Call the app's device destroyed callback
                if (GetState().DeviceObjectsCreated)
                {
                    var CallbackDeviceDestroyed = GetState().DeviceDestroyedFunction;
                    if (CallbackDeviceDestroyed != null) CallbackDeviceDestroyed(GetState().DeviceDestroyedFunctionUserContext);
                    GetState().DeviceObjectsCreated = false;
                }

                GetState().InsideDeviceCallback = false;

                // Release the swap chain
                GetState().ReleasingSwapChain = true;
                var SwapChain = GetSwapChain();
                if (SwapChain != null)
                {
                    SwapChain.SetFullscreenState(false, null);
                    SwapChain.Release();
                }
                GetState().SwapChain = null;
                GetState().ReleasingSwapChain = false;

                // Release the outputs.
                var Outputs = GetState().Outputs;
                foreach (var Output in Outputs)
                {
                    if (Output != null) Output.Release();
                }
                GetState().Outputs = null;

                // Release the D3D adapter.
                var Adapter = GetState().Adapter;
                if (Adapter != null) Adapter.Release();
                GetState().Adapter = null;

                // Release the counters
                DestroyCounters();

                // Release the D3D device and in debug configs, displays a message box if there 
                // are unrelease objects.
                var References = Device.Release();
                if (References > 0) DisplayErrorMessage((int)Error.NonZeroRefCount);
                GetState().Device = null;

                if (ReleaseSettings) GetState().CurrentDeviceSettings = null;

                GetState().BackBufferSurfaceDescription = new SurfaceDescription();

                GetState().DeviceCreated = false;
            }
        }

        static void DestroyCounters()
        {
            var Counter = GetState().CounterGPU_Idle;
            if (Counter != null) Counter.Release();
            GetState().CounterGPU_Idle = null;

            Counter = GetState().CounterVertexProcessing;
            if (Counter != null) Counter.Release();
            GetState().CounterVertexProcessing = null;

            Counter = GetState().CounterGeometryProcessing;
            if (Counter != null) Counter.Release();
            GetState().CounterGeometryProcessing = null;

            Counter = GetState().CounterPixelProcessing;
            if (Counter != null) Counter.Release();
            GetState().CounterPixelProcessing = null;

            Counter = GetState().CounterOtherGPU_Processing;
            if (Counter != null) Counter.Release();
            GetState().CounterOtherGPU_Processing = null;

            Counter = GetState().CounterHostAdapterBandwidthUtilization;
            if (Counter != null) Counter.Release();
            GetState().CounterHostAdapterBandwidthUtilization = null;

            Counter = GetState().CounterLocalVidmemBandwidthUtilization;
            if (Counter != null) Counter.Release();
            GetState().CounterLocalVidmemBandwidthUtilization = null;

            Counter = GetState().CounterVertexThroughputUtilization;
            if (Counter != null) Counter.Release();
            GetState().CounterVertexThroughputUtilization = null;

            Counter = GetState().CounterTriangleSetupThroughputUtilization;
            if (Counter != null) Counter.Release();
            GetState().CounterTriangleSetupThroughputUtilization = null;

            Counter = GetState().CounterFillrateThrougputUtilization;
            if (Counter != null) Counter.Release();
            GetState().CounterFillrateThrougputUtilization = null;

            Counter = GetState().CounterVS_MemoryLimited;
            if (Counter != null) Counter.Release();
            GetState().CounterVS_MemoryLimited = null;

            Counter = GetState().CounterVS_ComputationLimited;
            if (Counter != null) Counter.Release();
            GetState().CounterVS_ComputationLimited = null;

            Counter = GetState().CounterGS_MemoryLimited;
            if (Counter != null) Counter.Release();
            GetState().CounterGS_MemoryLimited = null;

            Counter = GetState().CounterGS_ComputationLimited;
            if (Counter != null) Counter.Release();
            GetState().CounterGS_ComputationLimited = null;

            Counter = GetState().CounterPS_MemoryLimited;
            if (Counter != null) Counter.Release();
            GetState().CounterPS_MemoryLimited = null;

            Counter = GetState().CounterPS_ComputationLimited;
            if (Counter != null) Counter.Release();
            GetState().CounterPS_ComputationLimited = null;

            Counter = GetState().CounterPostTransformCacheHitRate;
            if (Counter != null) Counter.Release();
            GetState().CounterPostTransformCacheHitRate = null;

            Counter = GetState().CounterTextureCacheHitRate;
            if (Counter != null) Counter.Release();
            GetState().CounterTextureCacheHitRate = null;
        }

        public static SwapChain GetSwapChain()
        {
            return GetState().SwapChain;
        }

        //--------------------------------------------------------------------------------------
        // Pauses time or rendering.  Keeps a ref count so pausing can be layered
        //--------------------------------------------------------------------------------------
        public static void Pause(bool PauseTime, bool PauseRendering)
        {
            var PauseTimeCount = GetState().PauseTimeCount;
            if (PauseTime) PauseTimeCount++;
            else PauseTimeCount--;
            if (PauseTimeCount < 0) PauseTimeCount = 0;
            GetState().PauseTimeCount = PauseTimeCount;

            var PauseRenderingCount = GetState().PauseRenderingCount;
            if (PauseRendering) PauseRenderingCount++;
            else PauseRenderingCount--;
            if (PauseRenderingCount < 0) PauseRenderingCount = 0;
            GetState().PauseRenderingCount = PauseRenderingCount;

            if (PauseTimeCount > 0)
            {
                // Stop the scene from animating
                GetGlobalTimer().Stop();
            }
            else
            {
                // Restart the timer
                GetGlobalTimer().Start();
            }

            GetState().RenderingPaused = PauseRenderingCount > 0;
            GetState().TimePaused = PauseTimeCount > 0;
        }

        //--------------------------------------------------------------------------------------
        // Resets the 3D environment by:
        //      - Calls the device lost callback 
        //      - Resets the device
        //      - Stores the back buffer description
        //      - Sets up the full screen Direct3D cursor if requested
        //      - Calls the device reset callback 
        //--------------------------------------------------------------------------------------
        static int ResetEnvironment()
        {
            int Result;

            GetState().DeviceObjectsReset = false;
            Pause(true, true);

            var DeferredDXGIAction = false;
            var DeviceSettings = GetState().CurrentDeviceSettings;
            var SwapChain = GetSwapChain();

            SwapChainDescription SwapChainDescription;
            SwapChain.GetDescription(out SwapChainDescription);

            // Resize backbuffer and target of the swapchain in case they have changed.
            // For windowed mode, use the client rect as the desired size. Unlike D3D9,
            // we can't use 0 for width or height.  Therefore, fill in the values from
            // the window size. For fullscreen mode, the width and height should have
            // already been filled with the desktop resolution, so don't change it.
            if (DeviceSettings.SwapChainDescription.Windowed && SwapChainDescription.Windowed)
            {
                var Rectangle = GetForm().ClientRectangle;
                DeviceSettings.SwapChainDescription.BufferDescription.Width = (uint)Rectangle.Width;
                DeviceSettings.SwapChainDescription.BufferDescription.Height = (uint)Rectangle.Height;
            }

            // If the app wants to switch from windowed to fullscreen or vice versa,
            // call the swapchain's SetFullscreenState
            // mode.
            if (SwapChainDescription.Windowed != DeviceSettings.SwapChainDescription.Windowed)
            {
                // Set the fullscreen state
                if (DeviceSettings.SwapChainDescription.Windowed)
                {
                    Result = SwapChain.SetFullscreenState(false, null);
                    if (Result < 0) return Result;
                    DeferredDXGIAction = true;
                }
                else
                {
                    // Set fullscreen state by setting the display mode to fullscreen, then changing the resolution
                    // to the desired value.

                    // SetFullscreenState causes a WM_SIZE message to be sent to the window.  The WM_SIZE message calls
                    // DXUTCheckForDXGIBufferChange which normally stores the new height and width in 
                    // DeviceSettings->d3d10.sd.BufferDesc.  SetDoNotStoreBufferSize tells DXUTCheckForDXGIBufferChange
                    // not to store the height and width so that we have the correct values when calling ResizeTarget.

                    GetState().DoNotStoreBufferSize = true;
                    Result = SwapChain.SetFullscreenState(true, null);
                    if (Result < 0) return Result;
                    GetState().DoNotStoreBufferSize = false;

                    Result = SwapChain.ResizeTarget(ref DeviceSettings.SwapChainDescription.BufferDescription);
                    if (Result < 0) return Result;
                    DeferredDXGIAction = true;
                }
            }
            else
            {
                if (DeviceSettings.SwapChainDescription.BufferDescription.Width == SwapChainDescription.BufferDescription.Width &&
                    DeviceSettings.SwapChainDescription.BufferDescription.Height == SwapChainDescription.BufferDescription.Height &&
                    DeviceSettings.SwapChainDescription.BufferDescription.Format != SwapChainDescription.BufferDescription.Format)
                {
                    ResizeBuffers(0, 0, !DeviceSettings.SwapChainDescription.Windowed);
                    DeferredDXGIAction = true;
                }
                else if (DeviceSettings.SwapChainDescription.BufferDescription.Width != SwapChainDescription.BufferDescription.Width ||
                         DeviceSettings.SwapChainDescription.BufferDescription.Height != SwapChainDescription.BufferDescription.Height)
                {
                    Result = SwapChain.ResizeTarget(ref DeviceSettings.SwapChainDescription.BufferDescription);
                    if (Result < 0) return Result;

                    DeferredDXGIAction = true;
                }
            }

            // If no deferred DXGI actions are to take place, mark the device as reset.
            // If there is a deferred DXGI action, then the device isn't reset until DXGI sends us a 
            // window message.  Only then can we mark the device as reset.
            if (!DeferredDXGIAction) GetState().DeviceObjectsReset = true;
            Pause(false, false);

            return 0;
        }

        //--------------------------------------------------------------------------------------
        // Stores back buffer surface desc in GetDXUTState().GetBackBufferSurfaceDesc10()
        //--------------------------------------------------------------------------------------
        static void UpdateBackBufferDescription()
        {
            Unknown Surface;
            var Result = GetState().SwapChain.GetBuffer(0, typeof(Texture2D), out Surface);
            var BackBuffer = (Texture2D)Surface;
            var BackBufferSurfaceDescription = new SurfaceDescription();
            if (Result >= 0 && BackBuffer != null)
            {
                Texture2D_Description TextureDescription;
                BackBuffer.GetDescription(out TextureDescription);
                BackBufferSurfaceDescription.Width = TextureDescription.Width;
                BackBufferSurfaceDescription.Height = TextureDescription.Height;
                BackBufferSurfaceDescription.Format = TextureDescription.Format;
                BackBufferSurfaceDescription.SampleDescription = TextureDescription.SampleDescription;
                BackBuffer.Release();
            }

            GetState().BackBufferSurfaceDescription = BackBufferSurfaceDescription;
        }

        //--------------------------------------------------------------------------------------
        // Updates the static part of the frame stats so it doesn't have be generated every frame
        //--------------------------------------------------------------------------------------
        static void UpdateStaticFrameStats()
        {
            if (GetState().NoStats) return;

            var DeviceSettings = GetState().CurrentDeviceSettings;
            if (DeviceSettings == null) return;

            var Enumeration = GetEnumeration();
            if (Enumeration == null) return;

            var DeviceSettingsCombo = Enumeration.GetDeviceSettingsCombo(DeviceSettings.AdapterOrdinal, DeviceSettings.SwapChainDescription.BufferDescription.Format, DeviceSettings.SwapChainDescription.Windowed);
            if (DeviceSettingsCombo == null) return;

            var FormatText = DeviceSettingsCombo.BackBufferFormat.ToString();

            var MultiSampleText = " (MS" + DeviceSettings.SwapChainDescription.SampleDescription.Count + ", Q" + DeviceSettings.SwapChainDescription.SampleDescription.Quality + ")";

            GetState().StaticFrameStats = "D3D10 Vsync " + ((DeviceSettings.SyncInterval == 0) ? "off" : "on") + " (" +
                DeviceSettings.SwapChainDescription.BufferDescription.Width + "x" + DeviceSettings.SwapChainDescription.BufferDescription.Height + "), " +
                FormatText + MultiSampleText;
        }

        //--------------------------------------------------------------------------------------
        // Updates the string which describes the device 
        //--------------------------------------------------------------------------------------
        static void UpdateDeviceStats(DriverType DeviceType, ref AdapterDescription AdapterDescription)
        {
            if (GetState().NoStats) return;

            // Store device description
            string DeviceStats = null;
            switch (DeviceType)
            {
            case DriverType.Reference:
                DeviceStats = "REFERENCE";
                break;
            case DriverType.Hardware:
                DeviceStats = "HARDWARE";
                break;
            }

            if (DeviceType == DriverType.Hardware)
            {
                // Be sure not to overflow m_strDeviceStats when appending the adapter 
                // description, since it can be long.  
                DeviceStats += ": ";

                // Try to get a unique description from the CD3D10EnumDeviceSettingsCombo
                var DeviceSettings = GetState().CurrentDeviceSettings;
                if (DeviceSettings == null) return;

                var Enumeration = GetEnumeration();
                var DeviceSettingsCombo = Enumeration.GetDeviceSettingsCombo(DeviceSettings.AdapterOrdinal, DeviceSettings.SwapChainDescription.BufferDescription.Format, DeviceSettings.SwapChainDescription.Windowed);
                if (DeviceSettingsCombo != null) DeviceStats += DeviceSettingsCombo.AdapterInfo.UniqueDescription;
                else DeviceStats += AdapterDescription.Description;
            }

            GetState().DeviceStats = DeviceStats;
        }

        //--------------------------------------------------------------------------------------
        // Sets the viewport, creates a render target view, and depth scencil texture and view.
        //--------------------------------------------------------------------------------------
        static int SetupViews(Device Device, DeviceSettings DeviceSettings)
        {
            var SwapChain = GetSwapChain();
            DepthStencilView DepthStencilView;
            RenderTargetView RenderTargetView;

            // Get the back buffer and desc
            Unknown Surface;
            var Result = SwapChain.GetBuffer(0, typeof(Texture2D), out Surface);
            var BackBuffer = (Texture2D)Surface;

            if (Result < 0) return Result;
            Texture2D_Description BackBufferSurfaceDescription;
            BackBuffer.GetDescription(out BackBufferSurfaceDescription);

            // Setup the viewport to match the backbuffer
            Viewport Viewport;
            Viewport.Width = BackBufferSurfaceDescription.Width;
            Viewport.Height = BackBufferSurfaceDescription.Height;
            Viewport.MinDepth = 0;
            Viewport.MaxDepth = 1;
            Viewport.TopLeftX = 0;
            Viewport.TopLeftY = 0;
            Device.RS_SetViewports(1, new[] { Viewport });

            // Create the render target view
            Result = Device.CreateRenderTargetView(BackBuffer, out RenderTargetView);
            BackBuffer.Release();
            if (Result < 0) return Result;
            GetState().RenderTargetView = RenderTargetView;

            if (DeviceSettings.AutoCreateDepthStencil)
            {
                // Create depth stencil texture
                Texture2D DepthStencil;
                var DepthStencilDescription = new Texture2D_Description
                {
                    Width = BackBufferSurfaceDescription.Width,
                    Height = BackBufferSurfaceDescription.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = DeviceSettings.AutoDepthStencilFormat,
                    SampleDescription =
                    {
                        Count = DeviceSettings.SwapChainDescription.SampleDescription.Count,
                        Quality = DeviceSettings.SwapChainDescription.SampleDescription.Quality
                    },
                    Usage = Usage.Default,
                    BindFlags = BindFlag.DepthStencil,
                    CPU_AccessFlags = 0,
                    MiscellaneousFlags = 0
                };
                Result = Device.CreateTexture2D(ref DepthStencilDescription, out DepthStencil);
                if (Result < 0) return Result;
                GetState().DepthStencil = DepthStencil;

                // Create the depth stencil view
                var DepthStencilViewDescription = new DepthStencilViewDescription
                {
                    Format = DepthStencilDescription.Format,
                    ViewDimension = DepthStencilDescription.SampleDescription.Count > 1 ? DepthStencilViewDimension.Texture2D_MultiSampling : DepthStencilViewDimension.Texture2D,
                    Texture2D =
                    {
                        MipSlice = 0
                    }
                };
                Result = Device.CreateDepthStencilView(DepthStencil, ref DepthStencilViewDescription, out DepthStencilView);
                if (Result < 0) return Result;
                GetState().DepthStencilView = DepthStencilView;

                // Create a default rasterizer state that enables MSAA
                var RasterizerDescription = new RasterizerDescription
                {
                    FillMode = FillMode.Solid,
                    CullMode = CullMode.Back,
                    FrontCounterClockwise = false,
                    DepthBias = 0,
                    SlopeScaledDepthBias = 0.0f,
                    DepthBiasClamp = 0,
                    DepthClipEnable = true,
                    ScissorEnable = false,
                    AntialiasedLineEnable = false,
                    MultisampleEnable = DepthStencilDescription.SampleDescription.Count > 1
                };

                RasterizerState RasterizerState;
                Result = Device.CreateRasterizerState(ref RasterizerDescription, out RasterizerState);
                if (Result < 0) return Result;

                GetState().DefaultRasterizerState = RasterizerState;
                Device.RS_SetState(RasterizerState);
            }

            // Set the render targets
            DepthStencilView = GetState().DepthStencilView;
            Device.OM_SetRenderTargets(1, new[] { RenderTargetView }, DepthStencilView);

            return Result;
        }

        static bool GetIsWindowedFromDeviceSettings(DeviceSettings NewDeviceSettings)
        {
            return NewDeviceSettings == null || NewDeviceSettings.SwapChainDescription.Windowed;
        }

        public static bool IsWindowed()
        {
            return GetIsWindowedFromDeviceSettings(GetState().CurrentDeviceSettings);
        }

        public static SurfaceDescription GetBackBufferSurfaceDescription() { return GetState().BackBufferSurfaceDescription; }

        //--------------------------------------------------------------------------------------
        // Setup cursor based on current settings (window/fullscreen mode, show cursor state, clip cursor state)
        //--------------------------------------------------------------------------------------
        static void SetupCursor()
        {
            // Clip cursor if requested
            if (!IsWindowed() && GetState().ClipCursorWhenFullScreen) Cursor.Clip = GetForm().DesktopBounds;
            else Cursor.Clip = Rectangle.Empty;
        }

        static void ResizeBuffers(uint Width, uint Height, bool FullScreen)
        {
            var Device = GetDevice();
            var CurrentClient = GetForm().ClientRectangle;

            var DeviceSettings = GetState().CurrentDeviceSettings;

            var SwapChain = GetSwapChain();

            // Determine if we're fullscreen
            DeviceSettings.SwapChainDescription.Windowed = !FullScreen;

            // Call releasing
            GetState().InsideDeviceCallback = true;
            var CallbackSwapChainReleasing = GetState().SwapChainReleasingFunction;
            if (CallbackSwapChainReleasing != null) CallbackSwapChainReleasing(GetState().SwapChainResizedFunctionUserContext);
            GetState().InsideDeviceCallback = false;

            // Release our old depth stencil texture and view 
            var DepthStencil = GetState().DepthStencil;
            if (DepthStencil != null) DepthStencil.Release();
            GetState().DepthStencil = null;
            var DepthStencilView = GetState().DepthStencilView;
            if (DepthStencilView != null) DepthStencilView.Release();
            GetState().DepthStencilView = null;

            // Release our old render target view
            var RenderTargetView = GetState().RenderTargetView;
            if (RenderTargetView != null) RenderTargetView.Release();
            GetState().RenderTargetView = null;

            // Release our rasterizer state
            var RasterizerState = GetState().DefaultRasterizerState;
            if (RasterizerState != null) RasterizerState.Release();
            GetState().DefaultRasterizerState = null;

            // Alternate between 0 and DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH when resizing buffers.
            // When in windowed mode, we want 0 since this allows the app to change to the desktop
            // resolution from windowed mode during alt+enter.  However, in fullscreen mode, we want
            // the ability to change display modes from the Device Settings dialog.  Therefore, we
            // want to set the DXGI_SWAP_CHAIN_FLAG_ALLOW_MODE_SWITCH flag.
            SwapChainFlag Flags = 0;
            if (FullScreen) Flags = SwapChainFlag.AllowModeSwitch;

            // ResizeBuffers
            SwapChain.ResizeBuffers(DeviceSettings.SwapChainDescription.BufferCount, Width, Height, DeviceSettings.SwapChainDescription.BufferDescription.Format, Flags);

            if (!GetState().DoNotStoreBufferSize)
            {
                DeviceSettings.SwapChainDescription.BufferDescription.Width = (uint)CurrentClient.Right;
                DeviceSettings.SwapChainDescription.BufferDescription.Height = (uint)CurrentClient.Bottom;
            }

            // Save off backbuffer desc
            UpdateBackBufferDescription();

            // Update the device stats text
            UpdateStaticFrameStats();

            // Setup the render target view and viewport
            var Result = SetupViews(Device, DeviceSettings);
            if (Result < 0) return;

            // Setup cursor based on current settings (window/fullscreen mode, show cursor state, clip cursor state)
            SetupCursor();

            // Call the app's SwapChain reset callback
            GetState().InsideDeviceCallback = true;
            var BackBufferSurfaceDescription = GetBackBufferSurfaceDescription();
            var CallbackSwapChainResized = GetState().SwapChainResizedFunction;
            Result = 0;
            if (CallbackSwapChainResized != null) Result = CallbackSwapChainResized(Device, SwapChain, ref BackBufferSurfaceDescription, GetState().SwapChainResizedFunctionUserContext);
            GetState().InsideDeviceCallback = false;
            if (Result < 0)
            {
                // If callback failed, cleanup
                // if( Result !=(int)Error.MediaNotFound)Result =(int)Error.ResettingDeviceObjects;

                GetState().InsideDeviceCallback = true;
                var CallbackSwapChainReleasing2 = GetState().SwapChainReleasingFunction;
                if (CallbackSwapChainReleasing2 != null) CallbackSwapChainReleasing2(GetState().SwapChainResizedFunctionUserContext);
                GetState().InsideDeviceCallback = false;
                Pause(false, false);
                GetForm().Close();
            }
            else
            {
                GetState().DeviceObjectsReset = true;
                Pause(false, false);
            }
        }

        //--------------------------------------------------------------------------------------
        // Check if the new device is close enough to the old device to simply reset or 
        // resize the back buffer
        //--------------------------------------------------------------------------------------
        static bool CanDeviceBeReset(DeviceSettings OldDeviceSettings, DeviceSettings NewDeviceSettings, Device DeviceFromApplication)
        {
            if (OldDeviceSettings == null) return false;

            return GetDevice() != null &&
                   (DeviceFromApplication == null || DeviceFromApplication == GetDevice()) &&
                   (OldDeviceSettings.AdapterOrdinal == NewDeviceSettings.AdapterOrdinal) &&
                   (OldDeviceSettings.DriverType == NewDeviceSettings.DriverType) &&
                   (OldDeviceSettings.CreateFlags == NewDeviceSettings.CreateFlags) &&
                   (OldDeviceSettings.SwapChainDescription.SampleDescription.Count == NewDeviceSettings.SwapChainDescription.SampleDescription.Count) &&
                   (OldDeviceSettings.SwapChainDescription.SampleDescription.Quality == NewDeviceSettings.SwapChainDescription.SampleDescription.Quality);
        }

        //--------------------------------------------------------------------------------------
        // Returns the HMONITOR attached to an adapter/output
        //--------------------------------------------------------------------------------------
        static Screen GetMonitorFromAdapter(DeviceSettings DeviceSettings)
        {
            var Enumeration = GetEnumeration();
            var OutputInfo = Enumeration.GetOutputInfo(DeviceSettings.AdapterOrdinal, DeviceSettings.Output);

            return OutputInfo == null ? null : Screen.FromRectangle(OutputInfo.Description.DesktopCoordinates);
        }

        public static RenderTargetView GetRenderTargetView()
        {
            return GetState().RenderTargetView;
        }

        public static DepthStencilView GetDepthStencilView()
        {
            return GetState().DepthStencilView;
        }

        public static DeviceSettings GetDeviceSettings()
        {
            // Return a copy of device settings of the current device.  If no device exists yet, then
            // return a blank device settings struct
            var DeviceSettings = GetState().CurrentDeviceSettings;
            return DeviceSettings != null ? DeviceSettings.Copy() : new DeviceSettings();
        }

        //--------------------------------------------------------------------------------------
        // Creates the 3D environment
        //--------------------------------------------------------------------------------------
        static int CreateEnvironment(Device DeviceFromApplication)
        {
            int Result;

            Device Device = null;
            SwapChain SwapChain = null;
            var NewDeviceSettings = GetState().CurrentDeviceSettings;

            var Factory = GetFactory();
            // DXUT bug. this should be called after CreateSwapChain: Factory.MakeWindowAssociation(IntPtr.Zero, 0);

            // Only create a Direct3D device if one hasn't been supplied by the app
            if (DeviceFromApplication == null)
            {
                // Try to create the device with the chosen settings
                Adapter Adapter = null;

                Result = 0;
                if (NewDeviceSettings.DriverType == DriverType.Hardware) Result = Factory.EnumAdapters(NewDeviceSettings.AdapterOrdinal, out Adapter);
                if (Result >= 0)
                {
                    Result = D3D10Functions.CreateDevice(Adapter, NewDeviceSettings.DriverType, null, NewDeviceSettings.CreateFlags, out Device);

                    if (Result >= 0)
                    {
                        if (NewDeviceSettings.DriverType != DriverType.Hardware)
                        {
                            object Object;
                            Result = Device.QueryInterface(typeof(DXGI_Device), out Object);
                            var Device2 = (DXGI_Device)Object;
                            if (Result >= 0 && Device2 != null) Device2.GetAdapter(out Adapter);
                            if (Device2 != null) Device2.Release();
                        }

                        GetState().Adapter = Adapter;
                    }
                }

                if (Result < 0 || Adapter == null) return (int)Error.CreatingDevice;

                // Enumerate its outputs.
                var Outputs = new List<Output>();
                for (uint OutputNo = 0; ; OutputNo++)
                {
                    Output Output;
                    if (Adapter.EnumerateOutputs(OutputNo, out Output) < 0) break;
                    Outputs.Add(Output);
                }
                GetState().Outputs = Outputs;

                // Create the swapchain
                Result = Factory.CreateSwapChain(Device, ref NewDeviceSettings.SwapChainDescription, out SwapChain);
                if (Result < 0) return (int)Error.CreatingDevice;
                Factory.MakeWindowAssociation(GetForm().Handle, MakeWindowAssociationFlag.NoWindowChanges | MakeWindowAssociationFlag.NoAltEnter | MakeWindowAssociationFlag.NoPrintScreen);
            }
            else
            {
                DeviceFromApplication.AddRef();
                Device = DeviceFromApplication;
            }

            GetState().Device = Device;
            GetState().SwapChain = SwapChain;

            // If switching to REF, set the exit code to 10.  If switching to HAL and exit code was 10, then set it back to 0.
            if (NewDeviceSettings.DriverType == DriverType.Reference && GetState().ExitCode == 0) GetState().ExitCode = 10;
            else if (NewDeviceSettings.DriverType == DriverType.Hardware && GetState().ExitCode == 10) GetState().ExitCode = 0;

            // Update back buffer desc before calling app's device callbacks
            UpdateBackBufferDescription();

            // Setup cursor based on current settings (window/fullscreen mode, show cursor state, clip cursor state)
            SetupCursor();

            // Update the device stats text
            var Enumeration = GetEnumeration();
            var AdapterInfo = Enumeration.GetAdapterInfo(NewDeviceSettings.AdapterOrdinal);
            UpdateDeviceStats(NewDeviceSettings.DriverType, ref AdapterInfo.AdapterDescription);

            // Call the app's device created callback if non-NULL
            var BackBufferSurfaceDescription = GetBackBufferSurfaceDescription();
            GetState().InsideDeviceCallback = true;
            var CallbackDeviceCreated = GetState().DeviceCreatedFunction;
            Result = 0;
            if (CallbackDeviceCreated != null) Result = CallbackDeviceCreated(GetDevice(), ref BackBufferSurfaceDescription, GetState().DeviceCreatedFunctionUserContext);
            GetState().InsideDeviceCallback = false;
            if (GetDevice() == null) return (int)Error.Fail; // Handle DXUTShutdown from inside callback
            if (Result < 0) return (int)((Result == (int)Error.MediaNotFound) ? Error.MediaNotFound : Error.CreatingDeviceObjects);
            GetState().DeviceObjectsCreated = true;

            // Setup the render target view and viewport
            Result = SetupViews(Device, NewDeviceSettings);
            if (Result < 0) return (int)Error.CreatingDeviceObjects;

            // Create performance counters
            //DXUTCreateD3D10Counters( pd3d10Device );

            // Call the app's swap chain reset callback if non-NULL
            GetState().InsideDeviceCallback = true;
            var CallbackSwapChainResized = GetState().SwapChainResizedFunction;
            Result = 0;
            if (CallbackSwapChainResized != null)
                Result = CallbackSwapChainResized(GetDevice(), SwapChain, ref BackBufferSurfaceDescription, GetState().SwapChainResizedFunctionUserContext);
            GetState().InsideDeviceCallback = false;
            if (GetDevice() == null) return (int)Error.Fail; // Handle DXUTShutdown from inside callback
            if (Result < 0) return (int)((Result == (int)Error.MediaNotFound) ? Error.MediaNotFound : Error.ResettingDeviceObjects);
            GetState().DeviceObjectsReset = true;

            return 0;
        }

        //--------------------------------------------------------------------------------------
        // All device changes are sent to this function.  It looks at the current 
        // device (if any) and the new device and determines the best course of action.  It 
        // also remembers and restores the window state if toggling between windowed and fullscreen
        // as well as sets the proper window and system state for switching to the new device.
        //--------------------------------------------------------------------------------------
        static int ChangeDevice(DeviceSettings NewDeviceSettings, Device DeviceFromApplication, bool ForceRecreate, bool ClipWindowToSingleAdapter)
        {
            var OldDeviceSettings = GetState().CurrentDeviceSettings;

            if (NewDeviceSettings == null) return (int)Error.False;

            var Result = DelayLoad();
            if (Result < 0) return (int)Error.NoDirect3D;

            // Make a copy of the NewDeviceSettings on the heap
            NewDeviceSettings = NewDeviceSettings.Copy();

            // If the ModifyDeviceSettings callback is non-NULL, then call it to let the app 
            // change the settings or reject the device change by returning false.
            var CallbackModifyDeviceSettings = GetState().ModifyDeviceSettingsFunction;
            if (CallbackModifyDeviceSettings != null && DeviceFromApplication == null)
            {
                var Continue = CallbackModifyDeviceSettings(NewDeviceSettings, GetState().ModifyDeviceSettingsFunctionUserContext);
                if (!Continue)
                {
                    // The app rejected the device change by returning false, so just use the current device if there is one.
                    if (OldDeviceSettings == null) DisplayErrorMessage((int)Error.NoCompatibleDevices);
                    return (int)Error.Abort;
                }
                if (GetState().Factory == null) return (int)Error.False; // if Shutdown() was called in the modify callback, just return
            }

            GetState().CurrentDeviceSettings = NewDeviceSettings;

            Pause(true, true);

            // Take notice if the backbuffer width & height are 0 now as they will change after Device->Reset()
            var KeepCurrentWindowSize = false;
            if (NewDeviceSettings.SwapChainDescription.BufferDescription.Width == 0 && NewDeviceSettings.SwapChainDescription.BufferDescription.Height == 0) KeepCurrentWindowSize = true;

            //////////////////////////
            // Before reset
            /////////////////////////

            if (GetIsWindowedFromDeviceSettings(NewDeviceSettings))
            {
                // Going to windowed mode
                if (OldDeviceSettings != null && !GetIsWindowedFromDeviceSettings(OldDeviceSettings))
                {
                    // Going from fullscreen -> windowed
                    GetState().FullScreenBackBufferWidthAtModeChange = OldDeviceSettings.SwapChainDescription.BufferDescription.Width;
                    GetState().FullScreenBackBufferHeightAtModeChange = OldDeviceSettings.SwapChainDescription.BufferDescription.Height;
                }
            }
            else
            {
                // Going to fullscreen mode
                if (OldDeviceSettings == null || GetIsWindowedFromDeviceSettings(OldDeviceSettings))
                {
                    // Transistioning to full screen mode from a standard window so 
                    if (OldDeviceSettings != null)
                    {
                        GetState().WindowBackBufferWidthAtModeChange = OldDeviceSettings.SwapChainDescription.BufferDescription.Width;
                        GetState().WindowBackBufferHeightAtModeChange = OldDeviceSettings.SwapChainDescription.BufferDescription.Height;
                    }
                }
            }

            // If API version, AdapterOrdinal and DeviceType are the same, we can just do a Reset().
            // If they've changed, we need to do a complete device tear down/rebuild.
            // Also only allow a reset if Device is the same as the current device 
            if (!ForceRecreate && CanDeviceBeReset(OldDeviceSettings, NewDeviceSettings, DeviceFromApplication))
            {
                // Reset the Direct3D device and call the app's device callbacks
                Result = ResetEnvironment();
                if (Result < 0)
                {
                    if (Result == (int)Error.ResettingDeviceObjects || Result == (int)Error.MediaNotFound)
                    {
                        // Something bad happened in the app callbacks
                        DisplayErrorMessage(Result);
                        Shutdown();
                        return Result;
                    }

                    // else // DXUTERR_RESETTINGDEVICE

                    // Reset failed and the device wasn't lost and it wasn't the apps fault, 
                    // so recreate the device to try to recover
                    GetState().CurrentDeviceSettings = OldDeviceSettings;
                    if (ChangeDevice(NewDeviceSettings, DeviceFromApplication, true, ClipWindowToSingleAdapter) < 0)
                    {
                        // If that fails, then shutdown
                        Shutdown();
                        return (int)Error.CreatingDevice;
                    }
                    Pause(false, false);
                    return 0;
                }
            }
            else
            {
                // Cleanup if not first device created
                if (OldDeviceSettings != null) CleanupEnvironment(false);

                // Create the D3D device and call the app's device callbacks
                Result = CreateEnvironment(DeviceFromApplication);
                if (Result < 0)
                {
                    CleanupEnvironment(true);
                    DisplayErrorMessage(Result);
                    Pause(false, false);
                    GetState().IgnoreSizeChange = false;
                    return Result;
                }
            }

            var AdapterMonitor = GetMonitorFromAdapter(NewDeviceSettings);
            GetState().AdapterMonitor = AdapterMonitor;

            // Update the device stats text
            UpdateStaticFrameStats();

            if (OldDeviceSettings != null && !GetIsWindowedFromDeviceSettings(OldDeviceSettings) && GetIsWindowedFromDeviceSettings(NewDeviceSettings))
            {
                // Going from fullscreen -> windowed

                // Restore the show state, and positions/size of the window to what it was
                // It is important to adjust the window size 
                // after resetting the device rather than beforehand to ensure 
                // that the monitor resolution is correct and does not limit the size of the new window.
                var WindowedPlacement = GetState().WindowedPlacement;
                WindowedPlacement.Length = Marshal.SizeOf(WindowedPlacement);
                Windows.SetWindowPlacement(GetForm().Handle, ref WindowedPlacement);

                // Also restore the z-order of window to previous state
                GetForm().TopMost = GetState().TopmostWhileWindowed;
            }

            // Check to see if the window needs to be resized.  
            // Handle cases where the window is minimized and maxmimized as well.
            var NeedToResize = false;
            if (GetIsWindowedFromDeviceSettings(NewDeviceSettings) && // only resize if in windowed mode
                !KeepCurrentWindowSize) // only resize if pp.BackbufferWidth/Height were not 0
            {
                var Client = new Rectangle();
                if (GetForm().WindowState == FormWindowState.Minimized)
                {
                    // Window is currently minimized. To tell if it needs to resize, 
                    // get the client rect of window when its restored the 
                    // hard way using GetWindowPlacement()
                    var WindowedPlacement = new Windows.WindowPlacement();
                    WindowedPlacement.Length = Marshal.SizeOf(WindowedPlacement);
                    Windows.GetWindowPlacement(GetForm().Handle, ref WindowedPlacement);

                    if ((WindowedPlacement.Flags & Windows.WindowPlacementFlag.RestoreToMaximized) != 0 && WindowedPlacement.ShowCommand == Windows.ShowWindowCommand.ShowMinimized)
                    {
                        // WPF_RESTORETOMAXIMIZED means that when the window is restored it will
                        // be maximized.  So maximize the window temporarily to get the client rect 
                        // when the window is maximized.  GetSystemMetrics( SM_CXMAXIMIZED ) will give this 
                        // information if the window is on the primary but this will work on multimon.
                        Windows.ShowWindow(GetForm().Handle, Windows.ShowWindowCommand.Restore);
                        Client = GetForm().ClientRectangle;
                        Windows.ShowWindow(GetForm().Handle, Windows.ShowWindowCommand.Minimize);
                    }
                    else
                    {
                        // Use wp.rcNormalPosition to get the client rect, but wp.rcNormalPosition 
                        // includes the window frame so subtract it
                        var Frame = new Windows.Rect32();
                        Windows.AdjustWindowRect(ref Frame, 0, false);
                        var FrameWidth = Frame.Right - Frame.Left;
                        var FrameHeight = Frame.Bottom - Frame.Top;
                        Client.Width = WindowedPlacement.NormalPosition.Right - WindowedPlacement.NormalPosition.Left - FrameWidth;
                        Client.Height = WindowedPlacement.NormalPosition.Bottom - WindowedPlacement.NormalPosition.Top - FrameHeight;
                    }
                }
                else
                {
                    // Window is restored or maximized so just get its client rect
                    Client = GetForm().ClientRectangle;
                }

                // Now that we know the client rect, compare it against the back buffer size
                // to see if the client rect is already the right size
                if (Client.Width != NewDeviceSettings.SwapChainDescription.BufferDescription.Width || Client.Height != NewDeviceSettings.SwapChainDescription.BufferDescription.Height) NeedToResize = true;

                if (ClipWindowToSingleAdapter && GetForm().WindowState != FormWindowState.Minimized)
                {
                    // Get the rect of the monitor attached to the adapter
                    var AdapterMonitor2 = GetMonitorFromAdapter(NewDeviceSettings);
                    var WindowMonitor = Screen.FromControl(GetForm());

                    // Get the rect of the window
                    var Window = GetForm().DesktopBounds;

                    // Check if the window rect is fully inside the adapter's vitural screen rect
                    if ((Window.Left < AdapterMonitor2.WorkingArea.Left ||
                         Window.Right > AdapterMonitor2.WorkingArea.Right ||
                         Window.Top < AdapterMonitor2.WorkingArea.Top ||
                         Window.Bottom > AdapterMonitor2.WorkingArea.Bottom))
                    {
                        if (WindowMonitor == AdapterMonitor2 && GetForm().WindowState == FormWindowState.Maximized)
                        {
                            // If the window is maximized and on the same monitor as the adapter, then 
                            // no need to clip to single adapter as the window is already clipped 
                            // even though the rcWindow rect is outside of the miAdapter.rcWork
                        }
                        else NeedToResize = true;
                    }
                }
            }

            // Only resize window if needed 
            if (NeedToResize)
            {
                // Need to resize, so if window is maximized or minimized then restore the window
                if (GetForm().WindowState == FormWindowState.Minimized) Windows.ShowWindow(GetForm().Handle, Windows.ShowWindowCommand.Restore);
                if (GetForm().WindowState == FormWindowState.Maximized) Windows.ShowWindow(GetForm().Handle, Windows.ShowWindowCommand.Restore); // doing the IsIconic() check first also handles the WPF_RESTORETOMAXIMIZED case
                // D3D10 software rasterizer don't need an associated monitor and don't need to be clipped to a single adapater
                if (ClipWindowToSingleAdapter && GetDeviceSettings().DriverType == DriverType.Hardware)
                {
                    // Get the rect of the monitor attached to the adapter
                    var Adapter = GetMonitorFromAdapter(NewDeviceSettings);

                    // Get the rect of the monitor attached to the window
                    //var Monitor = Screen.FromControl(GetForm());

                    // Do something reasonable if the BackBuffer size is greater than the monitor size
                    var AdapterMonitorWidth = Adapter.WorkingArea.Right - Adapter.WorkingArea.Left;
                    var AdapterMonitorHeight = Adapter.WorkingArea.Bottom - Adapter.WorkingArea.Top;

                    var ClientWidth = NewDeviceSettings.SwapChainDescription.BufferDescription.Width;
                    var ClientHeight = NewDeviceSettings.SwapChainDescription.BufferDescription.Height;

                    // Make a window rect with a client rect that is the same size as the backbuffer
                    var ResizedWindow = new Windows.Rect32
                    {
                        Left = 0,
                        Right = (int)ClientWidth,
                        Top = 0,
                        Bottom = (int)ClientHeight
                    };
                    Windows.AdjustWindowRect(ref ResizedWindow, Windows.GetWindowLong(GetForm().Handle, Windows.GetWindowLongConst.Style), false);

                    var WindowWidth = ResizedWindow.Right - ResizedWindow.Left;
                    var WindowHeight = ResizedWindow.Bottom - ResizedWindow.Top;

                    if (WindowWidth > AdapterMonitorWidth) WindowWidth = AdapterMonitorWidth;
                    if (WindowHeight > AdapterMonitorHeight) WindowHeight = AdapterMonitorHeight;

                    if (ResizedWindow.Left < Adapter.WorkingArea.Left ||
                        ResizedWindow.Top < Adapter.WorkingArea.Top ||
                        ResizedWindow.Right > Adapter.WorkingArea.Right ||
                        ResizedWindow.Bottom > Adapter.WorkingArea.Bottom)
                    {
                        var WindowOffsetX = (AdapterMonitorWidth - WindowWidth) / 2;
                        var WindowOffsetY = (AdapterMonitorHeight - WindowHeight) / 2;

                        ResizedWindow.Left = Adapter.WorkingArea.Left + WindowOffsetX;
                        ResizedWindow.Top = Adapter.WorkingArea.Top + WindowOffsetY;
                        ResizedWindow.Right = Adapter.WorkingArea.Left + WindowOffsetX + WindowWidth;
                        ResizedWindow.Bottom = Adapter.WorkingArea.Top + WindowOffsetY + WindowHeight;
                    }

                    // Resize the window.  It is important to adjust the window size 
                    // after resetting the device rather than beforehand to ensure 
                    // that the monitor resolution is correct and does not limit the size of the new window.
                    GetForm().Bounds = new Rectangle(ResizedWindow.Left, ResizedWindow.Top, WindowWidth, WindowHeight);
                }
                else
                {
                    // Make a window rect with a client rect that is the same size as the backbuffer
                    var Window2 = new Windows.Rect32
                    {
                        Right = (int)NewDeviceSettings.SwapChainDescription.BufferDescription.Width,
                        Bottom = (int)NewDeviceSettings.SwapChainDescription.BufferDescription.Height
                    };
                    Windows.AdjustWindowRect(ref Window2, Windows.GetWindowLong(GetForm().Handle, Windows.GetWindowLongConst.Style), false);

                    // Resize the window.  It is important to adjust the window size 
                    // after resetting the device rather than beforehand to ensure 
                    // that the monitor resolution is correct and does not limit the size of the new window.
                    var X = Window2.Right - Window2.Left;
                    var Y = Window2.Bottom - Window2.Top;
                    GetForm().Size = new Size(X, Y);
                }

                // Its possible that the new window size is not what we asked for.  
                // No window can be sized larger than the desktop, so see if the Windows OS resized the 
                // window to something smaller to fit on the desktop.  Also if WM_GETMINMAXINFO
                // will put a limit on the smallest/largest window size.
                var Client = GetForm().ClientRectangle;
                if (Client.Width != NewDeviceSettings.SwapChainDescription.BufferDescription.Width ||
                    Client.Height != NewDeviceSettings.SwapChainDescription.BufferDescription.Height)
                {
                    // If its different, then resize the backbuffer again.  This time create a backbuffer that matches the 
                    // client rect of the current window w/o resizing the window.
                    var DeviceSettings = GetDeviceSettings();
                    DeviceSettings.SwapChainDescription.BufferDescription.Width = 0;
                    DeviceSettings.SwapChainDescription.BufferDescription.Height = 0;

                    Result = ChangeDevice(DeviceSettings, null, false, ClipWindowToSingleAdapter);
                    if (Result < 0)
                    {
                        CleanupEnvironment(true);
                        Pause(false, false);
                        GetState().IgnoreSizeChange = false;
                        return Result;
                    }
                }
            }

            // Make the window visible
            if (!GetForm().Visible) Windows.ShowWindow(GetForm().Handle, Windows.ShowWindowCommand.Show);

            // Ensure that the display doesn't power down when fullscreen but does when windowed
            if (!IsWindowed()) Windows.SetThreadExecutionState(Windows.ExecutionState.DisplayRequired | Windows.ExecutionState.Continuous);
            else Windows.SetThreadExecutionState(Windows.ExecutionState.Continuous);

            GetState().IgnoreSizeChange = false;
            Pause(false, false);
            GetState().DeviceCreated = true;

            return 0;
        }

        //--------------------------------------------------------------------------------------
        // Sets a previously created window for the framework to use.  If DXUTInit() 
        // has not already been called, it will call it with the default parameters.  
        // Instead of calling this, you can call DXUTCreateWindow() to create a new window.  
        //--------------------------------------------------------------------------------------
        public static int SetWindow(Form Form)
        {
            // Not allowed to call this from inside the device callbacks
            if (GetState().InsideDeviceCallback) return ErrorBox((int)Error.Fail, "SetWindow");

            // To avoid confusion, we do not allow any HWND to be NULL here.  The
            // caller must pass in valid HWND for all three parameters.  The same
            // HWND may be used for more than one parameter.
            if (Form == null) return ErrorBox((int)Error.InvalidArgument, "SetWindow");

            if (!GetState().Initialized)
            {
                // If DXUTInit() was already called and failed, then fail.
                // DXUTInit() must first succeed for this function to succeed
                if (GetState().InitializeCalled) return (int)Error.Fail;

                // If DXUTInit() hasn't been called, then automatically call it
                // with default params
                var Result = Initialize();
                if (Result < 0) return Result;
            }

            GetState().WindowCreatedWithDefaultPositions = false;
            GetState().WindowCreated = true;
            GetState().Form = Form;

            return 0;
        }

        //--------------------------------------------------------------------------------------
        // If DXUTCreateDevice() or DXUTSetD3D*Device() 
        // has not already been called, it will call DXUTCreateWindow() with the default parameters.  
        //--------------------------------------------------------------------------------------
        public static int PreMainLoop()
        {
            // Not allowed to call this from inside the device callbacks or reenter
            if (GetState().InsideDeviceCallback || GetState().InsideMainloop)
            {
                if ((GetState().ExitCode == 0) || (GetState().ExitCode == 10)) GetState().ExitCode = 1;
                return ErrorBox((int)Error.Fail, "PreMainLoop");
            }

            GetState().InsideMainloop = true;

            // If DXUTCreateDevice() or DXUTSetD3D*Device() has not already been called, 
            // then call DXUTCreateDevice() with the default parameters.         
            if (!GetState().DeviceCreated)
            {
                if (GetState().DeviceCreateCalled)
                {
                    if ((GetState().ExitCode == 0) || (GetState().ExitCode == 10)) GetState().ExitCode = 1;
                    return (int)Error.Fail; // DXUTCreateDevice() must first succeed for this function to succeed
                }

                var Result = CreateDevice();
                if (Result < 0)
                {
                    if ((GetState().ExitCode == 0) || (GetState().ExitCode == 10)) GetState().ExitCode = 1;
                    return Result;
                }
            }

            // DXUTInit() must have been called and succeeded for this function to proceed
            // DXUTCreateWindow() or DXUTSetWindow() must have been called and succeeded for this function to proceed
            // DXUTCreateDevice() or DXUTCreateDeviceFromSettings() or DXUTSetD3D*Device() must have been called and succeeded for this function to proceed
            if (!GetState().Initialized || !GetState().WindowCreated || !GetState().DeviceCreated)
            {
                if ((GetState().ExitCode == 0) || (GetState().ExitCode == 10)) GetState().ExitCode = 1;
                return ErrorBox((int)Error.Fail, "PreMainLoop");
            }

            GetState().InsideMainloop = false;

            return 0;
        }

        public static int MainLoopStep()
        {
            // Not allowed to call this from inside the device callbacks or reenter
            if (GetState().InsideDeviceCallback || GetState().InsideMainloop)
            {
                if ((GetState().ExitCode == 0) || (GetState().ExitCode == 10)) GetState().ExitCode = 1;
                return ErrorBox((int)Error.Fail, "MainLoopStep");
            }

            GetState().InsideMainloop = true;

            // DXUTInit() must have been called and succeeded for this function to proceed
            // DXUTCreateWindow() or DXUTSetWindow() must have been called and succeeded for this function to proceed
            // DXUTCreateDevice() or DXUTCreateDeviceFromSettings() or DXUTSetD3D*Device() must have been called and succeeded for this function to proceed
            if (!GetState().Initialized || !GetState().WindowCreated || !GetState().DeviceCreated)
            {
                if ((GetState().ExitCode == 0) || (GetState().ExitCode == 10)) GetState().ExitCode = 1;
                return ErrorBox((int)Error.Fail, "MainLoopStep");
            }

            // Render a frame during idle time (no messages are waiting)
            RenderEnvironment();

            GetState().InsideMainloop = false;

            return 0;
        }

        //--------------------------------------------------------------------------------------
        // Closes down the window.  When the window closes, it will cleanup everything
        //--------------------------------------------------------------------------------------
        public static void Shutdown(int ExitCode = 0)
        {
            var Form = GetForm();
            if (Form != null) Form.Close();

            GetState().ExitCode = ExitCode;

            CleanupEnvironment(true);

            // Shutdown D3D10
            var Factory = GetState().Factory;
            if (Factory != null) Factory.Release();
            GetState().Factory = null;
        }


        public static bool IsInGammaCorrectMode()
        {
            return GetState().IsInGammaCorrectMode;
        }

        public static int Initialize(bool ShowMessageBoxOnError = true)
        {
            GetState().InitializeCalled = true;

            GetState().ShowMessageBoxOnError = ShowMessageBoxOnError;

            GetGlobalTimer().Reset();

            GetState().Initialized = true;

            return 0;
        }

        public static TimerClass GetGlobalTimer()
        {
            return Timer ?? (Timer = new TimerClass());
        }

        public static void SetCursorSettings(bool ShowCursorWhenFullScreen, bool ClipCursorWhenFullScreen)
        {
            GetState().ClipCursorWhenFullScreen = ClipCursorWhenFullScreen;
            GetState().ShowCursorWhenFullScreen = ShowCursorWhenFullScreen;
            SetupCursor();
        }

        public static int CreateDevice(bool Windowed = true, int SuggestedWidth = 0, int SuggestedHeight = 0)
        {
            // Not allowed to call this from inside the device callbacks
            if (GetState().InsideDeviceCallback) return ErrorBox((int)Error.Fail, "CreateWindow");

            GetState().DeviceCreateCalled = true;

            if (!GetState().WindowCreated) return (int)Error.Fail;

            var MatchOptions = new MatchOptions
            {
                AdapterOrdinal = MatchType.IgnoreInput,
                DeviceType = MatchType.IgnoreInput,
                Output = MatchType.IgnoreInput,
                Windowed = MatchType.PreserveInput,
                AdapterFormat = MatchType.IgnoreInput,
                VertexProcessing = MatchType.IgnoreInput,
                Resolution = (Windowed || (SuggestedWidth != 0 && SuggestedHeight != 0)) ? MatchType.ClosestToInput : MatchType.IgnoreInput,
                BackBufferFormat = MatchType.IgnoreInput,
                BackBufferCount = MatchType.IgnoreInput,
                MultiSample = MatchType.IgnoreInput,
                SwapEffect = MatchType.IgnoreInput,
                DepthFormat = MatchType.IgnoreInput,
                StencilFormat = MatchType.IgnoreInput,
                PresentFlags = MatchType.IgnoreInput,
                RefreshRate = MatchType.IgnoreInput,
                PresentInterval = MatchType.IgnoreInput
            };

            var DeviceSettings = new DeviceSettings
            {
#if DEBUG
                CreateFlags = CreateDeviceFlag.Debug,
#endif
                SyncInterval = 1,
                SwapChainDescription =
                {
                    BufferUsage = UsageFlag.RenderTargetOutput,
                    Windowed = Windowed,
                    BufferDescription =
                    {
                        Width = (uint)SuggestedWidth,
                        Height = (uint)SuggestedHeight
                    }
                }
            };

            var Result = FindValidDeviceSettings(DeviceSettings, DeviceSettings, MatchOptions);
            if (Result < 0) DisplayErrorMessage(Result);

            // Change to a Direct3D device created from the new device settings.  
            // If there is an existing device, then either reset or recreated the scene
            Result = ChangeDevice(DeviceSettings, null, false, true);

            return Result < 0 ? Result : 0;
        }

        public static bool GetShowMessageBoxOnError()
        {
            return GetState().ShowMessageBoxOnError;
        }

        public static Form GetForm()
        {
            return GetState().Form;
        }

        public static Factory GetFactory()
        {
            DelayLoad();

            return GetState().Factory;
        }

        public static bool GetAutomation()
        {
            return GetState().Automation;
        }

        public static int ErrorBox(int Result, string Message)
        {
            if (GetShowMessageBoxOnError()) MessageBox.Show("An error has occured with error " + Result + " : " + Message);

            return Result;
        }

        //--------------------------------------------------------------------------------------
        // Returns true if app has registered any D3D10 callbacks or 
        // used the DXUTSetD3DVersionSupport API and passed true for bAppCanUseD3D10
        //--------------------------------------------------------------------------------------
        public static bool DoesApplicationSupportDirect3D()
        {
            //if (GetState().GetUseD3DVersionOverride()) return GetState().GetAppSupportsD3D10Override();

            return GetState().IsDeviceAcceptableFunction != null ||
                   GetState().DeviceCreatedFunction != null ||
                   GetState().SwapChainResizedFunction != null ||
                   GetState().FrameRenderFunction != null ||
                   GetState().SwapChainReleasingFunction != null ||
                   GetState().DeviceDestroyedFunction != null;
        }

        // ReSharper disable InconsistentNaming
        public static bool IsDirect3D_Available()
        // ReSharper restore InconsistentNaming
        {
            DelayLoad();

            return GetState().Direct3D_Available;
        }

        //--------------------------------------------------------------------------------------
        // Returns the DXGI_MODE_DESC struct for a given adapter and output 
        //--------------------------------------------------------------------------------------
        public static int GetAdapterDisplayMode(uint AdapterOrdinal, uint Output, ref ModeDescription ModeDescription)
        {
            var Enumeration = GetEnumeration();
            var OutputInfo = Enumeration.GetOutputInfo(AdapterOrdinal, Output);
            if (OutputInfo != null)
            {
                ModeDescription.Width = 640;
                ModeDescription.Height = 480;
                ModeDescription.RefreshRate.Numerator = 0;
                ModeDescription.RefreshRate.Denominator = 0;
                ModeDescription.Format = Format.R8G8B8A8_UNorm_SRGB;
                ModeDescription.Scaling = ModeScaling.Unspecified;
                ModeDescription.ScanlineOrdering = ModeScanlineOrder.Unspecified;

                OutputDescription Description;
                OutputInfo.Output.GetDescription(out Description);
                ModeDescription.Width = (uint)(Description.DesktopCoordinates.Right - Description.DesktopCoordinates.Left);
                ModeDescription.Height = (uint)(Description.DesktopCoordinates.Bottom - Description.DesktopCoordinates.Top);
            }

            // xTODO: verify this is needed
            if (ModeDescription.Format == Format.B8G8R8A8_UNorm) ModeDescription.Format = Format.R8G8B8A8_UNorm;

            return 0;
        }

        //--------------------------------------------------------------------------------------
        // Internal helper function to build a D3D10 device settings structure based upon the match 
        // options.  If the match option is set to ignore, then a optimal default value is used.
        // The default value may not exist on the system, but later this will be taken 
        // into account.
        //--------------------------------------------------------------------------------------
        static void BuildOptimalDeviceSettings(DeviceSettings OptimalDeviceSettings, DeviceSettings DeviceSettingsIn, MatchOptions MatchOptions)
        {
            OptimalDeviceSettings.SwapChainDescription = new SwapChainDescription();

            // Retrieve the desktop display mode.
            var AdapterDesktopDisplayMode = new ModeDescription { Width = 640, Height = 480, Format = Format.R8G8B8A8_UNorm_SRGB };
            GetAdapterDisplayMode(OptimalDeviceSettings.AdapterOrdinal, 0, ref AdapterDesktopDisplayMode);

            //---------------------
            // Adapter ordinal
            //---------------------
            OptimalDeviceSettings.AdapterOrdinal = MatchOptions.AdapterOrdinal == (int)MatchType.IgnoreInput ? 0 : DeviceSettingsIn.AdapterOrdinal;

            //---------------------
            // Device type
            //---------------------
            OptimalDeviceSettings.DriverType = MatchOptions.DeviceType == (int)MatchType.IgnoreInput ? DriverType.Hardware : DeviceSettingsIn.DriverType;

            //---------------------
            // Windowed
            //---------------------
            OptimalDeviceSettings.SwapChainDescription.Windowed = MatchOptions.Windowed == (int)MatchType.IgnoreInput || DeviceSettingsIn.SwapChainDescription.Windowed;

            //---------------------
            // Output #
            //---------------------
            OptimalDeviceSettings.Output = MatchOptions.Output == (int)MatchType.IgnoreInput ? 0 : DeviceSettingsIn.Output;

            //---------------------
            // Create flags
            //---------------------
            OptimalDeviceSettings.CreateFlags = DeviceSettingsIn.CreateFlags;

            //---------------------
            // Resolution
            //---------------------
            if (MatchOptions.Resolution == (int)MatchType.IgnoreInput)
            {
                // If windowed, default to 640x480
                // If fullscreen, default to the desktop res for quick mode change
                if (OptimalDeviceSettings.SwapChainDescription.Windowed)
                {
                    OptimalDeviceSettings.SwapChainDescription.BufferDescription.Width = 640;
                    OptimalDeviceSettings.SwapChainDescription.BufferDescription.Height = 480;
                }
                else
                {
                    OptimalDeviceSettings.SwapChainDescription.BufferDescription.Width = AdapterDesktopDisplayMode.Width;
                    OptimalDeviceSettings.SwapChainDescription.BufferDescription.Height = AdapterDesktopDisplayMode.Height;
                }
            }
            else
            {
                OptimalDeviceSettings.SwapChainDescription.BufferDescription.Width = DeviceSettingsIn.SwapChainDescription.BufferDescription.Width;
                OptimalDeviceSettings.SwapChainDescription.BufferDescription.Height = DeviceSettingsIn.SwapChainDescription.BufferDescription.Height;
            }

            //---------------------
            // Back buffer format
            //---------------------
            OptimalDeviceSettings.SwapChainDescription.BufferDescription.Format = MatchOptions.BackBufferFormat == (int)MatchType.IgnoreInput ? AdapterDesktopDisplayMode.Format : DeviceSettingsIn.SwapChainDescription.BufferDescription.Format;

            //---------------------
            // Back buffer usage
            //---------------------
            OptimalDeviceSettings.SwapChainDescription.BufferUsage = UsageFlag.RenderTargetOutput;

            //---------------------
            // Back buffer count
            //---------------------
            OptimalDeviceSettings.SwapChainDescription.BufferCount = MatchOptions.BackBufferCount == (int)MatchType.IgnoreInput ? 2 : DeviceSettingsIn.SwapChainDescription.BufferCount;

            //---------------------
            // Multisample
            //---------------------
            if (MatchOptions.MultiSample == (int)MatchType.IgnoreInput)
            {
                // Default to no multisampling 
                OptimalDeviceSettings.SwapChainDescription.SampleDescription.Count = 0;
                OptimalDeviceSettings.SwapChainDescription.SampleDescription.Quality = 0;
            }
            else
            {
                OptimalDeviceSettings.SwapChainDescription.SampleDescription.Count = DeviceSettingsIn.SwapChainDescription.SampleDescription.Count;
                OptimalDeviceSettings.SwapChainDescription.SampleDescription.Quality = DeviceSettingsIn.SwapChainDescription.SampleDescription.Quality;
            }

            //---------------------
            // Swap effect
            //---------------------
            OptimalDeviceSettings.SwapChainDescription.SwapEffect = MatchOptions.SwapEffect == (int)MatchType.IgnoreInput ? SwapEffect.Discard : DeviceSettingsIn.SwapChainDescription.SwapEffect;

            //---------------------
            // Depth stencil 
            //---------------------
            if (MatchOptions.DepthFormat == (int)MatchType.IgnoreInput && MatchOptions.StencilFormat == (int)MatchType.IgnoreInput)
            {
                OptimalDeviceSettings.AutoCreateDepthStencil = true;
                OptimalDeviceSettings.AutoDepthStencilFormat = Format.D32_Float;
            }
            else
            {
                OptimalDeviceSettings.AutoCreateDepthStencil = DeviceSettingsIn.AutoCreateDepthStencil;
                OptimalDeviceSettings.AutoDepthStencilFormat = DeviceSettingsIn.AutoDepthStencilFormat;
            }

            //---------------------
            // Present flags
            //---------------------
            OptimalDeviceSettings.PresentFlags = MatchOptions.PresentFlags == (int)MatchType.IgnoreInput ? 0 : DeviceSettingsIn.PresentFlags;

            //---------------------
            // Refresh rate
            //---------------------
            if (MatchOptions.RefreshRate == (int)MatchType.IgnoreInput)
            {
                OptimalDeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Numerator = 0;
                OptimalDeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Denominator = 0;
            }
            else OptimalDeviceSettings.SwapChainDescription.BufferDescription.RefreshRate = DeviceSettingsIn.SwapChainDescription.BufferDescription.RefreshRate;

            //---------------------
            // Present interval
            //---------------------
            // For windowed and fullscreen, default to 1 which will
            // wait for the vertical retrace period to prevent tearing.
            // For benchmarking, use 0 which will not wait for the
            // vertical retrace period but may introduce tearing.
            OptimalDeviceSettings.SyncInterval = MatchOptions.PresentInterval == (int)MatchType.IgnoreInput ? 1 : DeviceSettingsIn.SyncInterval;
        }

        //--------------------------------------------------------------------------------------
        // Returns false for any CD3D9EnumDeviceSettingsCombo that doesn't meet the preserve 
        // match options against the input DeviceSettingsIn.
        //--------------------------------------------------------------------------------------
        static bool DoesDeviceComboMatchPreserveOptions(Enumeration.DeviceSettingsCombo DeviceSettingsCombo, DeviceSettings DeviceSettingsIn, MatchOptions MatchOptions)
        {
            //---------------------
            // Adapter ordinal
            //---------------------
            if (MatchOptions.AdapterOrdinal == MatchType.PreserveInput && (DeviceSettingsCombo.AdapterOrdinal != DeviceSettingsIn.AdapterOrdinal)) return false;

            //---------------------
            // Device type
            //---------------------
            if (MatchOptions.DeviceType == MatchType.PreserveInput && (DeviceSettingsCombo.DeviceType != DeviceSettingsIn.DriverType)) return false;

            //---------------------
            // Windowed
            //---------------------
            if (MatchOptions.Windowed == MatchType.PreserveInput && (DeviceSettingsCombo.Windowed != DeviceSettingsIn.SwapChainDescription.Windowed)) return false;

            //---------------------
            // Output
            //---------------------
            if (MatchOptions.Output == MatchType.PreserveInput && (DeviceSettingsCombo.OutputOrdinal != DeviceSettingsIn.Output)) return false;

            //---------------------
            // Resolution
            //---------------------
            // If keep resolution then check that width and height supported by this combo
            if (MatchOptions.Resolution == MatchType.PreserveInput)
            {
                var Found = DeviceSettingsCombo.OutputInfo.DisplayModes.Any(DisplayMode => DisplayMode.Width == DeviceSettingsIn.SwapChainDescription.BufferDescription.Width && DisplayMode.Height == DeviceSettingsIn.SwapChainDescription.BufferDescription.Height);

                // If the width and height are not supported by this combo, return false
                if (!Found) return false;
            }

            //---------------------
            // Back buffer format
            //---------------------
            if (MatchOptions.BackBufferFormat == MatchType.PreserveInput && DeviceSettingsCombo.BackBufferFormat != DeviceSettingsIn.SwapChainDescription.BufferDescription.Format) return false;

            //---------------------
            // Back buffer count
            //---------------------
            // No caps for the back buffer count

            //---------------------
            // Multisample
            //---------------------
            if (MatchOptions.MultiSample == MatchType.PreserveInput)
            {
                var Found = false;
                for (var I = 0; I < DeviceSettingsCombo.MultiSampleCounts.Count; I++)
                {
                    var Count = DeviceSettingsCombo.MultiSampleCounts[I];
                    var Quality = DeviceSettingsCombo.MultiSampleQualities[I];

                    if (Count == DeviceSettingsIn.SwapChainDescription.SampleDescription.Count && Quality > DeviceSettingsIn.SwapChainDescription.SampleDescription.Quality)
                    {
                        Found = true;
                        break;
                    }
                }

                // If multisample type/quality not supported by this combo, then return false
                if (!Found) return false;
            }

            //---------------------
            // Swap effect
            //---------------------
            // No caps for swap effects

            //---------------------
            // Depth stencil 
            //---------------------
            // No caps for depth stencil

            //---------------------
            // Present flags
            //---------------------
            // No caps for the present flags

            //---------------------
            // Refresh rate
            //---------------------
            // If keep refresh rate then check that the resolution is supported by this combo
            if (MatchOptions.RefreshRate == MatchType.PreserveInput)
            {
                var Found = false;
                foreach (var DisplayMode in DeviceSettingsCombo.OutputInfo.DisplayModes)
                {
                    var Denom1 = 1f;
                    var Denom2 = 1f;

                    if (DisplayMode.RefreshRate.Denominator > 0) Denom1 = DisplayMode.RefreshRate.Denominator;
                    if (DeviceSettingsIn.SwapChainDescription.BufferDescription.RefreshRate.Denominator > 0) Denom2 = DeviceSettingsIn.SwapChainDescription.BufferDescription.RefreshRate.Denominator;

                    if (Math.Abs(DisplayMode.RefreshRate.Numerator / Denom1 - DeviceSettingsIn.SwapChainDescription.BufferDescription.RefreshRate.Numerator / Denom2) < 0.1)
                    {
                        Found = true;
                        break;
                    }
                }

                // If refresh rate not supported by this combo, then return false
                if (!Found) return false;
            }

            //---------------------
            // Present interval
            //---------------------
            // No caps for present interval

            return true;
        }

        //--------------------------------------------------------------------------------------
        // Returns the number of color channel bits in the specified DXGI_FORMAT
        //--------------------------------------------------------------------------------------
        public static uint GetColorChannelBits(Format Format)
        {
            switch (Format)
            {
            case Format.R32G32B32A32_Typeless:
            case Format.R32G32B32A32_Float:
            case Format.R32G32B32A32_UInt:
            case Format.R32G32B32A32_SInt:
            case Format.R32G32B32_Typeless:
            case Format.R32G32B32_Float:
            case Format.R32G32B32_UInt:
            case Format.R32G32B32_SInt:
                return 32;

            case Format.R16G16B16A16_Typeless:
            case Format.R16G16B16A16_Float:
            case Format.R16G16B16A16_UNorm:
            case Format.R16G16B16A16_UInt:
            case Format.R16G16B16A16_SNorm:
            case Format.R16G16B16A16_SInt:
                return 16;

            case Format.R10G10B10A2_Typeless:
            case Format.R10G10B10A2_UNorm:
            case Format.R10G10B10A2_UInt:
                return 10;

            case Format.R8G8B8A8_Typeless:
            case Format.R8G8B8A8_UNorm:
            case Format.R8G8B8A8_UNorm_SRGB:
            case Format.R8G8B8A8_UInt:
            case Format.R8G8B8A8_SNorm:
            case Format.R8G8B8A8_SInt:
                return 8;

            case Format.B5G6R5_UNorm:
            case Format.B5G5R5A1_UNorm:
                return 5;

            default:
                return 0;
            }
        }

        //--------------------------------------------------------------------------------------
        // Returns a ranking number that describes how closely this device 
        // combo matches the optimal combo based on the match options and the optimal device settings
        //--------------------------------------------------------------------------------------
        static float RankDeviceCombo(Enumeration.DeviceSettingsCombo DeviceSettingsCombo, DeviceSettings OptimalDeviceSettings/*, ref ModeDescription AdapterDisplayMode*/)
        {
            var CurrentRanking = 0.0f;

            // Arbitrary weights.  Gives preference to the ordinal, device type, and windowed
            const float AdapterOrdinalWeight = 1000.0f;
            const float AdapterOutputWeight = 500.0f;
            const float DeviceTypeWeight = 100.0f;
            const float WindowWeight = 10.0f;
            const float ResolutionWeight = 1.0f;
            const float BackBufferFormatWeight = 1.0f;
            const float MultiSampleWeight = 1.0f;
            const float RefreshRateWeight = 1.0f;

            //---------------------
            // Adapter ordinal
            //---------------------
            if (DeviceSettingsCombo.AdapterOrdinal == OptimalDeviceSettings.AdapterOrdinal) CurrentRanking += AdapterOrdinalWeight;

            //---------------------
            // Adapter ordinal
            //---------------------
            if (DeviceSettingsCombo.OutputOrdinal == OptimalDeviceSettings.Output) CurrentRanking += AdapterOutputWeight;

            //---------------------
            // Device type
            //---------------------
            if (DeviceSettingsCombo.DeviceType == OptimalDeviceSettings.DriverType) CurrentRanking += DeviceTypeWeight;
            // Slightly prefer HAL 
            if (DeviceSettingsCombo.DeviceType == (DriverType)1 /*D3DDEVTYPE_HAL probably a DXUT bug it should be D3D10_DRIVER_TYPE_HARDWARE*/) CurrentRanking += 0.1f;

            //---------------------
            // Windowed
            //---------------------
            if (DeviceSettingsCombo.Windowed == OptimalDeviceSettings.SwapChainDescription.Windowed) CurrentRanking += WindowWeight;

            //---------------------
            // Resolution
            //---------------------
            if (DeviceSettingsCombo.OutputInfo != null)
            {
                var ResolutionFound = DeviceSettingsCombo.OutputInfo.DisplayModes.Any(DisplayMode => DisplayMode.Width == OptimalDeviceSettings.SwapChainDescription.BufferDescription.Width && DisplayMode.Height == OptimalDeviceSettings.SwapChainDescription.BufferDescription.Height);
                if (ResolutionFound) CurrentRanking += ResolutionWeight;
            }

            //---------------------
            // Back buffer format
            //---------------------
            if (DeviceSettingsCombo.BackBufferFormat == OptimalDeviceSettings.SwapChainDescription.BufferDescription.Format) CurrentRanking += BackBufferFormatWeight;
            else
            {
                var BitDepthDelta = Math.Abs(GetColorChannelBits(DeviceSettingsCombo.BackBufferFormat) - GetColorChannelBits(OptimalDeviceSettings.SwapChainDescription.BufferDescription.Format));
                var Scale = Math.Max(0.9f - BitDepthDelta * 0.2f, 0.0f);
                CurrentRanking += Scale * BackBufferFormatWeight;
            }

            //---------------------
            // Back buffer count
            //---------------------
            // No caps for the back buffer count

            //---------------------
            // Multisample
            //---------------------
            var MultiSampleFound = false;
            for (var I = 0; I < DeviceSettingsCombo.MultiSampleCounts.Count; I++)
            {
                var Count = DeviceSettingsCombo.MultiSampleCounts[I];
                var Quality = DeviceSettingsCombo.MultiSampleQualities[I];

                if (Count == OptimalDeviceSettings.SwapChainDescription.SampleDescription.Count && Quality > OptimalDeviceSettings.SwapChainDescription.SampleDescription.Quality)
                {
                    MultiSampleFound = true;
                    break;
                }
            }
            if (MultiSampleFound) CurrentRanking += MultiSampleWeight;

            //---------------------
            // Swap effect
            //---------------------
            // No caps for swap effects

            //---------------------
            // Depth stencil 
            //---------------------
            // No caps for swap effects

            //---------------------
            // Present flags
            //---------------------
            // No caps for the present flags

            //---------------------
            // Refresh rate
            //---------------------
            var RefreshFound = false;
            if (DeviceSettingsCombo.OutputInfo != null)
            {
                foreach (var DisplayMode in DeviceSettingsCombo.OutputInfo.DisplayModes)
                {
                    var Denom1 = 1f;
                    var Denom2 = 1f;

                    if (DisplayMode.RefreshRate.Denominator > 0) Denom1 = DisplayMode.RefreshRate.Denominator;

                    if (OptimalDeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Denominator > 0) Denom2 = OptimalDeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Denominator;

                    if (Math.Abs(DisplayMode.RefreshRate.Numerator / Denom1 - OptimalDeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Numerator / Denom2) < 0.1f) RefreshFound = true;
                }
            }
            if (RefreshFound) CurrentRanking += RefreshRateWeight;

            //---------------------
            // Present interval
            //---------------------
            // No caps for the present flags

            return CurrentRanking;
        }

        //--------------------------------------------------------------------------------------
        // Internal helper function to find the closest allowed display mode to the optimal 
        //--------------------------------------------------------------------------------------
        static void FindValidResolution(Enumeration.DeviceSettingsCombo BestDeviceSettingsCombo, ref ModeDescription DisplayModeIn, out ModeDescription BestDisplayMode)
        {
            if (BestDeviceSettingsCombo.Windowed)
            {
                BestDisplayMode = DisplayModeIn;

                // If our client rect size is smaller than our backbuffer size, use that size.
                // This would happen when we specify a windowed resolution larger than the screen.
                if (BestDeviceSettingsCombo.OutputInfo != null)
                {
                    var Info = Screen.AllScreens.First(X => X.DeviceName == BestDeviceSettingsCombo.OutputInfo.Description.DeviceName);

                    var Width = Info.WorkingArea.Width;
                    var Height = Info.WorkingArea.Height;

                    var Client = new Windows.Rect32
                    {
                        Bottom = Info.WorkingArea.Bottom,
                        Left = Info.WorkingArea.Left,
                        Right = Info.WorkingArea.Right,
                        Top = Info.WorkingArea.Top
                    };

                    Windows.AdjustWindowRect(ref Client, Windows.GetWindowLong(GetForm().Handle, Windows.GetWindowLongConst.Style), false);
                    Width = Width - (Client.Right - Client.Left - Width);
                    Height = Height - (Client.Bottom - Client.Top - Height);

                    BestDisplayMode.Width = (uint)Math.Min(BestDisplayMode.Width, Width);
                    BestDisplayMode.Height = (uint)Math.Min(BestDisplayMode.Height, Height);

                }
            }
            else
            {
                BestDisplayMode = new ModeDescription();
                var BestRanking = 100000;
                var DisplayModes = BestDeviceSettingsCombo.OutputInfo.DisplayModes;
                foreach (var DisplayMode in DisplayModes)
                {
                    // Find the delta between the current width/height and the optimal width/height
                    var CurrentRanking = Math.Abs(DisplayMode.Width - DisplayModeIn.Width) + Math.Abs(DisplayMode.Height - DisplayModeIn.Height);

                    if (CurrentRanking < BestRanking)
                    {
                        BestDisplayMode = DisplayMode;
                        BestRanking = (int)CurrentRanking;

                        // Stop if perfect match found
                        if (BestRanking == 0) break;
                    }
                }

                if (BestDisplayMode.Width == 0)
                {
                    BestDisplayMode = DisplayModeIn;
                    return;
                }

            }
        }

        //--------------------------------------------------------------------------------------
        // Builds valid device settings using the match options, the input device settings, and the 
        // best device settings combo found.
        //--------------------------------------------------------------------------------------
        static void BuildValidDeviceSettings(DeviceSettings ValidDeviceSettings, Enumeration.DeviceSettingsCombo BestDeviceSettingsCombo, DeviceSettings DeviceSettingsIn, MatchOptions MatchOptions)
        {
            var AdapterDisplayMode = new ModeDescription();
            GetAdapterDisplayMode(BestDeviceSettingsCombo.AdapterOrdinal, BestDeviceSettingsCombo.OutputOrdinal, ref AdapterDisplayMode);

            // For each setting pick the best, taking into account the match options and 
            // what's supported by the device

            //---------------------
            // Adapter Ordinal
            //---------------------
            // Just using pBestDeviceSettingsCombo->AdapterOrdinal

            //---------------------
            // Device Type
            //---------------------
            // Just using pBestDeviceSettingsCombo->DeviceType

            //---------------------
            // Windowed 
            //---------------------
            // Just using pBestDeviceSettingsCombo->Windowed

            //---------------------
            // Output
            //---------------------
            // Just using pBestDeviceSettingsCombo->Output

            //---------------------
            // Resolution
            //---------------------
            ModeDescription BestDisplayMode;
            if (MatchOptions.Resolution == MatchType.PreserveInput)
            {
                BestDisplayMode.Width = DeviceSettingsIn.SwapChainDescription.BufferDescription.Width;
                BestDisplayMode.Height = DeviceSettingsIn.SwapChainDescription.BufferDescription.Height;
            }
            else
            {
                var DisplayModeIn = new ModeDescription();
                if (MatchOptions.Resolution == MatchType.ClosestToInput && DeviceSettingsIn != null)
                {
                    DisplayModeIn.Width = DeviceSettingsIn.SwapChainDescription.BufferDescription.Width;
                    DisplayModeIn.Height = DeviceSettingsIn.SwapChainDescription.BufferDescription.Height;
                }
                else // if( pMatchOptions->eResolution == DXUTMT_IGNORE_INPUT )   
                {
                    if (BestDeviceSettingsCombo.Windowed)
                    {
                        // The framework defaults to 640x480 for windowed
                        DisplayModeIn.Width = 640;
                        DisplayModeIn.Height = 480;
                    }
                    else
                    {
                        // The framework defaults to desktop resolution for fullscreen to try to avoid slow mode change
                        DisplayModeIn.Width = AdapterDisplayMode.Width;
                        DisplayModeIn.Height = AdapterDisplayMode.Height;
                    }
                }

                // Call a helper function to find the closest valid display mode to the optimal
                FindValidResolution(BestDeviceSettingsCombo, ref DisplayModeIn, out BestDisplayMode);
            }

            //---------------------
            // Back Buffer Format
            //---------------------
            // Just using pBestDeviceSettingsCombo->BackBufferFormat

            //---------------------
            // Back Buffer usage
            //---------------------
            // Just using pDeviceSettingsIn->sd.BackBufferUsage | DXGI_USAGE_RENDERTARGETOUTPUT

            //---------------------
            // Back buffer count
            //---------------------
            uint BestBackBufferCount;
            // ReSharper disable ConvertIfStatementToSwitchStatement
            // ReSharper disable PossibleNullReferenceException
            if (MatchOptions.BackBufferCount == MatchType.PreserveInput) BestBackBufferCount = DeviceSettingsIn.SwapChainDescription.BufferCount;
            // ReSharper restore PossibleNullReferenceException
            // ReSharper restore ConvertIfStatementToSwitchStatement
            else if (MatchOptions.BackBufferCount == MatchType.IgnoreInput)
            {
                // The framework defaults to triple buffering 
                BestBackBufferCount = 2;
            }
            else // if( pMatchOptions->eBackBufferCount == DXUTMT_CLOSEST_TO_INPUT )   
            {
                // ReSharper disable PossibleNullReferenceException
                BestBackBufferCount = DeviceSettingsIn.SwapChainDescription.BufferCount;
                // ReSharper restore PossibleNullReferenceException
                if (BestBackBufferCount > 3) BestBackBufferCount = 3;
                if (BestBackBufferCount < 1) BestBackBufferCount = 1;
            }

            //---------------------
            // Multisample
            //---------------------
            uint BestMultiSampleCount;
            uint BestMultiSampleQuality;
            if (DeviceSettingsIn != null && DeviceSettingsIn.SwapChainDescription.SwapEffect != SwapEffect.Discard)
            {
                // Swap effect is not set to discard so multisampling has to off
                BestMultiSampleCount = 1;
                BestMultiSampleQuality = 0;
            }
            else
            {
                // ReSharper disable ConvertIfStatementToSwitchStatement
                if (MatchOptions.MultiSample == MatchType.PreserveInput)
                // ReSharper restore ConvertIfStatementToSwitchStatement
                {
                    // ReSharper disable PossibleNullReferenceException
                    BestMultiSampleCount = DeviceSettingsIn.SwapChainDescription.SampleDescription.Count;
                    // ReSharper restore PossibleNullReferenceException
                    BestMultiSampleQuality = DeviceSettingsIn.SwapChainDescription.SampleDescription.Quality;
                }
                else if (MatchOptions.MultiSample == MatchType.IgnoreInput)
                {
                    // Default to no multisampling (always supported)
                    BestMultiSampleCount = 1;
                    BestMultiSampleQuality = 0;
                }
                else if (MatchOptions.MultiSample == MatchType.ClosestToInput)
                {
                    // Default to no multisampling (always supported)
                    BestMultiSampleCount = 1;
                    BestMultiSampleQuality = 0;

                    for (var I = 0; I < BestDeviceSettingsCombo.MultiSampleCounts.Count; I++)
                    {
                        var Count = BestDeviceSettingsCombo.MultiSampleCounts[I];
                        var Quality = BestDeviceSettingsCombo.MultiSampleQualities[I];

                        // Check whether supported type is closer to the input than our current best
                        // ReSharper disable PossibleNullReferenceException
                        if (Math.Abs(Count - DeviceSettingsIn.SwapChainDescription.SampleDescription.Count) < Math.Abs(BestMultiSampleCount - DeviceSettingsIn.SwapChainDescription.SampleDescription.Count))
                        // ReSharper restore PossibleNullReferenceException
                        {
                            BestMultiSampleCount = Count;
                            BestMultiSampleQuality = Math.Min(Quality - 1, DeviceSettingsIn.SwapChainDescription.SampleDescription.Quality);
                        }
                    }
                }
                else
                {
                    // Error case
                    BestMultiSampleCount = 1;
                    BestMultiSampleQuality = 0;
                }
            }

            //---------------------
            // Swap effect
            //---------------------
            SwapEffect BestSwapEffect;
            // ReSharper disable ConvertIfStatementToSwitchStatement
            // ReSharper disable PossibleNullReferenceException
            if (MatchOptions.SwapEffect == MatchType.PreserveInput) BestSwapEffect = DeviceSettingsIn.SwapChainDescription.SwapEffect;
            // ReSharper restore PossibleNullReferenceException
            // ReSharper restore ConvertIfStatementToSwitchStatement
            else if (MatchOptions.SwapEffect == MatchType.IgnoreInput) BestSwapEffect = SwapEffect.Discard;
            else // if( pMatchOptions->eSwapEffect == DXUTMT_CLOSEST_TO_INPUT )
            {
                // ReSharper disable PossibleNullReferenceException
                BestSwapEffect = DeviceSettingsIn.SwapChainDescription.SwapEffect;
                // ReSharper restore PossibleNullReferenceException

                // Swap effect has to be one of these 2
                if (BestSwapEffect != SwapEffect.Discard && BestSwapEffect != SwapEffect.Sequential) BestSwapEffect = SwapEffect.Discard;
            }

            //---------------------
            // Depth stencil 
            //---------------------
            Format BestDepthStencilFormat;
            bool BestEnableAutoDepthStencil;

            if (MatchOptions.DepthFormat == MatchType.IgnoreInput && MatchOptions.StencilFormat == MatchType.IgnoreInput)
            {
                BestEnableAutoDepthStencil = true;
                BestDepthStencilFormat = Format.D32_Float;
            }
            else
            {
                // ReSharper disable PossibleNullReferenceException
                BestEnableAutoDepthStencil = DeviceSettingsIn.AutoCreateDepthStencil;
                // ReSharper restore PossibleNullReferenceException
                BestDepthStencilFormat = DeviceSettingsIn.AutoDepthStencilFormat;
            }

            //---------------------
            // Present flags
            //---------------------

            //---------------------
            // Refresh rate
            //---------------------
            if (BestDeviceSettingsCombo.Windowed)
            {
                // Must be 0 for windowed
                BestDisplayMode.RefreshRate.Numerator = 0;
                BestDisplayMode.RefreshRate.Denominator = 0;
            }
            else
            {
                // ReSharper disable PossibleNullReferenceException
                if (MatchOptions.RefreshRate == MatchType.PreserveInput) BestDisplayMode.RefreshRate = DeviceSettingsIn.SwapChainDescription.BufferDescription.RefreshRate;
                // ReSharper restore PossibleNullReferenceException
                else
                {
                    // ReSharper disable PossibleNullReferenceException
                    var RefreshRateMatch = MatchOptions.RefreshRate == MatchType.ClosestToInput ? DeviceSettingsIn.SwapChainDescription.BufferDescription.RefreshRate : AdapterDisplayMode.RefreshRate;
                    // ReSharper restore PossibleNullReferenceException

                    // Default to 0 in case no matching mode is found.
                    // 0, in this case means that we'll let DXGI choose the best one for us
                    BestDisplayMode.RefreshRate.Numerator = 0;
                    BestDisplayMode.RefreshRate.Denominator = 0;

                    //            if( refreshRateMatch != 0 )
                    {
                        var BestRefreshRanking = 100000.0f;
                        var Descriptions = BestDeviceSettingsCombo.OutputInfo.DisplayModes;
                        foreach (var DisplayMode in Descriptions)
                        {
                            if (DisplayMode.Height != BestDisplayMode.Height || DisplayMode.Width != BestDisplayMode.Width) continue; // Skip display modes that don't match

                            // Find the delta between the current refresh rate and the optimal refresh rate
                            float Denom1 = 1;
                            float Denom2 = 1;
                            if (DisplayMode.RefreshRate.Denominator > 0) Denom1 = DisplayMode.RefreshRate.Denominator;
                            if (RefreshRateMatch.Denominator > 0) Denom2 = RefreshRateMatch.Denominator;

                            var CurrentRanking = Math.Abs(DisplayMode.RefreshRate.Numerator / Denom1 - RefreshRateMatch.Numerator / Denom2);

                            if (CurrentRanking < BestRefreshRanking)
                            {
                                BestDisplayMode.RefreshRate = DisplayMode.RefreshRate;
                                BestRefreshRanking = CurrentRanking;

                                // Stop if good-enough match found
                                if (BestRefreshRanking < 0.1f)
                                    break;
                            }
                        }
                    }
                }
            }

            //---------------------
            // Present interval
            //---------------------
            uint BestPresentInterval;
            // ReSharper disable ConvertIfStatementToSwitchStatement
            // ReSharper disable PossibleNullReferenceException
            if (MatchOptions.PresentInterval == MatchType.PreserveInput) BestPresentInterval = DeviceSettingsIn.SyncInterval;
            // ReSharper restore PossibleNullReferenceException
            // ReSharper restore ConvertIfStatementToSwitchStatement
            else if (MatchOptions.PresentInterval == MatchType.IgnoreInput)
            {
                // For windowed and fullscreen, default to 1 which will wait for
                // the vertical retrace period to prevent tearing. For benchmarking,
                // use 0 which will will wait not for the vertical retrace period
                // but may introduce tearing.

                // The reference driver does not support v-syncing and will
                // produce _com_error exceptions when the sync interval is
                // anything but 0.
                BestPresentInterval = (uint)((DriverType.Reference == BestDeviceSettingsCombo.DeviceType) ? 0 : 1);
            }
            // ReSharper disable PossibleNullReferenceException
            else BestPresentInterval = DeviceSettingsIn.SyncInterval;
            // ReSharper restore PossibleNullReferenceException

            // Fill the device settings struct
            ValidDeviceSettings.PresentFlags = 0;
            ValidDeviceSettings.AdapterOrdinal = BestDeviceSettingsCombo.AdapterOrdinal;
            ValidDeviceSettings.Output = BestDeviceSettingsCombo.OutputOrdinal;
            ValidDeviceSettings.DriverType = BestDeviceSettingsCombo.DeviceType;
            ValidDeviceSettings.SwapChainDescription.BufferDescription.Width = BestDisplayMode.Width;
            ValidDeviceSettings.SwapChainDescription.BufferDescription.Height = BestDisplayMode.Height;
            ValidDeviceSettings.SwapChainDescription.BufferDescription.Format = BestDeviceSettingsCombo.BackBufferFormat;
            // ReSharper disable PossibleNullReferenceException
            ValidDeviceSettings.SwapChainDescription.BufferUsage = DeviceSettingsIn.SwapChainDescription.BufferUsage | UsageFlag.RenderTargetOutput;
            // ReSharper restore PossibleNullReferenceException
            ValidDeviceSettings.SwapChainDescription.BufferCount = BestBackBufferCount;
            ValidDeviceSettings.SwapChainDescription.SampleDescription.Count = BestMultiSampleCount;
            ValidDeviceSettings.SwapChainDescription.SampleDescription.Quality = BestMultiSampleQuality;
            ValidDeviceSettings.SwapChainDescription.SwapEffect = BestSwapEffect;
            ValidDeviceSettings.SwapChainDescription.OutputWindow = GetForm().Handle;
            ValidDeviceSettings.SwapChainDescription.Windowed = BestDeviceSettingsCombo.Windowed;
            ValidDeviceSettings.SwapChainDescription.BufferDescription.RefreshRate = BestDisplayMode.RefreshRate;
            ValidDeviceSettings.SwapChainDescription.Flags = 0;
            ValidDeviceSettings.SyncInterval = BestPresentInterval;
            ValidDeviceSettings.AutoCreateDepthStencil = BestEnableAutoDepthStencil;
            ValidDeviceSettings.AutoDepthStencilFormat = BestDepthStencilFormat;
            ValidDeviceSettings.CreateFlags = DeviceSettingsIn.CreateFlags;
        }

        static int FindValidDeviceSettings(DeviceSettings Out, DeviceSettings In, MatchOptions MatchOptions, DeviceSettings Optimal)
        {
            // Find the best combination of:
            //      Adapter Ordinal
            //      Device Type
            //      Back Buffer Format
            //      Windowed
            // given what's available on the system and the match options combined with the device settings input.
            // This combination of settings is encapsulated by the CD3D10EnumDeviceSettingsCombo class.
            var BestRanking = -1.0f;
            Enumeration.DeviceSettingsCombo BestDeviceSettingsCombo = null;
            var AdapterDisplayMode = new ModeDescription();

            var Enumeration = GetEnumeration();
            var AdapterInfos = Enumeration.AdapterInfos;
            foreach (var AdapterInfo in AdapterInfos)
            {
                // Get the desktop display mode of adapter

                GetAdapterDisplayMode(AdapterInfo.AdapterOrdinal, 0, ref AdapterDisplayMode);

                // Enum all the device settings combinations.  A device settings combination is 
                // a unique set of an adapter format, back buffer format, and IsWindowed.
                foreach (var DeviceSettingsCombo in AdapterInfo.DeviceSettingsCombos)
                {
                    // Skip any combo that doesn't meet the preserve match options
                    if (false == DoesDeviceComboMatchPreserveOptions(DeviceSettingsCombo, In, MatchOptions)) continue;

                    // Get a ranking number that describes how closely this device combo matches the optimal combo
                    var CurrentRanking = RankDeviceCombo(DeviceSettingsCombo, Optimal);

                    // If this combo better matches the input device settings then save it
                    if (CurrentRanking > BestRanking)
                    {
                        BestDeviceSettingsCombo = DeviceSettingsCombo;
                        BestRanking = CurrentRanking;
                    }
                }
            }

            // If no best device combination was found then fail
            if (BestDeviceSettingsCombo == null) return (int)Error.NoCompatibleDevices;

            // Using the best device settings combo found, build valid device settings taking heed of 
            // the match options and the input device settings
            var ValidDeviceSettings = new DeviceSettings();
            BuildValidDeviceSettings(ValidDeviceSettings, BestDeviceSettingsCombo, In, MatchOptions);
            Out.CopyFrom(ValidDeviceSettings);

            return 0;
        }

        public static int FindValidDeviceSettings(DeviceSettings Out, DeviceSettings In = null, MatchOptions MatchOptions = null)
        {
            var Result = 0;

            if (Out == null) return ErrorBox((int)Error.InvalidArgument, "FindValidDeviceSettings");

            if (MatchOptions == null) MatchOptions = new MatchOptions();

            var ApplicationSupportDirect3D = DoesApplicationSupportDirect3D();

            // Build an optimal device settings structure based upon the match 
            // options.  If the match option is set to ignore, then a optimal default value is used.
            // The default value may not exist on the system, but later this will be taken 
            // into account.
            var FoundValidDirect3D = false;

            // ReSharper disable PossibleNullReferenceException
            var ValidDeviceSettings = In.Copy();
            // ReSharper restore PossibleNullReferenceException
            var OptimalDeviceSettings = new DeviceSettings();

            if (ApplicationSupportDirect3D)
            {
                if (IsDirect3D_Available())
                {
                    // Force an enumeration with the IsDeviceAcceptable callback
                    GetEnumeration();

                    BuildOptimalDeviceSettings(OptimalDeviceSettings, In, MatchOptions);

                    Result = FindValidDeviceSettings(ValidDeviceSettings, In, MatchOptions, OptimalDeviceSettings);
                    if (Result >= 0) FoundValidDirect3D = true;
                }
                else Result = (int)Error.NoDirect3D;
            }

            if (FoundValidDirect3D)
            {
                Out.CopyFrom(ValidDeviceSettings);
                return 0;
            }

            return Result;
        }

        public static Enumeration GetEnumeration(bool ForceEnumerate = false, bool EnumerateAllAdapterFormats = false)
        {
            if (Enumeration.Singular == null) new Enumeration();

            if ((!Enumeration.Singular.HasEnumerated || ForceEnumerate))
            {
                Enumeration.Singular.SetEnumerateAllAdapterFormats(EnumerateAllAdapterFormats, false);
                Callbacks.IsDeviceAcceptable CallbackIsDeviceAcceptable;
                object UserContext;
                GetCallbackDeviceAcceptable(out CallbackIsDeviceAcceptable, out UserContext);
                Enumeration.Singular.Enumerate(CallbackIsDeviceAcceptable, UserContext);
            }

            return Enumeration.Singular;
        }

        public static bool IsRenderingPaused()
        {
            return GetState().PauseRenderingCount > 0;
        }

        public static bool IsActive()
        {
            return GetState().Active;
        }

        public static double GetTime()
        {
            return GetState().Time;
        }

        public static float GetElapsedTime()
        {
            return GetState().ElapsedTime;
        }

        static void StartPerformanceCounters()
        {
            if (GetState().NoStats) return;

            var Counter = GetState().CounterGPU_Idle;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterVertexProcessing;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterGeometryProcessing;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterPixelProcessing;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterOtherGPU_Processing;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterHostAdapterBandwidthUtilization;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterLocalVidmemBandwidthUtilization;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterVertexThroughputUtilization;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterTriangleSetupThroughputUtilization;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterFillrateThrougputUtilization;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterVS_MemoryLimited;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterVS_ComputationLimited;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterGS_MemoryLimited;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterGS_ComputationLimited;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterPS_MemoryLimited;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterPS_ComputationLimited;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterPostTransformCacheHitRate;
            if (Counter != null) Counter.Begin();

            Counter = GetState().CounterTextureCacheHitRate;
            if (Counter != null) Counter.Begin();
        }

        //--------------------------------------------------------------------------------------
        // Updates the frames/sec stat once per second
        //--------------------------------------------------------------------------------------
        static void UpdateFrameStats()
        {
            if (GetState().NoStats) return;

            // Keep track of the frame count
            var LastTime = GetState().LastStatsUpdateTime;
            var Frames = GetState().LastStatsUpdateFrames;
            var AbsoluteTime = GetState().AbsoluteTime;
            Frames++;
            GetState().LastStatsUpdateFrames = Frames;

            // Update the scene stats once per second
            if (AbsoluteTime - LastTime > 1.0f)
            {
                var FPS = (float)(Frames / (AbsoluteTime - LastTime));
                GetState().FPS = FPS;
                GetState().LastStatsUpdateTime = AbsoluteTime;
                GetState().LastStatsUpdateFrames = 0;

                GetState().FPS_Stats = string.Format("{0:0.00} fps ", FPS);
            }
        }

        //--------------------------------------------------------------------------------------
        // Internal helper function to handle calling the user defined timer callbacks
        //--------------------------------------------------------------------------------------
        static void HandleTimers()
        {
            var ElapsedTime = GetElapsedTime();

            var Timers = GetState().Timers;
            if (Timers == null) return;

            // Walk through the list of timer callbacks
            for (var I = 0; I < Timers.Count; I++)
            {
                var Timer = Timers[I];
                if (Timer.Enabled)
                {
                    Timer.Countdown -= ElapsedTime;

                    // Call the callback if count down expired
                    if (Timer.Countdown < 0)
                    {
                        Timer.CallbackTimer(Timer.ID, Timer.CallbackUserContext);
                        // The callback my have changed the timer.
                        Timer = Timers[I];
                        Timer.Countdown = Timer.TimeoutInSecs;
                    }
                    Timers[I] = Timer;
                }
            }
        }

        //--------------------------------------------------------------------------------------
        // This method is called when D3DERR_DEVICEREMOVED is returned from an API.  DXUT
        // calls the application's DeviceRemoved callback to inform it of the event.  The
        // application returns true if it wants DXUT to look for a closest device to run on.
        // If no device is found, or the app returns false, DXUT shuts down.
        //--------------------------------------------------------------------------------------
        static int HandleDeviceRemoved()
        {
            // Device has been removed. Call the application's callback if set.  If no callback
            // has been set, then just look for a new device
            var LookForNewDevice = true;
            var DeviceRemovedFunction = GetState().DeviceRemovedFunction;
            if (DeviceRemovedFunction != null) LookForNewDevice = DeviceRemovedFunction(GetState().DeviceRemovedFunctionUserContext);

            if (LookForNewDevice)
            {
                var DeviceSettings = GetState().CurrentDeviceSettings;

                var MatchOptions = new MatchOptions
                {
                    AdapterOrdinal = MatchType.ClosestToInput,
                    DeviceType = MatchType.ClosestToInput,
                    Windowed = MatchType.ClosestToInput,
                    AdapterFormat = MatchType.ClosestToInput,
                    VertexProcessing = MatchType.ClosestToInput,
                    Resolution = MatchType.ClosestToInput,
                    BackBufferFormat = MatchType.ClosestToInput,
                    BackBufferCount = MatchType.ClosestToInput,
                    MultiSample = MatchType.ClosestToInput,
                    SwapEffect = MatchType.ClosestToInput,
                    DepthFormat = MatchType.ClosestToInput,
                    StencilFormat = MatchType.ClosestToInput,
                    PresentFlags = MatchType.ClosestToInput,
                    RefreshRate = MatchType.ClosestToInput,
                    PresentInterval = MatchType.ClosestToInput
                };

                var Result = FindValidDeviceSettings(DeviceSettings, DeviceSettings, MatchOptions);
                if (Result >= 0)
                {
                    // Change to a Direct3D device created from the new device settings
                    // that is compatible with the removed device.
                    Result = ChangeDevice(DeviceSettings, null, true, false);
                    if (Result >= 0) return 0;
                }
            }

            // The app does not wish to continue or continuing is not possible.
            return (int)Error.DeviceRemoved;
        }

        static void StopPerformanceCounters()
        {
            if (GetState().NoStats) return;

            var Counter = GetState().CounterGPU_Idle;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterVertexProcessing;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterGeometryProcessing;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterPixelProcessing;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterOtherGPU_Processing;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterHostAdapterBandwidthUtilization;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterLocalVidmemBandwidthUtilization;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterVertexThroughputUtilization;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterTriangleSetupThroughputUtilization;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterFillrateThrougputUtilization;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterVS_MemoryLimited;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterVS_ComputationLimited;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterGS_MemoryLimited;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterGS_ComputationLimited;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterPS_MemoryLimited;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterPS_ComputationLimited;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterPostTransformCacheHitRate;
            if (Counter != null) Counter.End();

            Counter = GetState().CounterTextureCacheHitRate;
            if (Counter != null) Counter.End();
        }

        static void UpdateCounterStats()
        {
            if (GetState().NoStats) return;

            var CounterData = GetState().CounterData;

            var Data = new UnmanagedMemory<float>(sizeof(float));
            float F;

            var Counter = GetState().CounterGPU_Idle;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.GPU_Idle = F;
                }
            }

            Counter = GetState().CounterVertexProcessing;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.VertexProcessing = F;
                }
            }

            Counter = GetState().CounterGeometryProcessing;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.GeometryProcessing = F;
                }
            }

            Counter = GetState().CounterPixelProcessing;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.PixelProcessing = F;
                }
            }

            Counter = GetState().CounterOtherGPU_Processing;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.OtherGPU_Processing = F;
                }
            }

            Counter = GetState().CounterHostAdapterBandwidthUtilization;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.HostAdapterBandwidthUtilization = F;
                }
            }

            Counter = GetState().CounterLocalVidmemBandwidthUtilization;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.LocalVidmemBandwidthUtilization = F;
                }
            }

            Counter = GetState().CounterVertexThroughputUtilization;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.VertexThroughputUtilization = F;
                }
            }

            Counter = GetState().CounterTriangleSetupThroughputUtilization;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.TriangleSetupThroughputUtilization = F;
                }
            }

            Counter = GetState().CounterFillrateThrougputUtilization;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.FillrateThroughputUtilization = F;
                }
            }

            Counter = GetState().CounterVS_MemoryLimited;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.VS_MemoryLimited = F;
                }
            }

            Counter = GetState().CounterVS_ComputationLimited;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.VS_ComputationLimited = F;
                }
            }

            Counter = GetState().CounterGS_MemoryLimited;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.GS_MemoryLimited = F;
                }
            }

            Counter = GetState().CounterGS_ComputationLimited;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.GS_ComputationLimited = F;
                }
            }

            Counter = GetState().CounterPS_MemoryLimited;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.PS_MemoryLimited = F;
                }
            }

            Counter = GetState().CounterPS_ComputationLimited;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.PS_ComputationLimited = F;
                }
            }

            Counter = GetState().CounterPostTransformCacheHitRate;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.PostTransformCacheHitRate = F;
                }
            }

            Counter = GetState().CounterTextureCacheHitRate;
            if (Counter != null)
            {
                if (Counter.GetData(Data, sizeof(float), AsyncGetDataFlag.DoNotFlush) >= 0)
                {
                    Data.Get(out F);
                    CounterData.TextureCacheHitRate = F;
                }
            }

            // plop everything into a string
            const string Format = "GPUIdle: {0}\n" +
                                  "VertexProcessing: {1}\n" +
                                  "GeometryProcessing: {2}\n" +
                                  "PixelProcessing: {3}\n" +
                                  "OtherGPUProcessing: {4}\n" +
                                  "HostAdapterBandwidthUtilization: {5}\n" +
                                  "LocalVidmemBandwidthUtilization: {6}\n" +
                                  "VertexThroughputUtilization: {7}\n" +
                                  "TriangleSetupThroughputUtilization: {8}\n" +
                                  "FillrateThroughputUtilization: {9}\n" +
                                  "VSMemoryLimited: {10}\n" +
                                  "VSComputationLimited: {11}\n" +
                                  "GSMemoryLimited: {12}\n" +
                                  "GSComputationLimited: {13}\n" +
                                  "PSMemoryLimited: {14}\n" +
                                  "PSComputationLimited: {15}\n" +
                                  "PostTransformCacheHitRate: {16}\n" +
                                  "TextureCacheHitRate: {17}\n";

            GetState().CounterStats = string.Format(Format,
                             CounterData.GPU_Idle,
                             CounterData.VertexProcessing,
                             CounterData.GeometryProcessing,
                             CounterData.PixelProcessing,
                             CounterData.OtherGPU_Processing,
                             CounterData.HostAdapterBandwidthUtilization,
                             CounterData.LocalVidmemBandwidthUtilization,
                             CounterData.VertexThroughputUtilization,
                             CounterData.TriangleSetupThroughputUtilization,
                             CounterData.FillrateThroughputUtilization,
                             CounterData.VS_MemoryLimited,
                             CounterData.VS_ComputationLimited,
                             CounterData.GS_MemoryLimited,
                             CounterData.GS_ComputationLimited,
                             CounterData.PS_MemoryLimited,
                             CounterData.PS_ComputationLimited,
                             CounterData.PostTransformCacheHitRate,
                             CounterData.TextureCacheHitRate);
        }

        //--------------------------------------------------------------------------------------
        // Render the 3D environment by:
        //      - Checking if the device is lost and trying to reset it if it is
        //      - Get the elapsed time since the last frame
        //      - Calling the app's framemove and render callback
        //      - Calling Present()
        //--------------------------------------------------------------------------------------
        public static void RenderEnvironment()
        {
            var Device = GetDevice();
            if (Device == null) return;

            var SwapChain = GetSwapChain();
            if (SwapChain == null) return;

            if (IsRenderingPaused() || !IsActive() || GetState().RenderingOccluded)
            {
                // Window is minimized/paused/occluded/or not exclusive so yield CPU time to other processes
                Thread.Sleep(50);
            }

            // Get the app's time, in seconds. Skip rendering if no time elapsed
            double Time;
            double AbsoluteTime;
            float ElapsedTime;
            GetGlobalTimer().GetTimeValues(out Time, out AbsoluteTime, out ElapsedTime);

            // Store the time for the app
            if (GetState().ConstantFrameTime)
            {
                ElapsedTime = GetState().TimePerFrame;
                Time = GetTime() + ElapsedTime;
            }

            GetState().Time = Time;
            GetState().AbsoluteTime = AbsoluteTime;
            GetState().ElapsedTime = ElapsedTime;

            // Start Performance Counters
            StartPerformanceCounters();

            // Update the FPS stats
            UpdateFrameStats();

            HandleTimers();

            // Animate the scene by calling the app's frame move callback
            var CallbackFrameMove = GetState().FrameMoveFunction;
            if (CallbackFrameMove != null)
            {
                CallbackFrameMove(Time, ElapsedTime, GetState().FrameMoveFunctionUserContext);
                Device = GetDevice();
                if (Device == null) return; // Handle DXUTShutdown from inside callback
            }

            if (!GetState().RenderingPaused)
            {
                // Render the scene by calling the app's render callback
                var CallbackFrameRender = GetState().FrameRenderFunction;
                if (CallbackFrameRender != null && !GetState().RenderingOccluded)
                {
                    CallbackFrameRender(Device, Time, ElapsedTime, GetState().FrameRenderFunctionUserContext);
                    Device = GetDevice();
                    if (Device == null) return; // Handle DXUTShutdown from inside callback
                }
            }

            var Flags = GetState().RenderingOccluded ? PresentFlag.Test : GetState().CurrentDeviceSettings.PresentFlags;
            var SyncInterval = GetState().CurrentDeviceSettings.SyncInterval;

            // Show the frame on the primary surface.
            var Result = SwapChain.Present(SyncInterval, Flags);
            // ReSharper disable ConvertIfStatementToSwitchStatement
            if (Result == (int)Status.Occluded)
            // ReSharper restore ConvertIfStatementToSwitchStatement
            {
                // There is a window covering our entire rendering area.
                // Don't render until we're visible again.
                GetState().RenderingOccluded = true;
            }
            else if (Result == (int)DXGI_Error.DeviceReset)
            {
                // If a mode change happened, we must reset the device
                Result = ResetEnvironment();
                if (Result < 0)
                {
                    if (Result == (int)Error.ResettingDeviceObjects || Result == (int)Error.MediaNotFound)
                    {
                        DisplayErrorMessage(Result);
                        Shutdown();
                        return;
                    }

                    // Reset failed, but the device wasn't lost so something bad happened, 
                    // so recreate the device to try to recover
                    var DeviceSettings = GetState().CurrentDeviceSettings;
                    if (ChangeDevice(DeviceSettings, null, true, false) < 0)
                    {
                        Shutdown();
                        return;
                    }

                    // xTODO:  Handle display orientation changes in full-screen mode.
                }
            }
            else if (Result == (int)DXGI_Error.DeviceRemoved)
            {
                // Use a callback to ask the app if it would like to find a new device.  
                // If no device removed callback is set, then look for a new device
                if (HandleDeviceRemoved() < 0)
                {
                    // xTODO: use pD3DDevice->GetDeviceRemovedReason()
                    DisplayErrorMessage((int)Error.DeviceRemoved);
                    Shutdown();
                    return;
                }
            }
            else if (Result >= 0)
            {
                if (GetState().RenderingOccluded)
                {
                    // Now that we're no longer occluded
                    // allow us to render again
                    GetState().RenderingOccluded = false;
                }
            }

            // Update current frame #
            GetState().CurrentFrameNumber++;

            // Stop performance counters
            StopPerformanceCounters();

            // Update the D3D10 counter stats
            UpdateCounterStats();

            return;
        }

        public static void HandlePaintEvent()
        {
            // Handle paint messages when the app is paused
            if (IsRenderingPaused() && GetState().DeviceObjectsCreated && GetState().DeviceObjectsReset)
            {
                int Result;
                var Time = GetTime();
                var ElapsedTime = GetElapsedTime();

                var Device = GetDevice();
                if (Device != null)
                {
                    var CallbackFrameRender = GetState().FrameRenderFunction;
                    if (CallbackFrameRender != null && !GetState().RenderingOccluded)
                    {
                        CallbackFrameRender(Device, Time, ElapsedTime, GetState().FrameRenderFunctionUserContext);
                    }

                    //var Flags = GetState().RenderingOccluded ? PresentFlag.Test : GetState().CurrentDeviceSettings.PresentFlags;

                    var SwapChain = GetSwapChain();
                    Result = SwapChain.Present(0, GetState().CurrentDeviceSettings.PresentFlags);
                    if (Result == (int)Status.Occluded)
                    {
                        // There is a window covering our entire rendering area.
                        // Don't render until we're visible again.
                        GetState().RenderingOccluded = true;
                    }
                    else if (Result >= 0)
                    {
                        if (GetState().RenderingOccluded)
                        {
                            // Now that we're no longer occluded
                            // allow us to render again
                            GetState().RenderingOccluded = false;
                        }
                    }
                }
            }
        }

        public static void HandleResizeEvent()
        {
            if (GetForm().WindowState == FormWindowState.Minimized)
            {
                Pause(true, true); // Pause while we're minimized

                GetState().Minimized = true;
                GetState().Maximized = false;
            }
            else
            {
                var CurrentClient = GetForm().DesktopBounds;
                if (CurrentClient.Top == 0 && CurrentClient.Bottom == 0)
                {
                    // Rapidly clicking the task bar to minimize and restore a window
                    // can cause a WM_SIZE message with SIZE_RESTORED when 
                    // the window has actually become minimized due to rapid change
                    // so just ignore this message
                }
                else if (GetForm().WindowState == FormWindowState.Maximized)
                {
                    if (GetState().Minimized) Pause(false, false); // Unpause since we're no longer minimized
                    GetState().Minimized = false;
                    GetState().Maximized = true;
                    CheckForWindowSizeChange();
                    CheckForWindowChangingMonitors();
                }
                else if (GetForm().WindowState == FormWindowState.Normal)
                {
                    //DXUTCheckForDXGIFullScreenSwitch();
                    if (GetState().Maximized)
                    {
                        GetState().Maximized = false;
                        CheckForWindowSizeChange();
                        CheckForWindowChangingMonitors();
                    }
                    else if (GetState().Minimized)
                    {
                        Pause(false, false); // Unpause since we're no longer minimized
                        GetState().Minimized = false;
                        CheckForWindowSizeChange();
                        CheckForWindowChangingMonitors();
                    }
                    else if (GetState().InSizeMove)
                    {
                        // If we're neither maximized nor minimized, the window size 
                        // is changing by the user dragging the window edges.  In this 
                        // case, we don't reset the device yet -- we wait until the 
                        // user stops dragging, and a WM_EXITSIZEMOVE message comes.
                    }
                    else
                    {
                        // This WM_SIZE come from resizing the window via an API like SetWindowPos() so 
                        // resize and reset the device now.
                        CheckForWindowSizeChange();
                        CheckForWindowChangingMonitors();
                    }
                }
            }
        }

        //--------------------------------------------------------------------------------------
        // Checks if DXGI buffers need to change
        //--------------------------------------------------------------------------------------
        static void CheckForBufferChange()
        {
            if (GetSwapChain() != null && !GetState().ReleasingSwapChain)
            {
                var SwapChain = GetSwapChain();

                // Determine if we're fullscreen
                bool FullScreen;
                SwapChain.GetFullscreenState(out FullScreen);

                ResizeBuffers(0, 0, FullScreen);

                GetForm().Show();
            }
        }

        //--------------------------------------------------------------------------------------
        // Checks if the window client rect has changed and if it has, then reset the device
        //--------------------------------------------------------------------------------------
        static void CheckForWindowSizeChange()
        {
            // Skip the check for various reasons
            if (GetState().IgnoreSizeChange || !GetState().DeviceCreated) return;

            CheckForBufferChange();
        }

        //--------------------------------------------------------------------------------------
        // Look for an adapter ordinal that is tied to a HMONITOR
        //--------------------------------------------------------------------------------------
        static int GetAdapterOrdinalFromMonitor(Screen Monitor, out uint AdapterOrdinal)
        {
            AdapterOrdinal = 0;

            // Search for this monitor in our enumeration hierarchy.
            var Enumeration = GetEnumeration();
            var Adapters = Enumeration.AdapterInfos;
            foreach (var AdapterInfo in Adapters)
            {
                if (AdapterInfo.OutputInfos.Any(OutputInfo => OutputInfo.Description.DeviceName == Monitor.DeviceName))
                {
                    AdapterOrdinal = AdapterInfo.AdapterOrdinal;
                    return 0;
                }
            }

            return (int)Error.Fail;
        }

        //--------------------------------------------------------------------------------------
        // Look for a monitor ordinal that is tied to a HMONITOR (D3D10-only)
        //--------------------------------------------------------------------------------------
        static int GetOutputOrdinalFromMonitor(Screen Monitor, out uint OutputOrdinal)
        {
            // Search for this monitor in our enumeration hierarchy.
            var Enumeration = GetEnumeration();
            var Adapters = Enumeration.AdapterInfos;
            foreach (var AdapterInfo in Adapters)
            {
                foreach (var OutputInfo in AdapterInfo.OutputInfos)
                {
                    OutputDescription Description;
                    OutputInfo.Output.GetDescription(out Description);

                    if (Monitor.DeviceName == Description.DeviceName)
                    {
                        OutputOrdinal = OutputInfo.OutputOrdinal;
                        return 0;
                    }
                }
            }

            OutputOrdinal = 0;
            return (int)Error.Fail;
        }

        //--------------------------------------------------------------------------------------
        // Checks to see if the HWND changed monitors, and if it did it creates a device 
        // from the monitor's adapter and recreates the scene.
        //--------------------------------------------------------------------------------------
        static void CheckForWindowChangingMonitors()
        {
            // Skip this check for various reasons
            if (!GetState().AutoChangeAdapter || GetState().IgnoreSizeChange || !GetState().DeviceCreated || !IsWindowed()) return;

            int Result;
            var WindowMonitor = Screen.FromControl(GetForm());
            var AdapterMonitor = GetState().AdapterMonitor;
            if (WindowMonitor != AdapterMonitor)
            {
                uint NewOrdinal;
                if (GetAdapterOrdinalFromMonitor(WindowMonitor, out NewOrdinal) >= 0)
                {
                    // Find the closest valid device settings with the new ordinal
                    var DeviceSettings = GetDeviceSettings().Copy();
                    DeviceSettings.AdapterOrdinal = NewOrdinal;
                    uint NewOutput;
                    if (GetOutputOrdinalFromMonitor(WindowMonitor, out NewOutput) >= 0) DeviceSettings.Output = NewOutput;

                    var MatchOptions = new MatchOptions
                    {
                        AdapterOrdinal = MatchType.PreserveInput,
                        DeviceType = MatchType.ClosestToInput,
                        Windowed = MatchType.ClosestToInput,
                        AdapterFormat = MatchType.ClosestToInput,
                        VertexProcessing = MatchType.ClosestToInput,
                        Resolution = MatchType.ClosestToInput,
                        BackBufferFormat = MatchType.ClosestToInput,
                        BackBufferCount = MatchType.ClosestToInput,
                        MultiSample = MatchType.ClosestToInput,
                        SwapEffect = MatchType.ClosestToInput,
                        DepthFormat = MatchType.ClosestToInput,
                        StencilFormat = MatchType.ClosestToInput,
                        PresentFlags = MatchType.ClosestToInput,
                        RefreshRate = MatchType.ClosestToInput,
                        PresentInterval = MatchType.ClosestToInput
                    };

                    Result = FindValidDeviceSettings(DeviceSettings, DeviceSettings, MatchOptions);
                    if (Result >= 0)
                    {
                        // Create a Direct3D device using the new device settings.  
                        // If there is an existing device, then it will either reset or recreate the scene.
                        Result = ChangeDevice(DeviceSettings, null, false, false);

                        // If hr == E_ABORT, this means the app rejected the device settings in the ModifySettingsCallback
                        if (Result == (int)Error.Abort)
                        {
                            // so nothing changed and keep from attempting to switch adapters next time
                            GetState().AutoChangeAdapter = false;
                        }
                        else if (Result < 0)
                        {
                            Shutdown();
                            Pause(false, false);
                            return;
                        }
                    }
                }
            }
        }

        public static void HandleResizeBeginEvent()
        {
            Pause(true, true);
            GetState().InSizeMove = true;
        }

        public static void HandleResizeEndEvent()
        {
            Pause(false, false);
            CheckForWindowSizeChange();
            CheckForWindowChangingMonitors();
            GetState().InSizeMove = false;
        }

        public static void HandleCursorChangedEvent()
        {
            if (IsActive() && !IsWindowed())
            {
                if (!GetState().ShowCursorWhenFullScreen) GetForm().Cursor = null;
            }
        }

        public static void HandleActivatedEvent()
        {
            if (!IsActive()) // Handle only if previously not active 
            {
                GetState().Active = true;

                // Enable controller rumble & input when activating app
                EnableXInput(true);

                // The GetMinimizedWhileFullscreen() varible is used instead of !DXUTIsWindowed()
                // to handle the rare case toggling to windowed mode while the fullscreen application 
                // is minimized and thus making the pause count wrong
                if (GetState().MinimizedWhileFullscreen)
                {
                    GetState().MinimizedWhileFullscreen = false;

                    if (IsApplicationRendering())
                    {
                        ToggleFullScreen();
                    }
                }
            }
        }

        public static int ToggleFullScreen()
        {
            // Get the current device settings and flip the windowed state then
            // find the closest valid device settings with this change
            var DeviceSettings = GetDeviceSettings().Copy();
            var OrginalDeviceSettings = DeviceSettings;

            // Togggle windowed/fullscreen bit
            DeviceSettings.SwapChainDescription.Windowed = !DeviceSettings.SwapChainDescription.Windowed;

            var MatchOptions = new MatchOptions
            {
                AdapterOrdinal = MatchType.PreserveInput,
                DeviceType = MatchType.ClosestToInput,
                Windowed = MatchType.PreserveInput,
                AdapterFormat = MatchType.IgnoreInput,
                VertexProcessing = MatchType.ClosestToInput,
                BackBufferFormat = MatchType.IgnoreInput,
                BackBufferCount = MatchType.ClosestToInput,
                MultiSample = MatchType.ClosestToInput,
                SwapEffect = MatchType.ClosestToInput,
                DepthFormat = MatchType.ClosestToInput,
                StencilFormat = MatchType.ClosestToInput,
                PresentFlags = MatchType.ClosestToInput,
                RefreshRate = MatchType.IgnoreInput,
                PresentInterval = MatchType.ClosestToInput
            };

            // Go back to previous state

            var IsWindowed = GetIsWindowedFromDeviceSettings(DeviceSettings);
            var Width = (IsWindowed) ? GetState().WindowBackBufferWidthAtModeChange : GetState().FullScreenBackBufferWidthAtModeChange;
            var Height = (IsWindowed) ? GetState().WindowBackBufferHeightAtModeChange : GetState().FullScreenBackBufferHeightAtModeChange;

            if (Width > 0 && Height > 0)
            {
                MatchOptions.Resolution = MatchType.ClosestToInput;
                DeviceSettings.SwapChainDescription.BufferDescription.Width = Width;
                DeviceSettings.SwapChainDescription.BufferDescription.Height = Height;
            }
            else
            {
                // No previous data, so just switch to defaults
                MatchOptions.Resolution = MatchType.IgnoreInput;
            }

            var Result = FindValidDeviceSettings(DeviceSettings, DeviceSettings, MatchOptions);
            if (Result >= 0)
            {
                // Create a Direct3D device using the new device settings.  
                // If there is an existing device, then it will either reset or recreate the scene.
                Result = ChangeDevice(DeviceSettings, null, false, false);

                // If hr == E_ABORT, this means the app rejected the device settings in the ModifySettingsCallback so nothing changed
                if (Result < 0 && (Result != (int)Error.Abort))
                {
                    // Failed creating device, try to switch back.
                    var Result2 = ChangeDevice(OrginalDeviceSettings, null, false, false);
                    if (Result2 < 0)
                    {
                        // If this failed, then shutdown
                        Shutdown();
                    }
                }
            }

            return Result;
        }

        public static bool IsApplicationRendering()
        {
            return (GetState().Device != null);
        }

        public static void HandleDeactivateEvent()
        {
            if (IsActive()) // Handle only if previously active 
            {
                GetState().Active = false;

                // Disable any controller rumble & input when de-activating app
                EnableXInput(false);

                if (!IsWindowed())
                {
                    // Going from full screen to a minimized state 
                    Cursor.Clip = Rectangle.Empty; // don't limit the cursor anymore
                    GetState().MinimizedWhileFullscreen = true;
                }
            }
        }

        //--------------------------------------------------------------------------------------
        // Don't pause the game or deactive the window without first stopping rumble otherwise 
        // the controller will continue to rumble
        //--------------------------------------------------------------------------------------
        static void EnableXInput(bool B)
        {
        }

        public static void HandleKeyDownEvent(KeyEventArgs E)
        {
            switch (E.KeyCode)
            {
            case Keys.Escape:
                if (GetState().HandleEscape) GetForm().Close();
                break;
            case Keys.Pause:
                if (GetState().HandlePause) Pause(!IsTimePaused(), false);
                break;
            case Keys.F10:
                E.SuppressKeyPress = GetForm().Menu == null;
                break;
            case Keys.Enter:
                if (E.Alt) ToggleFullScreen();
                break;
            }
        }

        public static bool IsTimePaused()
        {
            return GetState().PauseTimeCount > 0;
        }

        public static ResourceCache GetGlobalResourceCache()
        {
            if (ResourceCache.Singular == null) new ResourceCache();

            return ResourceCache.Singular;
        }

        public static Format MakeTypeless(Format Format)
        {
            if (!IsInGammaCorrectMode()) return Format;

            switch (Format)
            {
            case Format.R8G8B8A8_UNorm_SRGB:
            case Format.R8G8B8A8_UNorm:
            case Format.R8G8B8A8_UInt:
            case Format.R8G8B8A8_SNorm:
            case Format.R8G8B8A8_SInt:
                return Format.R8G8B8A8_Typeless;

            case Format.BC1_UNorm_SRGB:
            case Format.BC1_UNorm:
                return Format.BC1_Typeless;
            case Format.BC2_UNorm_SRGB:
            case Format.BC2_UNorm:
                return Format.BC2_Typeless;
            case Format.BC3_UNorm_SRGB:
            case Format.BC3_UNorm:
                return Format.BC3_Typeless;
            };

            return Format;
        }

        public static Format MakeSRGB(Format Format)
        {
            if (!IsInGammaCorrectMode()) return Format;

            switch (Format)
            {
            case Format.R8G8B8A8_Typeless:
            case Format.R8G8B8A8_UNorm:
            case Format.R8G8B8A8_UInt:
            case Format.R8G8B8A8_SNorm:
            case Format.R8G8B8A8_SInt:
                return Format.R8G8B8A8_UNorm_SRGB;

            case Format.BC1_Typeless:
            case Format.BC1_UNorm:
                return Format.BC1_UNorm_SRGB;
            case Format.BC2_Typeless:
            case Format.BC2_UNorm:
                return Format.BC2_UNorm_SRGB;
            case Format.BC3_Typeless:
            case Format.BC3_UNorm:
                return Format.BC3_UNorm_SRGB;
            }

            return Format;
        }

        //--------------------------------------------------------------------------------------
        // Tries to find the location of a SDK media file
        //--------------------------------------------------------------------------------------
        public static int FindSDK_MediaFileCch(out string DestinationPath, string Filename)
        {
            DestinationPath = null;

            if (string.IsNullOrEmpty(Filename)) return (int)Error.InvalidArgument;

            // Get the exe name, and exe path
            var ExePath = System.Reflection.Assembly.GetEntryAssembly().Location;
            var LastSlashIndex = ExePath.LastIndexOf('\\');

            var ExeName = ExePath.Substring(LastSlashIndex + 1);
            ExeName = ExeName.Substring(0, ExeName.LastIndexOf('.'));

            ExePath = ExePath.Substring(0, LastSlashIndex);

            // Typical directories:
            //      .\
            //      ..\
            //      ..\..\
            //      %EXE_DIR%\
            //      %EXE_DIR%\..\
            //      %EXE_DIR%\..\..\
            //      %EXE_DIR%\..\%EXE_NAME%
            //      %EXE_DIR%\..\..\%EXE_NAME%

            // Typical directory search
            var Found = FindMediaSearchTypicalDirs(out DestinationPath, Filename, ExePath, ExeName);
            if (Found) return 0;

            // Typical directory search again, but also look in a subdir called "\media\" 
            var SearchFor = "media\\" + Filename;
            Found = FindMediaSearchTypicalDirs(out DestinationPath, SearchFor, ExePath, ExeName);
            if (Found) return 0;

            // Search all parent directories starting at .\ and using Filename as the leaf name
            var LeafName = Filename;
            Found = FindMediaSearchParentDirs(out DestinationPath, ".", LeafName);
            if (Found) return 0;

            // Search all parent directories starting at the exe's dir and using Filename as the leaf name
            Found = FindMediaSearchParentDirs(out DestinationPath, ExePath, LeafName);
            if (Found) return 0;

            // Search all parent directories starting at .\ and using "media\Filename" as the leaf name
            LeafName = "media\\" + Filename;
            Found = FindMediaSearchParentDirs(out DestinationPath, ".", LeafName);
            if (Found) return 0;

            // Search all parent directories starting at the exe's dir and using "media\Filename" as the leaf name
            Found = FindMediaSearchParentDirs(out DestinationPath, ExePath, LeafName);
            if (Found) return 0;

            // On failure, return the file as the path but also return an error code
            DestinationPath = Filename;

            return (int)Error.MediaNotFound;
        }

        //--------------------------------------------------------------------------------------
        // Search a set of typical directories
        //--------------------------------------------------------------------------------------
        static bool FindMediaSearchTypicalDirs(out string SearchPath, string Leaf, string ExePath, string ExeName)
        {
            // Typical directories:
            //      .\
            //      ..\
            //      ..\..\
            //      %EXE_DIR%\
            //      %EXE_DIR%\..\
            //      %EXE_DIR%\..\..\
            //      %EXE_DIR%\..\%EXE_NAME%
            //      %EXE_DIR%\..\..\%EXE_NAME%
            //      DXSDK media path

            // Search in .\  
            SearchPath = Leaf;
            if (File.Exists(SearchPath)) return true;

            // Search in ..\  
            SearchPath = "..\\" + Leaf;
            if (File.Exists(SearchPath)) return true;

            // Search in ..\..\ 
            SearchPath = "..\\..\\" + Leaf;
            if (File.Exists(SearchPath)) return true;

            // Search in ..\..\ 
            SearchPath = "..\\..\\" + Leaf;
            if (File.Exists(SearchPath)) return true;

            // Search in the %EXE_DIR%\ 
            SearchPath = ExePath + "\\" + Leaf;
            if (File.Exists(SearchPath)) return true;

            // Search in the %EXE_DIR%\..\ 
            SearchPath = ExePath + "\\..\\" + Leaf;
            if (File.Exists(SearchPath)) return true;

            // Search in the %EXE_DIR%\..\..\ 
            SearchPath = ExePath + "\\..\\..\\" + Leaf;
            if (File.Exists(SearchPath)) return true;

            // Search in "%EXE_DIR%\..\%EXE_NAME%\".  This matches the DirectX SDK layout
            SearchPath = ExePath + "\\..\\" + ExeName + "\\" + Leaf;
            if (File.Exists(SearchPath)) return true;

            // Search in "%EXE_DIR%\..\..\%EXE_NAME%\".  This matches the DirectX SDK layout
            SearchPath = ExePath + "\\..\\..\\" + ExeName + "\\" + Leaf;
            if (File.Exists(SearchPath)) return true;

            // Search in media search dir 
            if (!string.IsNullOrEmpty(MediaSearchPath))
            {
                SearchPath = MediaSearchPath + Leaf;
                if (File.Exists(SearchPath)) return true;
            }

            return false;
        }

        public static string MediaSearchPath { get; set; }

        //--------------------------------------------------------------------------------------
        // Search parent directories starting at strStartAt, and appending strLeafName
        // at each parent directory.  It stops at the root directory.
        //--------------------------------------------------------------------------------------
        static bool FindMediaSearchParentDirs(out string SearchPath, string StartAt, string LeafName)
        {
            SearchPath = null;

            var FullPath = Path.GetFullPath(StartAt);
            var LastSlashIndex = FullPath.LastIndexOf('\\');
            if (LastSlashIndex == -1) return false;
            var FilePart = FullPath.Substring(LastSlashIndex + 1);

            while (!string.IsNullOrEmpty(FilePart))
            {
                var FullFileName = FullPath + "\\" + LeafName;
                if (File.Exists(FullFileName))
                {
                    SearchPath = FullFileName;
                    return true;
                }

                var Search = FullPath + "\\..";
                FullPath = Path.GetFullPath(Search);
                LastSlashIndex = FullPath.LastIndexOf('\\');
                FilePart = FullPath.Substring(LastSlashIndex + 1);
            }

            return false;
        }

        public static void GetDesktopResolution(uint AdapterOrdinal, out uint Width, out uint Height)
        {
            var DeviceSettings = GetDeviceSettings();

            var DeviceName = "";

            var Enumeration = GetEnumeration();
            var OutputInfo = Enumeration.GetOutputInfo(AdapterOrdinal, DeviceSettings.Output);
            if (OutputInfo != null) DeviceName = OutputInfo.Description.DeviceName;

            var ScreenMode = Screen.AllScreens.First(X => X.DeviceName == DeviceName);
            Width = (uint)ScreenMode.Bounds.Width;
            Height = (uint)ScreenMode.Bounds.Height;
        }

        public static Rectangle GetFullsceenClientRectangleAtModeChange()
        {
            return new Rectangle(0, 0, (int)GetState().FullScreenBackBufferWidthAtModeChange, (int)GetState().FullScreenBackBufferHeightAtModeChange);
        }

        public static Rectangle GetWindowClientRectangleAtModeChange()
        {
            return new Rectangle(0, 0, (int)GetState().WindowBackBufferWidthAtModeChange, (int)GetState().WindowBackBufferHeightAtModeChange);
        }

        public static int CreateDeviceFromSettings(DeviceSettings DeviceSettings, bool PreserveInput = false, bool ClipWindowToSingleAdapter = false)
        {
            int Result;

            GetState().DeviceCreateCalled = true;

            if (!PreserveInput)
            {
                // If not preserving the input, then find the closest valid to it
                var MatchOptions = new MatchOptions
                {
                    AdapterOrdinal = MatchType.ClosestToInput,
                    Output = MatchType.ClosestToInput,
                    DeviceType = MatchType.ClosestToInput,
                    Windowed = MatchType.ClosestToInput,
                    AdapterFormat = MatchType.ClosestToInput,
                    VertexProcessing = MatchType.ClosestToInput,
                    Resolution = MatchType.ClosestToInput,
                    BackBufferFormat = MatchType.ClosestToInput,
                    BackBufferCount = MatchType.ClosestToInput,
                    MultiSample = MatchType.ClosestToInput,
                    SwapEffect = MatchType.ClosestToInput,
                    DepthFormat = MatchType.ClosestToInput,
                    StencilFormat = MatchType.ClosestToInput,
                    PresentFlags = MatchType.ClosestToInput,
                    RefreshRate = MatchType.ClosestToInput,
                    PresentInterval = MatchType.ClosestToInput
                };

                Result = FindValidDeviceSettings(DeviceSettings, DeviceSettings, MatchOptions);
                if (Result < 0) // the call will fail if no valid devices were found
                {
                    DisplayErrorMessage(Result);
                    return Result;
                }
            }

            // Change to a Direct3D device created from the new device settings.  
            // If there is an existing device, then either reset or recreate the scene
            Result = ChangeDevice(DeviceSettings, null, false, ClipWindowToSingleAdapter);
            return Result < 0 ? Result : 0;
        }

        public static int SetTimer(Callbacks.Timer CallbackTimer, float TimeoutInSeconds, out uint IDEvent, object CallbackUserContext)
        {
            IDEvent = 0;

            if (CallbackTimer == null) return ErrorBox((int)Error.InvalidArgument, "SetTimer");

            var CurrentTimer = new State.Timer
            {
                CallbackTimer = CallbackTimer,
                CallbackUserContext = CallbackUserContext,
                TimeoutInSecs = TimeoutInSeconds,
                Countdown = TimeoutInSeconds,
                Enabled = true,
                ID = GetState().TimerLastID + 1
            };
            GetState().TimerLastID = CurrentTimer.ID;

            var Timers = GetState().Timers;
            if (Timers == null)
            {
                Timers = new List<State.Timer>();
                GetState().Timers = Timers;
            }

            Timers.Add(CurrentTimer);

            IDEvent = CurrentTimer.ID;

            return 0;
        }

        public static int KillTimer(uint IDEvent)
        {
            var Timers = GetState().Timers;
            if (Timers == null) return (int)Error.False;

            var Found = false;

            for (var I = 0; I < Timers.Count; I++)
            {
                var CurrentTimer = Timers[I];
                if (CurrentTimer.ID == IDEvent)
                {
                    CurrentTimer.Enabled = false;
                    Timers[I] = CurrentTimer;
                    Found = true;
                    break;
                }
            }

            return !Found ? ErrorBox((int)Error.InvalidArgument, "KillTimer") : 0;
        }

        public static string GetFrameStats(bool ShowFPS)
        {
            var FPS = (ShowFPS) ? GetState().FPS_Stats : "";
            var FrameStats = string.Format(GetState().StaticFrameStats, FPS);
            GetState().FrameStats = FrameStats;
            return FrameStats;
        }

        public static bool IsVsyncEnabled()
        {
            var DeviceSettings = GetState().CurrentDeviceSettings;
            if (DeviceSettings != null)
            {
                return (DeviceSettings.SyncInterval == 0);
            }
            return true;
        }

        public static string GetDeviceStats()
        {
            return GetState().DeviceStats;
        }

        public static int ToggleREF()
        {
            var DeviceSettings = GetDeviceSettings();
            var OrginalDeviceSettings = GetDeviceSettings();

            // Toggle between REF & HAL
            object Object;
            var Result = GetDevice().QueryInterface(typeof(SwitchToRef), out Object);
            var SwitchToRef = (SwitchToRef)Object;
            if (Result >= 0)
            {
                SwitchToRef.SetUseRef(SwitchToRef.GetUseRef() ? false : true);
                if (SwitchToRef != null) SwitchToRef.Release();
                return 0;
            }

            if (DeviceSettings.DriverType == DriverType.Hardware || DeviceSettings.DriverType == DriverType.Software) DeviceSettings.DriverType = DriverType.Reference;
            else if (DeviceSettings.DriverType == DriverType.Reference) DeviceSettings.DriverType = DriverType.Hardware;

            var MatchOptions = new MatchOptions
            {
                AdapterOrdinal = MatchType.PreserveInput,
                DeviceType = MatchType.PreserveInput,
                Windowed = MatchType.ClosestToInput,
                AdapterFormat = MatchType.ClosestToInput,
                VertexProcessing = MatchType.ClosestToInput,
                Resolution = MatchType.ClosestToInput,
                BackBufferFormat = MatchType.ClosestToInput,
                BackBufferCount = MatchType.ClosestToInput,
                MultiSample = MatchType.ClosestToInput,
                SwapEffect = MatchType.ClosestToInput,
                DepthFormat = MatchType.ClosestToInput,
                StencilFormat = MatchType.ClosestToInput,
                PresentFlags = MatchType.ClosestToInput,
                RefreshRate = MatchType.ClosestToInput,
                PresentInterval = MatchType.ClosestToInput
            };

            Result = FindValidDeviceSettings(DeviceSettings, DeviceSettings, MatchOptions);
            if (Result >= 0)
            {
                // Create a Direct3D device using the new device settings.  
                // If there is an existing device, then it will either reset or recreate the scene.
                Result = ChangeDevice(DeviceSettings, null, false, false);

                // If hr == E_ABORT, this means the app rejected the device settings in the ModifySettingsCallback so nothing changed
                if (Result < 0 && (Result != (int)Error.Abort))
                {
                    // Failed creating device, try to switch back.
                    var Result2 = ChangeDevice(OrginalDeviceSettings, null, false, false);
                    if (Result2 < 0)
                    {
                        // If this failed, then shutdown
                        Shutdown();
                    }
                }
            }

            return Result;
        }
    }
}