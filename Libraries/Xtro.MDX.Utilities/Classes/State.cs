namespace Xtro.MDX.Utilities
{
    sealed class State
    {
        static readonly object Lock = new object(); // Lock Object

        Callbacks.IsDeviceAcceptable IsDeviceAcceptableFunction;
        object IsDeviceAcceptableFunctionUserContext;

        Callbacks.DeviceCreated DeviceCreatedFunction;
        object DeviceCreatedFunctionUserContext;

        Callbacks.DeviceDestroyed DeviceDestroyedFunction;
        object DeviceDestroyedFunctionUserContext;

        Callbacks.SwapChainResized SwapChainResizedFunction;
        object SwapChainResizedFunctionUserContext;

        Callbacks.SwapChainReleasing SwapChainReleasingFunction;
        object SwapChainReleasingFunctionUserContext;

        Callbacks.FrameRender FrameRenderFunction;
        object FrameRenderFunctionUserContext;

        Callbacks.FrameMove FrameMoveFunction;
        object FrameMoveFunctionUserContext;

        Callbacks.ModifyDeviceSettings ModifyDeviceSettingsFunction;
        object ModifyDeviceSettingsFunctionUserContext;

        bool InitializeCalled;
        bool DeviceCreateCalled;
        bool Initialized;
        bool ShowMessageBoxOnError;

        bool IsInGammaCorrectMode;

        public void SetIsDeviceAcceptableFunction(Callbacks.IsDeviceAcceptable Callback)
        {
            lock (Lock) { IsDeviceAcceptableFunction += Callback; }
        }

        public void SetIsDeviceAcceptableFunctionUserContext(object UserContext)
        {
            lock (Lock) { IsDeviceAcceptableFunctionUserContext = UserContext; }
        }

        public void SetDeviceCreatedFunction(Callbacks.DeviceCreated Callback)
        {
            lock (Lock) { DeviceCreatedFunction += Callback; }
        }

        public void SetDeviceCreatedFunctionUserContext(object UserContext)
        {
            lock (Lock) { DeviceCreatedFunctionUserContext = UserContext; }
        }

        public void SetDeviceDestroyedFunction(Callbacks.DeviceDestroyed Callback)
        {
            lock (Lock) { DeviceDestroyedFunction += Callback; }
        }

        public void SetDeviceDestroyedFunctionUserContext(object UserContext)
        {
            lock (Lock) { DeviceDestroyedFunctionUserContext = UserContext; }
        }

        public void SetSwapChainResizedFunction(Callbacks.SwapChainResized Callback)
        {
            lock (Lock) { SwapChainResizedFunction += Callback; }
        }

        public void SetSwapChainResizedFunctionUserContext(object UserContext)
        {
            lock (Lock) { SwapChainResizedFunctionUserContext = UserContext; }
        }

        public void SetSwapChainReleasingFunction(Callbacks.SwapChainReleasing Callback)
        {
            lock (Lock) { SwapChainReleasingFunction += Callback; }
        }

        public void SetSwapChainReleasingFunctionUserContext(object UserContext)
        {
            lock (Lock) { SwapChainReleasingFunctionUserContext = UserContext; }
        }

        public void SetFrameRenderFunction(Callbacks.FrameRender Callback)
        {
            lock (Lock) { FrameRenderFunction += Callback; }
        }

        public void SetFrameRenderFunctionUserContext(object UserContext)
        {
            lock (Lock) { FrameRenderFunctionUserContext = UserContext; }
        }

        public void SetFrameMoveFunction(Callbacks.FrameMove Callback)
        {
            lock (Lock) { FrameMoveFunction += Callback; }
        }

        public void SetFrameMoveFunctionUserContext(object UserContext)
        {
            lock (Lock) { FrameMoveFunctionUserContext = UserContext; }
        }

        public void SetModifyDeviceSettingsFunction(Callbacks.ModifyDeviceSettings Callback)
        {
            lock (Lock) { ModifyDeviceSettingsFunction += Callback; }
        }

        public void SetModifyDeviceSettingsFunctionUserContext(object UserContext)
        {
            lock (Lock) { ModifyDeviceSettingsFunctionUserContext = UserContext; }
        }

        public void SetInitializeCalled(bool Value)
        {
            lock (Lock) { InitializeCalled = Value; }
        }

        public void SetInitialized(bool Value)
        {
            lock (Lock) { Initialized = Value; }
        }

        public void SetShowMessageBoxOnError(bool Value)
        {
            lock (Lock) { ShowMessageBoxOnError = Value; }
        }

        public bool GetShowMessageBoxOnError()
        {
            lock (Lock) { return ShowMessageBoxOnError; }
        }

        public bool GetIsInGammaCorrectMode()
        {
            lock (Lock) { return IsInGammaCorrectMode; }
        }

        public void SetDeviceCreateCalled(bool Value)
        {
            lock (Lock) { DeviceCreateCalled = Value; }
        }

        public State()
        {
        }

        public void Delete()
        {
            SAFE_DELETE(m_state.m_TimerList);
            DXUTShutdown();
        }

        ~State()
        {
            Delete();
        }
    }
}
