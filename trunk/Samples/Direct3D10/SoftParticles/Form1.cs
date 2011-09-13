﻿using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Xtro.MDX;
using Xtro.MDX.Generic;
using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;
using D3D10Usage = Xtro.MDX.Direct3D10.Usage;
using Device = Xtro.MDX.Direct3D10.Device;
using Functions = Xtro.MDX.Direct3D10.Functions;
using Buffer = Xtro.MDX.Direct3D10.Buffer;
using Error = Xtro.MDX.Direct3D10.Error;
using UtilitiesError = Xtro.MDX.Utilities.Error;
using Xtro.MDX.Direct3DX10;
using D3DX10Constants = Xtro.MDX.Direct3DX10.Constants;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Xtro.MDX.Utilities;
using Button = Xtro.MDX.Utilities.Button;
using CheckBox = Xtro.MDX.Utilities.CheckBox;
using Color = Xtro.MDX.Direct3DX10.Color;
using UtilitiesComboBox = Xtro.MDX.Utilities.ComboBox;
using Control = Xtro.MDX.Utilities.Control;
using Font = Xtro.MDX.Direct3DX10.Font;
using UtilitiesFunctions = Xtro.MDX.Utilities.Functions;

namespace SoftParticles
{
    public enum ParticleTechnique
    {
        VolumeSoft = 0x0,
        VolumeHard,
        BillboardDepthSoft,
        BillboardDepth,
        BillboardSoft,
        BillboardHard
    };

    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        static extern uint GetTickCount();

        const uint MaximumParticles = 500;

        struct ParticleVertex
        {
            public Vector3 Position;
            public Vector3 Velocity;
            public float Life;
            public float Size;
        };

        ModelViewerCamera Camera;
        DialogResourceManager DialogResourceManager = new DialogResourceManager();// manager for shared resources of dialogs
        SettingsDialog SettingsDialog = new SettingsDialog();       // Device settings dialog
        Dialog HUD = new Dialog();                  // manages the 3D UI
        Dialog SampleUI = new Dialog();             // dialog for sample specific controls

        UnmanagedMemory<ParticleVertex> CpuParticles;
        UnmanagedMemory<uint> CpuParticleIndices;
        UnmanagedMemory<float> ParticleDepthArray;
        SDK_Mesh ObjectMesh = new SDK_Mesh();
        SDK_Mesh SkyMesh = new SDK_Mesh();

        Font D3DX10Font = null;         // Font for drawing text
        Sprite Sprite = null;       // Sprite for batching text drawing
        TextHelper TextHelper = null;
        Effect Effect;
        InputLayout SceneVertexLayout;
        InputLayout ParticleVertexLayout;

        Texture2D DepthStencilTexture;
        DepthStencilView DepthStencilDSV;
        ShaderResourceView DepthStencilSRV;

        Buffer ParticleVB;
        Buffer ParticleIB;
        ShaderResourceView ParticleTextureRV;
        Texture3D NoiseVolume;
        ShaderResourceView NoiseVolumeRV;
        ShaderResourceView ColorGradTextureRV;

        EffectTechnique RenderScene;
        EffectTechnique RenderSky;
        EffectTechnique RenderBillboardParticlesHard;
        EffectTechnique RenderBillboardParticlesODepth;
        EffectTechnique RenderBillboardParticlesSoft;
        EffectTechnique RenderVolumeParticlesHard;
        EffectTechnique RenderVolumeParticlesSoft;
        EffectTechnique RenderBillboardParticlesODepthSoft;
        EffectTechnique RenderVolumeParticlesSoftMSAA;
        EffectTechnique RenderVolumeParticlesHardMSAA;
        EffectTechnique RenderBillboardParticlesSoftMSAA;
        EffectTechnique RenderBillboardParticlesODepthSoftMSAA;
        EffectMatrixVariable WorldViewProj;
        EffectMatrixVariable WorldView;
        EffectMatrixVariable World;
        EffectMatrixVariable InvView;
        EffectMatrixVariable InvProj;
        EffectScalarVariable FadeDistanceVar;
        EffectScalarVariable SizeZScale;
        EffectVectorVariable ViewLightDir1;
        EffectVectorVariable ViewLightDir2;
        EffectVectorVariable WorldLightDir1;
        EffectVectorVariable WorldLightDir2;
        EffectVectorVariable EyePt;
        EffectVectorVariable ViewDir;
        EffectVectorVariable OctaveOffsets;
        EffectVectorVariable ScreenSize;
        EffectShaderResourceVariable DiffuseTexture;
        EffectShaderResourceVariable NormalTexture;
        EffectShaderResourceVariable ColorGradient;
        EffectShaderResourceVariable VolumeDiffTexture;
        EffectShaderResourceVariable VolumeNormTexture;
        EffectShaderResourceVariable DepthTexture;
        EffectShaderResourceVariable DepthMSAATexture;

        int BackBufferWidth = 640;
        int BackBufferHeight = 480;
        int SampleCount = 1;

        float FadeDistance = 1.0f;
        float ParticleLifeSpan = 5.0f;
        float EmitRate = 0.015f;

        float ParticleVel = 3.0f;
        float ParticleMaxSize = 1.25f;
        float ParticleMinSize = 1.0f;
        bool AnimateParticles = true;


        ParticleTechnique ParticleTechnique = ParticleTechnique.VolumeSoft;
        Vector3 LightDir1 = new Vector3(1.705f, 5.557f, -9.380f);
        Vector3 LightDir2 = new Vector3(-5.947f, -5.342f, -5.733f);

        enum ControlID
        {
            Static = -1,
            ToggleFullscreen = 1,
            ToggleREF = 3,
            ChangeDevice = 4,
            Technique = 5
        }

        static double DegreeToRadian(double Degree)
        {
            return Degree * D3DX10Constants.PI / 180.0f;
        }

        public Form1()
        {
            InitializeComponent();

            MouseWheel += Form1_MouseWheel;

            ArcBall.DefaultForm = this;
            UtilitiesFunctions.MediaSearchPath = @"C:\Program Files (x86)\Microsoft DirectX SDK (August 2009)\Samples\Media\";

            Camera = new ModelViewerCamera();
        }

