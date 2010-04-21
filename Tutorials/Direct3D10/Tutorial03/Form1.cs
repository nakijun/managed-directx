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

namespace Tutorial03
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        public static extern IntPtr PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);

        DriverType DriverType = DriverType.Null;
        Device Device = null;
        SwapChain SwapChain = null;
        RenderTargetView RenderTargetView = null;
        Effect Effect = null;
        EffectTechnique Technique = null;
        TechniqueDescription TechniqueDescription;
        InputLayout VertexLayout = null;
        Buffer VertexBuffer = null;

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
                Result = D3DX10Functions.CreateEffectFromFile("Tutorial03.fx", null, null, "fx_4_0", ShaderFlags, 0, Device, null, out Effect, out Errors);
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

            // Define the input layout
            InputElementDescription[] Layout = 
            {
                new InputElementDescription("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.InputPerVertexData, 0) 
            };

            // Create the input layout
            PassDescription PassDescription;
            Result = Technique.GetPassByIndex(0).GetDescription(out PassDescription);
            if (Result < 0) throw new Exception("GetDescription has failed : " + Result);
            Result = Device.CreateInputLayout(Layout, PassDescription.IAInputSignature, out VertexLayout);
            if (Result < 0) throw new Exception("Device.CreateInputLayout has failed : " + Result);

            // Set the input layout
            Device.IASetInputLayout(VertexLayout);

            // Create vertex buffer

            int VertexCount = 3;
            int VertexSize = Marshal.SizeOf(typeof(Vector3));
            UnmanagedMemory Vertices = new UnmanagedMemory((uint)(VertexSize * VertexCount));
            Vertices.Write(0, VertexCount, new Vector3[]
            {
                new Vector3(0.0f, 0.5f, 0.5f),
                new Vector3(0.5f, -0.5f, 0.5f),
                new Vector3(-0.5f, -0.5f, 0.5f)
            });
            SubResourceData InitData = new SubResourceData(Vertices, 0, 0);
            BufferDescription BufferDescription = new BufferDescription((uint)Vertices.Size, D3D10Usage.Default, BindFlag.VertexBuffer, 0, 0);

            Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out VertexBuffer);
            if (Result < 0) throw new Exception("Device.CreateBuffer has failed : " + Result);

            // Set vertex buffer
            Device.IASetVertexBuffers(0, new Buffer[] { VertexBuffer }, new uint[] { (uint)VertexSize }, new uint[] { 0 });

            // Set primitive topology
            Device.IASetPrimitiveTopology(PrimitiveTopology.TriangleList);

            return true;
        }

        void Render()
        {
            // Clear the backbuffer
            Float4 ClearColor = new Float4(0.0f, 0.125f, 0.3f, 1.0f); //red,green,blue,alpha
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);
            
            // Render a triangle
            for (uint PassNo = 0; PassNo < TechniqueDescription.Passes; ++PassNo)
            {
                Technique.GetPassByIndex(PassNo).Apply(0);
                Device.Draw(3, 0);
            }

            // Present the information rendered to the back buffer to the front buffer (the screen)
            SwapChain.Present(0, 0);
        }

        void CleanupDevice()
        {
            if (Device != null) Device.ClearState();

            if (VertexBuffer != null) VertexBuffer.Release();
            if (VertexLayout != null) VertexLayout.Release();
            if (Effect != null) Effect.Release();
            if (RenderTargetView != null) RenderTargetView.Release();
            if (SwapChain != null) SwapChain.Release();
            if (Device != null) Device.Release();
        }
    }
}
