using System.Collections.Generic;
using Xtro.MDX.DXGI;

namespace Xtro.MDX.Utilities
{
    public class TEnumeration
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

        List<TEnumerationAdapterInfo> AdapterInfoList;
    }
}
