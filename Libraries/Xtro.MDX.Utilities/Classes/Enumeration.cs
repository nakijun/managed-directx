using System.Collections.Generic;
using System.Windows.Forms;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.DXGI;
using Error = Xtro.MDX.DXGI.Error;

namespace Xtro.MDX.Utilities
{
    public class Enumeration
    {
        struct DeviceSettingsCombo
        {
            public uint AdapterOrdinal;
            public DriverType DeviceType;
            public Format BackBufferFormat;
            public bool Windowed;
            public int OutputOrdinal;

            public List<uint> MultiSampleCountList; // List of valid sampling counts (multisampling)
            public List<uint> MultiSampleQualityList; // List of number of quality levels for each multisample count

            public AdapterInfo AdapterInfo;
            public DeviceInfo DeviceInfo;
            public OutputInfo OutputInfo;
        }

        class AdapterInfo
        {
            public uint AdapterOrdinal;
            public AdapterDescription AdapterDescription;
            public string UniqueDescription;
            public Adapter Adapter;

            public List<OutputInfo> OutputInfoList = new List<OutputInfo>();
            public List<DeviceInfo> DeviceInfoList = new List<DeviceInfo>();
            public List<DeviceSettingsCombo> DeviceSettingsComboList = new List<DeviceSettingsCombo>();

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

        class DeviceInfo
        {
            public uint AdapterOrdinal;
            DriverType DeviceType;
        }

        class OutputInfo
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

        bool HasEnumerated;
        Callbacks.IsDeviceAcceptable IsDeviceAcceptableFunction;
        object IsDeviceAcceptableFunctionUserContext;

        List<Format> DepthStencilPossibleList = new List<Format>();

        uint MinWidth;
        uint MaxWidth;
        uint MinHeight;
        uint MaxHeight;
        uint RefreshMin;
        uint RefreshMax;
        uint MultisampleQualityMax;
        bool EnumerateAllAdapterFormats;

        List<AdapterInfo> AdapterInfoList = new List<AdapterInfo>();

        int EnumerateOutputs(AdapterInfo AdapterInfo)
        {
            for (uint OutputNo = 0; ; OutputNo++)
            {
                Output Output;
                var Result = AdapterInfo.Adapter.EnumerateOutputs(OutputNo, out Output);

                if (Result == (int)Error.NotFound) return 0;
                if (Result < 0) return Result; //Something bad happened.

                //Success!

                var OutputInfo = new OutputInfo();
                Output.GetDescription(out OutputInfo.Description);
                OutputInfo.OutputOrdinal = OutputNo;
                OutputInfo.Output = Output;

                EnumerateDisplayModes(OutputInfo);
                if (OutputInfo.DisplayModeList.Count <= 0)
                {
                    // If this output has no valid display mode, do not save it.
                    OutputInfo.Delete();
                    continue;
                }

                AdapterInfo.OutputInfoList.Add(OutputInfo);
            }
        }

        int EnumerateDisplayModes(OutputInfo OutputInfo)
        {
            var Result = 0;

            var AllowedAdapterFormats = new[]
            {
                Format.R8G8B8A8_UNorm_SRGB,
                Format.R8G8B8A8_UNorm,
                Format.R16G16B16A16_Float,
                Format.R10G10B10A2_UNorm
            };

            // Swap perferred modes for apps running in linear space
            if (!Functions.IsInGammaCorrectMode())
            {
                AllowedAdapterFormats[0] = Format.R8G8B8A8_UNorm;
                AllowedAdapterFormats[1] = Format.R8G8B8A8_UNorm_SRGB;
            }

            // The fast path only enumerates R8G8B8A8_UNORM_SRGB modes
            var AllowedAdapterFormatArrayCount = EnumerateAllAdapterFormats ? AllowedAdapterFormats.Length : 1;

            for (var F = 0; F < AllowedAdapterFormatArrayCount; F++)
            {
                uint ModeCount = 0;
                Result = OutputInfo.Output.GetDisplayModeList(AllowedAdapterFormats[F], EnumerationMode.Scaling, ref ModeCount, null);
                if (Result < 0) break;

                var ModeDescriptions = new ModeDescription[ModeCount];

                Result = OutputInfo.Output.GetDisplayModeList(AllowedAdapterFormats[F], EnumerationMode.Scaling, ref ModeCount, ModeDescriptions);
                if (Result == (int)Error.NotFound) break;
                if (Result < 0) continue;

                foreach (var Mode in ModeDescriptions) OutputInfo.DisplayModeList.Add(Mode);
            }

            return Result;
        }

        void ClearAdapterInfoList()
        {
            foreach (var AdapterInfo in AdapterInfoList) AdapterInfo.Delete();
            AdapterInfoList.Clear();
        }
    }
}
