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
using Functions = Xtro.MDX.Direct3D10.Functions;
using Buffer = Xtro.MDX.Direct3D10.Buffer;
using Error = Xtro.MDX.Direct3D10.Error;
using Xtro.MDX.Direct3DX10;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Xtro.MDX.Utilities;
using UtilitiesFunctions = Xtro.MDX.Utilities.Functions;

namespace Tutorial10
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        static extern uint GetTickCount();

        static double DegreeToRadian(double Degree)
        {
            return Degree * Math.PI / 180.0f;
        }

        Effect Effect;
        InputLayout VertexLayout;
        EffectTechnique Technique;
        SDK_Mesh Mesh = new SDK_Mesh();
        EffectShaderResourceVariable DiffuseVariable = null;
        EffectMatrixVariable WorldVariable = null;
        EffectMatrixVariable ViewVariable = null;
        EffectMatrixVariable ProjectionVariable = null;
        Matrix World;
        Matrix View;
        Matrix Projection;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object Sender, EventArgs E)
        {
            try
            {
                Application.ThreadException += Application_Exception;

                UtilitiesFunctions.SetCallbackDeviceAcceptable(IsDeviceAcceptable, null);
                UtilitiesFunctions.SetCallbackDeviceCreated(OnDeviceCreated, null);
                UtilitiesFunctions.SetCallbackSwapChainResized(OnSwapChainResized, null);
                UtilitiesFunctions.SetCallbackSwapChainReleasing(OnReleasingSwapChain, null);
                UtilitiesFunctions.SetCallbackDeviceDestroyed(OnDeviceDestroyed, null);
                UtilitiesFunctions.SetCallbackFrameRender(OnFrameRender, null);
                UtilitiesFunctions.SetCallbackFrameMove(OnFrameMove, null);
                UtilitiesFunctions.SetCallbackModifyDeviceSettings(OnModifyDeviceSettings, null);

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

        private void Form1_FormClosing(object Sender, FormClosingEventArgs E)
        {
            try
            {
                // do something

                Application.ThreadException -= Application_Exception;
                Application.Idle -= Application_Idle;
            }
            // if FormClosing event throws an exception, Closing gets canceled. So we call Application_Exception.
            catch (Exception Ex) { Application_Exception(null, new ThreadExceptionEventArgs(Ex)); }
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
            UtilitiesFunctions.MediaSearchPath = @"C:\Program Files (x86)\Microsoft DirectX SDK (August 2009)\Samples\Media\";

            // Find the D3DX effect file
            string DestinationPath;
            var Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "Tutorial10.fx");
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

            Technique = Effect.GetTechniqueByName("Render");
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
            Technique.GetPassByIndex(0).GetDescription(out PassDescription);
            Result = Device.CreateInputLayout(Layout, (uint)Layout.Length, PassDescription.IA_InputSignature, (uint)PassDescription.IA_InputSignature.Size, out VertexLayout);
            if (Result < 0) return Result;

            // Set the input layout
            Device.IA_SetInputLayout(VertexLayout);

            // Load the mesh
            Result = Mesh.Create(Device, "Tiny\\tiny.sdkmesh", true);
            if (Result < 0) return Result;

            // Initialize the world matrices
            D3DX10Functions.MatrixIdentity(out World);

            // Initialize the view matrix
            Vector3 Eye = new Vector3(0.0f, 3.0f, -500.0f);
            Vector3 At = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
            D3DX10Functions.MatrixLookAtLH(out View, ref Eye, ref At, ref Up);

            // Update Variables that never change
            ViewVariable.SetMatrix((float[])View);

            return 0;
        }

        void OnDeviceDestroyed(object UserContext)
        {
            UtilitiesFunctions.GetGlobalResourceCache().OnDestroyDevice();
            if (VertexLayout != null) VertexLayout.Release();
            if (Effect != null) Effect.Release();
            Mesh.Delete();
        }

        int OnSwapChainResized(Device Device, SwapChain SwapChain, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            // Setup the projection parameters again
            var Aspect = (float)BackBufferSurfaceDescription.Width / BackBufferSurfaceDescription.Height;
            D3DX10Functions.MatrixPerspectiveFovLH(out Projection, (float)Math.PI * 0.25f, Aspect, 0.5f, 1000.0f);
            ProjectionVariable.SetMatrix((float[])Projection);

            return 0;
        }

        void OnReleasingSwapChain(object UserContext)
        {
        }

        void OnFrameRender(Device Device, double Time, float ElapsedTime, object UserContext)
        {
            //
            // Clear the back buffer
            //
            Float4 ClearColor = new Float4(0.0f, 0.125f, 0.3f, 1.0f); //red,green,blue,alpha
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
            WorldVariable.SetMatrix((float[])World);

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
                    var Size = Marshal.SizeOf(typeof(SDK_Mesh.Subset));
                    var Subset = (SDK_Mesh.Subset)Marshal.PtrToStructure(new IntPtr(Mesh.SubsetArray.Pointer.ToInt64() + SubsetIndex * Size), typeof(SDK_Mesh.Subset));

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
        }

        void OnFrameMove(double Time, float ElapsedTime, object UserContext)
        {
            // Rotate cube around the origin
            D3DX10Functions.MatrixRotationY(out World, (float)(60 * DegreeToRadian(Time)));
        }

        bool OnModifyDeviceSettings(DeviceSettings DeviceSettings, object UserContext)
        {
            return true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            UtilitiesFunctions.HandlePaintEvent();
        }

        private void Form1_Resize(object sender, EventArgs e)
        {
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

        private void Form1_Activated(object sender, EventArgs e)
        {
            UtilitiesFunctions.HandleActivatedEvent();
        }

        private void Form1_Deactivate(object sender, EventArgs e)
        {
            UtilitiesFunctions.HandleDeactivateEvent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            UtilitiesFunctions.HandleKeyDownEvent(e);
        }
    }
}