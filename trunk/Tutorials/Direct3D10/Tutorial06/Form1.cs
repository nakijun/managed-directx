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
using Xtro.MDX.Generic;
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

namespace Tutorial06
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
            public Vector3 Normal;

            public SimpleVertex(Vector3 Position, Vector3 Normal)
            {
                this.Position = Position;
                this.Normal = Normal;
            }
        };

        DriverType DriverType = DriverType.Null;
        Device Device = null;
        SwapChain SwapChain = null;
        RenderTargetView RenderTargetView = null;
        Texture2D DepthStencil = null;
        DepthStencilView DepthStencilView = null;
        Effect Effect = null;
        EffectTechnique TechniqueRender = null;
        EffectTechnique TechniqueRenderLight = null;
        TechniqueDescription TechniqueDescriptionRender;
        TechniqueDescription TechniqueDescriptionRenderLight;
        InputLayout VertexLayout = null;
        Buffer VertexBuffer = null;
        Buffer IndexBuffer = null;
        EffectMatrixVariable WorldVariable = null;
        EffectMatrixVariable ViewVariable = null;
        EffectMatrixVariable ProjectionVariable = null;
        EffectVectorVariable LightDirectionVariable = null;
        EffectVectorVariable LightColorVariable = null;
        EffectVectorVariable OutputColorVariable = null;
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

            // Create depth stencil texture
            Texture2D_Description DepthStencilDescription;
            DepthStencilDescription.Width = (uint)ClientSize.Width;
            DepthStencilDescription.Height = (uint)ClientSize.Height;
            DepthStencilDescription.MipLevels = 1;
            DepthStencilDescription.ArraySize = 1;
            DepthStencilDescription.Format = Format.D32_Float;
            DepthStencilDescription.SampleDescription.Count = 1;
            DepthStencilDescription.SampleDescription.Quality = 0;
            DepthStencilDescription.Usage = D3D10Usage.Default;
            DepthStencilDescription.BindFlags = BindFlag.DepthStencil;
            DepthStencilDescription.CPU_AccessFlags = 0;
            DepthStencilDescription.MiscFlags = 0;
            Result = Device.CreateTexture2D(ref DepthStencilDescription, out DepthStencil);
            if (Result < 0) throw new Exception("Device.CreateTexture2D has failed : " + Result);

            // Create the depth stencil view
            DepthStencilViewDescription DepthStencilViewDescription = new DepthStencilViewDescription
            {
                Format = DepthStencilDescription.Format,
                ViewDimension = DSV_Dimension.Texture2D
            };
            DepthStencilViewDescription.Texture2D.MipSlice = 0;
            Result = Device.CreateDepthStencilView(DepthStencil, ref DepthStencilViewDescription, out DepthStencilView);
            if (Result < 0) throw new Exception("Device.CreateDepthStencilView has failed : " + Result);

            Device.OM_SetRenderTargets(1, new [] { RenderTargetView }, DepthStencilView);

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

            Result = D3DX10Functions.CreateEffectFromFile("Tutorial06.fx", null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect);
            if (Result == (int)Error.FileNotFound)
            {
                MessageBox.Show("The FX file cannot be located.  Please run this executable from the directory that contains the FX file.", "Error", MessageBoxButtons.OK);
                return false;
            }
            else if (Result < 0) throw new Exception("D3DX10Functions.CreateEffectFromFile has failed : " + Result);

            // Obtain the techniques

            TechniqueRender = Effect.GetTechniqueByName("Render");
            Result = TechniqueRender.GetDescription(out TechniqueDescriptionRender);
            if (Result < 0) throw new Exception("GetDescription has failed : " + Result);

            TechniqueRenderLight = Effect.GetTechniqueByName("RenderLight");
            Result = TechniqueRenderLight.GetDescription(out TechniqueDescriptionRenderLight);
            if (Result < 0) throw new Exception("GetDescription has failed : " + Result);

            // Obtain the variables
            WorldVariable = Effect.GetVariableByName("World").AsMatrix();
            ViewVariable = Effect.GetVariableByName("View").AsMatrix();
            ProjectionVariable = Effect.GetVariableByName("Projection").AsMatrix();
            LightDirectionVariable = Effect.GetVariableByName("vLightDir").AsVector();
            LightColorVariable = Effect.GetVariableByName("vLightColor").AsVector();
            OutputColorVariable = Effect.GetVariableByName("vOutputColor").AsVector();

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
                    Format = Format.R32G32B32A32_Float,
                    InputSlot = 0,
                    AlignedByteOffset = 12,
                    InputSlotClass = InputClassification.InputPerVertexData,
                    InstanceDataStepRate = 0
                }
            };

            // Create the input layout
            PassDescription PassDescription;
            Result = TechniqueRender.GetPassByIndex(0).GetDescription(out PassDescription);
            if (Result < 0) throw new Exception("GetDescription has failed : " + Result);
            Result = Device.CreateInputLayout(Layout, (uint)Layout.Length, PassDescription.IA_InputSignature, (uint)PassDescription.IA_InputSignature.Size, out VertexLayout);
            if (Result < 0) throw new Exception("Device.CreateInputLayout has failed : " + Result);

            // Set the input layout
            Device.IA_SetInputLayout(VertexLayout);

            SubResourceData InitData;
            BufferDescription BufferDescription;

            // Create vertex buffer

            var VertexCount = (uint)24;
            int VertexSize = Marshal.SizeOf(typeof(SimpleVertex));
            var Vertices = new UnmanagedMemory<SimpleVertex>((uint)(VertexSize * VertexCount));
            Vertices.Write(new SimpleVertex[]
            {
                new SimpleVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(0.0f, 1.0f, 0.0f)), 
                new SimpleVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(0.0f, -1.0f, 0.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(-1.0f, 0.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(1.0f, 0.0f, 0.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(1.0f, 0.0f, 0.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, -1.0f), new Vector3(0.0f, 0.0f, -1.0f)),
                new SimpleVertex(new Vector3(-1.0f, -1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f)),
                new SimpleVertex(new Vector3(1.0f, -1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f)),
                new SimpleVertex(new Vector3(1.0f, 1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f)),
                new SimpleVertex(new Vector3(-1.0f, 1.0f, 1.0f), new Vector3(0.0f, 0.0f, 1.0f)) 
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
            Device.IA_SetVertexBuffers(0,1, new [] { VertexBuffer }, new uint[] { (uint)(BufferDescription.ByteWidth / 24) }, new uint[] { 0 });

            // Create index buffer

            var IndexCount = (uint)36;
            var Indices = new UnmanagedMemory<int>(sizeof(int) * IndexCount);
            Indices.Write(new int[] 
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
            Vector3 Eye=new Vector3( 0.0f, 4.0f, -10.0f );
            Vector3 At = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
            D3DX10Functions.MatrixLookAtLH(out View, ref Eye, ref At, ref Up);

            // Initialize the projection matrix
            float FovY = (float)Math.PI * 0.25f;
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

            // Rotate cube around the origin
            D3DX10Functions.MatrixRotationY(out World, Time);

            // Setup our lighting parameters

            Vector4 Vector4;

            var LightDirections = new UnmanagedMemory<Vector4>((uint)Marshal.SizeOf(typeof(Vector4)) * 2);
            Vector4 = new Vector4(-0.577f, 0.577f, -0.577f, 1.0f);
            LightDirections.Set(0, ref Vector4);
            Vector4 = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);
            LightDirections.Set(1, ref Vector4);

            var LightColors = new UnmanagedMemory<Vector4>((uint)Marshal.SizeOf(typeof(Vector4)) * 2);
            Vector4 = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            LightColors.Set(0, ref Vector4);
            Vector4 = new Vector4(0.5f, 0.0f, 0.0f, 1.0f);
            LightColors.Set(1, ref Vector4);

            // Rotate the second light around the origin
            Matrix Rotate;
            D3DX10Functions.MatrixRotationY(out Rotate, -2.0f * Time);
            LightDirections.Get(1, out Vector4);
            Vector3 Vector3 = new Vector3((float[])Vector4);
            D3DX10Functions.Vector3Transform(out Vector4, ref Vector3, ref Rotate);
            LightDirections.Set(1, ref Vector4);

            // Clear the backbuffer
            Float4 ClearColor = new Float4(0.0f, 0.125f, 0.3f, 1.0f); //red,green,blue,alpha
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);

            // Clear the depth buffer to 1.0 (max depth)
            Device.ClearDepthStencilView(DepthStencilView, ClearFlag.Depth, 1.0f, 0);

            int Result = 0;

            // Update matrix variables
            Result = WorldVariable.SetMatrix((float[])World);//StructToFloatArray(World));
            if (Result < 0) throw new Exception("WorldVariable.SetMatrix has failed : " + Result);
            Result = ViewVariable.SetMatrix((float[])View);
            if (Result < 0) throw new Exception("ViewVariable.SetMatrix has failed : " + Result);
            Result = ProjectionVariable.SetMatrix((float[])Projection);
            if (Result < 0) throw new Exception("ProjectionVariable.SetMatrix has failed : " + Result);

            // Update lighting variables
            LightDirectionVariable.SetFloatVectorArray(LightDirections, 0, 2);
            LightColorVariable.SetFloatVectorArray(LightColors, 0, 2);

            // Render the cube
            for (uint PassNo = 0; PassNo < TechniqueDescriptionRender.Passes; PassNo++)
            {
                TechniqueRender.GetPassByIndex(PassNo).Apply(0);
                Device.DrawIndexed(36, 0, 0);
            }
                
            // Render each light
            //
            for (var M = (uint)0; M < 2; M++)
            {
                Matrix Light;
                Matrix LightScale;
                Vector4 LightPosition = new Vector4();
                LightDirections.Get(M, out LightPosition);
                LightPosition *= 5.0f;
                D3DX10Functions.MatrixTranslation(out Light, LightPosition.X, LightPosition.Y, LightPosition.Z);
                D3DX10Functions.MatrixScaling(out LightScale, 0.2f, 0.2f, 0.2f);
                Light = LightScale * Light;

                // Update the world variable to reflect the current light
                WorldVariable.SetMatrix((float[])Light);
                LightColors.Get(M, out Vector4);
                OutputColorVariable.SetFloatVector((float[])Vector4);

                for (uint PassNo = 0; PassNo < TechniqueDescriptionRenderLight.Passes; PassNo++)
                {
                    TechniqueRenderLight.GetPassByIndex(PassNo).Apply(0);
                    Device.DrawIndexed(36, 0, 0);
                }
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
            if (DepthStencil != null) DepthStencil.Release();
            if (DepthStencilView != null) DepthStencilView.Release();
            if (SwapChain != null) SwapChain.Release();
            if (Device != null) Device.Release();
        }
    }
}
