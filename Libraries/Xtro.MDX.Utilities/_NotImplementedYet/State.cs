using System.Windows.Forms;
using Xtro.MDX.DXGI;

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

        bool Direct3D_Available;

        Factory Factory;
        Form Form;

        public Callbacks.IsDeviceAcceptable GetIsDeviceAcceptableFunction()
        {
            lock (Lock) { return IsDeviceAcceptableFunction; }
        }

        public object GetIsDeviceAcceptableFunctionUserContext()
        {
            lock (Lock) { return IsDeviceAcceptableFunctionUserContext; }
        }

        public void SetIsDeviceAcceptableFunction(Callbacks.IsDeviceAcceptable Callback)
        {
            lock (Lock) { IsDeviceAcceptableFunction += Callback; }
        }

        public void SetIsDeviceAcceptableFunctionUserContext(object UserContext)
        {
            lock (Lock) { IsDeviceAcceptableFunctionUserContext = UserContext; }
        }

        public Callbacks.DeviceCreated GetDeviceCreatedFunction()
        {
            lock (Lock) { return DeviceCreatedFunction; }
        }

        public void SetDeviceCreatedFunction(Callbacks.DeviceCreated Callback)
        {
            lock (Lock) { DeviceCreatedFunction += Callback; }
        }

        public void SetDeviceCreatedFunctionUserContext(object UserContext)
        {
            lock (Lock) { DeviceCreatedFunctionUserContext = UserContext; }
        }

        public Callbacks.DeviceDestroyed GetDeviceDestroyedFunction()
        {
            lock (Lock) { return DeviceDestroyedFunction; }
        }

        public void SetDeviceDestroyedFunction(Callbacks.DeviceDestroyed Callback)
        {
            lock (Lock) { DeviceDestroyedFunction += Callback; }
        }

        public void SetDeviceDestroyedFunctionUserContext(object UserContext)
        {
            lock (Lock) { DeviceDestroyedFunctionUserContext = UserContext; }
        }

        public Callbacks.SwapChainResized GetSwapChainResizedFunction()
        {
            lock (Lock) { return SwapChainResizedFunction; }
        }

        public void SetSwapChainResizedFunction(Callbacks.SwapChainResized Callback)
        {
            lock (Lock) { SwapChainResizedFunction += Callback; }
        }

        public void SetSwapChainResizedFunctionUserContext(object UserContext)
        {
            lock (Lock) { SwapChainResizedFunctionUserContext = UserContext; }
        }

        public Callbacks.SwapChainReleasing GetSwapChainReleasingFunction()
        {
            lock (Lock) { return SwapChainReleasingFunction; }
        }

        public void SetSwapChainReleasingFunction(Callbacks.SwapChainReleasing Callback)
        {
            lock (Lock) { SwapChainReleasingFunction += Callback; }
        }

        public void SetSwapChainReleasingFunctionUserContext(object UserContext)
        {
            lock (Lock) { SwapChainReleasingFunctionUserContext = UserContext; }
        }

        public Callbacks.FrameRender GetFrameRenderFunction()
        {
            lock (Lock) { return FrameRenderFunction; }
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

        public bool GetDirect3D_Available()
        {
            lock (Lock) { return Direct3D_Available; }
        }

        public void SetDirect3D_Available(bool Value)
        {
            lock (Lock) { Direct3D_Available = Value; }
        }

        public Factory GetFactory()
        {
            lock (Lock) { return Factory; }
        }

        public void SetFactory(Factory Value)
        {
            lock (Lock) { Factory = Value; }
        }

        public Form GetForm()
        {
            lock (Lock) { return Form; }
        }

        public void SetForm(Form Value)
        {
            lock (Lock) { Form = Value; }
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
            SAFE_DELETE(m_TimerList);
            Functions.Shutdown();
        }

        ~State()
        {
            Delete();
        }
    }
}
