using System;
using System.Diagnostics;
using System.Windows.Forms;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using Xtro.MDX.DXGI;
using Device = Xtro.MDX.Direct3D10.Device;
using D3D10Functions = Xtro.MDX.Direct3D10.Functions;
using SystemColor = System.Drawing.Color;

namespace Xtro.MDX.Utilities
{
    public sealed class SettingsDialog
    {
        StateBlock StateBlock;
        Dialog ActiveDialog;
        Dialog Dialog = new Dialog();
        Dialog RevertModeDialog = new Dialog();
        int RevertModeTimeout;
        uint IDEvent;
        bool Active;

        // ReSharper disable InconsistentNaming
        public const int ID_Static = -1;
        public const int ID_Ok = 1;
        public const int ID_Cancel = 2;
        public const int ID_Adapter = 3;
        public const int ID_DeviceType = 4;
        public const int ID_Windowed = 5;
        public const int ID_FullScreen = 6;
        public const int ID_AdapterFormat = 7;
        public const int ID_AdapterFormatLabel = 8;
        //public const int ID_Resolution = 9;
        //public const int ID_ResolutionLabel = 10;
        //public const int ID_RefreshRate = 11;
        //public const int ID_RefreshRateLabel = 12;
        //public const int ID_BackBufferFormat = 13;
        //public const int ID_BackBufferFormatLabel = 14;
        //public const int ID_DepthStencil = 15;
        //public const int ID_DepthStencilLabel = 16;
        //public const int ID_MultiSampleType = 17;
        //public const int ID_MultiSampleTypeLabel = 18;
        //public const int ID_MultiSampleQuality = 19;
        //public const int ID_MultiSampleQualityLabel = 20;
        //public const int ID_VertexProcessing = 21;
        //public const int ID_VertexProcessingLabel = 22;
        //public const int ID_PresentInterval = 23;
        //public const int ID_PresentIntervalLabel = 24;
        //public const int ID_DeviceClip = 25;
        public const int ID_ResolutionShowAll = 26;
        //public const int ID_ApiVersion = 27;
        public const int ID_D3D10_AdapterOutput = 28;
        public const int ID_D3D10_AdapterOutputLabel = 29;
        public const int ID_D3D10_Resolution = 30;
        public const int ID_D3D10_ResolutionLabel = 31;
        public const int ID_D3D10_RefreshRate = 32;
        public const int ID_D3D10_RefreshRateLabel = 33;
        public const int ID_D3D10_BackBufferFormat = 34;
        public const int ID_D3D10_BackBufferFormatLabel = 35;
        public const int ID_D3D10_MultiSampleCount = 36;
        public const int ID_D3D10_MultiSampleCountLabel = 37;
        public const int ID_D3D10_MultiSampleQuality = 38;
        public const int ID_D3D10_MultiSampleQualityLabel = 39;
        public const int ID_D3D10_PresentInterval = 40;
        public const int ID_D3D10_PresentIntervalLabel = 41;
        public const int ID_D3D10_DebugDevice = 42;
        public const int ID_ModeChangeAccept = 43;
        public const int ID_ModeChangeRevert = 44;
        public const int ID_StaticModeChangeTimeout = 45;
        public const int ID_WindowedGroup = 0x0100;
        // ReSharper restore InconsistentNaming

        static SettingsDialog StaticDialog;

        // ReSharper disable UnusedMember.Local
        SettingsDialog GetSettingsDialog()
        // ReSharper restore UnusedMember.Local
        {
            // Using an accessor function gives control of the construction order
            if (StaticDialog == null) StaticDialog = new SettingsDialog();

            return StaticDialog;
        }

