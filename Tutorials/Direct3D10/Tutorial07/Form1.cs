﻿// ReSharper disable CSharpWarnings::CS0197
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

namespace Tutorial07
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
            public Vector2 Texture;
            // ReSharper restore NotAccessedField.Local
        };

        Device Device;
        SwapChain SwapChain;
        RenderTargetView RenderTargetView;
        Effect Effect;
        EffectTechnique Technique;
        TechniqueDescription TechniqueDescription;
        InputLayout VertexLayout;
        Buffer VertexBuffer;
        Buffer IndexBuffer;
        ShaderResourceView TextureResourceView;
        EffectMatrixVariable WorldVariable;
        EffectMatrixVariable ViewVariable;
        EffectMatrixVariable ProjectionVariable;
        EffectVectorVariable MeshColorVariable;
        EffectShaderResourceVariable DiffuseVariable;
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
#if DEBUG
            const CreateDeviceFlag CreateDeviceFlags = CreateDeviceFlag.Debug;
#else
            const CreateDeviceFlag CreateDeviceFlags = 0;
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

            Device.OM_SetRenderTargets(1, new[] { RenderTargetView }, null);

            // Setup the viewport
            var Viewport = new Viewport
            {
                TopLeftX = 0,
                TopLeftY = 0,
                Width = (uint)ClientSize.Width,
                Height = (uint)ClientSize.Height,
                MaximumDepth = 1.0f
            };
            Device.RS_SetViewports(1, new[] { Viewport });

            // Create the effect

#if DEBUG
            // Set the ShaderFlag.Debug flag to embed debug information in the shaders.
            // Setting this flag improves the shader debugging experience, but still allows 
            // the shaders to be optimized and to run exactly the way they will run in 
            // the release configuration of this program.
            const ShaderFlag ShaderFlags = ShaderFlag.EnableStrictness | ShaderFlag.Debug;
#else
            const ShaderFlag ShaderFlags = ShaderFlag.EnableStrictness;
#endif

            Result = D3DX10Functions.CreateEffectFromFile("Tutorial07.fx", null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect);
            if (Result == (int)Error.FileNotFound)
            {
                MessageBox.Show(@"The FX file cannot be located.  Please run this executable from the directory that contains the FX file.", @"Error", MessageBoxButtons.OK);
                return false;
            }
            if (Result < 0) throw new Exception("D3DX10Functions.CreateEffectFromFile has failed : " + Result);

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
            Result = Technique.GetPassByIndex(0).GetDescription(out PassDescription);
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
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, -1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, 1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)}, 
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, -1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, 1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, 1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, -1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, 1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, -1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, -1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, -1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, -1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, -1.0f),Texture= new Vector2(0.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, -1.0f, 1.0f),Texture= new Vector2(0.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, -1.0f, 1.0f),Texture= new Vector2(1.0f, 0.0f)},
                new SimpleVertex{Position = new Vector3(1.0f, 1.0f, 1.0f),Texture= new Vector2(1.0f, 1.0f)},
                new SimpleVertex{Position = new Vector3(-1.0f, 1.0f, 1.0f),Texture= new Vector2(0.0f, 1.0f)} 
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

            // Load the Texture
            Result = D3DX10Functions.CreateShaderResourceViewFromFile(Device, "seafloor.dds", out TextureResourceView);
            if (Result < 0) throw new Exception("D3DX10Functions.CreateShaderResourceViewFromFile has failed : " + Result);

            // Initialize the world matrix
            D3DX10Functions.MatrixIdentity(out World);

            // Initialize the view matrix
            var Eye = new Vector3(0.0f, 3.0f, -6.0f);
            var At = new Vector3(0.0f, 1.0f, 0.0f);
            var Up = new Vector3(0.0f, 1.0f, 0.0f);
            D3DX10Functions.MatrixLookAtLH(out View, ref Eye, ref At, ref Up);

            // Initialize the projection matrix
            const float FovY = (float)D3DX10Constants.PI * 0.25f;
            D3DX10Functions.MatrixPerspectiveFovLH(out Projection, FovY, ClientSize.Width / (float)ClientSize.Height, 0.1f, 100.0f);

            // Update Variables that never change
            var Data = new UnmanagedMemory<float>((uint)Marshal.SizeOf(View));
            Data.Set(0, ref View);
            ViewVariable.SetMatrix(Data);
            Data.Set(0, ref Projection);
            ProjectionVariable.SetMatrix(Data);
            DiffuseVariable.SetResource(TextureResourceView);

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

            // Modify the color
            MeshColor.X = (float)Math.Sin(Time * 1.0f) + 1.0f * 0.5f;
            MeshColor.Y = (float)Math.Cos(Time * 3.0f) + 1.0f * 0.5f;
            MeshColor.Z = (float)Math.Sin(Time * 5.0f) + 1.0f * 0.5f;

            // Clear the backbuffer
            var ClearColor = new Float4(new[] { 0.0f, 0.125f, 0.3f, 1.0f }); //red,green,blue,alpha
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);

            // Update variables that change once per frame
            var Data = new UnmanagedMemory<float>((uint)Marshal.SizeOf(World));
            Data.Set(0, ref World);
            var Result = WorldVariable.SetMatrix(Data);
            if (Result < 0) throw new Exception("WorldVariable.SetMatrix has failed : " + Result);
            Data.Set(0, ref MeshColor);
            Result = MeshColorVariable.SetFloatVector(Data);
            if (Result < 0) throw new Exception("MeshColorVariable.SetFloatVector has failed : " + Result);

            // Render the cube
            for (uint PassNo = 0; PassNo < TechniqueDescription.Passes; PassNo++)
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
    }
}
// ReSharper restore CSharpWarnings::CS0197