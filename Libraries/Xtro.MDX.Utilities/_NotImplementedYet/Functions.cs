using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.DXGI;
using DXGI_Functions = Xtro.MDX.DXGI.Functions;
using Usage = Xtro.MDX.DXGI.Usage;

namespace Xtro.MDX.Utilities
{
    public static class Functions
    {
        static State State;
        static Timer Timer;

        static State GetState()
        {
            CreateState();

            return State;
        }

        static int DelayLoad()
        {
            var State = GetState();

            var Factory = State.GetFactory();
            if (Factory == null)
            {
                DXGI_Functions.CreateFactory(typeof(Factory), out Factory);
                State.SetFactory(Factory);
                if (Factory == null)
                {
                    // If still NULL, then DXGI is not availible
                    State.SetDirect3D_Available(false);
                    return (int)Error.NoDirect3D;
                }

                State.SetDirect3D_Available(true);
            }

            return 0;
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
            Callback = GetState().GetIsDeviceAcceptableFunction();
            UserContext = GetState().GetIsDeviceAcceptableFunctionUserContext();
        }

        public static void SetCallbackDeviceAcceptable(Callbacks.IsDeviceAcceptable Callback, object UserContext)
        {
            GetState().SetIsDeviceAcceptableFunction(Callback);
            GetState().SetIsDeviceAcceptableFunctionUserContext(UserContext);
        }

        public static void SetCallbackDeviceCreated(Callbacks.DeviceCreated Callback, object UserContext)
        {
            GetState().SetDeviceCreatedFunction(Callback);
            GetState().SetDeviceCreatedFunctionUserContext(UserContext);
        }

        public static void SetCallbackDeviceDestroyed(Callbacks.DeviceDestroyed Callback, object UserContext)
        {
            GetState().SetDeviceDestroyedFunction(Callback);
            GetState().SetDeviceDestroyedFunctionUserContext(UserContext);
        }

        public static void SetCallbackSwapChainResized(Callbacks.SwapChainResized Callback, object UserContext)
        {
            GetState().SetSwapChainResizedFunction(Callback);
            GetState().SetSwapChainResizedFunctionUserContext(UserContext);
        }

        public static void SetCallbackSwapChainReleasing(Callbacks.SwapChainReleasing Callback, object UserContext)
        {
            GetState().SetSwapChainReleasingFunction(Callback);
            GetState().SetSwapChainReleasingFunctionUserContext(UserContext);
        }

        public static void SetCallbackFrameRender(Callbacks.FrameRender Callback, object UserContext)
        {
            GetState().SetFrameRenderFunction(Callback);
            GetState().SetFrameRenderFunctionUserContext(UserContext);
        }

        public static void SetCallbackFrameMove(Callbacks.FrameMove Callback, object UserContext)
        {
            GetState().SetFrameMoveFunction(Callback);
            GetState().SetFrameMoveFunctionUserContext(UserContext);
        }

        public static void SetCallbackModifyDeviceSettings(Callbacks.ModifyDeviceSettings Callback, object UserContext)
        {
            GetState().SetModifyDeviceSettingsFunction(Callback);
            GetState().SetModifyDeviceSettingsFunctionUserContext(UserContext);
        }

        public static bool IsInGammaCorrectMode()
        {
            return GetState().GetIsInGammaCorrectMode();
        }

        public static int Initialize(bool ShowMessageBoxOnError)
        {
            GetState().SetInitializeCalled(true);

            GetState().SetShowMessageBoxOnError(ShowMessageBoxOnError);

            GetGlobalTimer().Reset();

            GetState().SetInitialized(true);

            return 0;
        }

        public static Timer GetGlobalTimer()
        {
            if (Timer == null) Timer = new Timer();

            return Timer;
        }

