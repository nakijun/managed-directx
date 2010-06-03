using System.Collections.Generic;
using Xtro.MDX.DXGI;

namespace Xtro.MDX.Utilities
{
    public class Enumeration
    {
        bool HasEnumerated;
        Callbacks.IsDeviceAcceptable IsDeviceAcceptableFunction;
        object IsDeviceAcceptableFunctionUserContext;

        List<Format> DepthStencilPossibleList;

        uint MinWidth;
        uint MaxWidth;
        uint MinHeight;
        uint MaxHeight;
        uint RefreshMin;
        uint RefreshMax;
        uint MultisampleQualityMax;
        bool EnumerateAllAdapterFormats;

        List<EnumerationAdapterInfo> AdapterInfoList = new List<EnumerationAdapterInfo>();

        void ClearAdapterInfoList()
        {
            foreach (var AdapterInfo in AdapterInfoList) AdapterInfo.Delete();
            AdapterInfoList.Clear();
        }
    }
}
