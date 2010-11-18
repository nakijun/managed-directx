using System.Windows.Forms;

namespace Xtro.MDX.Utilities
{
// ReSharper disable InconsistentNaming
    public static class Functions
// ReSharper restore InconsistentNaming
    {
        static State State;
        static Timer Timer;

        static State GetState()
        {
            CreateState();

            return State;
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
            DeviceSettings.SwapChainDescription.BufferDescription.Width = SuggestedWidth;
            DeviceSettings.SwapChainDescription.BufferDescription.Height = SuggestedHeight;

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

        public static int Trace(int Result, string Message)
        {
            if (GetShowMessageBoxOnError()) MessageBox.Show("An error has occured with error " + Result + " : " + Message);

            return Result;
        }

        public static int FindValidDeviceSettings(out DeviceSettings Out, ref DeviceSettings In, MatchOptions? MatchOptions)
        {
            var Result = 0;

            // Default to MatchType.IgnoreInput for everything unless MatchOptions isn't null
            if (MatchOptions == null) MatchOptions = new MatchOptions();

            //GetEnumeration

            return Result;
        }

        public static Enumeration GetEnumeration(bool ForceEnumerate, bool EnumerateAllAdapterFormats)
        {
        }
    }
}
