using System.Collections.Generic;
using System.Linq;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.DXGI;
using D3D10Functions = Xtro.MDX.Direct3D10.Functions;
using Device = Xtro.MDX.Direct3D10.Device;
using DXGI_Error = Xtro.MDX.DXGI.Error;
using DXGI_Device = Xtro.MDX.DXGI.Device;

namespace Xtro.MDX.Utilities
{
    public sealed class Enumeration
    {
        public sealed class DeviceSettingsCombo
        {
            public uint AdapterOrdinal;
            public DriverType DeviceType;
            public Format BackBufferFormat;
            public bool Windowed;
            public uint OutputOrdinal;

            public readonly List<uint> MultiSampleCounts = new List<uint>(); // List of valid sampling counts (multisampling)
            public readonly List<uint> MultiSampleQualities = new List<uint>(); // List of number of quality levels for each multisample count

            public AdapterInfo AdapterInfo;
            public DeviceInfo DeviceInfo;
            public OutputInfo OutputInfo;
        }

        public sealed class AdapterInfo
        {
            public uint AdapterOrdinal;
            public AdapterDescription AdapterDescription;
            public string UniqueDescription;
            public Adapter Adapter;

            public readonly List<OutputInfo> OutputInfos = new List<OutputInfo>();
            public readonly List<DeviceInfo> DeviceInfos = new List<DeviceInfo>();
            public readonly List<DeviceSettingsCombo> DeviceSettingsCombos = new List<DeviceSettingsCombo>();

            public void Delete()
            {
                foreach (var OutputInfo in OutputInfos) OutputInfo.Delete();

                if (Adapter != null) Adapter.Release();
                Adapter = null;
            }
        }

        public sealed class DeviceInfo
        {
            public uint AdapterOrdinal;
            public DriverType DeviceType;
        }

        public sealed class OutputInfo
        {
            public uint AdapterOrdinal;
            public uint OutputOrdinal;
            public Output Output;
            public OutputDescription Description;

            public readonly List<ModeDescription> DisplayModes = new List<ModeDescription>();

            public void Delete()
            {
                if (Output != null)
                {
                    Output.Release();
                    Output = null;
                }
            }
        }

        internal Enumeration()
        {
            ResetPossibleDepthStencilFormats();

            Singular = this;
        }

        public void Delete()
        {
            ClearAdapterInfos();
        }

        ~Enumeration()
        {
            Delete();
        }

        public bool HasEnumerated { get; private set; }
        Callbacks.IsDeviceAcceptable IsDeviceAcceptableFunction;
        object IsDeviceAcceptableFunctionUserContext;

        public readonly List<Format> DepthStencilPossiblities = new List<Format>();
        bool EnumerateAllAdapterFormats;

        public readonly List<AdapterInfo> AdapterInfos = new List<AdapterInfo>();

        int EnumerateOutputs(AdapterInfo AdapterInfo)
        {
            for (uint OutputNo = 0; ; OutputNo++)
            {
                Output Output;
                var Result = AdapterInfo.Adapter.EnumerateOutputs(OutputNo, out Output);

                if (Result == (int)DXGI_Error.NotFound) return 0;
                if (Result < 0) return Result; //Something bad happened.

                //Success!

                var OutputInfo = new OutputInfo();
                Output.GetDescription(out OutputInfo.Description);
                OutputInfo.OutputOrdinal = OutputNo;
                OutputInfo.Output = Output;

                EnumerateDisplayModes(OutputInfo);
                if (OutputInfo.DisplayModes.Count <= 0)
                {
                    // If this output has no valid display mode, do not save it.
                    OutputInfo.Delete();
                    continue;
                }

                AdapterInfo.OutputInfos.Add(OutputInfo);
            }
        }