        private void Form1_Shown(object Sender, EventArgs E)
        {
            try
            {
                Application.ThreadException += Application_Exception;

                // Disable gamma correction on this sample
                UtilitiesFunctions.SetIsInGammaCorrectMode(false);

                UtilitiesFunctions.SetCallbackDeviceChanging(OnModifyDeviceSettings, null);
                UtilitiesFunctions.SetCallbackFrameMove(OnFrameMove, null);
                UtilitiesFunctions.SetCallbackDeviceAcceptable(IsDeviceAcceptable, null);
                UtilitiesFunctions.SetCallbackDeviceCreated(OnDeviceCreated, null);
                UtilitiesFunctions.SetCallbackSwapChainResized(OnSwapChainResized, null);
                UtilitiesFunctions.SetCallbackSwapChainReleasing(OnSwapChainReleasing, null);
                UtilitiesFunctions.SetCallbackDeviceDestroyed(OnDeviceDestroyed, null);
                UtilitiesFunctions.SetCallbackFrameRender(OnFrameRender, null);

                Init();
                UtilitiesFunctions.Initialize(true);
                UtilitiesFunctions.SetCursorSettings(true, true);
                UtilitiesFunctions.SetWindow(this);
                UtilitiesFunctions.CreateDevice(true, 640, 480);

                if (UtilitiesFunctions.PreMainLoop() < 0) Close();

                Application.Idle += Application_Idle;
            }
            // if Shown event throws an exception, Application_Exception event gets innermost exception as parameter. So we call it for outermost exception
            catch (Exception Ex) { Application_Exception(Sender, new ThreadExceptionEventArgs(Ex)); }
        }

        void Init()
        {
            SettingsDialog.Init(DialogResourceManager);
            HUD.Init(DialogResourceManager);
            SampleUI.Init(DialogResourceManager);

            HUD.SetCallback(OnGUI_Event);
            var Y = 10;
            Button CreatedButton;
            HUD.AddButton((int)ControlID.ToggleFullscreen, "Toggle full screen", 35, Y, 125, 22, 0, false, out CreatedButton);
            HUD.AddButton((int)ControlID.ChangeDevice, "Change device (F2)", 35, Y += 24, 125, 22, Keys.F2, false, out CreatedButton);
            HUD.AddButton((int)ControlID.ToggleREF, "Toggle REF (F3)", 35, Y += 24, 125, 22, Keys.F3, false, out CreatedButton);

            SampleUI.SetCallback(OnGUI_Event);
            Y = 10;

            UtilitiesComboBox ComboBox;

            Static CreatedStatic;
            SampleUI.AddStatic((int)ControlID.Static, "(T)echnique", 0, 0, 105, 25, false, out CreatedStatic);
            SampleUI.AddComboBox((int)ControlID.Technique, 0, 25, 140, 24, Keys.T, false, out ComboBox);
            if (ComboBox != null) ComboBox.SetDropHeight(80);

            ComboBox.AddItem("Volume Soft", ((int)ParticleTechnique.VolumeSoft).ToString());
            ComboBox.AddItem("Volume Hard", ((int)ParticleTechnique.VolumeHard).ToString());
            ComboBox.AddItem("Depth Sprites Soft", ((int)ParticleTechnique.BillboardDepthSoft).ToString());
            ComboBox.AddItem("Depth Sprites Hard", ((int)ParticleTechnique.BillboardDepth).ToString());
            ComboBox.AddItem("Billboard Soft", ((int)ParticleTechnique.BillboardSoft).ToString());
            ComboBox.AddItem("Billboard Hard", ((int)ParticleTechnique.BillboardHard).ToString());
        }

        bool FirstTime = true;

        bool OnModifyDeviceSettings(DeviceSettings DeviceSettings, object UserContext)
        {
            // For the first device created if its a REF device, optionally display a warning dialog box
            if (FirstTime)
            {
                FirstTime = false;
                if (DeviceSettings.DriverType == DriverType.Reference) UtilitiesFunctions.DisplaySwitchingToREF_Warning();
            }

            DeviceSettings.SyncInterval = 0;

            return true;
        }

        //--------------------------------------------------------------------------------------
        // This callback function will be called once at the beginning of every frame. This is the
        // best location for your application to handle updates to the scene, but is not 
        // intended to contain actual rendering calls, which should instead be placed in the 
        // OnFrameRender callback.  
        //--------------------------------------------------------------------------------------
        void OnFrameMove(double Time, float ElapsedTime, object UserContext)
        {
            // Update the camera's position based on user input 
            Camera.FrameMove(ElapsedTime);

            var Eye = Camera.GetEyePt();
            var Dir = Camera.GetLookAtPt() - Eye;
            D3DX10Functions.Vector3Normalize(out Dir, ref Dir);

            var Device = UtilitiesFunctions.GetDevice();
            AdvanceParticles(Device, Time, ElapsedTime);
            SortParticleBuffer(Eye, Dir);
            UpdateParticleBuffers(Device);

            // Update the movement of the noise octaves
            var OctaveOffsets = new UnmanagedMemory<Vector4>(4 * (uint)Marshal.SizeOf(typeof(Vector4)));
            for (uint I = 0; I < 4; I++)
            {
                var V = new Vector4(-(float)(Time * 0.05), 0, 0, 0);
                OctaveOffsets.Set(I, ref V);
            }
            this.OctaveOffsets.SetFloatVectorArray(OctaveOffsets, 0, 4);
        }

        void OnGUI_Event(Event Event, int ControlID, Control Control, object UserContext)
        {
            switch (ControlID)
            {
            case (int)Form1.ControlID.ToggleFullscreen:
                UtilitiesFunctions.ToggleFullScreen(); break;
            case (int)Form1.ControlID.ToggleREF:
                UtilitiesFunctions.ToggleREF(); break;
            case (int)Form1.ControlID.ChangeDevice:
                SettingsDialog.SetActive(!SettingsDialog.IsActive()); break;
            case (int)Form1.ControlID.Technique:
                {
                    var ComboBox = (UtilitiesComboBox)Control;
                    ParticleTechnique = (ParticleTechnique)Convert.ToUInt32(ComboBox.GetSelectedData());
                    break;
                }
            }
        }

        private void Form1_FormClosing(object Sender, FormClosingEventArgs E)
        {
            Application.ThreadException -= Application_Exception;
            Application.Idle -= Application_Idle;

            try
            {
                // do something
            }
            // if FormClosing event throws an exception, Closing gets canceled. So we handle the exception.
            catch (Exception Ex)
            {
                MessageBox.Show(Ex.ToString());
            }
        }

        void Application_Idle(object Sender, EventArgs E)
        {
            try
            {
                try { UtilitiesFunctions.MainLoopStep(); }
                finally { PostMessage(Handle, 1024/*WM_USER*/, 0, 0); }
            }
            // if Idle event throws an exception, Application can't fire Application_Exception event. So we do it.
            catch (Exception Ex) { Application_Exception(Sender, new ThreadExceptionEventArgs(Ex)); }
        }

        static void Application_Exception(object Sender, ThreadExceptionEventArgs E)
        {
            MessageBox.Show(E.Exception.ToString());
            Application.Exit();
        }

        bool IsDeviceAcceptable(uint Adapter, uint Output, DriverType DeviceType, Format BackBufferFormat, bool Windowed, object UserContext)
        {
            return true;
        }

