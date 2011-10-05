using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;

namespace Xtro.MDX.Utilities
{
    public sealed class DeviceSettings
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

        public DeviceSettings Copy()
        {    
            return (DeviceSettings)MemberwiseClone();
        }

        public void CopyFrom(DeviceSettings Value)
        {
            AdapterOrdinal = Value.AdapterOrdinal;
            DriverType = Value.DriverType;
            Output = Value.Output;
            SwapChainDescription = Value.SwapChainDescription;
            CreateFlags = Value.CreateFlags;
            SyncInterval = Value.SyncInterval;
            PresentFlags = Value.PresentFlags;
            AutoCreateDepthStencil = Value.AutoCreateDepthStencil;
            AutoDepthStencilFormat = Value.AutoDepthStencilFormat;
        }
    };
}