        static int EnumerateDevices(AdapterInfo AdapterInfo)
        {
            var DriverTypes = new[]
            {
                DriverType.Hardware,
                DriverType.Reference
            };

            // Enumerate each Direct3D device type
            foreach (var DeviceType in DriverTypes)
            {
                var DeviceInfo = new DeviceInfo
                    {
                        AdapterOrdinal = AdapterInfo.AdapterOrdinal,
                        DeviceType = DeviceType
                    };

                // Call CreateDevice to ensure that this is a D3D10 device.
                Device Device;
                Adapter Adapter = null;
                if (DeviceType == DriverType.Hardware) Adapter = AdapterInfo.Adapter;

                var Result = D3D10Functions.CreateDevice(Adapter, DeviceType, null, 0, out Device);
                if (Result < 0) continue;

                if (DeviceType != DriverType.Hardware)
                {
                    object Device2;
                    Result = Device.QueryInterface(typeof(DXGI_Device), out Device2);
                    if (Device2 != null)
                    {
                        if (Result >= 0)
                        {
                            if (AdapterInfo.Adapter != null) AdapterInfo.Adapter.Release();
                            ((DXGI_Device)Device2).GetAdapter(out AdapterInfo.Adapter);
                        }
                        ((DXGI_Device)Device2).Release();
                    }
                }

                if (Device != null) Device.Release();
                AdapterInfo.DeviceInfos.Add(DeviceInfo);
            }

            return 0;
        }

        int EnumerateDeviceCombos(/*Factory Factory,*/ AdapterInfo AdapterInfo)
        {
            // Iterate through each combination of device driver type, output,
            // adapter format, and backbuffer format to build the adapter's device combo list.
            for (var OutputNo = 0; OutputNo < AdapterInfo.OutputInfos.Count; OutputNo++)
            {
                var OutputInfo = AdapterInfo.OutputInfos[OutputNo];

                foreach (var DeviceInfo in AdapterInfo.DeviceInfos)
                {
                    var BackBufferFormats = new[]
                    {
                        Format.R8G8B8A8_UNorm_SRGB, //This is DXUT's preferred mode

                        Format.R8G8B8A8_UNorm,
                        Format.R16G16B16A16_Float,
                        Format.R10G10B10A2_UNorm
                    };

                    // Swap perferred modes for apps running in linear space
                    if (!Functions.GetIsInGammaCorrectMode())
                    {
                        BackBufferFormats[0] = Format.R8G8B8A8_UNorm;
                        BackBufferFormats[1] = Format.R8G8B8A8_UNorm_SRGB;
                    }

                    foreach (var BackBufferFormat in BackBufferFormats)
                    {
                        for (var Windowed = 0; Windowed < 2; Windowed++)
                        {
                            if (Windowed == 0 && OutputInfo.DisplayModes.Count == 0) continue;

                            // determine if there are any modes for this particular format
                            if (OutputInfo.DisplayModes.Count(DisplayMode => BackBufferFormat == DisplayMode.Format) < 1) continue;

                            // If an application callback function has been provided, make sure this device
                            // is acceptable to the app.
                            if (IsDeviceAcceptableFunction != null && !IsDeviceAcceptableFunction(AdapterInfo.AdapterOrdinal, (uint)OutputNo, DeviceInfo.DeviceType, BackBufferFormat, Windowed == 1, IsDeviceAcceptableFunctionUserContext)) continue;

                            // At this point, we have an adapter/device/backbufferformat/iswindowed
                            // DeviceCombo that is supported by the system. We still 
                            // need to find one or more suitable depth/stencil buffer format,
                            // multisample type, and present interval.
                            var DeviceCombo = new DeviceSettingsCombo
                            {
                                AdapterOrdinal = DeviceInfo.AdapterOrdinal,
                                DeviceType = DeviceInfo.DeviceType,
                                BackBufferFormat = BackBufferFormat,
                                Windowed = (Windowed == 1),
                                OutputOrdinal = OutputInfo.OutputOrdinal,
                                AdapterInfo = AdapterInfo,
                                DeviceInfo = DeviceInfo,
                                OutputInfo = OutputInfo
                            };

                            BuildMultiSampleQualityList(BackBufferFormat, DeviceCombo);

                            AdapterInfo.DeviceSettingsCombos.Add(DeviceCombo);
                        }
                    }
                }
            }

            return 0;
        }

