using System.Collections.Generic;
using Xtro.MDX.DXGI;

namespace Xtro.MDX.Utilities
{
    class EnumerationOutputInfo
    {
        public uint AdapterOrdinal;
        public uint OutputOrdinal;
        public Output Output;
        public OutputDescription Description;

        public List<ModeDescription> DisplayModeList = new List<ModeDescription>();

        public void Delete()
        {
            DisplayModeList = null;

            if (Output != null)
            {
                Output.Release();
                Output = null;
            }
        }
    }
}
