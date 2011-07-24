using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using Xtro.MDX;
using Xtro.MDX.Generic;
using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;
using Device = Xtro.MDX.Direct3D10.Device;
using Functions = Xtro.MDX.Direct3D10.Functions;
using Buffer = Xtro.MDX.Direct3D10.Buffer;
using Error = Xtro.MDX.Direct3D10.Error;
using Xtro.MDX.Direct3DX10;
using D3DX10Constants = Xtro.MDX.Direct3DX10.Constants;
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

        static double DegreeToRadian(double Degree)
        {
            return Degree * D3DX10Constants.PI / 180.0f;
        }

        struct SimpleVertex
        {
            public Vector3 Position;
            public Vector2 Texture;
        };

        Effect Effect = null;
        InputLayout VertexLayout = null;
        EffectTechnique Technique = null;
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
            // Read the D3DX effect file
            var ShaderFlags = ShaderFlag.EnableStrictness;
#if DEBUG
            // Set the D3D10_SHADER_DEBUG flag to embed debug information in the shaders.
            // Setting this flag improves the shader debugging experience, but still allows 
            // the shaders to be optimized and to run exactly the way they will run in 
            // the release configuration of this program.
            ShaderFlags |= ShaderFlag.Debug;
#endif

            var Result = D3DX10Functions.CreateEffectFromFile("Tutorial08.fx", null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect);
            if (Result < 0)
            {
                MessageBox.Show("The FX file cannot be located.  Please run this executable from the directory that contains the FX file.", "Error", MessageBoxButtons.OK);
                return Result;
            }

            Technique = Effect.GetTechniqueByName("Render");
            WorldVariable = Effect.GetVariableByName("World").AsMatrix();
            ViewVariable = Effect.GetVariableByName("View").AsMatrix();
            ProjectionVariable = Effect.GetVariableByName("Projection").AsMatrix();
            MeshColorVariable = Effect.GetVariableByName("vMeshColor").AsVector();
            DiffuseVariable = Effect.GetVariableByName("txDiffuse").AsShaderResource();

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
                    SemanticName = "TEXCOORD",
                    SemanticIndex = 0,
                    Format = Format.R32G32_Float,
                    InputSlot = 0,
                    AlignedByteOffset = 12,
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

            // Create vertex buffer
            var VertexCount = (uint)24;
            int VertexSize = Marshal.SizeOf(typeof(SimpleVertex));
            var Vertices = new UnmanagedMemory<SimpleVertex>((uint)(VertexSize * VertexCount));
            Vertices.Write(new SimpleVertex[]
            {
                new SimpleVertex{Position= new Vector3(-1.0f, 1.0f, -1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, 1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, 1.0f, 1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, 1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)}, 
                new SimpleVertex{Position= new Vector3(-1.0f, -1.0f, -1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, -1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, -1.0f, 1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, -1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, -1.0f, 1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, -1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, 1.0f, -1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, 1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, -1.0f, 1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, -1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, 1.0f, -1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, 1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, -1.0f, -1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, -1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, 1.0f, -1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, 1.0f, -1.0f),Texture= new Vector2(0.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, -1.0f, 1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, -1.0f, 1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position= new Vector3(1.0f, 1.0f, 1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position= new Vector3(-1.0f, 1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)} 
            });

            var BufferDescription = new BufferDescription
            {
                ByteWidth = (uint)Vertices.Size,
                Usage = Usage.Default,
                BindFlags = BindFlag.VertexBuffer,
                CPU_AccessFlags = 0,
                MiscellaneousFlags = 0
            };
            var InitData = new SubResourceData
            {
                SystemMemory = Vertices,
                SystemMemoryPitch = 0,
                SystemMemorySlicePitch = 0
            };
            Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out VertexBuffer);
            if (Result < 0) return Result;

            // Set vertex buffer
            Device.IA_SetVertexBuffers(0, 1, new[] { VertexBuffer }, new uint[] { (uint)(BufferDescription.ByteWidth / 24) }, new uint[] { 0 });

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

            BufferDescription = new BufferDescription
            {
                ByteWidth = (uint)Indices.Size,
                Usage = Usage.Default,
                BindFlags = BindFlag.IndexBuffer,
                CPU_AccessFlags = 0,
                MiscellaneousFlags = 0
            };
            InitData = new SubResourceData
            {
                SystemMemory = Indices,
                SystemMemoryPitch = 0,
                SystemMemorySlicePitch = 0
            };
            Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out IndexBuffer);
            if (Result < 0) return Result;

            // Set index buffer
            Device.IA_SetIndexBuffer(IndexBuffer, Format.R32_UInt, 0);

            // Set primitive topology
            Device.IA_SetPrimitiveTopology(PrimitiveTopology.TriangleList);

            // Load the Texture
            D3DX10Functions.CreateShaderResourceViewFromFile(Device, "seafloor.dds", out TextureResourceView);

            // Initialize the world matrices
            D3DX10Functions.MatrixIdentity(out World);

            // Initialize the view matrix
            Vector3 Eye = new Vector3(0.0f, 3.0f, -6.0f);
            Vector3 At = new Vector3(0.0f, 1.0f, 0.0f);
            Vector3 Up = new Vector3(0.0f, 1.0f, 0.0f);
            D3DX10Functions.MatrixLookAtLH(out View, ref Eye, ref At, ref Up);

            // Update Variables that never change
            ViewVariable.SetMatrix((float[])View);
            DiffuseVariable.SetResource(TextureResourceView);

            return 0;
        }

        void OnDeviceDestroyed(object UserContext)
        {
            if (VertexBuffer != null) VertexBuffer.Release();
            if (IndexBuffer != null) IndexBuffer.Release();
            if (VertexLayout != null) VertexLayout.Release();
            if (TextureResourceView != null) TextureResourceView.Release();
            if (Effect != null) Effect.Release();
        }

        int OnSwapChainResized(Device Device, SwapChain SwapChain, ref SurfaceDescription BackBufferSurfaceDescription, object UserContext)
        {
            // Setup the projection parameters again
            var Aspect = (float)BackBufferSurfaceDescription.Width / BackBufferSurfaceDescription.Height;
            D3DX10Functions.MatrixPerspectiveFovLH(out Projection, (float)D3DX10Constants.PI * 0.25f, Aspect, 0.1f, 100.0f);
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
            Float4 ClearColor = new Float4(new[]{0.0f, 0.125f, 0.3f, 1.0f}); //red,green,blue,alpha
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
            WorldVariable.SetMatrix((float[])World);//StructToFloatArray(World));
            MeshColorVariable.SetFloatVector((float[])MeshColor);

            //
            // Render the cube
            //
            TechniqueDescription TechniqueDescription;
            Technique.GetDescription(out TechniqueDescription);
            for (uint PassNo = 0; PassNo < TechniqueDescription.Passes; PassNo++)
            {
                Technique.GetPassByIndex(PassNo).Apply(0);
                Device.DrawIndexed(36, 0, 0);
            }
        }

        void OnFrameMove(double Time, float ElapsedTime, object UserContext)
        {
            // Rotate cube around the origin
            D3DX10Functions.MatrixRotationY(out World, (float)(60 * DegreeToRadian(Time)));

            // Modify the color
            MeshColor.X = (float)Math.Sin(Time * 1.0f) + 1.0f * 0.5f;
            MeshColor.Y = (float)Math.Cos(Time * 3.0f) + 1.0f * 0.5f;
            MeshColor.Z = (float)Math.Sin(Time * 5.0f) + 1.0f * 0.5f;
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