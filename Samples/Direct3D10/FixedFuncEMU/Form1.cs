﻿// ReSharper disable CSharpWarnings::CS0197
using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Xtro.MDX;
using Xtro.MDX.Generic;
using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;
using D3D10Usage = Xtro.MDX.Direct3D10.Usage;
using Device = Xtro.MDX.Direct3D10.Device;
using Buffer = Xtro.MDX.Direct3D10.Buffer;
using Xtro.MDX.Direct3DX10;
using D3DX10Constants = Xtro.MDX.Direct3DX10.Constants;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Xtro.MDX.Utilities;
using Button = Xtro.MDX.Utilities.Button;
using Color = Xtro.MDX.Direct3DX10.Color;
using Control = Xtro.MDX.Utilities.Control;
using Font = Xtro.MDX.Direct3DX10.Font;
using UtilitiesFunctions = Xtro.MDX.Utilities.Functions;

namespace FixedFuncEMU
{
    sealed partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity]
        static extern IntPtr PostMessage(IntPtr Wnd, int Msg, int ParamW, int ParamL);

        enum FogMode
        {
            // ReSharper disable UnusedMember.Local
            None = 0,
            Linear = 1,
            Exp = 2,
            Exp2 = 3,
            // ReSharper restore UnusedMember.Local
        };

        const byte MaximumBalls = 10;

        struct SceneVertex
        {
            // ReSharper disable NotAccessedField.Local
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 Texture;
            // ReSharper restore NotAccessedField.Local
        };

        struct SceneLight
        {
            // ReSharper disable NotAccessedField.Local
            public Vector4 Position;
            public Vector4 Diffuse;
            public Vector4 Specular;
            public Vector4 Ambient;
            public Vector4 Atten;
            // ReSharper restore NotAccessedField.Local
        };

        struct Ball
        {
            public double StartTime;
            public Matrix World;
            public Vector3 VelStart;
        };

        readonly ModelViewerCamera Camera;
        readonly DialogResourceManager DialogResourceManager = new DialogResourceManager();// manager for shared resources of dialogs
        readonly SettingsDialog SettingsDialog = new SettingsDialog();       // Device settings dialog
        readonly Dialog HUD = new Dialog();                  // manages the 3D UI
        readonly Dialog SampleUI = new Dialog();             // dialog for sample specific controls

        Matrix BlackHole;
        Matrix LightView;
        Matrix LightProj;
        readonly UnmanagedMemory<SceneLight> Lights = new UnmanagedMemory<SceneLight>(8 * (uint)Marshal.SizeOf(typeof(SceneLight)));
        readonly UnmanagedMemory<float> ClipPlanes = new UnmanagedMemory<float>(3 * (uint)Marshal.SizeOf(typeof(Vector4)));
        readonly Ball[] Balls = new Ball[MaximumBalls];
        const double LaunchInterval = 0.3f;
        const float RotateSpeed = 70.0f;

        Font Font2;         // Font for drawing text
        Sprite Sprite;       // Sprite for batching text drawing
        TextHelper TextHelper;
        Effect Effect;
        InputLayout VertexLayout;
        Buffer ScreenQuadVertexBuffer;

        ShaderResourceView ScreenTextureResourceView;
        ShaderResourceView ProjectedTextureResourceView;

        readonly SDK_Mesh BallMesh = new SDK_Mesh();
        readonly SDK_Mesh RoomMesh = new SDK_Mesh();
        readonly SDK_Mesh HoleMesh = new SDK_Mesh();

        EffectTechnique RenderSceneGouraudTech;
        EffectTechnique RenderSceneFlatTech;
        EffectTechnique RenderScenePointTech;
        EffectTechnique RenderScreenSpaceAlphaTestTech;

        EffectMatrixVariable World;
        EffectMatrixVariable View;
        EffectMatrixVariable Proj;
        EffectMatrixVariable InvProj;
        EffectMatrixVariable LightViewProj;
        EffectShaderResourceVariable DiffuseTex;
        EffectShaderResourceVariable ProjectedTex;
        EffectVariable SceneLights;
        EffectVectorVariable ClipPlanesVariable;
        EffectScalarVariable ViewportHeight;
        EffectScalarVariable ViewportWidth;
        EffectScalarVariable NearPlane;
        EffectScalarVariable PointSize;
        EffectScalarVariable EnableLighting;
        EffectScalarVariable EnableClipping;
        EffectScalarVariable FogModeVariable;
        EffectScalarVariable FogStart;
        EffectScalarVariable FogEnd;
        EffectScalarVariable FogDensity;
        EffectVectorVariable FogColor;

        enum ControlID
        {
            ToggleFullscreen = 1,
            ToggleREF = 3,
            ChangeDevice = 4,
        }

        public Form1()
        {
            InitializeComponent();

            MouseWheel += Form1_MouseWheel;

            ArcBall.DefaultForm = this;
            UtilitiesFunctions.MediaSearchPath = @"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Samples\Media\";

            Camera = new ModelViewerCamera();
        }

        private void Form1_Shown(object Sender, EventArgs E)
        {
            try
            {
                Application.ThreadException += Application_Exception;

                UtilitiesFunctions.SetCallbackDeviceChanging(OnModifyDeviceSettings, null);
                UtilitiesFunctions.SetCallbackFrameMove(OnFrameMove, null);

                UtilitiesFunctions.SetCallbackDeviceAcceptable(IsDeviceAcceptable, null);
                UtilitiesFunctions.SetCallbackDeviceCreated(OnDeviceCreated, null);
                UtilitiesFunctions.SetCallbackSwapChainResized(OnSwapChainResized, null);
                UtilitiesFunctions.SetCallbackFrameRender(OnFrameRender, null);
                UtilitiesFunctions.SetCallbackSwapChainReleasing(OnSwapChainReleasing, null);
                UtilitiesFunctions.SetCallbackDeviceDestroyed(OnDeviceDestroyed, null);

                Init();
                UtilitiesFunctions.Initialize();
                UtilitiesFunctions.SetCursorSettings(true, true);
                UtilitiesFunctions.SetWindow(this);
                UtilitiesFunctions.CreateDevice(true, 800, 600);

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
            HUD.AddButton((int)ControlID.ToggleREF, "Toggle REF (F3)", 35, Y + 24, 125, 22, Keys.F3, false, out CreatedButton);

            SampleUI.SetCallback(OnGUI_Event);
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

            return true;
        }

        //--------------------------------------------------------------------------------------
        // This callback function will be called once at the beginning of every frame. This is the
        // best location for your application to handle updates to the scene, but is not 
        // intended to contain actual rendering calls, which should instead be placed in the 
        // OnFrameRender callback.  
        //--------------------------------------------------------------------------------------
        double LastLaunch;
        void OnFrameMove(double Time, float ElapsedTime, object UserContext)
        {
            // Update the camera's position based on user input 
            Camera.FrameMove(ElapsedTime);

            var BlackHoleRads = (float)(Time * D3DX10Functions.ToRadian(RotateSpeed));
            D3DX10Functions.MatrixRotationY(out BlackHole, BlackHoleRads);

            // Rotate the clip planes to align with the black holes
            var Vector4 = new Vector4(0, 1.0f, 0, -0.8f);
            ClipPlanes.Set(0, 0, ref Vector4);
            var VectorPlane1 = new Vector3(0.707f, 0.707f, 0);
            var VectorPlane2 = new Vector3(-0.707f, 0.707f, 0);
            Vector3 Out;
            D3DX10Functions.Vector3TransformNormal(out Out, ref VectorPlane1, ref BlackHole);
            Vector4 = new Vector4(Out.X, Out.Y, Out.Z, 0.70f);
            ClipPlanes.Set(0, 1, ref Vector4);
            D3DX10Functions.Vector3TransformNormal(out Out, ref VectorPlane2, ref BlackHole);
            Vector4 = new Vector4(Out.X, Out.Y, Out.Z, 0.70f);
            ClipPlanes.Set(0, 2, ref Vector4);
            ClipPlanesVariable.SetFloatVectorArray(ClipPlanes, 0, 3);

            var BallLaunch = new Vector3(2.1f, 8.1f, 0);
            var BallStart = new Vector3(0, 0.45f, 0);
            var BallGravity = new Vector3(0, -9.8f, 0);

            var BallLife = 3.05f / BallLaunch.X;

            // Move existing balls
            for (var I = 0; I < MaximumBalls; I++)
            {
                var T = (float)(Time - Balls[I].StartTime);
                if (T < BallLife + 0.5f) // Live 1/2 second longer to fully show off clipping
                {
                    // Use the equation X = Xo + VoT + 1/2AT^2
                    var BallNow = BallStart + Balls[I].VelStart * T + BallGravity * 0.5f * T * T;

                    // Create a world matrix
                    D3DX10Functions.MatrixTranslation(out Balls[I].World, BallNow.X, BallNow.Y, BallNow.Z);
                }
                else
                {
                    Balls[I].StartTime = -1.0;
                }
            }

            // Launch a ball if it's time
            var Found = false;
            //LastLaunch = -LaunchInterval - 1;
            if ((Time - LastLaunch) > LaunchInterval)
            {
                for (var I = 0; I < MaximumBalls && !Found; I++)
                {
                    if (Balls[I].StartTime < 0.0)
                    {
                        // Found a free ball
                        Balls[I].StartTime = Time;
                        Matrix LaunchMatrix;
                        D3DX10Functions.MatrixRotationY(out LaunchMatrix, (I % 2) * (float)D3DX10Functions.ToRadian(180.0f) + BlackHoleRads + (float)D3DX10Functions.ToRadian(BallLife * RotateSpeed));
                        D3DX10Functions.Vector3TransformNormal(out Balls[I].VelStart, ref BallLaunch, ref LaunchMatrix);
                        D3DX10Functions.MatrixTranslation(out Balls[I].World, BallStart.X, BallStart.Y, BallStart.Z);
                        Found = true;
                    }
                }
                LastLaunch = Time;
            }

            // Rotate the cookie matrix
            Matrix LightRot;
            D3DX10Functions.MatrixRotationY(out LightRot, (float)D3DX10Functions.ToRadian(50.0f) * (float)Time);
            var LightEye = new Vector3(0, 5.65f, 0);
            var LightAt = new Vector3(0, 0, 0);
            var Up = new Vector3(0, 0, 1);
            D3DX10Functions.MatrixLookAtLH(out LightView, ref LightEye, ref LightAt, ref Up);
            LightView = LightRot * LightView;
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

        static bool IsDeviceAcceptable(uint Adapter, uint Output, DriverType DeviceType, Format BackBufferFormat, bool Windowed, object UserContext)
        {
            return true;
        }

        int OnDeviceCreated(Device Device, ref SurfaceDescription BackBufferSurfaceDesc, object UserContext)
        {
            var Result = DialogResourceManager.OnCreateDevice(Device);
            if (Result < 0) return Result;
            Result = SettingsDialog.OnCreateDevice(Device);
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateFont(Device, 15, 0, (uint)FontWeight.Bold, 1, false, FontCharacterSet.Default, FontPrecision.Default, FontQuality.Default, FontPitchAndFamilyFlag.Default | FontPitchAndFamilyFlag.DontCare, "Arial", out Font2);
            if (Result < 0) return Result;

            Result = D3DX10Functions.CreateSprite(Device, 512, out Sprite);
            if (Result < 0) return Result;
            TextHelper = new TextHelper(Font2, Sprite);

#if DEBUG
            // Set the D3D10_SHADER_DEBUG flag to embed debug information in the shaders.
            // Setting this flag improves the shader debugging experience, but still allows 
            // the shaders to be optimized and to run exactly the way they will run in 
            // the release configuration of this program.
            const ShaderFlag ShaderFlags = ShaderFlag.EnableStrictness | ShaderFlag.Debug;
#else
            const ShaderFlag ShaderFlags = ShaderFlag.EnableStrictness;
#endif

            // Read the D3DX effect file
            string DestinationPath;
            Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "FixedFuncEMU.fx");
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateEffectFromFile(DestinationPath, null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect);
            if (Result < 0) return Result;

            // Obtain the technique handles
            RenderSceneGouraudTech = Effect.GetTechniqueByName("RenderSceneGouraud");
            RenderSceneFlatTech = Effect.GetTechniqueByName("RenderSceneFlat");
            RenderScenePointTech = Effect.GetTechniqueByName("RenderScenePoint");
            RenderScreenSpaceAlphaTestTech = Effect.GetTechniqueByName("RenderScreenSpaceAlphaTest");

            // Obtain the parameter handles
            World = Effect.GetVariableByName("g_mWorld").AsMatrix();
            View = Effect.GetVariableByName("g_mView").AsMatrix();
            Proj = Effect.GetVariableByName("g_mProj").AsMatrix();
            InvProj = Effect.GetVariableByName("g_mInvProj").AsMatrix();
            LightViewProj = Effect.GetVariableByName("g_mLightViewProj").AsMatrix();
            DiffuseTex = Effect.GetVariableByName("g_txDiffuse").AsShaderResource();
            ProjectedTex = Effect.GetVariableByName("g_txProjected").AsShaderResource();
            SceneLights = Effect.GetVariableByName("g_lights");
            ClipPlanesVariable = Effect.GetVariableByName("g_clipplanes").AsVector();
            ViewportHeight = Effect.GetVariableByName("g_viewportHeight").AsScalar();
            ViewportWidth = Effect.GetVariableByName("g_viewportWidth").AsScalar();
            NearPlane = Effect.GetVariableByName("g_nearPlane").AsScalar();
            PointSize = Effect.GetVariableByName("g_pointSize").AsScalar();
            EnableLighting = Effect.GetVariableByName("g_bEnableLighting").AsScalar();
            EnableClipping = Effect.GetVariableByName("g_bEnableClipping").AsScalar();
            FogModeVariable = Effect.GetVariableByName("g_fogMode").AsScalar();
            FogStart = Effect.GetVariableByName("g_fogStart").AsScalar();
            FogEnd = Effect.GetVariableByName("g_fogEnd").AsScalar();
            FogDensity = Effect.GetVariableByName("g_fogDensity").AsScalar();
            FogColor = Effect.GetVariableByName("g_fogColor").AsVector();

            //set constant variables
            PointSize.SetFloat(3.0f);
            FogModeVariable.SetInt((int)FogMode.Linear);
            FogStart.SetFloat(12.0f);
            FogEnd.SetFloat(22.0f);
            FogDensity.SetFloat(0.05f);
            var VectorFogColor = new Vector4(0.7f, 1.0f, 1.0f, 1);
            var Data = new UnmanagedMemory<float>((uint)Marshal.SizeOf(VectorFogColor));
            Data.Set(0, ref VectorFogColor);
            FogColor.SetFloatVector(Data);

            // Create our vertex input layout
            var Layout = new[]
            {
                new InputElementDescription{SemanticName= "POSITION",Format=Format.R32G32B32_Float,InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName= "NORMAL",  Format= Format.R32G32B32_Float,AlignedByteOffset= 12, InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName= "TEXTURE",Format= Format.R32G32_Float ,  AlignedByteOffset=24, InputSlotClass=InputClassification.InputPerVertexData }
            };

            PassDescription PassDesc;
            RenderSceneGouraudTech.GetPassByIndex(0).GetDescription(out PassDesc);
            Result = Device.CreateInputLayout(Layout, 3, PassDesc.IA_InputSignature, PassDesc.IA_InputSignature.Size, out VertexLayout);
            if (Result < 0) return Result;

            // Load the meshes
            Result = BallMesh.Create(Device, "misc\\ball.sdkmesh", true);
            if (Result < 0) return Result;
            Result = RoomMesh.Create(Device, "BlackHoleRoom\\BlackHoleRoom.sdkmesh", true);
            if (Result < 0) return Result;
            Result = HoleMesh.Create(Device, "BlackHoleRoom\\BlackHole.sdkmesh", true);
            if (Result < 0) return Result;
            D3DX10Functions.MatrixIdentity(out BlackHole);

            // Initialize the balls
            for (var I = 0; I < MaximumBalls; I++) Balls[I].StartTime = -1.0;

            // Setup the Lights
            var LightIndex = 0u;
            for (var Y = 0; Y < 3; Y++)
            {
                for (var X = 0; X < 3; X++)
                {
                    if (X != 1 || Y != 1)
                    {
                        var Light = new SceneLight
                        {
                            Position = new Vector4(3.0f * (-1.0f + X), 5.65f, 5.0f * (-1.0f + Y), 1),
                            Atten = { X = 1.0f }
                        };
                        if (0 == LightIndex % 2)
                        {
                            Light.Diffuse = new Vector4(0.20f, 0.20f, 0.20f, 1.0f);
                            Light.Specular = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
                            Light.Ambient = new Vector4(0.03f, 0.03f, 0.03f, 0.0f);
                        }
                        else
                        {
                            Light.Diffuse = new Vector4(0.0f, 0.15f, 0.20f, 1.0f);
                            Light.Specular = new Vector4(0.15f, 0.25f, 0.3f, 1.0f);
                            Light.Ambient = new Vector4(0.00f, 0.02f, 0.03f, 0.0f);
                        }

                        Lights.Set(LightIndex, ref Light);

                        LightIndex++;
                    }
                }
            }
            SceneLights.SetRawValue(Lights, 0, 8 * (uint)Marshal.SizeOf(typeof(SceneLight)));

            D3DX10Functions.MatrixPerspectiveFovLH(out LightProj, (float)D3DX10Functions.ToRadian(90.0f), 1.0f, 0.1f, 100.0f);

            // Create the screenspace quad VB
            // This gets initialized in OnD3D10SwapChainResized
            var VertexBufferDescription = new BufferDescription
            {
                ByteWidth = 4 * (uint)Marshal.SizeOf(typeof(SceneVertex)),
                Usage = D3D10Usage.Default,
                BindFlags = BindFlag.VertexBuffer,
            };
            Result = Device.CreateBuffer(ref VertexBufferDescription, out ScreenQuadVertexBuffer);
            if (Result < 0) return Result;

            // Load the HUD and Cookie Textures
            Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "misc\\hud.dds");
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateShaderResourceViewFromFile(Device, DestinationPath, out ScreenTextureResourceView);
            if (Result < 0) return Result;
            Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "misc\\cookie.dds");
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateShaderResourceViewFromFile(Device, DestinationPath, out ProjectedTextureResourceView);
            if (Result < 0) return Result;
            ProjectedTex.SetResource(ProjectedTextureResourceView);

            // Setup the camera's view parameters
            var Eye = new Vector3(0.0f, 2.3f, -8.5f);
            var At = new Vector3(0.0f, 2.0f, 0.0f);
            Camera.SetViewParameters(ref Eye, ref At);
            Camera.SetRadius(9.0f, 1.0f, 15.0f);

            return 0;
        }

        void OnDeviceDestroyed(object UserContext)
        {
            DialogResourceManager.OnDestroyDevice();
            SettingsDialog.OnDestroyDevice();
            UtilitiesFunctions.GetGlobalResourceCache().OnDestroyDevice();
            if (Font2 != null) Font2.Release();
            if (Sprite != null) Sprite.Release();
            if (TextHelper != null) TextHelper.Delete();
            if (Effect != null) Effect.Release();
            if (VertexLayout != null) VertexLayout.Release();
            if (ScreenTextureResourceView != null) ScreenTextureResourceView.Release();
            if (ProjectedTextureResourceView != null) ProjectedTextureResourceView.Release();
            if (ScreenQuadVertexBuffer != null) ScreenQuadVertexBuffer.Release();

            BallMesh.Delete();
            RoomMesh.Delete();
            HoleMesh.Delete();

            Font2 = null;
            Sprite = null;
            TextHelper = null;
            Effect = null;
            VertexLayout = null;
            ScreenTextureResourceView = null;
            ProjectedTextureResourceView = null;
            ScreenQuadVertexBuffer = null;
        }

        int OnSwapChainResized(Device Device, SwapChain SwapChain, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            var Result = DialogResourceManager.OnResizedSwapChain(Device, ref BackBufferSurfaceDescription);
            if (Result < 0) return Result;
            Result = SettingsDialog.OnResizedSwapChain( /*Device,*/ ref BackBufferSurfaceDescription);
            if (Result < 0) return Result;

            float BackbufferWidth = BackBufferSurfaceDescription.Width;
            float BackbufferHeight = BackBufferSurfaceDescription.Height;

            // Set the viewport width/height
            ViewportWidth.SetFloat(BackbufferWidth);
            ViewportHeight.SetFloat(BackbufferHeight);

            // Setup the camera's projection parameters
            var AspectRatio = BackBufferSurfaceDescription.Width / (float)BackBufferSurfaceDescription.Height;
            Camera.SetProjectionParameters((float)D3DX10Constants.PI / 4, AspectRatio, 0.1f, 100.0f);
            Camera.SetWindow((int)BackBufferSurfaceDescription.Width, (int)BackBufferSurfaceDescription.Height);
            Camera.SetButtonMasks(MouseKeys.Left, MouseKeys.Wheel, MouseKeys.Middle);

            NearPlane.SetFloat(0.1f);

            HUD.SetLocation((int)BackBufferSurfaceDescription.Width - 170, 0);
            HUD.SetSize(170, 170);
            SampleUI.SetLocation((int)BackBufferSurfaceDescription.Width - 170, (int)BackBufferSurfaceDescription.Height - 300);
            SampleUI.SetSize(170, 300);

            // Update our Screen-space quad
            var Verts = new UnmanagedMemory<SceneVertex>(4 * (uint)Marshal.SizeOf(typeof(SceneVertex)));

            var Vertex = new SceneVertex { Position = new Vector3(0, 0, 0.5f), Normal = new Vector3(0, 0, 0), Texture = new Vector2(0, 0) };
            Verts.Set(0, ref Vertex);
            Vertex = new SceneVertex { Position = new Vector3(BackbufferWidth, 0, 0.5f), Normal = new Vector3(0, 0, 0), Texture = new Vector2(1, 0) };
            Verts.Set(1, ref Vertex);
            Vertex = new SceneVertex { Position = new Vector3(0, BackbufferHeight, 0.5f), Normal = new Vector3(0, 0, 0), Texture = new Vector2(0, 1) };
            Verts.Set(2, ref Vertex);
            Vertex = new SceneVertex { Position = new Vector3(BackbufferWidth, BackbufferHeight, 0.5f), Normal = new Vector3(0, 0, 0), Texture = new Vector2(1, 1) };
            Verts.Set(3, ref Vertex);

            Device.UpdateSubresource(ScreenQuadVertexBuffer, 0, Verts, 0, 0);

            return Result;
        }

        void RenderScreenQuad(Device Device, EffectTechnique Technique)
        {
            var Stride = (uint)Marshal.SizeOf(typeof(SceneVertex));
            const uint Offsets = 0u;
            var Buffers = new[] { ScreenQuadVertexBuffer };
            Device.IA_SetVertexBuffers(0, 1, Buffers, new[] { Stride }, new[] { Offsets });
            Device.IA_SetPrimitiveTopology(PrimitiveTopology.TriangleStrip);

            TechniqueDescription TechniqueDescription;
            Technique.GetDescription(out TechniqueDescription);
            DiffuseTex.SetResource(ScreenTextureResourceView);
            for (var P = 0u; P < TechniqueDescription.Passes; ++P)
            {
                Technique.GetPassByIndex(P).Apply(0);
                Device.Draw(4, 0);
            }
        }

        void OnSwapChainReleasing(object UserContext)
        {
            DialogResourceManager.OnReleasingSwapChain();
        }

        void OnFrameRender(Device Device, double Time, float ElapsedTime, object UserContext)
        {
            var ClearColor = new Float4(new[] { 0f, 0f, 1f, 1f });

            var RenderTargetView = UtilitiesFunctions.GetRenderTargetView();
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);
            var DepthStencilView = UtilitiesFunctions.GetDepthStencilView();
            Device.ClearDepthStencilView(DepthStencilView, ClearFlag.Depth, 1, 0);
            Device.OM_SetRenderTargets(1, new[] { RenderTargetView }, DepthStencilView);

            // If the settings dialog is being shown, then
            // render it instead of rendering the app's scene
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.OnRender(ElapsedTime);
                return;
            }

            Device.IA_SetInputLayout(VertexLayout);

            // Get the projection & view matrix from the camera class
            Matrix WorldMatrix;
            Matrix InvProjMatrix;
            D3DX10Functions.MatrixIdentity(out WorldMatrix);
            var ProjMatrix = Camera.GetProjectionMatrix();
            var ViewMatrix = Camera.GetViewMatrix();

            var Data = new UnmanagedMemory<float>((uint)Marshal.SizeOf(typeof(Matrix)));
            var LightViewProjMatrix = LightView * LightProj;
            Data.Set(0, ref LightViewProjMatrix);
            LightViewProj.SetMatrix(Data);
            Data.Set(0, ref WorldMatrix);
            World.SetMatrix(Data);
            Data.Set(0, ref ViewMatrix);
            View.SetMatrix(Data);
            Data.Set(0, ref ProjMatrix);
            Proj.SetMatrix(Data);
            D3DX10Functions.MatrixInverse(out InvProjMatrix, ref ProjMatrix);
            Data.Set(0, ref InvProjMatrix);
            InvProj.SetMatrix(Data);

            // Render the room and the blackholes
            EnableClipping.SetBool(false);
            EnableLighting.SetBool(false);
            RoomMesh.Render(Device, RenderSceneGouraudTech, DiffuseTex);
            Data.Set(0, ref BlackHole);
            World.SetMatrix(Data);
            HoleMesh.Render(Device, RenderSceneGouraudTech, DiffuseTex);

            // Render the balls
            EnableClipping.SetBool(true);
            EnableLighting.SetBool(true);
            for (var I = 0; I < MaximumBalls; I++)
            {
                if (Balls[I].StartTime > -1.0)
                {
                    Data.Set(0, ref Balls[I].World);
                    World.SetMatrix(Data);

                    switch (I % 3)
                    {
                    case 0: BallMesh.Render(Device, RenderSceneGouraudTech, DiffuseTex); break;
                    case 1: BallMesh.Render(Device, RenderSceneFlatTech, DiffuseTex); break;
                    default: BallMesh.Render(Device, RenderScenePointTech, DiffuseTex); break;
                    }
                }
            }

            EnableClipping.SetBool(false);
            EnableLighting.SetBool(false);
            RenderScreenQuad(Device, RenderScreenSpaceAlphaTestTech);

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

        private void Form1_Paint(object Sender, PaintEventArgs E)
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

        private void Form1_Resize(object Sender, EventArgs E)
        {
            if (HandleDialogResizeEvent(E)) return;

            UtilitiesFunctions.HandleResizeEvent();
        }

        private void Form1_ResizeBegin(object Sender, EventArgs E)
        {
            UtilitiesFunctions.HandleResizeBeginEvent();
        }

        private void Form1_ResizeEnd(object Sender, EventArgs E)
        {
            UtilitiesFunctions.HandleResizeEndEvent();
        }

        private void Form1_CursorChanged(object Sender, EventArgs E)
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

        private void Form1_Activated(object Sender, EventArgs E)
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

        private void Form1_Deactivate(object Sender, EventArgs E)
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

        private void Form1_KeyDown(object Sender, KeyEventArgs E)
        {
            if (E.KeyCode == Keys.F1) Camera.Reset();

            if (HandleDialogKeyDownEvent(E)) return;

            UtilitiesFunctions.HandleKeyDownEvent(E);
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

        private void Form1_MouseDown(object Sender, MouseEventArgs E)
        {
            if (HandleDialogMouseDownAndDoubleClickEvent(E)) return;
        }

        private void Form1_MouseDoubleClick(object Sender, MouseEventArgs E)
        {
            if (HandleDialogMouseDownAndDoubleClickEvent(E)) return;
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

        private void Form1_MouseUp(object Sender, MouseEventArgs E)
        {
            if (HandleDialogMouseUpEvent(E)) return;
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

        private void Form1_MouseWheel(object Sender, MouseEventArgs E)
        {
            if (HandleDialogMouseWheelEvent(E)) return;
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

        private void Form1_MouseMove(object Sender, MouseEventArgs E)
        {
            if (HandleDialogMouseMoveEvent(E)) return;
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

        private void Form1_Move(object Sender, EventArgs E)
        {
            if (HandleDialogMoveEvent(E)) return;
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

        private void Form1_MouseCaptureChanged(object Sender, EventArgs E)
        {
            if (HandleDialogMouseCaptureChangedEvent(E)) return;
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

        private void Form1_KeyUp(object Sender, KeyEventArgs E)
        {
            if (HandleDialogKeyUpEvent(E)) return;
        }
    }
}
// ReSharper restore CSharpWarnings::CS0197