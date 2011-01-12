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
using Xtro.MDX.Utilities;
using UtilitiesFunctions = Xtro.MDX.Utilities.Functions;

namespace Tutorial08
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        static extern uint GetTickCount();

        float DegreeToRadian(float Degree)
        {
            return (float)(Degree * Math.PI / 180.0f);
        }

        struct SimpleVertex
        {
            public Vector3 Position;
            public Vector2 Texture;

            public SimpleVertex(Vector3 Position, Vector2 Texture)
            {
                this.Position = Position;
                this.Texture = Texture;
            }
        };

        Effect Effect = null;
        InputLayout VertexLayout = null;
        Buffer VertexBuffer = null;
        Buffer IndexBuffer = null;
        ShaderResourceView TextureResourceView = null;
        EffectMatrixVariable WorldVariable = null;
        EffectMatrixVariable ViewVariable = null;
        EffectMatrixVariable ProjectionVariable = null;
        EffectVectorVariable MeshColorVariable = null;
        EffectShaderResourceVariable DiffuseVariable = null;
        Matrix World;
        Matrix View;
        Matrix Projection;
        Vector4 MeshColor = new Vector4(0.7f, 0.7f, 0.7f, 1.0f);

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            UtilitiesFunctions.SetCallbackDeviceAcceptable(IsDeviceAcceptable, null);
            UtilitiesFunctions.SetCallbackDeviceCreated(OnDeviceCreated, null);
            UtilitiesFunctions.SetCallbackSwapChainResized(OnSwapChainResized, null);
            UtilitiesFunctions.SetCallbackSwapChainReleasing(OnReleasingSwapChain, null);
            UtilitiesFunctions.SetCallbackDeviceDestroyed(OnDeviceDestroyed, null);
            UtilitiesFunctions.SetCallbackFrameRender(OnFrameRender, null);
            UtilitiesFunctions.SetCallbackFrameMove(OnFrameMove, null);
            UtilitiesFunctions.SetCallbackModifyDeviceSettings(OnModifyDeviceSettings, null);

            UtilitiesFunctions.Initialize(true);
            UtilitiesFunctions.CreateDevice(true, 640, 480);

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
            SwapChainDescription.BufferDescription.Width = ClientSize.Width;
            SwapChainDescription.BufferDescription.Height = ClientSize.Height;
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

            Result = Device.CreateRenderTargetView(BackBuffer, null, out RenderTargetView);
            if (Result < 0) throw new Exception("Device.CreateRenderTargetView has failed : " + Result);

            if (BackBuffer != null) BackBuffer.Release();

            Device.OMSetRenderTargets(new RenderTargetView[] { RenderTargetView }, null);

            // Setup the viewport
            Viewport Viewport = new Viewport(0, 0, (uint)ClientSize.Width, (uint)ClientSize.Height, 0.0f, 1.0f);
            Device.RSSetViewports(new Viewport[] { Viewport });

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
                Result = D3DX10Functions.CreateEffectFromFile("Tutorial08.fx", null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect, out Errors);
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

            // Obtain the techniques

            Technique = Effect.GetTechniqueByName("Render");
            Result = Technique.GetDescription(out TechniqueDescription);
            if (Result < 0) throw new Exception("GetDescription has failed : " + Result);

            // Obtain the variables
            WorldVariable = Effect.GetVariableByName("World").AsMatrix();
            ViewVariable = Effect.GetVariableByName("View").AsMatrix();
            ProjectionVariable = Effect.GetVariableByName("Projection").AsMatrix();
            MeshColorVariable = Effect.GetVariableByName("vMeshColor").AsVector();
            DiffuseVariable = Effect.GetVariableByName("txDiffuse").AsShaderResource();

            // Define the input layout
            InputElementDescription[] Layout = 
            { 
                new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.InputPerVertexData, 0),
                new InputElementDescription("TEXCOORD", 0, Format.R32G32_Float, 0, 12, InputClassification.InputPerVertexData, 0)
            };

            // Create the input layout
            PassDescription PassDescription;
            Result = Technique.GetPassByIndex(0).GetDescription(out PassDescription);
            if (Result < 0) throw new Exception("GetDescription has failed : " + Result);
            Result = Device.CreateInputLayout(Layout, PassDescription.IAInputSignature, out VertexLayout);
            if (Result < 0) throw new Exception("Device.CreateInputLayout has failed : " + Result);

            // Set the input layout
            Device.IASetInputLayout(VertexLayout);

            SubResourceData InitData;
            BufferDescription BufferDescription;

            // Create vertex buffer

            int VertexCount = 24;
            int VertexSize = Marshal.SizeOf(typeof(SimpleVertex));
            UnmanagedMemory Vertices = new UnmanagedMemory((uint)(VertexSize * VertexCount));
            Vertices.Write(0, VertexCount, new SimpleVertex[]
            {
                new SimpleVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector2(1.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector2(0.0f, 1.0f)), 
                new SimpleVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector2(1.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector2(0.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector2(0.0f, 0.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(1.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector2(0.0f, 1.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector2(0.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector2(1.0f, 1.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector2(0.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector2(0.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector2(1.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector2(1.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector2(0.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector2(0.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector2(1.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector2(1.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector2(0.0f, 1.0f)) 
            });
            InitData = new SubResourceData(Vertices, 0, 0);
            BufferDescription = new BufferDescription((uint)Vertices.Size, D3D10Usage.Default, BindFlag.VertexBuffer, 0, 0);

            Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out VertexBuffer);
            if (Result < 0) throw new Exception("Device.CreateBuffer has failed : " + Result);

            // Set vertex buffer
            Device.IASetVertexBuffers(0, new Buffer[] { VertexBuffer }, new uint[] { (uint)(BufferDescription.ByteWidth / 24) }, new uint[] { 0 });

            // Create index buffer

            int IndexCount = 36;
            UnmanagedMemory Indices = new UnmanagedMemory((uint)(Marshal.SizeOf(typeof(int)) * IndexCount));
            Indices.Write(0, IndexCount, new int[] 
            {
                3, 1, 0,
                2, 1, 3,
                6, 4, 5,
                7, 4, 6,
                11, 9, 8,
                10, 9, 11,
                14, 12, 13,
                15, 12, 14,
                19, 17, 16,
                18, 17, 19,
                22, 20, 21,
                23, 20, 22
            });
            InitData = new SubResourceData(Indices, 0, 0);
            BufferDescription = new BufferDescription((uint)Indices.Size, D3D10Usage.Default, BindFlag.IndexBuffer, 0, 0);

            Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out IndexBuffer);
            if (Result < 0) throw new Exception("Device.CreateBuffer has failed : " + Result);

            // Set index buffer
            Device.IASetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            // Set primitive topology
            Device.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

            // Load the Texture
            Result = D3DX10Functions.CreateShaderResourceViewFromFile(Device, "seafloor.dds", null, out TextureResourceView);
            if (Result < 0) throw new Exception("D3DX10Functions.CreateShaderResourceViewFromFile has failed : " + Result);

            // Initialize the world matrix
            D3DX10Functions.MatrixIdentity(out World);

            // Initialize the view matrix
            Vector3 Eye = new Vector3(0.0f, 3.0f, -6.0f);
            Vector3 At = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
            D3DX10Functions.MatrixLookAtLH(out View, ref Eye, ref At, ref Up);

            // Initialize the projection matrix
            float FovY = (float)Math.PI * 0.25f;
            D3DX10Functions.MatrixPerspectiveFovLH(out Projection, FovY, ClientSize.Width / (float)ClientSize.Height, 0.1f, 100.0f);

            // Update Variables that never change
            ViewVariable.SetMatrix((float[])View);
            ProjectionVariable.SetMatrix((float[])Projection);
            DiffuseVariable.SetResource(TextureResourceView);

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

            // Rotate cube around the origin
            D3DX10Functions.MatrixRotationY(out World, Time);

            // Modify the color
            MeshColor.X = (float)Math.Sin(Time * 1.0f) + 1.0f * 0.5f;
            MeshColor.Y = (float)Math.Cos(Time * 3.0f) + 1.0f * 0.5f;
            MeshColor.Z = (float)Math.Sin(Time * 5.0f) + 1.0f * 0.5f;

            // Clear the backbuffer
            Float4 ClearColor = new Float4(0.0f, 0.125f, 0.3f, 1.0f); //red,green,blue,alpha
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);

            int Result = 0;

            // Update variables that change once per frame
            Result = WorldVariable.SetMatrix((float[])World);//StructToFloatArray(World));
            if (Result < 0) throw new Exception("WorldVariable.SetMatrix has failed : " + Result);
            Result = MeshColorVariable.SetFloatVector((float[])MeshColor);
            if (Result < 0) throw new Exception("MeshColorVariable.SetFloatVector has failed : " + Result);

            // Render the cube
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
            if (TextureResourceView != null) TextureResourceView.Release();
            if (Effect != null) Effect.Release();
            if (RenderTargetView != null) RenderTargetView.Release();
            if (SwapChain != null) SwapChain.Release();
            if (Device != null) Device.Release();
        }

        bool IsDeviceAcceptable(uint Adapter, uint Output, DriverType DeviceType, Format BackBufferFormat, bool Windowed, object UserContext)
        {
            return true;
        }

        int OnDeviceCreated(Device Device, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            return 0;
        }

        void OnDeviceDestroyed(object UserContext)
        {
        }

        int OnSwapChainResized(Device Device, SwapChain SwapChain, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            return 0;
        }

        void OnReleasingSwapChain(object UserContext)
        {
        }

        void OnFrameRender(Device Device, double Time, float ElapsedTime, object UserContext)
        {
        }

        void OnFrameMove(double Time, float ElapsedTime, object UserContext)
        {
        }

        bool OnModifyDeviceSettings(DeviceSettings DeviceSettings, object UserContext)
        {
            return true;
        }
    }
}