        int OnDeviceCreated(Device Device, ref SurfaceDescription BackBufferSurfaceDesc, object UserContext)
        {
            var Result = DialogResourceManager.OnCreateDevice(Device);
            if (Result < 0) return Result;
            Result = SettingsDialog.OnCreateDevice(Device);
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateFont(Device, 15, 0, (uint)FontWeight.Bold, 1, false, FontCharacterSet.Default, FontPrecision.Default, FontQuality.Default, FontPitchAndFamily.Default | FontPitchAndFamily.DontCare, "Arial", out D3DX10Font);
            if (Result < 0) return Result;

            Result = D3DX10Functions.CreateSprite(Device, 512, out Sprite);
            if (Result < 0) return Result;
            TextHelper = new TextHelper(D3DX10Font,Sprite, 15);

            // Read the D3DX effect file
            string DestinationPath;
            Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "SoftParticles.fx");
            if (Result < 0) return Result;
            var ShaderFlags = ShaderFlag.EnableStrictness;
#if DEBUG
            // Set the D3D10_SHADER_DEBUG flag to embed debug information in the shaders.
            // Setting this flag improves the shader debugging experience, but still allows 
            // the shaders to be optimized and to run exactly the way they will run in 
            // the release configuration of this program.
            ShaderFlags |= ShaderFlag.Debug;
#endif

            Result = D3DX10Functions.CreateEffectFromFile(DestinationPath, null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect);
            if (Result < 0) return Result;

            // Obtain the technique handles
            RenderScene = Effect.GetTechniqueByName("RenderScene");
            RenderSky = Effect.GetTechniqueByName("RenderSky");
            RenderBillboardParticlesHard = Effect.GetTechniqueByName("RenderBillboardParticles_Hard");
            RenderBillboardParticlesODepth = Effect.GetTechniqueByName("RenderBillboardParticles_ODepth");
            RenderBillboardParticlesSoft = Effect.GetTechniqueByName("RenderBillboardParticles_Soft");
            RenderBillboardParticlesODepthSoft = Effect.GetTechniqueByName("RenderBillboardParticles_ODepthSoft");
            RenderVolumeParticlesHard = Effect.GetTechniqueByName("RenderVolumeParticles_Hard");
            RenderVolumeParticlesSoft = Effect.GetTechniqueByName("RenderVolumeParticles_Soft");
            RenderVolumeParticlesSoftMSAA = Effect.GetTechniqueByName("RenderVolumeParticles_Soft_MSAA");
            RenderVolumeParticlesHardMSAA = Effect.GetTechniqueByName("RenderVolumeParticles_Hard_MSAA");
            RenderBillboardParticlesSoftMSAA = Effect.GetTechniqueByName("RenderBillboardParticles_Soft_MSAA");
            RenderBillboardParticlesODepthSoftMSAA = Effect.GetTechniqueByName("RenderBillboardParticles_ODepthSoft_MSAA");

            // Obtain the parameter handles
            WorldViewProj = Effect.GetVariableByName("g_mWorldViewProj").AsMatrix();
            WorldView = Effect.GetVariableByName("g_mWorldView").AsMatrix();
            World = Effect.GetVariableByName("g_mWorld").AsMatrix();
            InvView = Effect.GetVariableByName("g_mInvView").AsMatrix();
            InvProj = Effect.GetVariableByName("g_mInvProj").AsMatrix();
            FadeDistanceVar = Effect.GetVariableByName("g_fFadeDistance").AsScalar();
            SizeZScale = Effect.GetVariableByName("g_fSizeZScale").AsScalar();
            ViewLightDir1 = Effect.GetVariableByName("g_vViewLightDir1").AsVector();
            ViewLightDir2 = Effect.GetVariableByName("g_vViewLightDir2").AsVector();
            WorldLightDir1 = Effect.GetVariableByName("g_vWorldLightDir1").AsVector();
            WorldLightDir2 = Effect.GetVariableByName("g_vWorldLightDir2").AsVector();
            EyePt = Effect.GetVariableByName("g_vEyePt").AsVector();
            ViewDir = Effect.GetVariableByName("g_vViewDir").AsVector();
            OctaveOffsets = Effect.GetVariableByName("g_OctaveOffsets").AsVector();
            ScreenSize = Effect.GetVariableByName("g_vScreenSize").AsVector();
            DiffuseTexture = Effect.GetVariableByName("g_txDiffuse").AsShaderResource();
            NormalTexture = Effect.GetVariableByName("g_txNormal").AsShaderResource();
            ColorGradient = Effect.GetVariableByName("g_txColorGradient").AsShaderResource();
            VolumeDiffTexture = Effect.GetVariableByName("g_txVolumeDiff").AsShaderResource();
            VolumeNormTexture = Effect.GetVariableByName("g_txVolumeNorm").AsShaderResource();
            DepthTexture = Effect.GetVariableByName("g_txDepth").AsShaderResource();
            DepthMSAATexture = Effect.GetVariableByName("g_txDepthMSAA").AsShaderResource();

            // Create our vertex input layouts
            var SceneLayout = new[]
            {
                new InputElementDescription{SemanticName= "POSITION",Format=Format.R32G32B32_Float,InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName= "NORMAL",  Format= Format.R32G32B32_Float,AlignedByteOffset= 12, InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName= "TEXCOORD",Format= Format.R32G32_Float ,  AlignedByteOffset=24, InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName= "TANGENT",  Format=Format.R32G32B32_Float, AlignedByteOffset=32, InputSlotClass=InputClassification.InputPerVertexData }
            };
            PassDescription PassDescription;
            RenderScene.GetPassByIndex(0).GetDescription(out PassDescription);
            Result = Device.CreateInputLayout(SceneLayout, (uint)SceneLayout.Length, PassDescription.IA_InputSignature, PassDescription.IA_InputSignature.Size, out SceneVertexLayout);
            if (Result < 0) return Result;

            var ParticleLayout = new[]
            {
                new InputElementDescription{SemanticName= "POSITION", Format=Format.R32G32B32_Float,  InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName= "VELOCITY", Format=Format.R32G32B32_Float,AlignedByteOffset=   12, InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName= "LIFE",     Format=Format.R32_Float,AlignedByteOffset=   24, InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName= "SIZE",     Format=Format.R32_Float, AlignedByteOffset=  28, InputSlotClass=InputClassification.InputPerVertexData }
            };
            RenderBillboardParticlesHard.GetPassByIndex(0).GetDescription(out PassDescription);
            Result = Device.CreateInputLayout(ParticleLayout, (uint)ParticleLayout.Length, PassDescription.IA_InputSignature, PassDescription.IA_InputSignature.Size, out ParticleVertexLayout);
            if (Result < 0) return Result;

