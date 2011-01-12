using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Xtro.MDX;
using Xtro.MDX.DXGI;
using Usage = Xtro.MDX.DXGI.Usage;
using Xtro.MDX.Direct3D10;
using D3D10Usage = Xtro.MDX.Direct3D10.Usage;
using Device = Xtro.MDX.Direct3D10.Device;
using Functions = Xtro.MDX.Direct3D10.Functions;
using Buffer = Xtro.MDX.Direct3D10.Buffer;
using Error = Xtro.MDX.Direct3D10.Error;
using Xtro.MDX.Direct3DX10;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
       
namespace Tutorial04
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        static extern uint GetTickCount();

        struct SimpleVertex
        {
            public Vector3 Position;
            public Vector4 Color;

            public SimpleVertex(Vector3 Position, Vector4 Color)
            {
                this.Position = Position;
                this.Color = Color;
            }
        };

        DriverType DriverType = DriverType.Null;
        Device Device = null;
        SwapChain SwapChain = null;
        RenderTargetView RenderTargetView = null;
        Effect Effect = null;
        EffectTechnique Technique = null;
        TechniqueDescription TechniqueDescription;
        InputLayout VertexLayout = null;
        Buffer VertexBuffer = null;
        Buffer IndexBuffer = null;
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

        private void Form1_Shown(object sender, EventArgs e)
        {
            if (InitDevice()) Application.Idle += Application_Idle;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            CleanupDevice();
        }

        void Application_Idle(object Sender, EventArgs E)
        {
            try
            {
                try { Render(); }
                finally { PostMessage(Handle, 1024/*WM_USER*/, 0, 0); }
            }
            // if Idle event throws an exception, Application can't fire Application_Exception event. So we do it.
            catch (Exception Ex) { Application_Exception(Sender, new ThreadExceptionEventArgs(Ex)); }
        }

        void Application_Exception(object Sender, ThreadExceptionEventArgs E)
        {
            MessageBox.Show(E.Exception.ToString());
            Application.Exit();
        }

        bool InitDevice()
        {
            CreateDeviceFlag CreateDeviceFlags = 0;
#if DEBUG
            CreateDeviceFlags |= CreateDeviceFlag.Debug;
#endif

            DriverType[] DriverTypes = { DriverType.Hardware, DriverType.Reference };

            SwapChainDescription SwapChainDescription = new SwapChainDescription();
            SwapChainDescription.BufferCount = 1;
            SwapChainDescription.BufferDescription.Width = (uint)ClientSize.Width;
            SwapChainDescription.BufferDescription.Height = (uint)ClientSize.Height;
            SwapChainDescription.BufferDescription.Format = Format.R8G8B8A8_UNorm;
            SwapChainDescription.BufferDescription.RefreshRate.Numerator = 60;
            SwapChainDescription.BufferDescription.RefreshRate.Denominator = 1;
            SwapChainDescription.BufferUsage = Usage.RenderTargetOutput;
            SwapChainDescription.OutputWindow = Handle;
            SwapChainDescription.SampleDescription.Count = 1;
            SwapChainDescription.SampleDescription.Quality = 0;
            SwapChainDescription.Windowed = true;

            int Result = 0;

            for (int DriverTypeIndex = 0; DriverTypeIndex < DriverTypes.Length; DriverTypeIndex++)
            {
                Result = Functions.CreateDeviceAndSwapChain(null, DriverTypes[DriverTypeIndex], null, CreateDeviceFlags, ref SwapChainDescription, out SwapChain, out Device);
                if (Result >= 0) break;
            }

            if (Result < 0) throw new Exception("No device could be created : " + Result);

            // Create a render target view

            Unknown Surface;
            Result = SwapChain.GetBuffer(0, typeof(Texture2D), out Surface);
            if (Result < 0) throw new Exception("SwapChain.GetBuffer has failed : " + Result);

            Texture2D BackBuffer;
            BackBuffer = (Texture2D)Surface;

            Result = Device.CreateRenderTargetView(BackBuffer, out RenderTargetView);
            if (Result < 0) throw new Exception("Device.CreateRenderTargetView has failed : " + Result);

            if (BackBuffer != null) BackBuffer.Release();

            Device.OM_SetRenderTargets(1,new [] { RenderTargetView }, null);

            // Setup the viewport
            Viewport Viewport = new Viewport()
            {
                TopLeftX = 0,
                TopLeftY = 0,
                Width = (uint)ClientSize.Width,
                Height = (uint)ClientSize.Height,
                MinDepth = 0.0f,
                MaxDepth = 1.0f
            };
            Device.RS_SetViewports(1, new[] { Viewport });

            // Create the effect

            ShaderFlag ShaderFlags = ShaderFlag.EnableStrictness;
#if DEBUG
            // Set the ShaderFlag.Debug flag to embed debug information in the shaders.
            // Setting this flag improves the shader debugging experience, but still allows 
            // the shaders to be optimized and to run exactly the way they will run in 
            // the release configuration of this program.
            ShaderFlags |= ShaderFlag.Debug;
#endif

            Blob Errors = null;
            try
            {
                Result = D3DX10Functions.CreateEffectFromFile("Tutorial04.fx", null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect, out Errors);
                if (Result == (int)Error.FileNotFound)
                {
                    MessageBox.Show("The FX file cannot be located.  Please run this executable from the directory that contains the FX file.", "Error", MessageBoxButtons.OK);
                    return false;
                }
                else if (Result < 0) throw new Exception("D3DX10Functions.CreateEffectFromFile has failed : " + Result);
            }
            finally
            {
                if (Errors != null) Errors.Release();
            }

            // Obtain the technique
            Technique = Effect.GetTechniqueByName("Render");
            Result = Technique.GetDescription(out TechniqueDescription);
            if (Result < 0) throw new Exception("GetDescription has failed : " + Result);

            // Obtain the variables
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
                    SemanticName = "COLOR",
                    SemanticIndex = 0,
                    Format = Format.R32G32B32A32_Float,
                    InputSlot = 0,
                    AlignedByteOffset = 12,
                    InputSlotClass = InputClassification.InputPerVertexData,
                    InstanceDataStepRate = 0
                }
            };

            // Create the input layout
            PassDescription PassDescription;
            Result = Technique.GetPassByIndex(0).GetDescription(out PassDescription);
            if (Result < 0) throw new Exception("GetDescription has failed : " + Result);
            Result = Device.CreateInputLayout(Layout, PassDescription.IAInputSignature, out VertexLayout);
            if (Result < 0) throw new Exception("Device.CreateInputLayout has failed : " + Result);

            // Set the input layout
            Device.IA_SetInputLayout(VertexLayout);

            SubResourceData InitData;
            BufferDescription BufferDescription;

            // Create vertex buffer

            var VertexCount = (uint)8;
            int VertexSize = Marshal.SizeOf(typeof(SimpleVertex));
            UnmanagedMemory Vertices = new UnmanagedMemory((uint)(VertexSize * VertexCount));
            Vertices.Write(0, VertexCount, new SimpleVertex[]
            {
                new SimpleVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector4(0.0f, 0.0f, 1.0f, 1.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector4(0.0f, 1.0f, 0.0f, 1.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector4(0.0f, 1.0f, 1.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector4(1.0f, 0.0f, 0.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector4(1.0f, 0.0f, 1.0f, 1.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector4(1.0f, 1.0f, 0.0f, 1.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector4(1.0f, 1.0f, 1.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector4(0.0f, 0.0f, 0.0f, 1.0f))
            });
            InitData = new SubResourceData
            {
                SystemMemory = Vertices,
                SystemMemoryPitch = 0,
                SystemMemorySlicePitch = 0
            };
            BufferDescription = new BufferDescription
            {
                ByteWidth = (uint)Vertices.Size,
                Usage = D3D10Usage.Default,
                BindFlags = BindFlag.VertexBuffer,
                CPU_AccessFlags = 0,
                MiscFlags = 0
            };

            Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out VertexBuffer);
            if (Result < 0) throw new Exception("Device.CreateBuffer has failed : " + Result);

            // Set vertex buffer
            Device.IA_SetVertexBuffers(0,1, new [] { VertexBuffer }, new uint[] { (uint)VertexSize }, new uint[] { 0 });

            // Create index buffer

            var IndexCount = (uint)36;
            UnmanagedMemory Indices = new UnmanagedMemory((uint)(Marshal.SizeOf(typeof(int)) * IndexCount));
            Indices.Write(0, IndexCount, new int[] 
            {
                3, 1, 0,
                2, 1, 3,
                0, 5, 4,
                1, 5, 0,
                3, 4, 7,
                0, 4, 3,
                1, 6, 5,
                2, 6, 1,
                2, 7, 6,
                3, 7, 2,
                6, 4, 5,
                7, 4, 6
            });
            InitData = new SubResourceData
            {
                SystemMemory = Indices,
                SystemMemoryPitch = 0,
                SystemMemorySlicePitch = 0
            };
            BufferDescription = new BufferDescription
            {
                ByteWidth = (uint)Indices.Size,
                Usage = D3D10Usage.Default,
                BindFlags = BindFlag.IndexBuffer,
                CPU_AccessFlags = 0,
                MiscFlags = 0
            };

            Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out IndexBuffer);
            if (Result < 0) throw new Exception("Device.CreateBuffer has failed : " + Result);

            // Set index buffer
            Device.IA_SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            // Set primitive topology
            Device.IA_SetPrimitiveTopology(PrimitiveTopology.TriangleList);

            // Initialize the world matrix
            D3DX10Functions.MatrixIdentity(out World);

            // Initialize the view matrix
            Vector3 Eye = new Vector3(0.0f, 1.0f, -5.0f);
            Vector3 At = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
            D3DX10Functions.MatrixLookAtLH(out View, ref Eye, ref At, ref Up);

            // Initialize the projection matrix
            float FovY = (float)Math.PI * 0.5f;
            D3DX10Functions.MatrixPerspectiveFovLH(out Projection, FovY, ClientSize.Width / (float)ClientSize.Height, 0.1f, 100.0f);

            return true;
        }

        float Time = 0.0f;
        uint TimeStart = 0;

        void Render()
        {
            // Update our time
            uint TimeCurrent = GetTickCount();
            if (TimeStart == 0) TimeStart = TimeCurrent;
            Time = (TimeCurrent - TimeStart) / 1000.0f;

            // Animate the cube
            D3DX10Functions.MatrixRotationY(out World, Time);

            // Clear the backbuffer
            Float4 ClearColor = new Float4(0.0f, 0.125f, 0.3f, 1.0f); //red,green,blue,alpha
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);

            int Result = 0;

            // Update variables
            Result = WorldVariable.SetMatrix((float[])World);
            if (Result < 0) throw new Exception("WorldVariable.SetMatrix has failed : " + Result);
            Result = ViewVariable.SetMatrix((float[])View);
            if (Result < 0) throw new Exception("ViewVariable.SetMatrix has failed : " + Result);
            Result = ProjectionVariable.SetMatrix((float[])Projection);
            if (Result < 0) throw new Exception("ProjectionVariable.SetMatrix has failed : " + Result);

            // Render a cube
            for (uint PassNo = 0; PassNo < TechniqueDescription.Passes; ++PassNo)
            {
                Technique.GetPassByIndex(PassNo).Apply(0);
                Device.DrawIndexed(36, 0, 0);
            }

            // Present our back buffer to our front buffer
            SwapChain.Present(0, 0);
        }

        void CleanupDevice()
        {
            if (Device != null) Device.ClearState();

            if (VertexBuffer != null) VertexBuffer.Release();
            if (IndexBuffer != null) IndexBuffer.Release();
            if (VertexLayout != null) VertexLayout.Release();
            if (Effect != null) Effect.Release();
            if (RenderTargetView != null) RenderTargetView.Release();
            if (SwapChain != null) SwapChain.Release();
            if (Device != null) Device.Release();
        }
    }
}
