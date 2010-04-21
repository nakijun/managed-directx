using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;

namespace Xtro.MDX.Utilities
{
    public struct DeviceSettings
    {
        public uint AdapterOrdinal;
        public DriverType DriverType;
        public uint Output;
        public SwapChainDescription SwapChainDescription;
        public CreateDeviceFlag CreateFlags;
        public uint SyncInterval;
        public PresentFlag PresentFlags;
        public bool AutoCreateDepthStencil; // Utilities will create the a depth stencil resource and view if true
        public Format AutoDepthStencilFormat;
    };
}