            // Load meshes
            Result = ObjectMesh.Create(Device, "SoftParticles\\TankScene.sdkmesh", true);
            if (Result < 0) return Result;
            Result = SkyMesh.Create(Device, "SoftParticles\\desertsky.sdkmesh", true);
            if (Result < 0) return Result;

            // Create the particles
            Result = CreateParticleBuffers(Device);
            if (Result < 0) return Result;

            // Create the noise volume
            Result = CreateNoiseVolume(Device, 32);
            if (Result < 0) return Result;

            // Load the Particle Texture
            Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "SoftParticles\\smokevol1.dds");
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateShaderResourceViewFromFile(Device, DestinationPath, out ParticleTextureRV);
            if (Result < 0) return Result;

            Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "SoftParticles\\colorgradient.dds");
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateShaderResourceViewFromFile(Device, DestinationPath, out ColorGradTextureRV);
            if (Result < 0) return Result;

            // Setup the camera's view parameters
            var Eye = new Vector3(2, 1, -10);
            var At = new Vector3(2, 1, 0);
            Camera.SetViewParameters(ref Eye, ref At);
            Camera.SetRadius(10.0f, 1.0f, 20.0f);

            FadeDistanceVar.SetFloat(FadeDistance);

            // Enable/Disable MSAA settings from the settings dialog based on whether we have dx10.1 
            // support or not.
            SettingsDialog.GetDialogControl().GetComboBox(SettingsDialog.ID_D3D10_MultiSampleCount).SetEnabled(false);
            SettingsDialog.GetDialogControl().GetComboBox(SettingsDialog.ID_D3D10_MultiSampleQuality).SetEnabled(false);
            SettingsDialog.GetDialogControl().GetStatic(SettingsDialog.ID_D3D10_MultiSampleCountLabel).SetEnabled(false);
            SettingsDialog.GetDialogControl().GetStatic(SettingsDialog.ID_D3D10_MultiSampleQualityLabel).SetEnabled(false);

            return 0;
        }

        void OnDeviceDestroyed(object UserContext)
        {
            DialogResourceManager.OnDestroyDevice();
            SettingsDialog.OnDestroyDevice();
            UtilitiesFunctions.GetGlobalResourceCache().OnDestroyDevice();
            if (D3DX10Font != null) D3DX10Font.Release();
            if (Sprite != null) Sprite.Release();
            if (TextHelper != null) TextHelper.Delete();
            if (Effect != null) Effect.Release();
            if (SceneVertexLayout != null) SceneVertexLayout.Release();
            if (ParticleVertexLayout != null) ParticleVertexLayout.Release();
            if (ParticleVB != null) ParticleVB.Release();
            if (ParticleIB != null) ParticleIB.Release();
            if(ParticleTextureRV!=null)ParticleTextureRV.Release();
            if(NoiseVolume!=null)NoiseVolume.Release();
            if(NoiseVolumeRV!=null)NoiseVolumeRV.Release();
            if(ColorGradTextureRV!=null)ColorGradTextureRV.Release();

            ObjectMesh.Delete();
            SkyMesh.Delete();

            CpuParticles=null;
            CpuParticleIndices=null;
            ParticleDepthArray=null;

            D3DX10Font = null;
            Sprite = null;
            TextHelper = null;
            Effect = null;
            SceneVertexLayout = null;
            ParticleVertexLayout =null;
            ParticleVB = null;
            ParticleIB = null;
            ParticleTextureRV=null;
            NoiseVolume=null;
            NoiseVolumeRV=null;
            ColorGradTextureRV=null;
        }

        int OnSwapChainResized(Device Device, SwapChain SwapChain, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            var Result = DialogResourceManager.OnResizedSwapChain(Device, ref BackBufferSurfaceDescription);
            if (Result < 0) return Result;
            Result = SettingsDialog.OnResizedSwapChain(Device, ref BackBufferSurfaceDescription);
            if (Result < 0) return Result;

            // Setup the camera's projection parameters
            var NearPlane = 0.1f;
            var FarPlane = 150.0f;
            var AspectRatio = BackBufferSurfaceDescription.Width / (float)BackBufferSurfaceDescription.Height;
            Camera.SetProjectionParameters(54.43f * ((float)D3DX10Constants.PI / 180.0f), AspectRatio, NearPlane, FarPlane);
            Camera.SetWindow((int)BackBufferSurfaceDescription.Width, (int)BackBufferSurfaceDescription.Height);
            Camera.SetButtonMasks(0, MouseKeys.Wheel, MouseKeys.Right | MouseKeys.Left);

            // Set the effect variable
            SizeZScale.SetFloat(1.0f / (FarPlane - NearPlane));

            HUD.SetLocation((int)BackBufferSurfaceDescription.Width - 170, 0);
            HUD.SetSize(170, 170);
            SampleUI.SetLocation((int)BackBufferSurfaceDescription.Width - 170, (int)BackBufferSurfaceDescription.Height - 200);
            SampleUI.SetSize(170, 200);

            // Create a new Depth-Stencil texture to replace the DXUT created one
            var DepthDescription = new Texture2D_Description
                {
                    Width = BackBufferSurfaceDescription.Width,
                    Height = BackBufferSurfaceDescription.Height,
                    MipLevels = 1,
                    ArraySize = 1,
                    Format = Format.R32_Typeless,
                    SampleDescription = BackBufferSurfaceDescription.SampleDescription,
                    Usage = D3D10Usage.Default,
                    BindFlags = BindFlag.DepthStencil | BindFlag.ShaderResource,
                    CpuAccessFlags = 0,
                    MiscellaneousFlags = 0
                };

            Result = Device.CreateTexture2D(ref DepthDescription, null, out DepthStencilTexture);
            if (Result < 0) return Result;

            // Create the depth stencil view
            var DepthStencilViewDescription = new DepthStencilViewDescription
                {
                    ViewDimension = 1 == DepthDescription.SampleDescription.Count ? DepthStencilViewDimension.Texture2D : DepthStencilViewDimension.Texture2D_MultiSampling,
                    Format = Format.D32_Float,
                    Texture2D =
                        {
                            MipSlice = 0
                        }
                };

            Result = Device.CreateDepthStencilView(DepthStencilTexture, ref DepthStencilViewDescription, out DepthStencilDSV);
            if (Result < 0) return Result;

            // Create the shader resource view
            if (1 == DepthDescription.SampleDescription.Count)
            {

                var ShaderResourceViewDescription = new ShaderResourceViewDescription
                    {
                        Format = Format.R32_Float,
                        ViewDimension = ShaderResourceViewDimension.Texture2D,
                        Texture2D =
                            {
                                MipLevels = 1,
                                MostDetailedMip = 0
                            }
                    };
                Result = Device.CreateShaderResourceView(DepthStencilTexture, ref ShaderResourceViewDescription, out DepthStencilSRV);
                if (Result < 0) return Result;

            }
            else
            {
                // Inconsistent state, we are trying to use MSAA with a device that does not support dx10.1
                Result = (int)UtilitiesError.Fail;
                if (Result < 0) return Result;
            }
            BackBufferWidth = (int)BackBufferSurfaceDescription.Width;
            BackBufferHeight = (int)BackBufferSurfaceDescription.Height;
            SampleCount = (int)BackBufferSurfaceDescription.SampleDescription.Count;

            return Result;
        }

        void OnSwapChainReleasing(object UserContext)
        {
            DialogResourceManager.OnReleasingSwapChain();

            if (DepthStencilTexture != null) DepthStencilTexture.Release();
            if (DepthStencilDSV != null) DepthStencilDSV.Release();
            if (DepthStencilSRV != null) DepthStencilSRV.Release();
        }

        void OnFrameRender(Device Device, double Time, float ElapsedTime, object UserContext)
        {
            var ClearColor = new Float4(new[] { 0f, 0f, 0f, 0f });

            var RenderTargetView = UtilitiesFunctions.GetRenderTargetView();
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);
            Device.ClearDepthStencilView(DepthStencilDSV, ClearFlag.Depth, 1, 0);
            Device.OM_SetRenderTargets(1, new[] { RenderTargetView }, DepthStencilDSV);

            // If the settings dialog is being shown, then
            // render it instead of rendering the app's scene
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.OnRender(ElapsedTime);
                return;
            }

            // Get the projection & view matrix from the camera class
            Matrix World;
            Matrix View;
            Matrix Proj;
            Matrix InvView;
            Matrix InvProj;
            D3DX10Functions.MatrixIdentity(out World);
            Proj = Camera.GetProjectionMatrix();
            View = Camera.GetViewMatrix();
            var WorldViewProj = World * View * Proj;
            var WorldView = World * View;
            D3DX10Functions.MatrixInverse(out InvView, ref View);
            D3DX10Functions.MatrixInverse(out InvProj, ref Proj);
            var ViewLightDir1=new Vector4();
            var WorldLightDir1 = new Vector4();
            var ViewLightDir2 = new Vector4();
            var WorldLightDir2 = new Vector4();
            Vector3 OutVector3;
            D3DX10Functions.Vector3Normalize(out OutVector3, ref LightDir1);
            WorldLightDir1.X = OutVector3.X;
            WorldLightDir1.Y = OutVector3.Y;
            WorldLightDir1.Z = OutVector3.Z;
            D3DX10Functions.Vector3TransformNormal(out OutVector3, ref LightDir1, ref View);
            D3DX10Functions.Vector3Normalize(out OutVector3, ref OutVector3);
            ViewLightDir1.X = OutVector3.X;
            ViewLightDir1.Y = OutVector3.Y;
            ViewLightDir1.Z = OutVector3.Z;
            D3DX10Functions.Vector3Normalize(out OutVector3, ref LightDir2);
            WorldLightDir2.X = OutVector3.X;
            WorldLightDir2.Y = OutVector3.Y;
            WorldLightDir2.Z = OutVector3.Z;
            D3DX10Functions.Vector3TransformNormal(out OutVector3, ref LightDir2, ref View);
            D3DX10Functions.Vector3Normalize(out OutVector3, ref OutVector3);
            ViewLightDir2.X = OutVector3.X;
            ViewLightDir2.Y = OutVector3.Y;
            ViewLightDir2.Z = OutVector3.Z;
            var ViewDir = Camera.GetLookAtPt() - Camera.GetEyePt();
            D3DX10Functions.Vector3Normalize(out ViewDir, ref ViewDir);
            var Vec3 = Camera.GetEyePt();
            var EyePt = new Vector4
                {
                    X = Vec3.X,
                    Y = Vec3.Y,
                    Z = Vec3.Z
                };
            var ScreenSize = new[] { (float)BackBufferWidth, (float)BackBufferHeight };

            this.WorldViewProj.SetMatrix((float[])WorldViewProj);
            this.WorldView.SetMatrix((float[])WorldView);
            this.World.SetMatrix((float[])World);
            this.InvView.SetMatrix((float[])InvView);
            this.InvProj.SetMatrix((float[])InvProj);
            this.ViewLightDir1.SetFloatVector((float[])ViewLightDir1);
            this.WorldLightDir1.SetFloatVector((float[])WorldLightDir1);
            this.ViewLightDir2.SetFloatVector((float[])ViewLightDir2);
            this.WorldLightDir2.SetFloatVector((float[])WorldLightDir2);
            this.ViewDir.SetFloatVector((float[])ViewDir);
            this.EyePt.SetFloatVector((float[])EyePt);
            this.ScreenSize.SetFloatVector(ScreenSize);

            // Render the scene
            Device.IA_SetInputLayout(SceneVertexLayout);
            SkyMesh.Render(Device, RenderSky, DiffuseTexture);
            ObjectMesh.Render(Device, RenderScene, DiffuseTexture, NormalTexture);

            EffectTechnique ParticleTech=null;

            if (1 == SampleCount)
            {
                switch (ParticleTechnique)
                {
                case ParticleTechnique.BillboardHard:
                    ParticleTech = RenderBillboardParticlesHard;
                    break;
                case ParticleTechnique.BillboardDepth:
                    ParticleTech = RenderBillboardParticlesODepth;
                    break;
                case ParticleTechnique.BillboardSoft:
                    ParticleTech = RenderBillboardParticlesSoft;
                    break;
                case ParticleTechnique.BillboardDepthSoft:
                    ParticleTech = RenderBillboardParticlesODepthSoft;
                    break;
                case ParticleTechnique.VolumeHard:
                    ParticleTech = RenderVolumeParticlesHard;
                    break;
                case ParticleTechnique.VolumeSoft:
                    ParticleTech = RenderVolumeParticlesSoft;
                    break;
                };
            }
            else
            {
                switch (ParticleTechnique)
                {
                case ParticleTechnique.BillboardHard:
                    ParticleTech = RenderBillboardParticlesHard;
                    break;
                case ParticleTechnique.BillboardDepth:
                    ParticleTech = RenderBillboardParticlesODepth;
                    break;
                case ParticleTechnique.BillboardSoft:
                    ParticleTech = RenderBillboardParticlesSoftMSAA;
                    break;
                case ParticleTechnique.BillboardDepthSoft:
                    ParticleTech = RenderBillboardParticlesODepthSoftMSAA;
                    break;
                case ParticleTechnique.VolumeHard:
                    ParticleTech = RenderVolumeParticlesHardMSAA;
                    break;
                case ParticleTechnique.VolumeSoft:
                    ParticleTech = RenderVolumeParticlesSoftMSAA;
                    break;
                };
            }

            if (ParticleTechnique.BillboardHard != ParticleTechnique && ParticleTechnique.BillboardDepth != ParticleTechnique)
            {
                // Unbind the depth stencil texture from the device
                Device.OM_SetRenderTargets(1, new[] { RenderTargetView }, null);
                // Bind it instead as a shader resource view
                if (1 == SampleCount) DepthTexture.SetResource(DepthStencilSRV);
                else DepthMSAATexture.SetResource(DepthStencilSRV);
            }

            // Render the particles
            Device.IA_SetInputLayout(ParticleVertexLayout);
            var Buffers = new[] { ParticleVB };
            var Stride = new[] { (uint)Marshal.SizeOf(typeof(ParticleVertex)) };
            var Offset = new[] { 0u };
            Device.IA_SetVertexBuffers(0, 1, Buffers, Stride, Offset);
            Device.IA_SetPrimitiveTopology(PrimitiveTopology.PointList);
            Device.IA_SetIndexBuffer(ParticleIB, Format.R32_UInt, 0);

            if (ParticleTechnique.VolumeHard == ParticleTechnique || ParticleTechnique.VolumeSoft == ParticleTechnique)
            {
                VolumeDiffTexture.SetResource(NoiseVolumeRV);
                VolumeNormTexture.SetResource(null);
            }
            else
            {
                VolumeDiffTexture.SetResource(ParticleTextureRV);
            }
            ColorGradient.SetResource(ColorGradTextureRV);

            TechniqueDescription TechniqueDescription;
            ParticleTech.GetDescription(out TechniqueDescription);

            for (uint P = 0; P < TechniqueDescription.Passes; ++P)
            {
                ParticleTech.GetPassByIndex(P).Apply(0);
                Device.DrawIndexed(MaximumParticles, 0, 0);
            }

            // unbind the depth from the resource so we can set it as depth next time around
            var Nulls = new ShaderResourceView[] { null, null };
            Device.PS_SetShaderResources(0, 2, Nulls);

            RenderText();
            HUD.OnRender(ElapsedTime);
            SampleUI.OnRender(ElapsedTime);
        }

        //--------------------------------------------------------------------------------------
        // Render the help and statistics text
        //--------------------------------------------------------------------------------------
        void RenderText()
        {
            TextHelper.Begin();
            TextHelper.SetInsertionPosition(2, 0);
            TextHelper.SetForegroundColor(new Color(1.0f, 1.0f, 0.0f, 1.0f));
            TextHelper.DrawTextLine(UtilitiesFunctions.GetFrameStats(UtilitiesFunctions.IsVsyncEnabled()));
            TextHelper.DrawTextLine(UtilitiesFunctions.GetDeviceStats());
            TextHelper.End();
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            UtilitiesFunctions.HandlePaintEvent();
        }

        bool HandleDialogResizeEvent(EventArgs E)
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleResizeEvent(E);
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleResizeEvent(E);
            if (NoFurtherProcessing) return false;
            SampleUI.HandleResizeEvent(E);

            return false;
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
            if (HandleDialogResizeEvent(e)) return;

            UtilitiesFunctions.HandleResizeEvent();
        }

        private void Form1_ResizeBegin(object sender, EventArgs e)
        {
            UtilitiesFunctions.HandleResizeBeginEvent();
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            UtilitiesFunctions.HandleResizeEndEvent();
        }

        private void Form1_CursorChanged(object sender, EventArgs e)
        {
            UtilitiesFunctions.HandleCursorChangedEvent();
        }

        bool HandleDialogActivatedEvent()
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleActivatedEvent();
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleActivatedEvent();
            if (NoFurtherProcessing) return false;
            SampleUI.HandleActivatedEvent();

            return false;
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            if (HandleDialogActivatedEvent()) return;

            UtilitiesFunctions.HandleActivatedEvent();
        }

        bool HandleDialogDeactivatedEvent()
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleDeactivateEvent();
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleDeactivateEvent();
            if (NoFurtherProcessing) return false;
            SampleUI.HandleDeactivateEvent();

            return false;
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            if (HandleDialogDeactivatedEvent()) return;

            UtilitiesFunctions.HandleDeactivateEvent();
        }

        bool HandleDialogKeyDownEvent(KeyEventArgs E)
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleKeyDownEvent(E);
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleKeyDownEvent(E);
            if (NoFurtherProcessing) return false;
            NoFurtherProcessing = SampleUI.HandleKeyDownEvent(E);
            if (NoFurtherProcessing) return false;

            // Pass all remaining windows messages to camera so it can respond to user input
            Camera.HandleKeyDownEvent(E);

            return false;
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F1) Camera.Reset();

            if (HandleDialogKeyDownEvent(e)) return;

            UtilitiesFunctions.HandleKeyDownEvent(e);
        }

        bool HandleDialogMouseDownAndDoubleClickEvent(MouseEventArgs E)
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleMouseDownAndDoubleClickEvent(E);
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleMouseDownAndDoubleClickEvent(E);
            if (NoFurtherProcessing) return false;
            NoFurtherProcessing = SampleUI.HandleMouseDownAndDoubleClickEvent(E);
            if (NoFurtherProcessing) return false;

            // Pass all remaining windows messages to camera so it can respond to user input
            Camera.HandleMouseDownAndDoubleClickEvent(E);

            return false;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (HandleDialogMouseDownAndDoubleClickEvent(e)) return;
        }

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (HandleDialogMouseDownAndDoubleClickEvent(e)) return;
        }

        bool HandleDialogMouseUpEvent(MouseEventArgs E)
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleMouseUpEvent(E);
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleMouseUpEvent(E);
            if (NoFurtherProcessing) return false;
            NoFurtherProcessing = SampleUI.HandleMouseUpEvent(E);
            if (NoFurtherProcessing) return false;

            // Pass all remaining windows messages to camera so it can respond to user input
            Camera.HandleMouseUpEvent(E);

            return false;
        }

        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if (HandleDialogMouseUpEvent(e)) return;
        }

        bool HandleDialogMouseWheelEvent(MouseEventArgs E)
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleMouseWheelEvent(E);
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleMouseWheelEvent(E);
            if (NoFurtherProcessing) return false;
            NoFurtherProcessing = SampleUI.HandleMouseWheelEvent(E);
            if (NoFurtherProcessing) return false;

            // Pass all remaining windows messages to camera so it can respond to user input
            Camera.HandleMouseWheelEvent(E);

            return false;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (HandleDialogMouseWheelEvent(e)) return;
        }

        bool HandleDialogMouseMoveEvent(MouseEventArgs E)
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleMouseMoveEvent(E);
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleMouseMoveEvent(E);
            if (NoFurtherProcessing) return false;
            NoFurtherProcessing = SampleUI.HandleMouseMoveEvent(E);
            if (NoFurtherProcessing) return false;

            // Pass all remaining windows messages to camera so it can respond to user input
            Camera.HandleMouseMoveEvent(E);

            return false;
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (HandleDialogMouseMoveEvent(e)) return;
        }

        bool HandleDialogMoveEvent(EventArgs E)
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleMoveEvent(E);
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleMoveEvent(E);
            if (NoFurtherProcessing) return false;
            SampleUI.HandleMoveEvent(E);

            return false;
        }

        private void Form1_Move(object sender, EventArgs e)
        {
            if (HandleDialogMoveEvent(e)) return;
        }

        bool HandleDialogMouseCaptureChangedEvent(EventArgs E)
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleMouseCaptureChangedEvent(E);
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleMouseCaptureChangedEvent(E);
            if (NoFurtherProcessing) return false;
            SampleUI.HandleMouseCaptureChangedEvent(E);

            return false;
        }

        private void Form1_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (HandleDialogMouseCaptureChangedEvent(e)) return;
        }

        bool HandleDialogKeyUpEvent(KeyEventArgs E)
        {
            // Pass messages to settings dialog if its active
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.HandleKeyUpEvent(E);
                return false;
            }

            // Give the dialogs a chance to handle the message first
            var NoFurtherProcessing = HUD.HandleKeyUpEvent(E);
            if (NoFurtherProcessing) return false;
            NoFurtherProcessing = SampleUI.HandleKeyUpEvent(E);
            if (NoFurtherProcessing) return false;

            // Pass all remaining windows messages to camera so it can respond to user input
            Camera.HandleKeyUpEvent(E);

            return false;
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (HandleDialogKeyUpEvent(e)) return;
        }

        float RPercent()
        {
            var Ret = (float)(new Random().Next(20000) - 10000);
            return Ret / 10000.0f;
        }

        //--------------------------------------------------------------------------------------
        void EmitParticle(out ParticleVertex Particle)
        {
            Particle.Position.X = 0.0f;
            Particle.Position.Y = 0.7f;
            Particle.Position.Z = 3.0f;

            Particle.Velocity.X = 1.0f;
            Particle.Velocity.Y = 0.3f * RPercent();
            Particle.Velocity.Z = 0.3f * RPercent();

            D3DX10Functions.Vector3Normalize(out Particle.Velocity, ref Particle.Velocity);
            Particle.Velocity *= ParticleVel;

            Particle.Life = 0.0f;
            Particle.Size = 0.0f;
        }

        //--------------------------------------------------------------------------------------
        // Create a VB for particles
        //--------------------------------------------------------------------------------------
        int CreateParticleBuffers(Device Device)
        {
            var ByteWidth = MaximumParticles * (uint)Marshal.SizeOf(typeof(ParticleVertex));
            var VertexBufferDescription = new BufferDescription
                {
                    BindFlags = BindFlag.VertexBuffer,
                    ByteWidth = ByteWidth,
                    CpuAccessFlags = 0,
                    MiscellaneousFlags = 0,
                    Usage = Usage.Default
                };
            var Result = Device.CreateBuffer(ref VertexBufferDescription, out ParticleVB);
            if (Result < 0) return Result;

            VertexBufferDescription.BindFlags = BindFlag.IndexBuffer;
            VertexBufferDescription.ByteWidth = MaximumParticles * 4;
            Result = Device.CreateBuffer(ref VertexBufferDescription, out ParticleIB);
            if (Result < 0) return Result;

            CpuParticles = new UnmanagedMemory<ParticleVertex>(ByteWidth);

            for (uint I = 0; I < MaximumParticles; I++)
            {
                var Temp = new ParticleVertex { Life = -1 };//kill all particles
                CpuParticles.Set(I, ref Temp);
            }

            CpuParticleIndices = new UnmanagedMemory<uint>(MaximumParticles*4);
            ParticleDepthArray = new UnmanagedMemory<float>(MaximumParticles*4);

            return Result;
        }

        struct Byte4
        {
            public byte X, Y, Z, W;
        };

        //--------------------------------------------------------------------------------------
        float GetDensity(int X, int Y, int Z, UnmanagedMemory<Byte4> Texels, uint VolumeSize)
        {
            if (X < 0) X += (int)VolumeSize;
            if (Y < 0) Y += (int)VolumeSize;
            if (Z < 0) Z += (int)VolumeSize;

            X = X % (int)VolumeSize;
            Y = Y % (int)VolumeSize;
            Z = Z % (int)VolumeSize;

            var Index = X + Y * VolumeSize + Z * VolumeSize * VolumeSize;

            Byte4 Out;
            Texels.Get((uint)Index, out Out);

            return Out.W / 128.0f;
        }

        void SetNormal(Vector3 Normal, int X, int Y, int Z, UnmanagedMemory<Byte4> Texels, uint VolumeSize)
        {
            if (X < 0) X += (int)VolumeSize;
            if (Y < 0) Y += (int)VolumeSize;
            if (Z < 0) Z += (int)VolumeSize;

            X = X % (int)VolumeSize;
            Y = Y % (int)VolumeSize;
            Z = Z % (int)VolumeSize;

            var Index = X + Y * VolumeSize + Z * VolumeSize * VolumeSize;

            Byte4 Out;
            Texels.Get((uint)Index, out Out);

            Out.X = (byte)(Normal.X * 128.0f);
            Out.Y = (byte)(Normal.Y * 128.0f);
            Out.Z = (byte)(Normal.Z * 128.0f);

            Texels.Set((uint)Index, ref Out);
        }

        //--------------------------------------------------------------------------------------
        // Create and blur a noise volume texture
        //--------------------------------------------------------------------------------------
        int CreateNoiseVolume(Device Device, uint VolumeSize)
        {
            var InitData = new SubResourceData
                {
                    SystemMemory = new UnmanagedMemory<Byte4>(VolumeSize * VolumeSize * VolumeSize * 4),
                    SystemMemoryPitch = VolumeSize * 4,
                    SystemMemorySlicePitch = VolumeSize * VolumeSize * 4
                };

            var Data = (UnmanagedMemory<Byte4>)InitData.SystemMemory;

            // Gen a bunch of random values
            for (uint I = 0; I < VolumeSize * VolumeSize * VolumeSize; I++)
            {
                var Temp = new Byte4 { W = (byte)(RPercent() * 128.0f) };
                Data.Set(I, ref Temp);
            }

            // Generate normals from the density gradient
            var HeightAdjust = 0.5f;
            Vector3 Normal;
            Vector3 DensityGradient;
            for (var Z = 0; Z < VolumeSize; Z++)
            {
                for (var Y = 0; Y < VolumeSize; Y++)
                {
                    for (var X = 0; X < VolumeSize; X++)
                    {
                        DensityGradient.X = GetDensity(X + 1, Y, Z, Data, VolumeSize) - GetDensity(X - 1, Y, Z, Data, VolumeSize) / HeightAdjust;
                        DensityGradient.Y = GetDensity(X, Y + 1, Z, Data, VolumeSize) - GetDensity(X, Y - 1, Z, Data, VolumeSize) / HeightAdjust;
                        DensityGradient.Z = GetDensity(X, Y, Z + 1, Data, VolumeSize) - GetDensity(X, Y, Z - 1, Data, VolumeSize) / HeightAdjust;

                        D3DX10Functions.Vector3Normalize(out Normal, ref DensityGradient);
                        SetNormal(Normal, X, Y, Z, Data, VolumeSize);
                    }
                }
            }

            var Description = new Texture3D_Description();
            Description.BindFlags = BindFlag.ShaderResource;
            Description.CpuAccessFlags = 0;
            Description.Depth = Description.Height = Description.Width = VolumeSize;
            Description.Format = Format.R8G8B8A8_SNorm;
            Description.MipLevels = 1;
            Description.MiscellaneousFlags = 0;
            Description.Usage = Usage.Immutable;
            var Result = Device.CreateTexture3D(ref Description, new[] { InitData }, out NoiseVolume);
            if (Result < 0) return Result;

            var ShaderResourceViewDescription = new ShaderResourceViewDescription();
            ShaderResourceViewDescription.Format = Description.Format;
            ShaderResourceViewDescription.ViewDimension = ShaderResourceViewDimension.Texture3D;
            ShaderResourceViewDescription.Texture3D.MipLevels = Description.MipLevels;
            ShaderResourceViewDescription.Texture3D.MostDetailedMip = 0;
            Result = Device.CreateShaderResourceView(NoiseVolume, ref ShaderResourceViewDescription, out NoiseVolumeRV);
            if (Result < 0) return Result;

            InitData.SystemMemory.Dispose();
            return Result;
        }

        //--------------------------------------------------------------------------------------
        // Update the particle VB using UpdateSubresource
        //--------------------------------------------------------------------------------------
        void QuickDepthSort(UnmanagedMemory<uint> Indices, UnmanagedMemory<float> Depths, int Lo, int Hi)
        {
            //  lo is the lower index, hi is the upper index
            //  of the region of array a that is to be sorted
            int I = Lo, J = Hi;
            float H;
            int Index;
            float X;
            Depths.Get((uint)((Lo + Hi) / 2), out X);

            //  partition
            do
            {
                float D;
                while (true)
                {
                    Depths.Get((uint)I, out D);
                    if (D <= X) break;
                    I++;
                }
                while (true)
                {
                    Depths.Get((uint)J, out D);
                    if (D >= X) break;
                    J--;
                }
                if (I <= J)
                {
                    Depths.Get((uint)I, out H);
                    Depths.Get((uint)J, out D); Depths.Set((uint)I, ref D);
                    Depths.Set((uint)J, ref H);

                    int II;
                    Indices.Get((uint)I, out Index);
                    Indices.Get((uint)J, out II); Indices.Set((uint)I, ref D);
                    Indices.Set((uint)J, ref Index);
                    I++; J--;
                }
            } while (I <= J);

            //  recursion
            if (Lo < J) QuickDepthSort(Indices, Depths, Lo, J);
            if (I < Hi) QuickDepthSort(Indices, Depths, I, Hi);
        }

        //--------------------------------------------------------------------------------------
        // Sort the particle buffer
        //--------------------------------------------------------------------------------------
        void SortParticleBuffer(Vector3 Eye, Vector3 Dir)
        {
            if (ParticleDepthArray == null || CpuParticleIndices == null) return;

            // assume vDir is normalized
            Vector3 ToParticle;
            //init indices and depths
            for (uint I = 0; I < MaximumParticles; I++)
            {
                CpuParticleIndices.Set(I, ref I);
                ParticleVertex Out;
                CpuParticles.Get(I, out Out);
                ToParticle = Out.Position - Eye;
                var Dot = D3DX10Functions.Vector3Dot(ref Dir, ref ToParticle);
                ParticleDepthArray.Set(I, ref Dot);
            }

            // Sort
            QuickDepthSort(CpuParticleIndices, ParticleDepthArray, 0, (int)(MaximumParticles - 1));
        }

        //--------------------------------------------------------------------------------------
        // Create a VB for particles
        //--------------------------------------------------------------------------------------
        double LastEmitTime;
        uint LastParticleEmitted;
        void AdvanceParticles(Device Device, double Time, float TimeDelta)
        {
            //emit new particles

            if (!AnimateParticles)
            {
                LastEmitTime = Time;
                return;
            }

            var EmitRate = this.EmitRate;
            var ParticleMaxSize = this.ParticleMaxSize;
            var ParticleMinSize = this.ParticleMinSize;

            if (ParticleTechnique.VolumeHard == ParticleTechnique || ParticleTechnique.VolumeSoft == ParticleTechnique)
            {
                EmitRate *= 3.0f;	//emit 1/3 less particles if we're doing volume
                ParticleMaxSize *= 1.5f;	//1.5x the max radius
                ParticleMinSize *= 1.5f;	//1.5x the min radius
            }

            var NumParticlesToEmit = (uint)((Time - LastEmitTime) / EmitRate);
            if (NumParticlesToEmit > 0)
            {
                for (var I = 0; I < NumParticlesToEmit; I++)
                {
                    ParticleVertex Out;
                    EmitParticle(out Out);
                    CpuParticles.Set(LastParticleEmitted, ref Out);
                    LastParticleEmitted = (LastParticleEmitted + 1) % MaximumParticles;
                }
                LastEmitTime = Time;
            }

            Vector3 Vel;
            float LifeSq;
            for (uint I = 0; I < MaximumParticles; I++)
            {
                ParticleVertex P;
                CpuParticles.Get(I, out P);
                if (P.Life > -1)
                {
                    // squared velocity falloff
                    LifeSq = P.Life * P.Life;

                    // Slow down by 50% as we age
                    Vel = P.Velocity * (1 - 0.5f * LifeSq);
                    Vel.Y += 0.5f;	//(add some to the up direction, becuase of buoyancy)

                    P.Position += Vel * TimeDelta;
                    P.Life += TimeDelta / ParticleLifeSpan;
                    P.Size = ParticleMinSize + (ParticleMaxSize - ParticleMinSize) * P.Life;

                    if (P.Life > 0.99f) P.Life = -1;

                    CpuParticles.Set(I, ref P);
                }
            }
        }

        //--------------------------------------------------------------------------------------
        // Update the particle VB using UpdateSubresource
        //--------------------------------------------------------------------------------------
        void UpdateParticleBuffers(Device Device)
        {
            Device.UpdateSubresource(ParticleVB, 0, CpuParticles, 0, 0);
            Device.UpdateSubresource(ParticleIB, 0, CpuParticleIndices, 0, 0);
        }
    }
}