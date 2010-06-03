using System.Collections.Generic;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.DXGI;

namespace Xtro.MDX.Utilities
{
    struct EnumerationDeviceSettingsCombo
    {
        public uint AdapterOrdinal;
        public DriverType DeviceType;
        public Format BackBufferFormat;
        public bool Windowed;
        public int OutputOrdinal;

        public List<uint> MultiSampleCountList = new List<uint>(); // List of valid sampling counts (multisampling)
        public List<uint> MultiSampleQualityList = new List<uint>(); // List of number of quality levels for each multisample count

        public EnumerationAdapterInfo AdapterInfo;
        public EnumerationDeviceInfo DeviceInfo;
        public EnumerationOutputInfo OutputInfo;
    }
}
