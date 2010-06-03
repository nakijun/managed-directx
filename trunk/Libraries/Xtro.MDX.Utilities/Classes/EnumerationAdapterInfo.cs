using System.Collections.Generic;
using Xtro.MDX.DXGI;

namespace Xtro.MDX.Utilities
{
    class EnumerationAdapterInfo
    {
        public uint AdapterOrdinal;
        public AdapterDescription AdapterDescription;
        public string UniqueDescription;
        public Adapter Adapter;

        public List<EnumerationOutputInfo> OutputInfoList = new List<EnumerationOutputInfo>();
        public List<EnumerationDeviceInfo> DeviceInfoList = new List<EnumerationDeviceInfo>();
        public List<EnumerationDeviceSettingsCombo> DeviceSettingsComboList = new List<EnumerationDeviceSettingsCombo>();

        public void Delete()
        {
            foreach (var OutputInfo in OutputInfoList) OutputInfo.Delete();
            OutputInfoList = null;
            DeviceInfoList = null;
            DeviceSettingsComboList = null;

            if (Adapter != null) Adapter.Release();
            Adapter = null;
        }
    }
}
