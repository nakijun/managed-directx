using System.Collections.Generic;
using Xtro.MDX.DXGI;

namespace Xtro.MDX.Utilities
{
    class TEnumerationAdapterInfo
    {
        public uint AdapterOrdinal;
        public AdapterDescription AdapterDescription;
        public string UniqueDescription;
        public Adapter Adapter;

        public List<TEnumerationOutputInfo> OutputInfoList;
        public List<TEnumerationDeviceInfo> DeviceInfoList;
        public List<SEnumerationDeviceSettingsCombo> DeviceSettingsComboList;
    }
}
