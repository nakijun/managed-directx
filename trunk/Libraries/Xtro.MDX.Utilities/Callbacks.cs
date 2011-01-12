using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;
using Device = Xtro.MDX.Direct3D10.Device;

namespace Xtro.MDX.Utilities
{
    public static class Callbacks
    {
        public delegate void FrameMove(double Time, float ElapsedTime, object UserContext);
        public delegate void Timer(uint IdEvent, object UserContext);
        public delegate bool ModifyDeviceSettings(DeviceSettings DeviceSettings, object UserContext);
        public delegate bool DeviceRemoved(object UserContext);

        public delegate bool IsDeviceAcceptable(uint Adapter, uint Output, DriverType DeviceType, Format BackBufferFormat, bool Windowed, object UserContext);
        public delegate int DeviceCreated(Device Device, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext);
        public delegate int SwapChainResized(Device Device, SwapChain SwapChain, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext);
        public delegate void FrameRender(Device Device, double Time, float ElapsedTime, object UserContext);
        public delegate void SwapChainReleasing(object UserContext);
        public delegate void DeviceDestroyed(object UserContext);
    }
}
