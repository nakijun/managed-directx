// ReSharper disable CSharpWarnings::CS0197
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
using Constants = Xtro.MDX.Direct3D10.Constants;
using D3DX10Constants = Xtro.MDX.Direct3DX10.Constants;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;

namespace Tutorial06
{
    sealed partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity]
        static extern IntPtr PostMessage(IntPtr Wnd, int Msg, int ParamW, int ParamL);

        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity]
        static extern uint GetTickCount();

        struct SimpleVertex
        {
            // ReSharper disable NotAccessedField.Local
            public Vector3 Position;
            public Vector3 Normal;
            // ReSharper restore NotAccessedField.Local
        };

        Device Device;
        SwapChain SwapChain;
        RenderTargetView RenderTargetView;
        Texture2D DepthStencil;
        DepthStencilView DepthStencilView;
        Effect Effect;
        EffectTechnique TechniqueRender;
        EffectTechnique TechniqueRenderLight;
        TechniqueDescription TechniqueDescriptionRender;
        TechniqueDescription TechniqueDescriptionRenderLight;
        InputLayout VertexLayout;
        Buffer VertexBuffer;
        Buffer IndexBuffer;
        EffectMatrixVariable WorldVariable;
        EffectMatrixVariable ViewVariable;
        EffectMatrixVariable ProjectionVariable;
        EffectVectorVariable LightDirectionVariable;
        EffectVectorVariable LightColorVariable;
        EffectVectorVariable OutputColorVariable;
        Matrix World;
        Matrix View;
        Matrix Projection;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object Sender, EventArgs E)
        {
            if (InitDevice()) Application.Idle += Application_Idle;
        }

        private void Form1_FormClosing(object Sender, FormClosingEventArgs E)
        {
            Application.Idle -= Application_Idle;

            try { CleanupDevice(); }
            // if FormClosing event throws an exception, Closing gets canceled. So we handle the exception.
            catch (Exception Ex) { MessageBox.Show(Ex.ToString()); }
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

        // ReSharper disable UnusedParameter.Local
        static void Application_Exception(object Sender, ThreadExceptionEventArgs E)
        // ReSharper restore UnusedParameter.Local
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

            var SwapChainDescription = new SwapChainDescription
            {
                BufferCount = 1,
                BufferDescription =
                {
                    Width = (uint)ClientSize.Width,
                    Height = (uint)ClientSize.Height,
                    Format = Format.R8G8B8A8_UNorm,
                    RefreshRate =
                    {
                        Numerator = 60,
                        Denominator = 1
                    }
                },
                BufferUsage = UsageFlag.RenderTargetOutput,
                OutputWindow = Handle,
                SampleDescription =
                {
                    Count = 1,
                    Quality = 0
                },
                Windowed = true
            };

            var Result = 0;

            foreach (var Type in DriverTypes)
            {
                Result = Functions.CreateDeviceAndSwapChain(null, Type, null, CreateDeviceFlags, Constants.SdkVersion, ref SwapChainDescription, out SwapChain, out Device);
                if (Result >= 0) break;
            }

            if (Result < 0) throw new Exception("No device could be created : " + Result);

            // Create a render target view

            Unknown Surface;
            Result = SwapChain.GetBuffer(0, typeof(Texture2D), out Surface);
            if (Result < 0) throw new Exception("SwapChain.GetBuffer has failed : " + Result);

            var BackBuffer = (Texture2D)Surface;

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
            DepthStencilDescription.Usage = Usage.Default;
            DepthStencilDescription.BindFlags = BindFlag.DepthStencil;
            DepthStencilDescription.CpuAccessFlags = 0;
            DepthStencilDescription.MiscellaneousFlags = 0;
            Result = Device.CreateTexture2D(ref DepthStencilDescription, null, out DepthStencil);
            if (Result < 0) throw new Exception("Device.CreateTexture2D has failed : " + Result);

            // Create the depth stencil view
            var DepthStencilViewDescription = new DepthStencilViewDescription
            {
                Format = DepthStencilDescription.Format,
                ViewDimension = DepthStencilViewDimension.Texture2D,
                Texture2D =
                {
                    MipSlice = 0
                }
            };
            Result = Device.CreateDepthStencilView(DepthStencil, ref DepthStencilViewDescription, out DepthStencilView);
            if (Result < 0) throw new Exception("Device.CreateDepthStencilView has failed : " + Result);

            Device.OM_SetRenderTargets(1, new[] { RenderTargetView }, DepthStencilView);

            // Setup the viewport
            var Viewport = new Viewport
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

            var ShaderFlags = ShaderFlag.EnableStrictness;
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
                MessageBox.Show(@"The FX file cannot be located.  Please run this executable from the directory that contains the FX file.", @"Error", MessageBoxButtons.OK);
                return false;
            }
            if (Result < 0) throw new Exception("D3DX10Functions.CreateEffectFromFile has failed : " + Result);

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
            Result = Device.CreateInputLayout(Layout, (uint)Layout.Length, PassDescription.IA_InputSignature, PassDescription.IA_InputSignature.Size, out VertexLayout);
            if (Result < 0) throw new Exception("Device.CreateInputLayout has failed : " + Result);

            // Set the input layout
            Device.IA_SetInputLayout(VertexLayout);

            // Create vertex buffer

            const uint VertexCount = 24;
            var VertexSize = Marshal.SizeOf(typeof(SimpleVertex));
            var Vertices = new UnmanagedMemory<SimpleVertex>((uint)(VertexSize * VertexCount));
            Vertices.Write(new[]
            {
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, -1.0f),Normal = new Vector3(0.0f, 1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, -1.0f),Normal = new Vector3(0.0f, 1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, 1.0f),Normal = new Vector3(0.0f, 1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, 1.0f),Normal = new Vector3(0.0f, 1.0f, 0.0f)}, 
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, -1.0f),Normal = new Vector3(0.0f, -1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, -1.0f),Normal = new Vector3(0.0f, -1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, 1.0f),Normal = new Vector3(0.0f, -1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, 1.0f),Normal = new Vector3(0.0f, -1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, 1.0f),Normal = new Vector3(-1.0f, 0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, -1.0f),Normal = new Vector3(-1.0f, 0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, -1.0f),Normal = new Vector3(-1.0f, 0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, 1.0f),Normal = new Vector3(-1.0f, 0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, 1.0f),Normal = new Vector3(1.0f, 0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, -1.0f),Normal = new Vector3(1.0f, 0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, -1.0f),Normal = new Vector3(1.0f, 0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, 1.0f),Normal = new Vector3(1.0f, 0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, -1.0f),Normal = new Vector3(0.0f, 0.0f, -1.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, -1.0f),Normal = new Vector3(0.0f, 0.0f, -1.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, -1.0f),Normal = new Vector3(0.0f, 0.0f, -1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, -1.0f),Normal = new Vector3(0.0f, 0.0f, -1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, 1.0f),Normal = new Vector3(0.0f, 0.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, 1.0f),Normal = new Vector3(0.0f, 0.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, 1.0f),Normal = new Vector3(0.0f, 0.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, 1.0f),Normal = new Vector3(0.0f, 0.0f, 1.0f)} 
            });
            var InitData = new SubResourceData
            {
                SystemMemory = Vertices,
                SystemMemoryPitch = 0,
                SystemMemorySlicePitch = 0
            };
            var BufferDescription = new BufferDescription
            {
                ByteWidth = Vertices.Size,
                Usage = Usage.Default,
                BindFlags = BindFlag.VertexBuffer,
                CpuAccessFlags = 0,
                MiscellaneousFlags = 0
            };

            Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out VertexBuffer);
            if (Result < 0) throw new Exception("Device.CreateBuffer has failed : " + Result);

            // Set vertex buffer
            Device.IA_SetVertexBuffers(0, 1, new[] { VertexBuffer }, new[] { BufferDescription.ByteWidth / 24 }, new uint[] { 0 });

            // Create index buffer

            const uint IndexCount = 36;
            var Indices = new UnmanagedMemory<int>(sizeof(int) * IndexCount);
            Indices.Write(new[] 
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
                ByteWidth = Indices.Size,
                Usage = Usage.Default,
                BindFlags = BindFlag.IndexBuffer,
                CpuAccessFlags = 0,
                MiscellaneousFlags = 0
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
            var Eye = new Vector3(0.0f, 4.0f, -10.0f);
            var At = new Vector3(0.0f, 1.0f, 0.0f);
            var Up = new Vector3(0.0f, 1.0f, 0.0f);
            D3DX10Functions.MatrixLookAtLH(out View, ref Eye, ref At, ref Up);

            // Initialize the projection matrix
            const float FovY = (float)D3DX10Constants.PI * 0.25f;
            D3DX10Functions.MatrixPerspectiveFovLH(out Projection, FovY, ClientSize.Width / (float)ClientSize.Height, 0.1f, 100.0f);

            return true;
        }

        float Time;
        uint TimeStart;

        void Render()
        {
            // Update our time
            var TimeCurrent = GetTickCount();
            if (TimeStart == 0) TimeStart = TimeCurrent;
            Time = (TimeCurrent - TimeStart) / 1000.0f;

            // Rotate cube around the origin
            D3DX10Functions.MatrixRotationY(out World, Time);

            // Setup our lighting parameters

            var LightDirections = new UnmanagedMemory<float>((uint)Marshal.SizeOf(typeof(Vector4)) * 2);
            var Vector4 = new Vector4(-0.577f, 0.577f, -0.577f, 1.0f);
            LightDirections.Set(0, 0, ref Vector4);
            Vector4 = new Vector4(0.0f, 0.0f, -1.0f, 1.0f);
            LightDirections.Set(0, 1, ref Vector4);

            var LightColors = new UnmanagedMemory<float>((uint)Marshal.SizeOf(typeof(Vector4)) * 2);
            Vector4 = new Vector4(0.5f, 0.5f, 0.5f, 1.0f);
            LightColors.Set(0, 0, ref Vector4);
            Vector4 = new Vector4(0.5f, 0.0f, 0.0f, 1.0f);
            LightColors.Set(0, 1, ref Vector4);

            // Rotate the second light around the origin
            Matrix Rotate;
            D3DX10Functions.MatrixRotationY(out Rotate, -2.0f * Time);
            LightDirections.Get(0, 1, out Vector4);
            var Vector3 = new Vector3((float[])Vector4);
            D3DX10Functions.Vector3Transform(out Vector4, ref Vector3, ref Rotate);
            LightDirections.Set(0, 1, ref Vector4);

            // Clear the backbuffer
            var ClearColor = new Float4(new[] { 0.0f, 0.125f, 0.3f, 1.0f }); //red,green,blue,alpha
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);

            // Clear the depth buffer to 1.0 (max depth)
            Device.ClearDepthStencilView(DepthStencilView, ClearFlag.Depth, 1.0f, 0);

            // Update matrix variables
            var Data = new UnmanagedMemory<float>((uint)Marshal.SizeOf(typeof(Matrix)));
            Data.Set(0, ref World);
            var Result = WorldVariable.SetMatrix(Data);
            if (Result < 0) throw new Exception("WorldVariable.SetMatrix has failed : " + Result);
            Data.Set(0, ref View);
            Result = ViewVariable.SetMatrix(Data);
            if (Result < 0) throw new Exception("ViewVariable.SetMatrix has failed : " + Result);
            Data.Set(0, ref Projection);
            Result = ProjectionVariable.SetMatrix(Data);
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
                Vector4 LightPosition;
                LightDirections.Get(M, out LightPosition);
                LightPosition *= 5.0f;
                D3DX10Functions.MatrixTranslation(out Light, LightPosition.X, LightPosition.Y, LightPosition.Z);
                D3DX10Functions.MatrixScaling(out LightScale, 0.2f, 0.2f, 0.2f);
                Light = LightScale * Light;

                // Update the world variable to reflect the current light
                Data.Set(0, ref Light);
                WorldVariable.SetMatrix(Data);
                LightColors.CopyTo(M, Data, 0, 1);
                OutputColorVariable.SetFloatVector(Data);

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
// ReSharper restore CSharpWarnings::CS0197