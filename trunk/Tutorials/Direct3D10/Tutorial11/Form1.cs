// ReSharper disable CSharpWarnings::CS0197
using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Xtro.MDX;
using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;
using Device = Xtro.MDX.Direct3D10.Device;
using Xtro.MDX.Direct3DX10;
using D3DX10Constants = Xtro.MDX.Direct3DX10.Constants;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Xtro.MDX.Utilities;
using Button = Xtro.MDX.Utilities.Button;
using CheckBox = Xtro.MDX.Utilities.CheckBox;
using Color = Xtro.MDX.Direct3DX10.Color;
using Control = Xtro.MDX.Utilities.Control;
using Font = Xtro.MDX.Direct3DX10.Font;
using UtilitiesFunctions = Xtro.MDX.Utilities.Functions;

namespace Tutorial11
{
    sealed partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity]
        static extern IntPtr PostMessage(IntPtr Wnd, int Msg, int ParamW, int ParamL);

        enum ControlID
        {
            ToggleFullscreen = 1,
            ToggleREF = 2,
            ChangeDevice = 3,
            ToggleSpin = 4,
            PuffScale = 5,
            PuffStatic = 6
        }

        readonly ModelViewerCamera Camera;
        readonly DialogResourceManager DialogResourceManager = new DialogResourceManager();// manager for shared resources of dialogs
        readonly SettingsDialog SettingsDialog = new SettingsDialog();       // Device settings dialog
        readonly Dialog HUD = new Dialog();                  // manages the 3D UI
        readonly Dialog SampleUI = new Dialog();             // dialog for sample specific controls

        Matrix World;
        float ModelWaviness;
        bool Spinning = true;
        Font Font2;         // Font for drawing text
        Sprite Sprite;       // Sprite for batching text drawing
        TextHelper TextHelper;

        Effect Effect;
        InputLayout VertexLayout;
        EffectTechnique Technique;
        readonly SDK_Mesh Mesh = new SDK_Mesh();
        EffectShaderResourceVariable DiffuseVariable;
        EffectMatrixVariable WorldVariable;
        EffectMatrixVariable ViewVariable;
        EffectMatrixVariable ProjectionVariable;
        EffectScalarVariable WavinessVariable;
        EffectScalarVariable TimeVariable;

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
                UtilitiesFunctions.SetCallbackDeviceChanging(OnModifyDeviceSettings, null);

                UtilitiesFunctions.Initialize();
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
            ModelWaviness = 0.0f;
            Spinning = true;

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
            Y = 10;

            Y += 24;
            var S = string.Format("Waviness: {0:0.00}", ModelWaviness);
            Static CreatedStatic;
            SampleUI.AddStatic((int)ControlID.PuffStatic, S, 35, Y += 24, 125, 22, false, out CreatedStatic);
            Slider CreatedSlider;
            SampleUI.AddSlider((int)ControlID.PuffScale, 50, Y += 24, 100, 22, 0, 2000, (int)(ModelWaviness * 100.0f), false, out CreatedSlider);

            Y += 24;
            CheckBox CreatedCheckBox;
            SampleUI.AddCheckBox((int)ControlID.ToggleSpin, "Toggle Spinning", 35, Y + 24, 125, 22, Spinning, 0, false, out CreatedCheckBox);
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

        int OnDeviceCreated(Device Device, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            var Result = DialogResourceManager.OnCreateDevice(Device);
            if (Result < 0) return Result;
            Result = SettingsDialog.OnCreateDevice(Device);
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateFont(Device, 15, 0, (uint)FontWeight.Bold, 1, false, FontCharacterSet.Default, FontPrecision.Default, FontQuality.Default, FontPitchAndFamily.Default | FontPitchAndFamily.DontCare, "Arial", out Font2);
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateSprite(Device, 512, out Sprite);
            if (Result < 0) return Result;
            TextHelper = new TextHelper(Font2, Sprite);

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
            Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "Tutorial11.fx");
            if (Result < 0) return Result;
            Result = D3DX10Functions.CreateEffectFromFile(DestinationPath, null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect);
            if (Result < 0) return Result;

            Technique = Effect.GetTechniqueByName("Render");
            DiffuseVariable = Effect.GetVariableByName("g_txDiffuse").AsShaderResource();
            WorldVariable = Effect.GetVariableByName("World").AsMatrix();
            ViewVariable = Effect.GetVariableByName("View").AsMatrix();
            ProjectionVariable = Effect.GetVariableByName("Projection").AsMatrix();
            WavinessVariable = Effect.GetVariableByName("Waviness").AsScalar();
            TimeVariable = Effect.GetVariableByName("Time").AsScalar();

            // Set Waviness
            WavinessVariable.SetFloat(ModelWaviness);

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
            Technique.GetPassByIndex(0).GetDescription(out PassDescription);
            Result = Device.CreateInputLayout(Layout, (uint)Layout.Length, PassDescription.IA_InputSignature, PassDescription.IA_InputSignature.Size, out VertexLayout);
            if (Result < 0) return Result;

            // Set the input layout
            Device.IA_SetInputLayout(VertexLayout);

            // Load the mesh
            Result = Mesh.Create(Device, "Tiny\\tiny.sdkmesh", true);
            if (Result < 0) return Result;

            // Initialize the world matrices
            D3DX10Functions.MatrixIdentity(out World);

            // Initialize the camera
            var Eye = new Vector3(0.0f, 0.0f, -800.0f);
            var At = new Vector3(0.0f, 1.0f, 0.0f);
            Camera.SetViewParameters(ref Eye, ref At);

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
            if (VertexLayout != null) VertexLayout.Release();
            if (Effect != null) Effect.Release();
            Mesh.Delete();

            Font2 = null;
            Sprite = null;
            TextHelper = null;
            VertexLayout = null;
            Effect = null;
        }

        int OnSwapChainResized(Device Device, SwapChain SwapChain, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            var Result = DialogResourceManager.OnResizedSwapChain(Device, ref BackBufferSurfaceDescription);
            if (Result < 0) return Result;
            Result = SettingsDialog.OnResizedSwapChain(/*Device,*/ ref BackBufferSurfaceDescription);
            if (Result < 0) return Result;

            // Setup the camera's projection parameters
            var AspectRatio = (float)BackBufferSurfaceDescription.Width / BackBufferSurfaceDescription.Height;
            Camera.SetProjectionParameters((float)(D3DX10Constants.PI / 4), AspectRatio, 0.1f, 5000.0f);
            Camera.SetWindow((int)BackBufferSurfaceDescription.Width, (int)BackBufferSurfaceDescription.Height);
            Camera.SetButtonMasks(MouseKeys.Middle, MouseKeys.Wheel, MouseKeys.Left);

            HUD.SetLocation((int)BackBufferSurfaceDescription.Width - 170, 0);
            HUD.SetSize(170, 170);
            SampleUI.SetLocation((int)BackBufferSurfaceDescription.Width - 170, (int)BackBufferSurfaceDescription.Height - 300);
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
            Device.ClearDepthStencilView(DepthStencilView, ClearFlag.Depth, 1.0f, 0);

            //
            // Update variables that change once per frame
            //
            ProjectionVariable.SetMatrix((float[])Camera.GetProjectionMatrix());
            ViewVariable.SetMatrix((float[])Camera.GetViewMatrix());
            WorldVariable.SetMatrix((float[])World);
            TimeVariable.SetFloat((float)Time);

            //
            // Set the Vertex Layout
            //
            Device.IA_SetInputLayout(VertexLayout);

            //
            // Render the mesh
            //
            var VertexBuffer = Mesh.GetVertexBuffer(0, 0);
            var Strides = Mesh.GetVertexStride(0, 0);
            Device.IA_SetVertexBuffers(0, 1, new[] { VertexBuffer }, new[] { Strides }, new uint[] { 0 });
            Device.IA_SetIndexBuffer(Mesh.GetIndexBuffer(0), Mesh.GetIndexBufferFormat(0), 0);

            TechniqueDescription TechniqueDescription;
            Technique.GetDescription(out TechniqueDescription);

            for (uint P = 0; P < TechniqueDescription.Passes; P++)
            {
                for (uint S = 0; S < Mesh.GetNumberOfSubsets(0); S++)
                {
                    uint SubsetIndex;
                    Mesh.MeshPairArray[0].Subsets.Get(S, out SubsetIndex);

                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    var SizeOfSubset = Marshal.SizeOf(typeof(SDK_Mesh.Subset));
                    var Subset = (SDK_Mesh.Subset)Marshal.PtrToStructure(new IntPtr(Mesh.SubsetArray.Pointer.ToInt64() + SubsetIndex * SizeOfSubset), typeof(SDK_Mesh.Subset));

                    var PrimitiveType = SDK_Mesh.GetPrimitiveType((PrimitiveType)Subset.PrimitiveType);
                    Device.IA_SetPrimitiveTopology(PrimitiveType);

                    var DiffuseResourceView = Mesh.MaterialPairArray[Subset.MaterialID].DiffuseResourceView;
                    DiffuseVariable.SetResource(DiffuseResourceView);

                    Technique.GetPassByIndex(P).Apply(0);
                    Device.DrawIndexed((uint)Subset.IndexCount, 0, (int)Subset.VertexStart);
                }
            }

            //the mesh class also had a render method that allows rendering the mesh with the most common options
            //g_Mesh.Render( pd3dDevice, g_pTechnique, g_ptxDiffuseVariable );

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

            if (Spinning) D3DX10Functions.MatrixRotationY(out World, (float)(60.0f * D3DX10Functions.ToRadian(Time)));
            else D3DX10Functions.MatrixRotationY(out World, (float)D3DX10Functions.ToRadian(180.0f));

            Matrix Rotation;
            D3DX10Functions.MatrixRotationX(out Rotation, (float)D3DX10Functions.ToRadian(-90.0f));
            World = Rotation * World;
        }

        static bool OnModifyDeviceSettings(DeviceSettings DeviceSettings, object UserContext)
        {
            return true;
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

            case (int)Form1.ControlID.PuffScale:
                {
                    ModelWaviness = SampleUI.GetSlider((int)Form1.ControlID.PuffScale).GetValue() * 0.01f;
                    var S = string.Format("Waviness: {0:0.00}", ModelWaviness);
                    SampleUI.GetStatic((int)Form1.ControlID.PuffStatic).Text = S;
                    WavinessVariable.SetFloat(ModelWaviness);
                    break;
                }
            }
        }
    }
}
// ReSharper restore CSharpWarnings::CS0197