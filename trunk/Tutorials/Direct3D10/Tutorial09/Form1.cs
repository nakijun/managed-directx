// ReSharper disable CSharpWarnings::CS0197
using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Xtro.MDX;
using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Generic;
using Device = Xtro.MDX.Direct3D10.Device;
using Xtro.MDX.Direct3DX10;
using D3DX10Constants = Xtro.MDX.Direct3DX10.Constants;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Xtro.MDX.Utilities;
using UtilitiesFunctions = Xtro.MDX.Utilities.Functions;

namespace Tutorial09
{
    sealed partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity]
        static extern IntPtr PostMessage(IntPtr Wnd, int Msg, int ParamW, int ParamL);

        Effect Effect;
        InputLayout VertexLayout;
        EffectTechnique Technique;
        readonly SDK_Mesh Mesh = new SDK_Mesh();
        EffectShaderResourceVariable DiffuseVariable;
        EffectMatrixVariable WorldVariable;
        EffectMatrixVariable ViewVariable;
        EffectMatrixVariable ProjectionVariable;
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
                UtilitiesFunctions.SetCallbackDeviceChanging(OnModifyDeviceSettings, null);

                UtilitiesFunctions.Initialize();
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
            UtilitiesFunctions.MediaSearchPath = @"C:\Program Files (x86)\Microsoft DirectX SDK (June 2010)\Samples\Media\";

            // Find the D3DX effect file
            string DestinationPath;
            var Result = UtilitiesFunctions.FindSDK_MediaFileCch(out DestinationPath, "Tutorial09.fx");
            if (Result < 0) return Result;
#if DEBUG
            // Set the ShaderFlag.Debug flag to embed debug information in the shaders.
            // Setting this flag improves the shader debugging experience, but still allows 
            // the shaders to be optimized and to run exactly the way they will run in 
            // the release configuration of this program.
            const ShaderFlag ShaderFlags = ShaderFlag.EnableStrictness | ShaderFlag.Debug;
#else
            const ShaderFlag ShaderFlags = ShaderFlag.EnableStrictness;
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
            Result = Device.CreateInputLayout(Layout, (uint)Layout.Length, PassDescription.IA_InputSignature, PassDescription.IA_InputSignature.Size, out VertexLayout);
            if (Result < 0) return Result;

            // Set the input layout
            Device.IA_SetInputLayout(VertexLayout);

            // Load the mesh
            Result = Mesh.Create(Device, "Tiny\\tiny.sdkmesh", true);
            if (Result < 0) return Result;

            // Initialize the world matrices
            D3DX10Functions.MatrixIdentity(out World);

            // Initialize the view matrix
            var Eye = new Vector3(0.0f, 3.0f, -500.0f);
            var At = new Vector3(0.0f, 1.0f, 0.0f);
            var Up = new Vector3(0.0f, 1.0f, 0.0f);
            D3DX10Functions.MatrixLookAtLH(out View, ref Eye, ref At, ref Up);

            // Update Variables that never change
            var Data = new UnmanagedMemory<float>((uint)Marshal.SizeOf(typeof(Matrix)));
            Data.Set(0, ref View);
            ViewVariable.SetMatrix(Data);

            return 0;
        }

        void OnDeviceDestroyed(object UserContext)
        {
            UtilitiesFunctions.GetGlobalResourceCache().OnDestroyDevice();
            if (VertexLayout != null) VertexLayout.Release();
            if (Effect != null) Effect.Release();
            Mesh.Delete();

            VertexLayout = null;
            Effect = null;
        }

        int OnSwapChainResized(Device Device, SwapChain SwapChain, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            // Setup the projection parameters again
            var Aspect = (float)BackBufferSurfaceDescription.Width / BackBufferSurfaceDescription.Height;
            D3DX10Functions.MatrixPerspectiveFovLH(out Projection, (float)D3DX10Constants.PI * 0.25f, Aspect, 0.5f, 1000.0f);
            var Data = new UnmanagedMemory<float>((uint)Marshal.SizeOf(typeof(Matrix)));
            Data.Set(0, ref Projection);
            ProjectionVariable.SetMatrix(Data);

            return 0;
        }

        static void OnReleasingSwapChain(object UserContext)
        {
        }

        void OnFrameRender(Device Device, double Time, float ElapsedTime, object UserContext)
        {
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
            var Data = new UnmanagedMemory<float>((uint)Marshal.SizeOf(typeof(Matrix)));
            Data.Set(0, ref World);
            WorldVariable.SetMatrix(Data);

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
        }

        void OnFrameMove(double Time, float ElapsedTime, object UserContext)
        {
            // Rotate cube around the origin
            D3DX10Functions.MatrixRotationY(out World, (float)(60 * D3DX10Functions.ToRadian(Time)));
        }

        static bool OnModifyDeviceSettings(DeviceSettings DeviceSettings, object UserContext)
        {
            return true;
        }

        private void Form1_Paint(object Sender, PaintEventArgs E)
        {
            UtilitiesFunctions.HandlePaintEvent();
        }

        private void Form1_Resize(object Sender, EventArgs E)
        {
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

        private void Form1_Activated(object Sender, EventArgs E)
        {
            UtilitiesFunctions.HandleActivatedEvent();
        }

        private void Form1_Deactivate(object Sender, EventArgs E)
        {
            UtilitiesFunctions.HandleDeactivateEvent();
        }

        private void Form1_KeyDown(object Sender, KeyEventArgs E)
        {
            UtilitiesFunctions.HandleKeyDownEvent(E);
        }
    }
}
// ReSharper restore CSharpWarnings::CS0197