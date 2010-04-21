using System.Collections.Generic;
using Xtro.MDX.DXGI;

namespace Xtro.MDX.Utilities
{
    class TEnumerationOutputInfo
    {
        public uint AdapterOrdinal;
        public uint OutputOrdinal;
        public Output Output;
        public OutputDescription Description;

        public List<ModeDescription> DisplayModeList;

        public void Delete()
        {
            if (Output != null)
            {
                Output.Release();
                Output = null;
            }
        }
    }
}