        int EnumerateDeviceCombosNoAdapter(AdapterInfo AdapterInfo)
        {
            // Iterate through each combination of device driver type, output,
            // adapter format, and backbuffer format to build the adapter's device combo list.
            foreach (var DeviceInfo in AdapterInfo.DeviceInfos)
            {
                var BackBufferFormats = new[]
                {
                    Format.R8G8B8A8_UNorm_SRGB, //This is DXUT's preferred mode

                    Format.R8G8B8A8_UNorm,
                    Format.R16G16B16A16_Float,
                    Format.R10G10B10A2_UNorm
                };

                // Swap perferred modes for apps running in linear space
                if (!Functions.GetIsInGammaCorrectMode())
                {
                    BackBufferFormats[0] = Format.R8G8B8A8_UNorm;
                    BackBufferFormats[1] = Format.R8G8B8A8_UNorm_SRGB;
                }

                foreach (var BackBufferFormat in BackBufferFormats)
                {
                    // If an application callback function has been provided, make sure this device
                    // is acceptable to the app.
                    if (IsDeviceAcceptableFunction != null && !IsDeviceAcceptableFunction(AdapterInfo.AdapterOrdinal, 0, DeviceInfo.DeviceType, BackBufferFormat, true, IsDeviceAcceptableFunctionUserContext)) continue;

                    // At this point, we have an adapter/device/backbufferformat/iswindowed
                    // DeviceCombo that is supported by the system. We still 
                    // need to find one or more suitable depth/stencil buffer format,
                    // multisample type, and present interval.
                    var DeviceCombo = new DeviceSettingsCombo
                    {
                        AdapterOrdinal = DeviceInfo.AdapterOrdinal,
                        DeviceType = DeviceInfo.DeviceType,
                        BackBufferFormat = BackBufferFormat,
                        Windowed = true,
                        OutputOrdinal = 0,
                        AdapterInfo = AdapterInfo,
                        DeviceInfo = DeviceInfo,
                        OutputInfo = null
                    };

                    BuildMultiSampleQualityList(BackBufferFormat, DeviceCombo);

                    AdapterInfo.DeviceSettingsCombos.Add(DeviceCombo);
                }
            }

            return 0;
        }

        void EnumerateDisplayModes(OutputInfo OutputInfo)
        {
            var AllowedAdapterFormats = new[]
            {
                Format.R8G8B8A8_UNorm_SRGB,
                Format.R8G8B8A8_UNorm,
                Format.R16G16B16A16_Float,
                Format.R10G10B10A2_UNorm
            };

            // Swap perferred modes for apps running in linear space
            if (!Functions.GetIsInGammaCorrectMode())
            {
                AllowedAdapterFormats[0] = Format.R8G8B8A8_UNorm;
                AllowedAdapterFormats[1] = Format.R8G8B8A8_UNorm_SRGB;
            }

            // The fast path only enumerates R8G8B8A8_UNORM_SRGB modes
            var AllowedAdapterFormatArrayCount = EnumerateAllAdapterFormats ? AllowedAdapterFormats.Length : 1;

            for (var F = 0; F < AllowedAdapterFormatArrayCount; F++)
            {
                uint ModeCount = 0;
                var Result = OutputInfo.Output.GetDisplayModeList(AllowedAdapterFormats[F], EnumerationModeFlag.Scaling, ref ModeCount, null);
                if (Result < 0) break;

                var ModeDescriptions = new ModeDescription[ModeCount];

                Result = OutputInfo.Output.GetDisplayModeList(AllowedAdapterFormats[F], EnumerationModeFlag.Scaling, ref ModeCount, ModeDescriptions);
                if (Result == (int)DXGI_Error.NotFound) break;
                if (Result < 0) continue;

                foreach (var Mode in ModeDescriptions) OutputInfo.DisplayModes.Add(Mode);
            }

            return;
        }

