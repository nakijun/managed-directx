using System;
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
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        static extern uint GetTickCount();

        enum ControlID
        {
            ToggleFullscreen = 1,
            ToggleREF = 2,
            ChangeDevice = 3,
            ToggleSpin = 4,
            QuadRenderMode = 5,
            SceneDepthStencilMode = 6,
            ScreenRasterizerMode = 7
        }

        static double DegreeToRadian(double Degree)
        {
            return Degree * D3DX10Constants.PI / 180.0f;
        }

        static readonly string[] QuadTechniques =
        {
            "RenderQuadSolid",
            "RenderQuadSrcAlphaAdd",
            "RenderQuadSrcAlphaSub",
            "RenderQuadSrcColorAdd",
            "RenderQuadSrcColorSub"
        };

        static readonly string[] DepthStencilModes =
        {
            "DepthOff/StencilOff",
            "DepthLess/StencilOff",
            "DepthGreater/StencilOff",

            "DepthOff/StencilIncOnFail",
            "DepthLess/StencilIncOnFail",
            "DepthGreater/StencilIncOnFail",

            "DepthOff/StencilIncOnPass",
            "DepthLess/StencilIncOnPass",
            "DepthGreater/StencilIncOnPass",
        };

        static readonly string[] RasterizerModes =
        {
            "CullOff/FillSolid",
            "CullFront/FillSolid",
            "CullBack/FillSolid",

            "CullOff/FillWire",
            "CullFront/FillWire",
            "CullBack/FillWire",
        };

        ModelViewerCamera Camera;
        DialogResourceManager DialogResourceManager = new DialogResourceManager();// manager for shared resources of dialogs
        SettingsDialog SettingsDialog = new SettingsDialog();       // Device settings dialog
        Dialog HUD = new Dialog();                  // manages the 3D UI
        Dialog SampleUI = new Dialog();             // dialog for sample specific controls

        Matrix World;
        bool Spinning = true;
        Font D3DX10Font = null;         // Font for drawing text
        Sprite Sprite = null;       // Sprite for batching text drawing
        TextHelper TextHelper = null;

        Effect Effect;
        InputLayout SceneLayout;
        InputLayout QuadLayout;
        Buffer ScreenQuadVertexBuffer;
        SDK_Mesh Mesh = new SDK_Mesh();
        ShaderResourceView[] ScreenRecourceView = new ShaderResourceView[2];

        uint SceneDepthStencilMode;
        DepthStencilState[] DepthStencilStates = new DepthStencilState[DepthStencilModes.Length]; // Depth Stencil states for non-FX 
        // depth stencil state managment
        uint SceneRasterizerMode;
        RasterizerState[] RasterStates = new RasterizerState[RasterizerModes.Length];  // Rasterizer states for non-FX 
        // rasterizer state management
        uint QuadRenderMode;
        EffectTechnique[] TechniqueQuad = new EffectTechnique[QuadTechniques.Length]; // Quad Techniques from the FX file for 
        // FX based alpha blend state management
        EffectTechnique TechniqueScene;             // FX technique for rendering the scene
        EffectTechnique TechniqueRenderWithStencil; // FX technique for rendering using FX based depth
        // stencil state management

        EffectShaderResourceVariable DiffuseVariable = null;
        EffectMatrixVariable WorldVariable = null;
        EffectMatrixVariable ViewVariable = null;
        EffectMatrixVariable ProjectionVariable = null;

        struct ScreenVertex
        {
            public Vector4 Position;
            public Vector2 Texture;
        };

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

                UtilitiesFunctions.SetCallbackDeviceAcceptable(IsDeviceAcceptable, null);
                UtilitiesFunctions.SetCallbackDeviceCreated(OnDeviceCreated, null);
                UtilitiesFunctions.SetCallbackSwapChainResized(OnSwapChainResized, null);
                UtilitiesFunctions.SetCallbackSwapChainReleasing(OnSwapChainReleasing, null);
                UtilitiesFunctions.SetCallbackDeviceDestroyed(OnDeviceDestroyed, null);
                UtilitiesFunctions.SetCallbackFrameRender(OnFrameRender, null);
                UtilitiesFunctions.SetCallbackFrameMove(OnFrameMove, null);
                UtilitiesFunctions.SetCallbackModifyDeviceSettings(OnModifyDeviceSettings, null);

                UtilitiesFunctions.Initialize(true);
                UtilitiesFunctions.SetCursorSettings(true, true);
                Init();
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
            Spinning = true;

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

            UtilitiesComboBox ComboBox;

            Y = 0;
            Static CreatedStatic;
            SampleUI.AddStatic(-1, "(Q)uad Render Mode", 0, Y, 200, 25, false, out CreatedStatic);
            Y += 25;
            SampleUI.AddComboBox((int)ControlID.QuadRenderMode, 0, Y, 220, 24, Keys.Q, false, out ComboBox);
            if (ComboBox != null) ComboBox.SetDropHeight(150);

            Y += 40;
            SampleUI.AddStatic(-1, "Scene (R)asterizer Mode", 0, Y, 200, 25, false, out CreatedStatic);
            Y += 25;
            SampleUI.AddComboBox((int)ControlID.ScreenRasterizerMode, 0, Y, 220, 24, Keys.R, false, out ComboBox);
            if (ComboBox != null) ComboBox.SetDropHeight(150);

            Y += 40;
            SampleUI.AddStatic(-1, "Scene Depth/(S)tencil Mode", 0, Y, 200, 25, false, out CreatedStatic);
            Y += 25;
            SampleUI.AddComboBox((int)ControlID.SceneDepthStencilMode, 0, Y, 220, 24, Keys.S, false, out ComboBox);
            if (ComboBox != null) ComboBox.SetDropHeight(150);

            Y += 24;
            CheckBox CreatedCheckBox;
            SampleUI.AddCheckBox((int)ControlID.ToggleSpin, "Toggle Spinning", 35, Y += 24, 125, 22, Spinning, 0, false, out CreatedCheckBox);
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
            catch (Exception Ex) { MessageBox.Show(Ex.ToString()); }
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

        int OnDeviceCreated(Device Device, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            var Result = DialogResourceManager.OnCreateDevice(Device);
            if (Result < 0) return Result;
            Result = SettingsDialog.OnCreateDevice(Device);
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateFont(Device, 15, 0, (uint)FontWeight.Bold, 1, false, FontCharacterSet.Default, FontPrecision.Default, FontQuality.Default, FontPitchAndFamily.Default | FontPitchAndFamily.DontCare, "Arial", out D3DX10Font);
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateSprite(Device, 512, out Sprite);
            if (Result < 0) return Result;
            TextHelper = new TextHelper(D3DX10Font, Sprite, 15);

            var ShaderFlags = ShaderFlag.EnableStrictness;
#if DEBUG
            // Set the D3D10_SHADER_DEBUG flag to embed debug information in the shaders.
            // Setting this flag improves the shader debugging experience, but still allows 
            // the shaders to be optimized and to run exactly the way they will run in 
            // the release configuration of this program.
            ShaderFlags |= ShaderFlag.Debug;
#endif

            // Read the D3DX effect file
            string DestinationPath;
            Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "SoftParticles.fx");
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateEffectFromFile(DestinationPath, null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect);
            if (Result < 0) return Result;

            // Obtain the technique handles
            TechniqueScene = Effect.GetTechniqueByName("RenderScene");
            TechniqueRenderWithStencil = Effect.GetTechniqueByName("RenderWithStencil");
            LoadQuadTechniques();
            LoadDepthStencilStates(Device);
            LoadRasterizerStates(Device);

            DiffuseVariable = Effect.GetVariableByName("g_txDiffuse").AsShaderResource();
            WorldVariable = Effect.GetVariableByName("World").AsMatrix();
            ViewVariable = Effect.GetVariableByName("View").AsMatrix();
            ProjectionVariable = Effect.GetVariableByName("Projection").AsMatrix();

            // Define the input layout
            InputElementDescription[] Layout = 
            { 
                new InputElementDescription
                {
                    SemanticName = "POSITION",
                    SemanticIndex = 0,
                    Format = Format.R32G32B32_Float,
                    InputSlot = 0,
                    AlignedByteOffset = 0,
                    InputSlotClass = InputClassification.InputPerVertexData,
                    InstanceDataStepRate = 0
                },
                new InputElementDescription
                {
                            SemanticName = "NORMAL",
                            SemanticIndex = 0,
                            Format = Format.R32G32B32_Float,
                            InputSlot = 0,
                            AlignedByteOffset = 12,
                            InputSlotClass = InputClassification.InputPerVertexData,
                            InstanceDataStepRate = 0
                },
                new InputElementDescription
                {
                    SemanticName = "TEXCOORD",
                    SemanticIndex = 0,
                    Format = Format.R32G32_Float,
                    InputSlot = 0,
                    AlignedByteOffset = 24,
                    InputSlotClass = InputClassification.InputPerVertexData,
                    InstanceDataStepRate = 0
                }
            };

            // Create the input layout
            PassDescription PassDescription;
            TechniqueScene.GetPassByIndex(0).GetDescription(out PassDescription);
            Result = Device.CreateInputLayout(Layout, (uint)Layout.Length, PassDescription.IA_InputSignature, (uint)PassDescription.IA_InputSignature.Size, out SceneLayout);
            if (Result < 0) return Result;

            // Load the mesh
            Result = Mesh.Create(Device, "Tiny\\tiny.sdkmesh", true);
            if (Result < 0) return Result;

            // Initialize the world matrices
            D3DX10Functions.MatrixIdentity(out World);

            // Create a screen quad
            var ScreenQuadLayout = new[]
            {
                new InputElementDescription{SemanticName= "POSITION",SemanticIndex= 0,Format=Format.R32G32B32A32_Float, InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName=  "TEXCOORD", SemanticIndex= 0, Format=Format.R32G32_Float,AlignedByteOffset= 16, InputSlotClass=InputClassification.InputPerVertexData  },
            };

            TechniqueQuad[0].GetPassByIndex(0).GetDescription(out PassDescription);
            InputLayout InputLayout;
            Result = Device.CreateInputLayout(ScreenQuadLayout, 2, PassDescription.IA_InputSignature, PassDescription.IA_InputSignature.Size, out InputLayout);
            if (Result < 0) return Result;
            QuadLayout = InputLayout;

            const float Size = 1.0f;

            const uint VertexCount = 4;
            var VertexSize = Marshal.SizeOf(typeof(ScreenVertex));
            var Quad = new UnmanagedMemory<ScreenVertex>((uint)(VertexSize * VertexCount));
            Quad.Write(new[]
            {
                new ScreenVertex
                {
                    Position = new Vector4( -Size, Size, 0.0f, 1.0f ),
                    Texture = new Vector2( 0.0f, 0.0f ),
                },
                new ScreenVertex
                {
                    Position = new Vector4( Size, Size, 0.0f, 1.0f ),
                    Texture = new Vector2( 1.0f, 0.0f ),
                },
                new ScreenVertex
                {
                    Position = new Vector4( -Size, -Size, 0.0f, 1.0f ),
                    Texture = new Vector2( 0.0f, 1.0f ),
                },
                new ScreenVertex
                {
                    Position = new Vector4( Size, -Size, 0.0f, 1.0f ),
                    Texture = new Vector2( 1.0f, 1.0f )
                }
            });

            var VertexBufferDescription = new BufferDescription
            {
                ByteWidth = (uint)(4 * Marshal.SizeOf(typeof(ScreenVertex))),
                Usage = D3D10Usage.Default,
                BindFlags = BindFlag.VertexBuffer,
            };

            var InitData = new SubResourceData
            {
                SystemMemory = Quad,
                SystemMemoryPitch = 0,
                SystemMemorySlicePitch = 0,
            };
            Result = Device.CreateBuffer(ref VertexBufferDescription, ref InitData, out ScreenQuadVertexBuffer);
            if (Result < 0) return Result;

            // Load the texture for the screen quad
            var ScreenTextures = new[]
            {
                "misc\\MarbleClouds.dds",
                "misc\\NormTest.dds"
            };

            for (var I = 0; I < 2; I++)
            {
                Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, ScreenTextures[I]);
                if (Result < 0) return Result;
                Result = D3DX10Functions.CreateShaderResourceViewFromFile(Device, DestinationPath, out ScreenRecourceView[I]);
                if (Result < 0) return Result;
            }

            // Initialize the camera
            Vector3 Eye = new Vector3(0.0f, 0.0f, -800.0f);
            Vector3 At = new Vector3(0.0f, 1.0f, 0.0f);
            Camera.SetViewParameters(ref Eye, ref At);

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
            if (SceneLayout != null) SceneLayout.Release();
            if (QuadLayout != null) QuadLayout.Release();
            if (Effect != null) Effect.Release();
            if (ScreenQuadVertexBuffer != null) ScreenQuadVertexBuffer.Release();

            foreach (var S in ScreenRecourceView)
            {
                S.Release();
            }

            Mesh.Delete();

            foreach (var D in DepthStencilStates)
            {
                D.Release();
            }

            foreach (var R in RasterStates)
            {
                R.Release();
            }

            D3DX10Font = null;
            Sprite = null;
            TextHelper = null;
            SceneLayout = null;
            QuadLayout = null;
            Effect = null;
            ScreenQuadVertexBuffer = null;
        }

        int OnSwapChainResized(Device Device, SwapChain SwapChain, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            var Result = DialogResourceManager.OnResizedSwapChain(Device, ref BackBufferSurfaceDescription);
            if (Result < 0) return Result;
            Result = SettingsDialog.OnResizedSwapChain(Device, BackBufferSurfaceDescription);
            if (Result < 0) return Result;

            // Setup the camera's projection parameters
            var AspectRatio = (float)BackBufferSurfaceDescription.Width / BackBufferSurfaceDescription.Height;
            Camera.SetProjectionParameters((float)(D3DX10Constants.PI / 4), AspectRatio, 0.1f, 5000.0f);
            Camera.SetWindow((int)BackBufferSurfaceDescription.Width, (int)BackBufferSurfaceDescription.Height);
            Camera.SetButtonMasks(MouseKeys.Middle, MouseKeys.Wheel, MouseKeys.Left);

            HUD.SetLocation((int)BackBufferSurfaceDescription.Width - 170, 0);
            HUD.SetSize(170, 170);
            SampleUI.SetLocation((int)BackBufferSurfaceDescription.Width - 230, (int)BackBufferSurfaceDescription.Height - 300);
            SampleUI.SetSize(170, 300);

            return 0;
        }

        void OnSwapChainReleasing(object UserContext)
        {
            DialogResourceManager.OnReleasingSwapChain();
        }

        void OnFrameRender(Device Device, double Time, float ElapsedTime, object UserContext)
        {
            // If the settings dialog is being shown, then
            // render it instead of rendering the app's scene
            if (SettingsDialog.IsActive())
            {
                SettingsDialog.OnRender(ElapsedTime);
                return;
            }

            //
            // Clear the back buffer
            //
            var ClearColor = new Float4(new[] { 0.0f, 0.125f, 0.3f, 1.0f }); //red,green,blue,alpha
            var RenderTargetView = UtilitiesFunctions.GetRenderTargetView();
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);

            //
            // Clear the depth stencil
            //
            var DepthStencilView = UtilitiesFunctions.GetDepthStencilView();
            Device.ClearDepthStencilView(DepthStencilView, ClearFlag.Depth | ClearFlag.Stencil, 1.0f, 0);

            //
            // Update variables that change once per frame
            //
            ProjectionVariable.SetMatrix((float[])Camera.GetProjectionMatrix());
            ViewVariable.SetMatrix((float[])Camera.GetViewMatrix());
            WorldVariable.SetMatrix((float[])World);

            // Update the Cull Mode (non-FX method)
            //
            Device.RS_SetState(RasterStates[SceneRasterizerMode]);

            //
            // Update the Depth Stencil States (non-FX method)
            //
            Device.OM_SetDepthStencilState(DepthStencilStates[SceneDepthStencilMode], 0);

            //
            // Render the mesh
            //
            Device.IA_SetInputLayout(SceneLayout);

            var VertexBuffer = Mesh.GetVertexBuffer(0, 0);
            var Strides = Mesh.GetVertexStride(0, 0);
            Device.IA_SetVertexBuffers(0, 1, new[] { VertexBuffer }, new[] { Strides }, new uint[] { 0 });
            Device.IA_SetIndexBuffer(Mesh.GetIndexBuffer(0), Mesh.GetIndexBufferFormat(0), 0);

            TechniqueDescription TechniqueDescription;
            TechniqueScene.GetDescription(out TechniqueDescription);

            for (uint P = 0; P < TechniqueDescription.Passes; P++)
            {
                for (uint S = 0; S < Mesh.GetNumberOfSubsets(0); S++)
                {
                    uint SubsetIndex;
                    Mesh.MeshPairArray[0].Subsets.Get(S, out SubsetIndex);

                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    var Size = Marshal.SizeOf(typeof(SDK_Mesh.Subset));
                    var Subset = (SDK_Mesh.Subset)Marshal.PtrToStructure(new IntPtr(Mesh.SubsetArray.Pointer.ToInt64() + SubsetIndex * Size), typeof(SDK_Mesh.Subset));

                    var PrimitiveType = SDK_Mesh.GetPrimitiveType((PrimitiveType)Subset.PrimitiveType);
                    Device.IA_SetPrimitiveTopology(PrimitiveType);

                    var DiffuseResourceView = Mesh.MaterialPairArray[Subset.MaterialID].DiffuseResourceView;
                    DiffuseVariable.SetResource(DiffuseResourceView);

                    TechniqueScene.GetPassByIndex(P).Apply(0);
                    Device.DrawIndexed((uint)Subset.IndexCount, 0, (int)Subset.VertexStart);
                }
            }

            //the mesh class also had a render method that allows rendering the mesh with the most common options
            //g_Mesh.Render( pd3dDevice, g_pTechnique, g_ptxDiffuseVariable );

            //
            // Reset the world transform
            //
            Matrix WorldMatrix;
            D3DX10Functions.MatrixScaling(out WorldMatrix, 150.0f, 150.0f, 1.0f);
            WorldVariable.SetMatrix(((float[])WorldMatrix));

            //
            // Render the screen space quad
            //
            var Technique = TechniqueQuad[QuadRenderMode];
            DiffuseVariable.SetResource(ScreenRecourceView[0]);
            var Stride = (uint)Marshal.SizeOf(typeof(ScreenVertex));
            uint Offset = 0;
            var Buffers = new[] { ScreenQuadVertexBuffer };

            Device.IA_SetInputLayout(QuadLayout);
            Device.IA_SetVertexBuffers(0, 1, Buffers, new[] { Stride }, new[] { Offset });
            Device.IA_SetPrimitiveTopology(PrimitiveTopology.TriangleStrip);

            Technique.GetDescription(out TechniqueDescription);

            for (uint Pass = 0; Pass < TechniqueDescription.Passes; Pass++)
            {
                Technique.GetPassByIndex(Pass).Apply(0);

                Device.Draw(4, 0);
            }

            //
            // Render the screen space quad again, but this time with a different texture
            //  and only render where the stencil buffer is != 0
            //  Look at the FX file for the state settings
            //
            Technique = TechniqueRenderWithStencil;
            DiffuseVariable.SetResource(ScreenRecourceView[1]);
            Device.IA_SetInputLayout(QuadLayout);
            Device.IA_SetVertexBuffers(0, 1, Buffers, new[] { Stride }, new[] { Offset });
            Device.IA_SetPrimitiveTopology(PrimitiveTopology.TriangleStrip);

            Technique.GetDescription(out TechniqueDescription);
            for (uint Pass = 0; Pass < TechniqueDescription.Passes; Pass++)
            {
                Technique.GetPassByIndex(Pass).Apply(0);
                Device.Draw(4, 0);
            }

            //
            // Reset our Cull Mode (non-FX method)
            //
            Device.RS_SetState(RasterStates[0]);

            //
            // Reset the Depth Stencil State
            //
            Device.OM_SetDepthStencilState(DepthStencilStates[1], 0);

            //
            // Render the UI
            //
            HUD.OnRender(ElapsedTime);
            SampleUI.OnRender(ElapsedTime);

            RenderText();
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

            if (Spinning) D3DX10Functions.MatrixRotationY(out World, (float)(60.0f * DegreeToRadian(Time)));
            else D3DX10Functions.MatrixRotationY(out World, (float)DegreeToRadian(180.0f));

            Matrix Rotation;
            D3DX10Functions.MatrixRotationX(out Rotation, (float)DegreeToRadian(-90.0f));
            World = Rotation * World;
        }

        bool OnModifyDeviceSettings(DeviceSettings DeviceSettings, object UserContext)
        {
            DeviceSettings.AutoDepthStencilFormat = Format.D24_UNorm_S8_UInt;

            return true;
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
            case (int)Form1.ControlID.ToggleSpin:
                {
                    Spinning = SampleUI.GetCheckBox((int)Form1.ControlID.ToggleSpin).Checked;
                    break;
                }

            case (int)Form1.ControlID.QuadRenderMode:
                {
                    var ComboBox = (UtilitiesComboBox)Control;
                    QuadRenderMode = Convert.ToUInt32(ComboBox.GetSelectedData());
                    break;
                }
            case (int)Form1.ControlID.SceneDepthStencilMode:
                {
                    var ComboBox = (UtilitiesComboBox)Control;
                    SceneDepthStencilMode = Convert.ToUInt32(ComboBox.GetSelectedData());
                    break;
                }

            case (int)Form1.ControlID.ScreenRasterizerMode:
                {
                    var ComboBox = (UtilitiesComboBox)Control;
                    SceneRasterizerMode = Convert.ToUInt32(ComboBox.GetSelectedData());
                    break;
                }
            }
        }

        //--------------------------------------------------------------------------------------
        // LoadQuadTechniques
        // Load the techniques for rendering the quad from the FX file.  The techniques in the
        // FX file contain the alpha blending state setup.
        //--------------------------------------------------------------------------------------
        void LoadQuadTechniques()
        {
            for (var I = 0; I < QuadTechniques.Length; I++)
            {
                TechniqueQuad[I] = Effect.GetTechniqueByName(QuadTechniques[I]);

                SampleUI.GetComboBox((int)ControlID.QuadRenderMode).AddItem(QuadTechniques[I], I.ToString());
            }
        }

        //--------------------------------------------------------------------------------------
        // LoadDepthStencilStates
        // Create a set of depth stencil states for non-FX state managment.  These states
        // will later be set using OMSetDepthStencilState in OnD3D10FrameRender.
        //--------------------------------------------------------------------------------------
        void LoadDepthStencilStates(Device Device)
        {
            var DepthEnable = new[]
    {
        false,
        true,
        true,
        false,
        true,
        true,
        false,
        true,
        true
    };

            var StencilEnable = new[]
    {
        false,
        false,
        false,
        true,
        true,
        true,
        true,
        true,
        true
    };

            var ComparisonFunctions = new[]
    {
        ComparisonFunction.Less,
        ComparisonFunction.Less,
        ComparisonFunction.Greater,
        ComparisonFunction.Less,
        ComparisonFunction.Less,
        ComparisonFunction.Greater,
        ComparisonFunction.Less,
        ComparisonFunction.Less,
        ComparisonFunction.Greater,
    };

            var FailOperation = new[] 
    {
        StencilOperation.Keep,
        StencilOperation.Keep,
        StencilOperation.Keep,

        StencilOperation.Increment,
        StencilOperation.Increment,
        StencilOperation.Increment,

        StencilOperation.Keep,
        StencilOperation.Keep,
        StencilOperation.Keep,
    };

            var PassOperation = new[]
    {
        StencilOperation.Keep,
        StencilOperation.Keep,
        StencilOperation.Keep,

        StencilOperation.Keep,
        StencilOperation.Keep,
        StencilOperation.Keep,

        StencilOperation.Increment,
        StencilOperation.Increment,
        StencilOperation.Increment,
    };

            for (var I = 0; I < DepthStencilModes.Length; I++)
            {
                var DepthStencilDescription = new DepthStencilDescription
                {
                    DepthEnable = DepthEnable[I],
                    DepthWriteMask = DepthWriteMask.All,
                    DepthFunction = ComparisonFunctions[I],

                    // Stencil test parameters
                    StencilEnable = StencilEnable[I],
                    StencilReadMask = 0xFF,
                    StencilWriteMask = 0xFF,

                    // Stencil operations if pixel is front-facing
                    FrontFace = new DepthStencilOperationDescription
                    {
                        StencilFailOperation = StencilOperation.Keep,
                        StencilDepthFailOperation = FailOperation[I],
                        StencilPassOperation = PassOperation[I],
                        StencilFunction = ComparisonFunction.Always
                    },

                    // Stencil operations if pixel is back-facing
                    BackFace = new DepthStencilOperationDescription
                    {
                        StencilFailOperation = StencilOperation.Keep,
                        StencilDepthFailOperation = FailOperation[I],
                        StencilPassOperation = PassOperation[I],
                        StencilFunction = ComparisonFunction.Always
                    }
                };

                // Create depth stencil state
                Device.CreateDepthStencilState(ref DepthStencilDescription, out DepthStencilStates[I]);

                SampleUI.GetComboBox((int)ControlID.SceneDepthStencilMode).AddItem(DepthStencilModes[I], I.ToString());
            }
        }

        //--------------------------------------------------------------------------------------
        // LoadRasterizerStates
        // Create a set of rasterizer states for non-FX state managment.  These states
        // will later be set using RSSetState in OnD3D10FrameRender.
        //--------------------------------------------------------------------------------------
        void LoadRasterizerStates(Device Device)
        {
            var Fill = new[]
    {
        FillMode.Solid,
        FillMode.Solid,
        FillMode.Solid,
        FillMode.Wireframe,
        FillMode.Wireframe,
        FillMode.Wireframe
    };
            var Cull = new[]
    {
        CullMode.None,
        CullMode.Front,
        CullMode.Back,
        CullMode.None,
        CullMode.Front,
        CullMode.Back
    };

            for (var I = 0; I < RasterizerModes.Length; I++)
            {
                var RasterizerDescription = new RasterizerDescription
                    {
                        FillMode = Fill[I],
                        CullMode = Cull[I],
                        FrontCounterClockwise = false,
                        DepthBias = 0,
                        DepthBiasClamp = 0,
                        SlopeScaledDepthBias = 0,
                        DepthClipEnable = true,
                        ScissorEnable = false,
                        MultisampleEnable = false,
                        AntialiasedLineEnable = false
                    };
                Device.CreateRasterizerState(ref RasterizerDescription, out RasterStates[I]);

                SampleUI.GetComboBox((int)ControlID.ScreenRasterizerMode).AddItem(RasterizerModes[I], I.ToString());
            }
        }
    }
}