        public static int CreateDevice(bool Windowed, int SuggestedWidth, int SuggestedHeight)
        {
            var Result = 0;

            GetState().SetDeviceCreateCalled(true);

            MatchOptions MatchOptions;
            MatchOptions.AdapterOrdinal = MatchType.IgnoreInput;
            MatchOptions.DeviceType = MatchType.IgnoreInput;
            MatchOptions.Output = MatchType.IgnoreInput;
            MatchOptions.Windowed = MatchType.PreserveInput;
            MatchOptions.AdapterFormat = MatchType.IgnoreInput;
            MatchOptions.VertexProcessing = MatchType.IgnoreInput;
            if (Windowed || (SuggestedWidth != 0 && SuggestedHeight != 0))
                MatchOptions.Resolution = MatchType.ClosestToInput;
            else
                MatchOptions.Resolution = MatchType.IgnoreInput;
            MatchOptions.BackBufferFormat = MatchType.IgnoreInput;
            MatchOptions.BackBufferCount = MatchType.IgnoreInput;
            MatchOptions.MultiSample = MatchType.IgnoreInput;
            MatchOptions.SwapEffect = MatchType.IgnoreInput;
            MatchOptions.DepthFormat = MatchType.IgnoreInput;
            MatchOptions.StencilFormat = MatchType.IgnoreInput;
            MatchOptions.PresentFlags = MatchType.IgnoreInput;
            MatchOptions.RefreshRate = MatchType.IgnoreInput;
            MatchOptions.PresentInterval = MatchType.IgnoreInput;

            DeviceSettings DeviceSettings;
            DeviceSettings.SwapChainDescription.Windowed = Windowed;
            DeviceSettings.SwapChainDescription.BufferDescription.Width = (uint)SuggestedWidth;
            DeviceSettings.SwapChainDescription.BufferDescription.Height = (uint)SuggestedHeight;

            //Result = FindValidDeviceSettings
            /*
                hr = DXUTFindValidDeviceSettings( &deviceSettings, &deviceSettings, &matchOptions );
                if( FAILED( hr ) ) // the call will fail if no valid devices were found
                {
                    DXUTDisplayErrorMessage( hr );
                    return DXUT_ERR( L"DXUTFindValidDeviceSettings", hr );
                }

                // Change to a Direct3D device created from the new device settings.  
                // If there is an existing device, then either reset or recreated the scene
                hr = DXUTChangeDevice( &deviceSettings, NULL, NULL, false, true );
                if( FAILED( hr ) )
                    return hr;
            */
            return Result;
        }

        public static bool GetShowMessageBoxOnError()
        {
            return GetState().GetShowMessageBoxOnError();
        }

        public static Form GetForm()
        {
            return GetState().GetForm();
        }

        public static Factory GetFactory()
        {
            DelayLoad();

            return GetState().GetFactory();
        }

        public static int Trace(int Result, string Message)
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

            return GetState().GetIsDeviceAcceptableFunction() != null ||
                   GetState().GetDeviceCreatedFunction() != null ||
                   GetState().GetSwapChainResizedFunction() != null ||
                   GetState().GetFrameRenderFunction() != null ||
                   GetState().GetSwapChainReleasingFunction() != null ||
                   GetState().GetDeviceDestroyedFunction() != null;
        }

        public static bool GetDirect3D_Available()
        {
            DelayLoad();

            return GetState().GetDirect3D_Available();
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

            // TODO: verify this is needed
            if (ModeDescription.Format == Format.B8G8R8A8_UNorm) ModeDescription.Format = Format.R8G8B8A8_UNorm;

            return 0;
        }