        static void BuildMultiSampleQualityList(Format Format, DeviceSettingsCombo DeviceCombo)
        {
            Device Device;
            Adapter Adapter = null;
            if (DeviceCombo.DeviceType == DriverType.Hardware) Functions.GetFactory().EnumerateAdapters(DeviceCombo.AdapterInfo.AdapterOrdinal, out Adapter);

            if (D3D10Functions.CreateDevice(Adapter, DeviceCombo.DeviceType, null, 0, out Device) < 0) return;

            for (var I = (uint)1; I <= Constants.MaximumMultisampleSampleCount; I++)
            {
                uint Quality;
                if (Device.CheckMultisampleQualityLevels(Format, I, out Quality) >= 0 && Quality > 0)
                {
                    DeviceCombo.MultiSampleCounts.Add(I);
                    DeviceCombo.MultiSampleQualities.Add(Quality);
                }
            }

            if (Adapter != null) Adapter.Release();
            if (Device != null) Device.Release();
        }

        void ClearAdapterInfos()
        {
            foreach (var AdapterInfo in AdapterInfos) AdapterInfo.Delete();
            AdapterInfos.Clear();
        }

        // These should be called before Enumerate(). 
        //
        // Use these calls and the IsDeviceAcceptable to control the contents of 
        // the enumeration object, which affects the device selection and the device settings dialog.
        /*
                public void SetResolutionMinMax(uint MinWidth, uint MinHeight, uint MaxWidth, uint MaxHeight)
                {
                    this.MinWidth = MinWidth;
                    this.MinHeight = MinHeight;
                    this.MaxWidth = MaxWidth;
                    this.MaxHeight = MaxHeight;
                }

                public void SetRefreshMinMax(uint Min, uint Max)
                {
                    RefreshMin = Min;
                    RefreshMax = Max;
                }

                public void SetMultisampleQualityMax(uint Max)
                {
                    if (Max > 0xFFFF) Max = 0xFFFF;
                    MultisampleQualityMax = Max;
                }
        */
        public void ResetPossibleDepthStencilFormats()
        {
            DepthStencilPossiblities.Clear();
            DepthStencilPossiblities.Add(Format.D32_Float_S8X24_UInt);
            DepthStencilPossiblities.Add(Format.D32_Float);
            DepthStencilPossiblities.Add(Format.D24_UNorm_S8_UInt);
            DepthStencilPossiblities.Add(Format.D16_UNorm);
        }

        internal static Enumeration Singular;

        public void SetEnumerateAllAdapterFormats(bool EnumerateAllAdapterFormats, bool EnumerateNow = true)
        {
            this.EnumerateAllAdapterFormats = EnumerateAllAdapterFormats;

            if (EnumerateNow)
            {
                Callbacks.IsDeviceAcceptable CallbackIsDeviceAcceptable;
                object UserContext;
                Functions.GetCallbackDeviceAcceptable(out CallbackIsDeviceAcceptable, out UserContext);
                Singular.Enumerate(CallbackIsDeviceAcceptable, UserContext);
            }
        }