        void CreateControls()
        {
            // Set up main settings dialog
            Dialog.KeyboardInput = true;
            Dialog.SetFont(0, "Arial", 15, FontWeight.Normal);
            Dialog.SetFont(1, "Arial", 28, FontWeight.Bold);

            // Right-justify static controls
            var Element = Dialog.GetDefaultElement(ControlType.Static, 0);
            if (Element != null)
            {
                Element.TextFormat = FontDrawFlag.VerticalCenter | FontDrawFlag.Right;

                // Title
                Static Static;
                Dialog.AddStatic(ID_Static, "Direct3D Settings", 10, 5, 400, 50, false, out Static);
                Element = Static.GetElement(0);
                Element.Font = 1;
                Element.TextFormat = FontDrawFlag.Top | FontDrawFlag.Left;
            }

            // DXUTSETTINGSDLG_ADAPTER
            Static CreatedStatic;
            Dialog.AddStatic(ID_Static, "Display Adapter", 10, 60, 180, 23, false, out CreatedStatic);
            ComboBox CreatedComboBox;
            Dialog.AddComboBox(ID_Adapter, 200, 60, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_DEVICE_TYPE
            Dialog.AddStatic(ID_Static, "Render Device", 10, 85, 180, 23, false, out CreatedStatic);
            Dialog.AddComboBox(ID_DeviceType, 200, 85, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_WINDOWED, DXUTSETTINGSDLG_FULLSCREEN
            RadioButton CreatedRadioButton;
            Dialog.AddRadioButton(ID_Windowed, ID_WindowedGroup, "Windowed", 240, 115, 300, 16, false, 0, false, out CreatedRadioButton);
            CheckBox CreatedCheckBox;
            //Dialog.AddCheckBox(ID_DeviceClip, "Clip to device when window spans across multiple monitors", 250, 136, 400, 16, false, 0, false, out CreatedCheckBox);
            Dialog.AddRadioButton(ID_FullScreen, ID_WindowedGroup, "Full Screen", 240, 157, 300, 16, false, 0, false, out CreatedRadioButton);

            // DXUTSETTINGSDLG_ADAPTER_FORMAT
            Dialog.AddStatic(ID_AdapterFormatLabel, "Adapter Format", 10, 175, 180, 23, false, out CreatedStatic);
            Dialog.AddComboBox(ID_AdapterFormat, 200, 175, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_RESOLUTION
            //Dialog.AddStatic(ID_ResolutionLabel, "Resolution", 10, 200, 180, 23, false, out CreatedStatic);
            //Dialog.AddComboBox(ID_Resolution, 200, 200, 200, 23, 0, false, out CreatedComboBox);
            //Dialog.GetComboBox(ID_Resolution).SetDropHeight(106);

            // DXUTSETTINGSDLG_RES_SHOW_ALL
            Dialog.AddCheckBox(ID_ResolutionShowAll, "Show All Aspect Ratios", 420, 200, 200, 23, false, 0, false, out CreatedCheckBox);

            // DXUTSETTINGSDLG_REFRESH_RATE
            //Dialog.AddStatic(ID_RefreshRateLabel, "Refresh Rate", 10, 225, 180, 23, false, out CreatedStatic);
            //Dialog.AddComboBox(ID_RefreshRate, 200, 225, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_BACK_BUFFER_FORMAT
            //Dialog.AddStatic(ID_BackBufferFormatLabel, "Back Buffer Format", 10, 260, 180, 23, false, out CreatedStatic);
            //Dialog.AddComboBox(ID_BackBufferFormat, 200, 260, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_DEPTH_STENCIL
            //Dialog.AddStatic(ID_DepthStencilLabel, "Depth/Stencil Format", 10, 285, 180, 23, false, out CreatedStatic);
            //Dialog.AddComboBox(ID_DepthStencil, 200, 285, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_MULTISAMPLE_TYPE
            //Dialog.AddStatic(ID_MultiSampleTypeLabel, "Multisample Type", 10, 310, 180, 23, false, out CreatedStatic);
            //Dialog.AddComboBox(ID_MultiSampleType, 200, 310, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_MULTISAMPLE_QUALITY
            //Dialog.AddStatic(ID_MultiSampleQualityLabel, "Multisample Quality", 10, 335, 180, 23, false, out CreatedStatic);
            //Dialog.AddComboBox(ID_MultiSampleQuality, 200, 335, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_VERTEX_PROCESSING
            //Dialog.AddStatic(ID_VertexProcessingLabel, "Vertex Processing", 10, 360, 180, 23, false, out CreatedStatic);
            //Dialog.AddComboBox(ID_VertexProcessing, 200, 360, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_PRESENT_INTERVAL
            //Dialog.AddStatic(ID_PresentIntervalLabel, "Vertical Sync", 10, 385, 180, 23, false, out CreatedStatic);
            //Dialog.AddComboBox(ID_PresentInterval, 200, 385, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_D3D10_ADAPTER_OUTPUT
            Dialog.AddStatic(ID_D3D10_AdapterOutputLabel, "Adapter Output", 10, 175, 180, 23, false, out CreatedStatic);
            Dialog.AddComboBox(ID_D3D10_AdapterOutput, 200, 175, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_D3D10_RESOLUTION
            Dialog.AddStatic(ID_D3D10_ResolutionLabel, "Resolution", 10, 200, 180, 23, false, out CreatedStatic);
            Dialog.AddComboBox(ID_D3D10_Resolution, 200, 200, 200, 23, 0, false, out CreatedComboBox);
            Dialog.GetComboBox(ID_D3D10_Resolution).SetDropHeight(106);

            // DXUTSETTINGSDLG_D3D10_REFRESH_RATE
            Dialog.AddStatic(ID_D3D10_RefreshRateLabel, "Refresh Rate", 10, 225, 180, 23, false, out CreatedStatic);
            Dialog.AddComboBox(ID_D3D10_RefreshRate, 200, 225, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_D3D10_BACK_BUFFER_FORMAT
            Dialog.AddStatic(ID_D3D10_BackBufferFormatLabel, "Back Buffer Format", 10, 260, 180, 23, false, out CreatedStatic);
            Dialog.AddComboBox(ID_D3D10_BackBufferFormat, 200, 260, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_D3D10_MULTISAMPLE_COUNT
            Dialog.AddStatic(ID_D3D10_MultiSampleCountLabel, "Multisample Count", 10, 285, 180, 23, false, out CreatedStatic);
            Dialog.AddComboBox(ID_D3D10_MultiSampleCount, 200, 285, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_D3D10_MULTISAMPLE_QUALITY
            Dialog.AddStatic(ID_D3D10_MultiSampleQualityLabel, "Multisample Quality", 10, 310, 180, 23, false, out CreatedStatic);
            Dialog.AddComboBox(ID_D3D10_MultiSampleQuality, 200, 310, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_D3D10_PRESENT_INTERVAL
            Dialog.AddStatic(ID_D3D10_PresentIntervalLabel, "Vertical Sync", 10, 335, 180, 23, false, out CreatedStatic);
            Dialog.AddComboBox(ID_D3D10_PresentInterval, 200, 335, 300, 23, 0, false, out CreatedComboBox);

            // DXUTSETTINGSDLG_D3D10_DEBUG_DEVICE
            Dialog.AddCheckBox(ID_D3D10_DebugDevice, "Create Debug Device", 200, 365, 180, 23, false, 0, false, out CreatedCheckBox);

            // DXUTSETTINGSDLG_OK, DXUTSETTINGSDLG_CANCEL
            Button CreatedButton;
            Dialog.AddButton(ID_Ok, "OK", 230, 440, 73, 31, 0, false, out CreatedButton);
            Dialog.AddButton(ID_Cancel, "Cancel", 315, 440, 73, 31, 0, true, out CreatedButton);

            // Set up mode change dialog
            RevertModeDialog.KeyboardInput = true;
            RevertModeDialog.NonUserEvents = true;
            RevertModeDialog.SetFont(0, "Arial", 15, FontWeight.Normal);
            RevertModeDialog.SetFont(1, "Arial", 28, FontWeight.Bold);

            Element = RevertModeDialog.GetDefaultElement(ControlType.Static, 0);
            if (Element != null)
            {
                Element.TextFormat = FontDrawFlag.VerticalCenter | FontDrawFlag.Right;

                // Title
                Static Static;
                RevertModeDialog.AddStatic(ID_Static, "Do you want to keep these display settings?", 10, 5, 640, 50, false, out Static);
                Element = Static.GetElement(0);
                Element.Font = 1;
                Element.TextFormat = FontDrawFlag.Top | FontDrawFlag.Left;

                // Timeout static text control
                RevertModeDialog.AddStatic(ID_StaticModeChangeTimeout, "", 10, 90, 640, 30, false, out Static);
                Element = Static.GetElement(0);
                Element.Font = 0;
                Element.TextFormat = FontDrawFlag.Top | FontDrawFlag.Left;
            }

            // DXUTSETTINGSDLG_MODE_CHANGE_ACCEPT, DXUTSETTINGSDLG_MODE_CHANGE_REVERT
            RevertModeDialog.AddButton(ID_ModeChangeAccept, "Yes", 230, 50, 73, 31, 0, false, out CreatedButton);
            RevertModeDialog.AddButton(ID_ModeChangeRevert, "No", 315, 50, 73, 31, 0, true, out CreatedButton);
        }

        void SetSelectedRefreshRate(Rational RefreshRate)
        {
            var RefreshRateComboBox = Dialog.GetComboBox(ID_D3D10_RefreshRate);

            for (var I = 0; I < RefreshRateComboBox.GetNumberOfItems(); I++)
            {
                var Rate = RefreshRateComboBox.GetItemData(I);

                if (Rate != null && Rate == RefreshRate.Numerator + "|" + RefreshRate.Denominator)
                {
                    RefreshRateComboBox.SetSelectedByIndex((uint)I);
                    return;
                }
            }
        }

        DeviceSettings DeviceSettings = new DeviceSettings();

        int UpdateResolutions()
        {
            var Width = DeviceSettings.SwapChainDescription.BufferDescription.Width;
            var Height = DeviceSettings.SwapChainDescription.BufferDescription.Height;

            // DXUTSETTINGSDLG_D3D10_RESOLUTION
            var ResolutionComboBox = Dialog.GetComboBox(ID_D3D10_Resolution);
            ResolutionComboBox.RemoveAllItems();

            var OutputInfo = GetCurrentOutputInfo();
            if (OutputInfo == null) return (int)Error.Fail;

            var ShowAll = Dialog.GetCheckBox(ID_ResolutionShowAll).Checked;

            // Get the desktop aspect ratio
            ModeDescription Desktop;
            Functions.GetDesktopResolution(DeviceSettings.AdapterOrdinal, out Desktop.Width, out Desktop.Height);
            var DesktopAspectRatio = Desktop.Width / (float)Desktop.Height;

            foreach (var DisplayMode in OutputInfo.DisplayModes)
            {
                var Aspect = DisplayMode.Width / (float)DisplayMode.Height;

                if (DisplayMode.Format == DeviceSettings.SwapChainDescription.BufferDescription.Format)
                {
                    // If "Show All" is not checked, then hide all resolutions
                    // that don't match the aspect ratio of the desktop resolution
                    if (ShowAll || Math.Abs(DesktopAspectRatio - Aspect) < 0.05f) AddResolution(DisplayMode.Width, DisplayMode.Height);
                }
            }

            var CurrentResolution = MakeLong((short)DeviceSettings.SwapChainDescription.BufferDescription.Width, (short)DeviceSettings.SwapChainDescription.BufferDescription.Height);

            ResolutionComboBox.SetSelectedByData(CurrentResolution.ToString());

            var Result = OnResolutionChanged();
            if (Result < 0) return Result;

            var Windowed = IsWindowed();
            if (Windowed)
            {
                ResolutionComboBox.RemoveAllItems();
                AddResolution(Width, Height);

                ResolutionComboBox.SetSelectedByData(MakeLong((short)Width, (short)Height).ToString());

                Result = OnResolutionChanged();
                if (Result < 0) return Result;
            }

            return 0;
        }

        uint GetSelectedAdapter()
        {
            var ComboBox = Dialog.GetComboBox(ID_Adapter);

            return Convert.ToUInt32(ComboBox.GetSelectedData());
        }

        Enumeration.AdapterInfo GetCurrentAdapterInfo()
        {
            var Enumeration = Functions.GetEnumeration();
            return Enumeration.GetAdapterInfo(DeviceSettings.AdapterOrdinal);
        }

        // ReSharper disable UnusedMember.Local
        Enumeration.DeviceInfo GetCurrentDeviceInfo()
        // ReSharper restore UnusedMember.Local
        {
            var Enumeration = Functions.GetEnumeration();
            return Enumeration.GetDeviceInfo(DeviceSettings.AdapterOrdinal, DeviceSettings.DriverType);
        }

        void AddDeviceType(DriverType DeviceType)
        {
            var ComboBox = Dialog.GetComboBox(ID_DeviceType);

            if (!ComboBox.ContainsItem(DeviceType.ToString())) ComboBox.AddItem(DeviceType.ToString(), ((uint)DeviceType).ToString());
        }

        void SetWindowed(bool Windowed)
        {
            var RadioButton = Dialog.GetRadioButton(ID_Windowed);
            RadioButton.SetChecked(Windowed);

            RadioButton = Dialog.GetRadioButton(ID_FullScreen);
            RadioButton.SetChecked(!Windowed);
        }

        DriverType GetSelectedDeviceType()
        {
            var ComboBox = Dialog.GetComboBox(ID_DeviceType);

            return (DriverType)Convert.ToUInt32(ComboBox.GetSelectedData());
        }

        void AddAdapterOutput(string Name, uint Output)
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_AdapterOutput);

            if (!ComboBox.ContainsItem(Name)) ComboBox.AddItem(Name, Output.ToString());
        }

        // ReSharper disable UnusedMember.Local
        uint GetSelectedAdapterOutput()
        // ReSharper restore UnusedMember.Local
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_AdapterOutput);

            return Convert.ToUInt32(ComboBox.GetSelectedData());
        }

        int OnAdapterOutputChanged()
        {
            int Result;

            var Windowed = IsWindowed();
            DeviceSettings.SwapChainDescription.Windowed = Windowed;

            // If windowed, get the appropriate adapter format from Direct3D
            if (DeviceSettings.SwapChainDescription.Windowed)
            {
                var Mode = new ModeDescription();
                Result = Functions.GetAdapterDisplayMode(DeviceSettings.AdapterOrdinal, DeviceSettings.Output, ref Mode);
                if (Result < 0) return Result;

                // Default resolution to the fullscreen res that was last used
                var Rectangle = Functions.GetFullsceenClientRectangleAtModeChange();
                if (Rectangle.Right == 0 || Rectangle.Bottom == 0)
                {
                    // If nothing last used, then default to the adapter desktop res
                    DeviceSettings.SwapChainDescription.BufferDescription.Width = Mode.Width;
                    DeviceSettings.SwapChainDescription.BufferDescription.Height = Mode.Height;
                }
                else
                {
                    DeviceSettings.SwapChainDescription.BufferDescription.Width = (uint)Rectangle.Right;
                    DeviceSettings.SwapChainDescription.BufferDescription.Height = (uint)Rectangle.Bottom;
                }

                DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate = Mode.RefreshRate;
            }

            var RefreshRate = DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate;

            var AdapterInfo = GetCurrentAdapterInfo();
            if (AdapterInfo == null) return (int)Error.Fail;

            // DXUTSETTINGSDLG_D3D10_RESOLUTION
            Result = UpdateResolutions();
            if (Result < 0) return Result;

            // DXUTSETTINGSDLG_D3D10_BACK_BUFFER_FORMAT
            var BackBufferFormatComboBox = Dialog.GetComboBox(ID_D3D10_BackBufferFormat);
            BackBufferFormatComboBox.RemoveAllItems();

            foreach (var DeviceCombo in AdapterInfo.DeviceSettingsCombos)
            {
                if (DeviceCombo.Windowed == Windowed) AddBackBufferFormat(DeviceCombo.BackBufferFormat);
            }

            BackBufferFormatComboBox.SetSelectedByData(((uint)DeviceSettings.SwapChainDescription.BufferDescription.Format).ToString());

            Result = OnBackBufferFormatChanged();
            if (Result < 0) return Result;

            // DXUTSETTINGSDLG_D3D10_REFRESH_RATE
            if (Windowed)
            {
                var RefreshRateComboBox = Dialog.GetComboBox(ID_D3D10_RefreshRate);
                RefreshRateComboBox.RemoveAllItems();
                AddRefreshRate(RefreshRate);
            }

            SetSelectedRefreshRate(RefreshRate);

            Result = OnRefreshRateChanged();
            return Result < 0 ? Result : 0;
        }

        Format GetSelectedBackBufferFormat()
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_BackBufferFormat);

            return (Format)Convert.ToUInt32(ComboBox.GetSelectedData());
        }

        //void AddDepthStencilBufferFormat(Format Format)
        //{
        //    var ComboBox = Dialog.GetComboBox(ID_DepthStencil);

        //    if (!ComboBox.ContainsItem(Format.ToString())) ComboBox.AddItem(Format.ToString(), ((uint)Format).ToString());
        //}

        int OnBackBufferFormatChanged()
        {
            DeviceSettings.SwapChainDescription.BufferDescription.Format = GetSelectedBackBufferFormat();

            var BackBufferFormat = DeviceSettings.SwapChainDescription.BufferDescription.Format;

            var AdapterInfo = GetCurrentAdapterInfo();
            if (AdapterInfo == null) return (int)Error.Fail;

            foreach (var DeviceCombo in AdapterInfo.DeviceSettingsCombos)
            {
                if (DeviceCombo.Windowed == (DeviceSettings.SwapChainDescription.Windowed) && DeviceCombo.BackBufferFormat == BackBufferFormat && DeviceCombo.DeviceType == DeviceSettings.DriverType)
                {
                    var MultisampleCountCombo = Dialog.GetComboBox(ID_D3D10_MultiSampleCount);
                    MultisampleCountCombo.RemoveAllItems();
                    foreach (var MultiSampleCount in DeviceCombo.MultiSampleCounts) AddMultisampleCount(MultiSampleCount);
                    MultisampleCountCombo.SetSelectedByData(DeviceSettings.SwapChainDescription.SampleDescription.Count.ToString());

                    var Result = OnMultisampleTypeChanged();
                    if (Result < 0) return Result;

                    var PresentIntervalComboBox = Dialog.GetComboBox(ID_D3D10_PresentInterval);
                    PresentIntervalComboBox.RemoveAllItems();
                    PresentIntervalComboBox.AddItem("On", "1");
                    PresentIntervalComboBox.AddItem("Off", "0");

                    PresentIntervalComboBox.SetSelectedByData(DeviceSettings.SyncInterval.ToString());

                    Result = OnPresentIntervalChanged();
                    if (Result < 0) return Result;

                    Result = UpdateResolutions();
                    if (Result < 0) return Result;
                }
            }

            return 0;
        }

        public int OnPresentIntervalChanged()
        {
            DeviceSettings.SyncInterval = GetSelectedPresentInterval();

            return 0;
        }

        uint GetSelectedPresentInterval()
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_PresentInterval);

            return Convert.ToUInt32(ComboBox.GetSelectedData());
        }