        //--------------------------------------------------------------------------------------
        // Internal helper function to build a D3D10 device settings structure based upon the match 
        // options.  If the match option is set to ignore, then a optimal default value is used.
        // The default value may not exist on the system, but later this will be taken 
        // into account.
        //--------------------------------------------------------------------------------------
        static void BuildOptimalDeviceSettings(out DeviceSettings OptimalDeviceSettings, ref DeviceSettings DeviceSettingsIn, ref MatchOptions MatchOptions)
        {
            OptimalDeviceSettings = new DeviceSettings();

            // Retrieve the desktop display mode.
            var AdapterDesktopDisplayMode = new ModeDescription {Width= 640,Height= 480,Format=Format.R8G8B8A8_UNorm_SRGB };
            GetAdapterDisplayMode(OptimalDeviceSettings.AdapterOrdinal, 0,ref AdapterDesktopDisplayMode);

            //---------------------
            // Adapter ordinal
            //---------------------
            OptimalDeviceSettings.AdapterOrdinal = MatchOptions.AdapterOrdinal ==(int)MatchType.IgnoreInput ? 0 : DeviceSettingsIn.AdapterOrdinal;

            //---------------------
            // Device type
            //---------------------
            OptimalDeviceSettings.DriverType = MatchOptions.DeviceType ==(int)MatchType.IgnoreInput ? DriverType.Hardware : DeviceSettingsIn.DriverType;

            //---------------------
            // Windowed
            //---------------------
            OptimalDeviceSettings.SwapChainDescription.Windowed = MatchOptions.Windowed ==(int)MatchType.IgnoreInput || DeviceSettingsIn.SwapChainDescription.Windowed;

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
            OptimalDeviceSettings.SwapChainDescription.BufferUsage =Usage.RenderTargetOutput;

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
            OptimalDeviceSettings.SwapChainDescription.SwapEffect = MatchOptions.SwapEffect ==(int)MatchType.IgnoreInput ? SwapEffect.Discard : DeviceSettingsIn.SwapChainDescription.SwapEffect;

            //---------------------
            // Depth stencil 
            //---------------------
            if (MatchOptions.DepthFormat ==(int)MatchType.IgnoreInput && MatchOptions.StencilFormat == (int)MatchType.IgnoreInput)
            {
                OptimalDeviceSettings.AutoCreateDepthStencil = true;
                OptimalDeviceSettings.AutoDepthStencilFormat =Format.D32_Float;
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
        static bool DoesDeviceComboMatchPreserveOptions(Enumeration.DeviceSettingsCombo DeviceSettingsCombo, ref DeviceSettings DeviceSettingsIn, ref MatchOptions MatchOptions)
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
                var Found=false;
                foreach (var DisplayMode in DeviceSettingsCombo.OutputInfo.DisplayModes)
                {
                    if (DisplayMode.Width == DeviceSettingsIn.SwapChainDescription.BufferDescription.Width && DisplayMode.Height == DeviceSettingsIn.SwapChainDescription.BufferDescription.Height)
                    {
                        Found = true;
                        break;
                    }
                }

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
        static float RankDeviceCombo(Enumeration.DeviceSettingsCombo DeviceSettingsCombo, ref DeviceSettings OptimalDeviceSettings, ref ModeDescription AdapterDisplayMode)
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
            if (DeviceSettingsCombo.DeviceType == DriverType.Hardware) CurrentRanking += 0.1f;

            //---------------------
            // Windowed
            //---------------------
            if (DeviceSettingsCombo.Windowed == OptimalDeviceSettings.SwapChainDescription.Windowed) CurrentRanking += WindowWeight;

            //---------------------
            // Resolution
            //---------------------
            var ResolutionFound = false;
            if (DeviceSettingsCombo.OutputInfo != null)
            {
                foreach (var DisplayMode in DeviceSettingsCombo.OutputInfo.DisplayModes)
                {
                    if (DisplayMode.Width == OptimalDeviceSettings.SwapChainDescription.BufferDescription.Width && DisplayMode.Height == OptimalDeviceSettings.SwapChainDescription.BufferDescription.Height) ResolutionFound = true;
                }
            }
            if (ResolutionFound) CurrentRanking += ResolutionWeight;

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

        [DllImport("user32")]
        static extern bool GetMonitorInfo(IntPtr MonitorHandle, [Out] MonitorInfo Info);

        [StructLayout(LayoutKind.Sequential)]
        struct Rect32
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        sealed class MonitorInfo
        {
            public int Size;
            public Rect32 Monitor;
            public Rect32 Work;
            public UInt32 Flags;
        }
        
        //--------------------------------------------------------------------------------------
        // Internal helper function to find the closest allowed display mode to the optimal 
        //--------------------------------------------------------------------------------------
        static int FindValidResolution(Enumeration.DeviceSettingsCombo BestDeviceSettingsCombo,ref ModeDescription DisplayModeIn,out ModeDescription BestDisplayMode)
        {
            if (BestDeviceSettingsCombo.Windowed)
            {
                BestDisplayMode = DisplayModeIn;

                // If our client rect size is smaller than our backbuffer size, use that size.
                // This would happen when we specify a windowed resolution larger than the screen.
                var Info = new MonitorInfo();
                if (BestDeviceSettingsCombo.OutputInfo != null)
                {
                    GetMonitorInfo(BestDeviceSettingsCombo.OutputInfo.Description.Monitor, Info);

                    var Width = Info.Work.Right - Info.Work.Left;
                    var Height = Info.Work.Bottom - Info.Work.Top;

                    var Client = Info.Work;

                    // vvv AdjustWindowRect(&Client, GetWindowLong(DXUTGetHWNDDeviceWindowed(), GWL_STYLE), FALSE);
                    var Form = GetForm();
                    var DeltaWidth = Form.Size.Width - Form.ClientSize.Width;
                    var DeltaHeight = Form.Size.Height - Form.ClientSize.Height;
                    Client.Right += DeltaWidth;
                    Client.Bottom += DeltaHeight;
                    // ^^^

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
                    return -2147467259;// E_FAIL; // No valid display modes found
                }

            }

            return 0;
        }

        //--------------------------------------------------------------------------------------
        // Builds valid device settings using the match options, the input device settings, and the 
        // best device settings combo found.
        //--------------------------------------------------------------------------------------
        static void BuildValidDeviceSettings(out DeviceSettings ValidDeviceSettings, Enumeration.DeviceSettingsCombo BestDeviceSettingsCombo, ref DeviceSettings DeviceSettingsIn, ref MatchOptions MatchOptions)
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
                if (MatchOptions.Resolution == MatchType.ClosestToInput)
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
            if (MatchOptions.BackBufferCount == MatchType.PreserveInput)
            {
                BestBackBufferCount = DeviceSettingsIn.SwapChainDescription.BufferCount;
            }
            else if (MatchOptions.BackBufferCount == MatchType.IgnoreInput)
            {
                // The framework defaults to triple buffering 
                BestBackBufferCount = 2;
            }
            else // if( pMatchOptions->eBackBufferCount == DXUTMT_CLOSEST_TO_INPUT )   
            {
                BestBackBufferCount = DeviceSettingsIn.SwapChainDescription.BufferCount;
                if (BestBackBufferCount > 3) BestBackBufferCount = 3;
                if (BestBackBufferCount < 1) BestBackBufferCount = 1;
            }

            //---------------------
            // Multisample
            //---------------------
            uint BestMultiSampleCount;
            uint BestMultiSampleQuality;
            if (DeviceSettingsIn.SwapChainDescription.SwapEffect != SwapEffect.Discard)
            {
                // Swap effect is not set to discard so multisampling has to off
                BestMultiSampleCount = 1;
                BestMultiSampleQuality = 0;
            }
            else
            {
                if (MatchOptions.MultiSample == MatchType.PreserveInput)
                {
                    BestMultiSampleCount = DeviceSettingsIn.SwapChainDescription.SampleDescription.Count;
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
                        if (Math.Abs(Count - DeviceSettingsIn.SwapChainDescription.SampleDescription.Count) < Math.Abs(BestMultiSampleCount - DeviceSettingsIn.SwapChainDescription.SampleDescription.Count))
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
            if (MatchOptions.SwapEffect == MatchType.PreserveInput) BestSwapEffect = DeviceSettingsIn.SwapChainDescription.SwapEffect;
            else if (MatchOptions.SwapEffect == MatchType.IgnoreInput) BestSwapEffect = SwapEffect.Discard;
            else // if( pMatchOptions->eSwapEffect == DXUTMT_CLOSEST_TO_INPUT )
            {
                BestSwapEffect = DeviceSettingsIn.SwapChainDescription.SwapEffect;

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
                BestEnableAutoDepthStencil = DeviceSettingsIn.AutoCreateDepthStencil;
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
                if (MatchOptions.RefreshRate == MatchType.PreserveInput) BestDisplayMode.RefreshRate = DeviceSettingsIn.SwapChainDescription.BufferDescription.RefreshRate;
                else
                {
                    var RefreshRateMatch = MatchOptions.RefreshRate == MatchType.ClosestToInput ? DeviceSettingsIn.SwapChainDescription.BufferDescription.RefreshRate : AdapterDisplayMode.RefreshRate;

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
            if (MatchOptions.PresentInterval == MatchType.PreserveInput) BestPresentInterval = DeviceSettingsIn.SyncInterval;
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
            else BestPresentInterval = DeviceSettingsIn.SyncInterval;

            // Fill the device settings struct
            ValidDeviceSettings = new DeviceSettings
                {
                    AdapterOrdinal = BestDeviceSettingsCombo.AdapterOrdinal,
                    Output = BestDeviceSettingsCombo.OutputOrdinal,
                    DriverType = BestDeviceSettingsCombo.DeviceType
                };

            ValidDeviceSettings.SwapChainDescription.BufferDescription.Width = BestDisplayMode.Width;
            ValidDeviceSettings.SwapChainDescription.BufferDescription.Height = BestDisplayMode.Height;
            ValidDeviceSettings.SwapChainDescription.BufferDescription.Format = BestDeviceSettingsCombo.BackBufferFormat;
            ValidDeviceSettings.SwapChainDescription.BufferUsage = DeviceSettingsIn.SwapChainDescription.BufferUsage | Usage.RenderTargetOutput;
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

        public static int FindValidDeviceSettings(out DeviceSettings Out, ref DeviceSettings In, ref MatchOptions MatchOptions, ref DeviceSettings Optimal)
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
            var AdapterDisplayMode=new ModeDescription();

            var Enumeration = GetEnumeration();
            var AdapterInfos = Enumeration.AdapterInfos;
            foreach (var AdapterInfo in AdapterInfos)
            {
                // Get the desktop display mode of adapter

                GetAdapterDisplayMode(AdapterInfo.AdapterOrdinal, 0,ref AdapterDisplayMode);

                // Enum all the device settings combinations.  A device settings combination is 
                // a unique set of an adapter format, back buffer format, and IsWindowed.
                foreach (var DeviceSettingsCombo in AdapterInfo.DeviceSettingsCombos)
                {
                    // Skip any combo that doesn't meet the preserve match options
                    if (false == DoesDeviceComboMatchPreserveOptions(DeviceSettingsCombo, ref In, ref MatchOptions)) continue;

                    // Get a ranking number that describes how closely this device combo matches the optimal combo
                    var CurrentRanking = RankDeviceCombo(DeviceSettingsCombo, ref Optimal, ref AdapterDisplayMode);

                    // If this combo better matches the input device settings then save it
                    if (CurrentRanking > BestRanking)
                    {
                        BestDeviceSettingsCombo = DeviceSettingsCombo;
                        BestRanking = CurrentRanking;
                    }
                }
            }

            // If no best device combination was found then fail
            if (BestDeviceSettingsCombo == null)
            {
                Out = new DeviceSettings();
                return (int)Error.NoCompatibleDevices;
            }

            // Using the best device settings combo found, build valid device settings taking heed of 
            // the match options and the input device settings
            DeviceSettings ValidDeviceSettings;
            BuildValidDeviceSettings(out ValidDeviceSettings, BestDeviceSettingsCombo, ref In, ref MatchOptions);
            Out = ValidDeviceSettings;

            return 0;
        }

        public static int FindValidDeviceSettings(out DeviceSettings Out, ref DeviceSettings In, ref MatchOptions MatchOptions)
        {
            var Result = 0;

            var ApplicationSupportDirect3D = DoesApplicationSupportDirect3D();

            // Build an optimal device settings structure based upon the match 
            // options.  If the match option is set to ignore, then a optimal default value is used.
            // The default value may not exist on the system, but later this will be taken 
            // into account.
            var FoundValidDirect3D = false;

            Out=In;

            if (ApplicationSupportDirect3D)
            {
                if (GetDirect3D_Available())
                {
                    // Force an enumeration with the IsDeviceAcceptable callback
                    GetEnumeration();

                    DeviceSettings OptimalDeviceSettings;
                    BuildOptimalDeviceSettings(out OptimalDeviceSettings, ref In, ref MatchOptions);

                    Result = FindValidDeviceSettings(out Out, ref In, ref MatchOptions, ref OptimalDeviceSettings);
                    if (Result>=0) FoundValidDirect3D = true;
                }
                else Result =(int)Error.NoDirect3D;
            }
                             
            return FoundValidDirect3D ? 0 : Result;
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
    }
}