        // Call Enumerate() to enumerate available D3D10 adapters, devices, modes, etc.
        public int Enumerate(Callbacks.IsDeviceAcceptable IsDeviceAcceptableFunction, object IsDeviceAcceptableFunctionUserContext)
        {
            int Result;
            var Factory = Functions.GetFactory();
            if (Factory == null) return (int)Error.Fail;

            HasEnumerated = true;
            this.IsDeviceAcceptableFunction = IsDeviceAcceptableFunction;
            this.IsDeviceAcceptableFunctionUserContext = IsDeviceAcceptableFunctionUserContext;

            ClearAdapterInfos();

            for (var I = (uint)0; ; I++)
            {
                Adapter Adapter;
                Result = Factory.EnumerateAdapters(I, out Adapter);
                if (Result < 0) break; // DXGIERR_NOT_FOUND is expected when the end of the list is hit

                var AdapterInfo = new AdapterInfo
                {
                    AdapterOrdinal = I,
                    Adapter = Adapter
                };
                Adapter.GetDescription(out AdapterInfo.AdapterDescription);

                // Enumerate the device driver types on the adapter.
                Result = EnumerateDevices(AdapterInfo);
                if (Result < 0)
                {
                    AdapterInfo.Delete();
                    continue;
                }

                Result = EnumerateOutputs(AdapterInfo);
                if (Result < 0 || AdapterInfo.OutputInfos.Count <= 0)
                {
                    AdapterInfo.Delete();
                    continue;
                }

                // Get info for each devicecombo on this device
                Result = EnumerateDeviceCombos(/*Factory,*/ AdapterInfo);
                if ((Result < 0))
                {
                    AdapterInfo.Delete();
                    continue;
                }

                AdapterInfos.Add(AdapterInfo);
            }

            //  If we did not get an adapter then we should still enumerate WARP and Ref.
            if (AdapterInfos.Count == 0)
            {
                var AdapterInfo = new AdapterInfo();

                EnumerateDevices(AdapterInfo);

                // Get info for each devicecombo on this device
                Result = EnumerateDeviceCombosNoAdapter(AdapterInfo);
                if (Result < 0) AdapterInfo.Delete();
                else AdapterInfos.Add(AdapterInfo);
            }

            // Check for 2 or more adapters with the same name. Append the name
            // with some instance number if that's the case to help distinguish
            // them.
            //
            var UniqueDescription = true;
            for (var I = 0; I < AdapterInfos.Count; I++)
            {
                var AdapterInfo1 = AdapterInfos[I];

                for (var J = I + 1; J < AdapterInfos.Count; J++)
                {
                    var AdapterInfo2 = AdapterInfos[J];
                    if (AdapterInfo1.AdapterDescription.Description == AdapterInfo2.AdapterDescription.Description)
                    {
                        UniqueDescription = false;
                        break;
                    }
                }

                if (!UniqueDescription) break;
            }

            foreach (var AdapterInfo in AdapterInfos)
            {
                AdapterInfo.UniqueDescription = AdapterInfo.AdapterDescription.Description;
                if (!UniqueDescription) AdapterInfo.UniqueDescription += " " + AdapterInfo.AdapterOrdinal;
            }

            return 0;
        }

        // These should be called after Enumerate() is called
        public AdapterInfo GetAdapterInfo(uint AdapterOrdinal)
        {
            return AdapterInfos.FirstOrDefault(AdapterInfo => AdapterInfo.AdapterOrdinal == AdapterOrdinal);
        }

        public DeviceInfo GetDeviceInfo(uint AdapterOrdinal, DriverType DeviceType)
        {
            var AdapterInfo = GetAdapterInfo(AdapterOrdinal);

            return AdapterInfo == null ? null : AdapterInfo.DeviceInfos.FirstOrDefault(DeviceInfo => DeviceInfo.DeviceType == DeviceType);
        }

        public OutputInfo GetOutputInfo(uint AdapterOrdinal, uint Output)
        {
            var AdapterInfo = GetAdapterInfo(AdapterOrdinal);

            if (AdapterInfo != null && AdapterInfo.OutputInfos.Count > Output)
            {
                return AdapterInfo.OutputInfos[(int)Output];
            }

            return null;
        }

        public DeviceSettingsCombo GetDeviceSettingsCombo(uint AdapterOrdinal, /*DriverType DeviceType, uint OutputOrdinal,*/ Format BackBufferFormat, bool Windowed)
        {
            var AdapterInfo = GetAdapterInfo(AdapterOrdinal);
            return AdapterInfo == null ? null : AdapterInfo.DeviceSettingsCombos.FirstOrDefault(DeviceSettingsCombo => DeviceSettingsCombo.BackBufferFormat == BackBufferFormat && DeviceSettingsCombo.Windowed == Windowed);
        }

        public DeviceSettingsCombo GetDeviceSettingsCombo(DeviceSettings DeviceSettings)
        {
            return GetDeviceSettingsCombo(DeviceSettings.AdapterOrdinal, /*DeviceSettings.DriverType, DeviceSettings.Output,*/ DeviceSettings.SwapChainDescription.BufferDescription.Format, DeviceSettings.SwapChainDescription.Windowed);
        }
    }
}