        Enumeration.DeviceSettingsCombo GetCurrentDeviceSettingsCombo()
        {
            var Enumeration = Functions.GetEnumeration();
            return Enumeration.GetDeviceSettingsCombo(DeviceSettings.AdapterOrdinal,/*DeviceSettings.DriverType,DeviceSettings.Output,*/DeviceSettings.SwapChainDescription.BufferDescription.Format, DeviceSettings.SwapChainDescription.Windowed);
        }

        //void AddVertexProcessingType(uint Type)
        //{
        //    var ComboBox = Dialog.GetComboBox(ID_VertexProcessing);

        //    if (!ComboBox.ContainsItem(Type.ToString())) ComboBox.AddItem(Type.ToString(), Type.ToString());
        //}

        //uint GetSelectedVertexProcessingType()
        //{
        //    var ComboBox = Dialog.GetComboBox(ID_VertexProcessing);

        //    return (uint)ComboBox.GetSelectedData();
        //}

        uint GetSelectedMultisampleCount()
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_MultiSampleCount);

            return Convert.ToUInt32(ComboBox.GetSelectedData());
        }

        int OnMultisampleTypeChanged()
        {
            var MultisampleCount = GetSelectedMultisampleCount();
            DeviceSettings.SwapChainDescription.SampleDescription.Count = MultisampleCount;

            var DeviceSettingsCombo = GetCurrentDeviceSettingsCombo();
            if (DeviceSettingsCombo == null) return (int)Error.Fail;

            uint MaxQuality = 0;
            for (var I = 0; I < DeviceSettingsCombo.MultiSampleCounts.Count; I++)
            {
                var Count = DeviceSettingsCombo.MultiSampleCounts[I];
                if (Count == MultisampleCount)
                {
                    MaxQuality = DeviceSettingsCombo.MultiSampleQualities[I];
                    break;
                }
            }

            // DXUTSETTINGSDLG_D3D10_MULTISAMPLE_QUALITY
            var MultisampleQualityCombo = Dialog.GetComboBox(ID_D3D10_MultiSampleQuality);
            MultisampleQualityCombo.RemoveAllItems();

            for (uint Quality = 0; Quality < MaxQuality; Quality++)
            {
                AddMultisampleQuality(Quality);
            }

            MultisampleQualityCombo.SetSelectedByData(DeviceSettings.SwapChainDescription.SampleDescription.Quality.ToString());

            var Result = OnMultisampleQualityChanged();
            return Result < 0 ? Result : 0;
        }

        uint GetSelectedMultisampleQuality()
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_MultiSampleQuality);

            return Convert.ToUInt32(ComboBox.GetSelectedData());
        }

        int OnMultisampleQualityChanged()
        {
            DeviceSettings.SwapChainDescription.SampleDescription.Quality = GetSelectedMultisampleQuality();

            return 0;
        }

        void AddMultisampleQuality(uint Quality)
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_MultiSampleQuality);

            var QualityText = Quality.ToString();

            if (!ComboBox.ContainsItem(QualityText))
                ComboBox.AddItem(QualityText, Quality.ToString());
        }

        void AddMultisampleCount(uint Count)
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_MultiSampleCount);

            if (!ComboBox.ContainsItem(Count.ToString())) ComboBox.AddItem(Count.ToString(), Count.ToString());
        }

        void AddBackBufferFormat(Format Format)
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_BackBufferFormat);

            if (!ComboBox.ContainsItem(Format.ToString())) ComboBox.AddItem(Format.ToString(), ((uint)Format).ToString());
        }

        int OnWindowedFullScreenChanged()
        {
            var Windowed = IsWindowed();

            Dialog.SetControlEnabled(ID_AdapterFormatLabel, !Windowed);
            //Dialog.SetControlEnabled(ID_ResolutionLabel, !Windowed);
            //Dialog.SetControlEnabled(ID_RefreshRateLabel, !Windowed);
            Dialog.SetControlEnabled(ID_D3D10_AdapterOutputLabel, !Windowed);
            Dialog.SetControlEnabled(ID_D3D10_ResolutionLabel, !Windowed);
            Dialog.SetControlEnabled(ID_D3D10_RefreshRateLabel, !Windowed);

            Dialog.SetControlEnabled(ID_AdapterFormat, !Windowed);
            //Dialog.SetControlEnabled(ID_Resolution, !Windowed);
            Dialog.SetControlEnabled(ID_ResolutionShowAll, !Windowed);
            //Dialog.SetControlEnabled(ID_RefreshRate, !Windowed);
            //Dialog.SetControlEnabled(ID_DeviceClip, Windowed);
            Dialog.SetControlEnabled(ID_D3D10_AdapterOutput, !Windowed);
            Dialog.SetControlEnabled(ID_D3D10_Resolution, !Windowed);
            Dialog.SetControlEnabled(ID_D3D10_RefreshRate, !Windowed);

            DeviceSettings.SwapChainDescription.Windowed = Windowed;

            // Get available adapter output
            var Enumeration = Functions.GetEnumeration();

            var OutputComboBox = Dialog.GetComboBox(ID_D3D10_AdapterOutput);
            OutputComboBox.RemoveAllItems();

            var AdapterInfo = Enumeration.GetAdapterInfo(DeviceSettings.AdapterOrdinal);
            foreach (var OutputInfo in AdapterInfo.OutputInfos)
            {
                AddAdapterOutput(OutputInfo.Description.DeviceName, OutputInfo.OutputOrdinal);
            }

            OutputComboBox.SetSelectedByData(DeviceSettings.Output.ToString());

            var Result = OnAdapterOutputChanged();
            return Result < 0 ? Result : 0;
        }

        int OnDeviceTypeChanged()
        {
            DeviceSettings.DriverType = GetSelectedDeviceType();

            // DXUTSETTINGSDLG_WINDOWED, DXUTSETTINGSDLG_FULLSCREEN
            Dialog.SetControlEnabled(ID_Windowed, true);
            Dialog.SetControlEnabled(ID_FullScreen, true);

            SetWindowed(DeviceSettings.SwapChainDescription.Windowed);

            var Result = OnWindowedFullScreenChanged();
            return Result < 0 ? Result : 0;
        }

        int OnAdapterChanged()
        {
            // Store the adapter index
            DeviceSettings.AdapterOrdinal = GetSelectedAdapter();

            // DXUTSETTINGSDLG_DEVICE_TYPE
            var DeviceTypeComboBox = Dialog.GetComboBox(ID_DeviceType);
            DeviceTypeComboBox.RemoveAllItems();

            var AdapterInfo = GetCurrentAdapterInfo();
            if (AdapterInfo == null) return (int)Error.Fail;

            foreach (var DeviceInfo in AdapterInfo.DeviceInfos)
            {
                AddDeviceType(DeviceInfo.DeviceType);
            }

            DeviceTypeComboBox.SetSelectedByData(((uint)DeviceSettings.DriverType).ToString());

            var Result = OnDeviceTypeChanged();
            return Result < 0 ? Result : 0;
        }

        //Format GetSelectedDepthStencilBufferFormat()
        //{
        //    var ComboBox = Dialog.GetComboBox(ID_DepthStencil);

        //    return (Format)ComboBox.GetSelectedData();
        //}

        public void ShowControlSet()
        {
            Dialog.GetControl(ID_AdapterFormat).SetVisible(false);
            Dialog.GetControl(ID_AdapterFormatLabel).SetVisible(false);
            //Dialog.GetControl(ID_Resolution).SetVisible(false);
            //Dialog.GetControl(ID_ResolutionLabel).SetVisible(false);
            //Dialog.GetControl(ID_RefreshRate).SetVisible(false);
            //Dialog.GetControl(ID_RefreshRateLabel).SetVisible(false);
            //Dialog.GetControl(ID_BackBufferFormat).SetVisible(false);
            //Dialog.GetControl(ID_BackBufferFormatLabel).SetVisible(false);
            //Dialog.GetControl(ID_DepthStencil).SetVisible(false);
            //Dialog.GetControl(ID_DepthStencilLabel).SetVisible(false);
            //Dialog.GetControl(ID_MultiSampleType).SetVisible(false);
            //Dialog.GetControl(ID_MultiSampleTypeLabel).SetVisible(false);
            //Dialog.GetControl(ID_MultiSampleQuality).SetVisible(false);
            //Dialog.GetControl(ID_MultiSampleQualityLabel).SetVisible(false);
            //Dialog.GetControl(ID_VertexProcessing).SetVisible(false);
            //Dialog.GetControl(ID_VertexProcessingLabel).SetVisible(false);
            //Dialog.GetControl(ID_PresentInterval).SetVisible(false);
            //Dialog.GetControl(ID_PresentIntervalLabel).SetVisible(false);
            //Dialog.GetControl(ID_DeviceClip).SetVisible(false);
            Dialog.GetControl(ID_D3D10_AdapterOutput).SetVisible(true);
            Dialog.GetControl(ID_D3D10_AdapterOutputLabel).SetVisible(true);
            Dialog.GetControl(ID_D3D10_Resolution).SetVisible(true);
            Dialog.GetControl(ID_D3D10_ResolutionLabel).SetVisible(true);
            Dialog.GetControl(ID_D3D10_RefreshRate).SetVisible(true);
            Dialog.GetControl(ID_D3D10_RefreshRateLabel).SetVisible(true);
            Dialog.GetControl(ID_D3D10_BackBufferFormat).SetVisible(true);
            Dialog.GetControl(ID_D3D10_BackBufferFormatLabel).SetVisible(true);
            Dialog.GetControl(ID_D3D10_MultiSampleCount).SetVisible(true);
            Dialog.GetControl(ID_D3D10_MultiSampleCountLabel).SetVisible(true);
            Dialog.GetControl(ID_D3D10_MultiSampleQuality).SetVisible(true);
            Dialog.GetControl(ID_D3D10_MultiSampleQualityLabel).SetVisible(true);
            Dialog.GetControl(ID_D3D10_PresentInterval).SetVisible(true);
            Dialog.GetControl(ID_D3D10_PresentIntervalLabel).SetVisible(true);
            Dialog.GetControl(ID_D3D10_DebugDevice).SetVisible(true);
        }

        int OnAPIVersionChanged(bool Refresh = false)
        {
            // Show/hide appropriate dialog controls based on version.
            ShowControlSet();

            if (!Refresh)
            {
                // Obtain a set of valid D3D10 device settings.
                var CreateFlags = DeviceSettings.CreateFlags;
                DeviceSettings = new DeviceSettings { CreateFlags = CreateFlags };
                // We want a specific API version, so set up match option to preserve it.
                var MatchOptions = new MatchOptions();
                Functions.FindValidDeviceSettings(DeviceSettings, DeviceSettings, MatchOptions);
            }

            var Enumeration = Functions.GetEnumeration();
            var AdapterInfos = Enumeration.AdapterInfos;

            var AdapterComboBox = Dialog.GetComboBox(ID_Adapter);
            AdapterComboBox.RemoveAllItems();

            foreach (var AdapterInfo in AdapterInfos)
            {
                AddAdapter(AdapterInfo.UniqueDescription, AdapterInfo.AdapterOrdinal);
            }

            AdapterComboBox.SetSelectedByData(DeviceSettings.AdapterOrdinal.ToString());

            var CheckBox = Dialog.GetCheckBox(ID_D3D10_DebugDevice);
            CheckBox.Checked = 0 != (DeviceSettings.CreateFlags & CreateDeviceFlag.Debug);

            var Result = OnAdapterChanged();
            return Result < 0 ? Result : 0;
        }

        void AddAdapter(string Description, uint Adapter)
        {
            var ComboBox = Dialog.GetComboBox(ID_Adapter);

            if (!ComboBox.ContainsItem(Description)) ComboBox.AddItem(Description, Adapter.ToString());
        }

        // ReSharper disable UnusedMember.Local
        void AddAdapterFormat(Format Format)
        // ReSharper restore UnusedMember.Local
        {
            var ComboBox = Dialog.GetComboBox(ID_AdapterFormat);

            if (!ComboBox.ContainsItem(Format.ToString())) ComboBox.AddItem(Format.ToString(), ((uint)Format).ToString());
        }

        // ReSharper disable UnusedMember.Local
        Format GetSelectedAdapterFormat()
        // ReSharper restore UnusedMember.Local
        {
            var ComboBox = Dialog.GetComboBox(ID_AdapterFormat);

            return (Format)Convert.ToUInt32(ComboBox.GetSelectedData());
        }

        void OnDebugDeviceChanged()
        {
            var DebugDevice = GetSelectedDebugDeviceValue();
            if (DebugDevice) DeviceSettings.CreateFlags |= CreateDeviceFlag.Debug;
            else DeviceSettings.CreateFlags &= ~CreateDeviceFlag.Debug;
        }

        bool GetSelectedDebugDeviceValue()
        {
            var CheckBox = Dialog.GetCheckBox(ID_D3D10_DebugDevice);

            return CheckBox.Checked;
        }

        void OnEvent(/*Event Event,*/ int ControlID/*, Control Control*/)
        {
            switch (ControlID)
            {
            case ID_Adapter: OnAdapterChanged(); break;
            case ID_DeviceType:
                OnDeviceTypeChanged(); break;
            case ID_Windowed:
                OnWindowedFullScreenChanged(); break;
            case ID_FullScreen:
                OnWindowedFullScreenChanged(); break;
            case ID_AdapterFormat:
                //OnAdapterFormatChanged();
                break;
            case ID_ResolutionShowAll:
                {
                    OnBackBufferFormatChanged();
                    break;
                }
            //case ID_Resolution:
            //    throw new NotImplementedException();
            //OnResolutionChanged(); 
            //break;
            //case ID_RefreshRate:
            //    OnRefreshRateChanged(); break;
            //case ID_BackBufferFormat:
            //    OnBackBufferFormatChanged(); break;
            //case ID_DepthStencil:
            //    throw new NotImplementedException();
            //OnDepthStencilBufferFormatChanged();
            //break;
            //case ID_MultiSampleType:
            //    OnMultisampleTypeChanged(); break;
            //case ID_MultiSampleQuality:
            //    OnMultisampleQualityChanged(); break;
            //case ID_VertexProcessing:
            //    throw new NotImplementedException();
            //OnVertexProcessingChanged(); 
            //break;
            //case ID_PresentInterval:
            //    OnPresentIntervalChanged(); break;
            //case ID_DeviceClip:
            //    throw new NotImplementedException();
            //OnDeviceClipChanged(); 
            //break;
            case ID_D3D10_AdapterOutput:
                OnAdapterOutputChanged(); break;
            case ID_D3D10_Resolution:
                OnResolutionChanged(); break;
            case ID_D3D10_RefreshRate:
                OnRefreshRateChanged(); break;
            case ID_D3D10_BackBufferFormat:
                OnBackBufferFormatChanged(); break;
            case ID_D3D10_MultiSampleCount:
                OnMultisampleTypeChanged(); break;
            case ID_D3D10_MultiSampleQuality:
                OnMultisampleQualityChanged(); break;
            case ID_D3D10_PresentInterval:
                OnPresentIntervalChanged(); break;
            case ID_D3D10_DebugDevice:
                OnDebugDeviceChanged(); break;

            case ID_Ok:
                {
                    var FullScreenModeChange = false;
                    var CurrentSettings = Functions.GetDeviceSettings();

                    if (DeviceSettings.SwapChainDescription.Windowed)
                    {
                        DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Denominator = DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Numerator = 0;

                        var ClientRectangle = Functions.IsWindowed() ? Functions.GetForm().ClientRectangle : Functions.GetWindowClientRectangleAtModeChange();
                        var WindowWidth = ClientRectangle.Right - ClientRectangle.Left;
                        var WindowHeight = ClientRectangle.Bottom - ClientRectangle.Top;

                        DeviceSettings.SwapChainDescription.BufferDescription.Width = (uint)WindowWidth;
                        DeviceSettings.SwapChainDescription.BufferDescription.Height = (uint)WindowHeight;
                    }
                    else
                    {
                        // Check for fullscreen mode change
                        FullScreenModeChange = DeviceSettings.SwapChainDescription.BufferDescription.Width != CurrentSettings.SwapChainDescription.BufferDescription.Width || DeviceSettings.SwapChainDescription.BufferDescription.Height != CurrentSettings.SwapChainDescription.BufferDescription.Height || DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Denominator != CurrentSettings.SwapChainDescription.BufferDescription.RefreshRate.Denominator || DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Numerator != CurrentSettings.SwapChainDescription.BufferDescription.RefreshRate.Numerator;
                    }

                    if (FullScreenModeChange)
                    {
                        // set appropriate global device settings to that of the current device
                        // settings.  These will get set to the user-defined settings once the
                        // user accepts the mode change
                        var Settings = DeviceSettings;
                        DeviceSettings.SwapChainDescription.BufferDescription.Width = CurrentSettings.SwapChainDescription.BufferDescription.Width;
                        DeviceSettings.SwapChainDescription.BufferDescription.Height = CurrentSettings.SwapChainDescription.BufferDescription.Height;
                        DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Denominator = CurrentSettings.SwapChainDescription.BufferDescription.RefreshRate.Denominator;
                        DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate.Numerator = CurrentSettings.SwapChainDescription.BufferDescription.RefreshRate.Numerator;
                        DeviceSettings.SwapChainDescription.Windowed = CurrentSettings.SwapChainDescription.Windowed;

                        // apply the user-defined settings
                        Functions.CreateDeviceFromSettings(Settings);
                        // create the mode change timeout dialog
                        ActiveDialog = RevertModeDialog;
                        RevertModeTimeout = 15;
                        UpdateModeChangeTimeoutText(RevertModeTimeout);
                        // activate a timer for 1-second updates
                        Functions.SetTimer(StaticOnModeChangeTimer, 1.0f, out IDEvent, this);
                    }
                    else
                    {
                        Functions.CreateDeviceFromSettings(DeviceSettings);
                        Active = false;
                    }
                    break;
                }

            case ID_Cancel:
                {
                    Active = false;
                    break;
                }

            case ID_ModeChangeAccept:
                {
                    Functions.KillTimer(IDEvent);
                    DeviceSettings = Functions.GetDeviceSettings();
                    ActiveDialog = Dialog;
                    Active = false;
                    break;
                }

            case ID_ModeChangeRevert:
                {
                    Functions.KillTimer(IDEvent);
                    ActiveDialog = Dialog;
                    IDEvent = 0;
                    RevertModeTimeout = 0;
                    Functions.CreateDeviceFromSettings(DeviceSettings);
                    Refresh();
                    break;
                }
            }
        }

        static void StaticOnEvent(Event Event, int ControlID, Control Control, object UserData)
        {
            var Settings = (SettingsDialog)UserData;
            if (Settings != null) Settings.OnEvent(/*Event, */ControlID/*, Control*/);
        }

        static void StaticOnModeChangeTimer(uint IDEvent, object UserContext)
        {
            var Settings = (SettingsDialog)UserContext;
            Debug.Assert(Settings.ActiveDialog == Settings.RevertModeDialog);
            Debug.Assert(Settings.IDEvent == IDEvent);

            if (0 == --Settings.RevertModeTimeout)
            {
                var Control = Settings.RevertModeDialog.GetControl(ID_ModeChangeRevert);
                Settings.RevertModeDialog.SendEvent(Event.ButtonClicked, false, Control);
            }
            Settings.UpdateModeChangeTimeoutText(Settings.RevertModeTimeout);
        }

        void UpdateModeChangeTimeoutText(int SecondsRemaining)
        {
            var Timeout = string.Format("Reverting to previous display settings in {0} seconds", SecondsRemaining);

            var Static = RevertModeDialog.GetStatic(ID_StaticModeChangeTimeout);
            Static.Text = Timeout;
        }

        bool IsWindowed()
        {
            var RadioButton = Dialog.GetRadioButton(ID_Windowed);
            return RadioButton.Checked;
        }

        Enumeration.OutputInfo GetCurrentOutputInfo()
        {
            var Enumeration = Functions.GetEnumeration();
            return Enumeration.GetOutputInfo(DeviceSettings.AdapterOrdinal, DeviceSettings.Output);
        }

        static int MakeLong(short LowPart, short HighPart)
        {
            return (int)(((ushort)LowPart) | (uint)(HighPart << 16));
        }

        void AddResolution(uint Width, uint Height)
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_Resolution);

            var ResolutionData = (uint)MakeLong((short)Width, (short)Height);
            var Resolution = string.Format("{0} by {1}", Width, Height);

            if (!ComboBox.ContainsItem(Resolution)) ComboBox.AddItem(Resolution, ResolutionData.ToString());
        }

        int OnResolutionChanged()
        {
            var OutputInfo = GetCurrentOutputInfo();
            if (OutputInfo == null) return (int)Error.Fail;

            // Set resolution
            uint Width, Height;
            GetSelectedResolution(out Width, out Height);
            DeviceSettings.SwapChainDescription.BufferDescription.Width = Width;
            DeviceSettings.SwapChainDescription.BufferDescription.Height = Height;

            var RefreshRate = DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate;

            // Update the refresh rate list
            var RefreshRateComboBox = Dialog.GetComboBox(ID_D3D10_RefreshRate);
            RefreshRateComboBox.RemoveAllItems();

            var Format = DeviceSettings.SwapChainDescription.BufferDescription.Format;
            foreach (var DisplayMode in OutputInfo.DisplayModes)
            {
                if (DisplayMode.Format == Format && DisplayMode.Width == Width && DisplayMode.Height == Height) AddRefreshRate(DisplayMode.RefreshRate);
            }

            SetSelectedRefreshRate(RefreshRate);

            var Result = OnRefreshRateChanged();
            return Result < 0 ? Result : 0;
        }

        public void GetSelectedResolution(out uint Width, out uint Height)
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_Resolution);

            var Resolution = Convert.ToUInt32(ComboBox.GetSelectedData());

            Width = Resolution & 0xffff;
            Height = (Resolution >> 16) & 0xffff;
        }

        int OnRefreshRateChanged()
        {
            // Set refresh rate
            DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate = GetSelectedRefreshRate();

            return 0;
        }

        static Rational GetSelectedRefreshRate()
        {
            var Rational = new Rational
            {
                Numerator = 0,
                Denominator = 1
            };
            return Rational;
            /*
              CDXUTComboBox* pComboBox = m_Dialog.GetComboBox( DXUTSETTINGSDLG_D3D10_REFRESH_RATE );
      
              return *reinterpret_cast<DXGI_RATIONAL*>( pComboBox->GetSelectedData() );
             */
        }

        void AddRefreshRate(Rational RefreshRate)
        {
            var ComboBox = Dialog.GetComboBox(ID_D3D10_RefreshRate);

            string RefreshRateText;

            if (RefreshRate.Numerator == 0 && RefreshRate.Denominator == 0) RefreshRateText = "Default Rate";
            else RefreshRateText = string.Format("{0} Hz", RefreshRate.Numerator / RefreshRate.Denominator);

            if (!ComboBox.ContainsItem(RefreshRateText))
            {
                ComboBox.AddItem(RefreshRateText, RefreshRate.Numerator + "|" + RefreshRate.Denominator);
            }
        }

        public void Init(DialogResourceManager Manager, string ControlTextureFileName = null)
        {
            Debug.Assert(Manager != null);
            Dialog.Init(Manager, false, ControlTextureFileName);  // Don't register this dialog.
            RevertModeDialog.Init(Manager, false, ControlTextureFileName);   // Don't register this dialog.
            ActiveDialog = Dialog;
            CreateControls();
        }

        public int Refresh()
        {
            DeviceSettings = Functions.GetDeviceSettings();

            // Show all D3D10-specific controls and hide controls for all other D3D versions.
            ShowControlSet();

            var Enumeration = Functions.GetEnumeration();

            // Fill the UI with the current settings
            AddDeviceType(DeviceSettings.DriverType);
            SetWindowed(DeviceSettings.SwapChainDescription.Windowed);
            var OutputInfo = GetCurrentOutputInfo();
            if (OutputInfo != null) AddAdapterOutput(OutputInfo.Description.DeviceName, DeviceSettings.Output);
            AddResolution(DeviceSettings.SwapChainDescription.BufferDescription.Width, DeviceSettings.SwapChainDescription.BufferDescription.Height);
            AddRefreshRate(DeviceSettings.SwapChainDescription.BufferDescription.RefreshRate);
            AddBackBufferFormat(DeviceSettings.SwapChainDescription.BufferDescription.Format);
            AddMultisampleCount(DeviceSettings.SwapChainDescription.SampleDescription.Count);
            AddMultisampleQuality(DeviceSettings.SwapChainDescription.SampleDescription.Quality);

            var BestDeviceSettingsCombo = Enumeration.GetDeviceSettingsCombo(DeviceSettings.AdapterOrdinal,/* DeviceSettings.DriverType,DeviceSettings.Output,*/ DeviceSettings.SwapChainDescription.BufferDescription.Format, DeviceSettings.SwapChainDescription.Windowed);
            if (BestDeviceSettingsCombo == null) return Functions.ErrorBox((int)Error.InvalidArgument, "GetDeviceSettingsCombo");

            // Get the adapters list from CD3D10Enumeration object
            var AdapterInfos = Enumeration.AdapterInfos;

            if (AdapterInfos.Count == 0) return Functions.ErrorBox((int)Error.NoCompatibleDevices, "SettingsDialog.OnCreatedDevice");

            var AdapterCombo = Dialog.GetComboBox(ID_Adapter);
            AdapterCombo.RemoveAllItems();

            // Add adapters
            foreach (var AdapterInfo in AdapterInfos)
            {
                AddAdapter(AdapterInfo.UniqueDescription, AdapterInfo.AdapterOrdinal);
            }

            AdapterCombo.SetSelectedByData(DeviceSettings.AdapterOrdinal.ToString());

            var Result = OnAPIVersionChanged(true);
            if (Result < 0) return Result;

            //m_Dialog.Refresh();
            Dialog.SetRefreshTime((float)Functions.GetTime());

            return 0;
        }

        public void OnRender(float ElapsedTime)
        {
            // Render the scene
            StateBlock.Capture();
            //    Device->SetRenderState( D3DRS_FILLMODE, D3DFILL_SOLID );
            ActiveDialog.OnRender(ElapsedTime);
            StateBlock.Apply();
        }

        public int HandleKeyDownEvent(KeyEventArgs E)
        {
            ActiveDialog.HandleKeyDownEvent(E);
            if (E.KeyCode == Keys.F2) SetActive(false);
            return 0;
        }

        public int OnCreateDevice(Device Device)
        {
            if (Device == null) return Functions.ErrorBox((int)Error.InvalidArgument, "SettingsDialog.OnCreateDevice");

            // Create the fonts/textures 
            Dialog.SetCallback(StaticOnEvent, this);
            RevertModeDialog.SetCallback(StaticOnEvent, this);

            StateBlockMask StateBlockMask;
            D3D10Functions.StateBlockMaskDisableAll(out StateBlockMask);
            D3D10Functions.StateBlockMaskEnableCapture(ref StateBlockMask, DeviceStateType.RasterizerState, 0, 1);
            var Result = D3D10Functions.CreateStateBlock(Device, ref StateBlockMask, out StateBlock);
            return Result < 0 ? Result : 0;
        }

        public int OnResizedSwapChain(Device Device, SurfaceDescription BackBufferSurfaceDescription)
        {
            Dialog.SetLocation(0, 0);
            Dialog.SetSize((int)BackBufferSurfaceDescription.Width, (int)BackBufferSurfaceDescription.Height);
            Dialog.SetBackgroundColors((uint)SystemColor.FromArgb(255, 98, 138, 206).ToArgb(), (uint)SystemColor.FromArgb(255, 54, 105, 192).ToArgb(), (uint)SystemColor.FromArgb(255, 54, 105, 192).ToArgb(), (uint)SystemColor.FromArgb(255, 10, 73, 179).ToArgb());

            RevertModeDialog.SetLocation(0, 0);
            RevertModeDialog.SetSize((int)BackBufferSurfaceDescription.Width, (int)BackBufferSurfaceDescription.Height);
            RevertModeDialog.SetBackgroundColors((uint)SystemColor.FromArgb(255, 98, 138, 206).ToArgb(), (uint)SystemColor.FromArgb(255, 54, 105, 192).ToArgb(), (uint)SystemColor.FromArgb(255, 54, 105, 192).ToArgb(), (uint)SystemColor.FromArgb(255, 10, 73, 179).ToArgb());

            return 0;
        }

        public void OnDestroyDevice()
        {
            if (StateBlock != null) StateBlock.Release();
            StateBlock = null;
        }

        public Dialog GetDialogControl()
        {
            return Dialog;
        }

        public bool IsActive()
        {
            return Active;
        }

        public void SetActive(bool Active)
        {
            this.Active = Active;
            if (Active) Refresh();
        }
    }
}