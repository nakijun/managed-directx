// ReSharper disable CSharpWarnings::CS0197
using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;

using Xtro.MDX;
using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;
using Device = Xtro.MDX.Direct3D10.Device;
using Functions = Xtro.MDX.Direct3D10.Functions;

namespace Tutorial01
{
    sealed partial class Form1 : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [System.Security.SuppressUnmanagedCodeSecurity]
        static extern IntPtr PostMessage(IntPtr Wnd, int Msg, int ParamW, int ParamL);

        Device Device;
        SwapChain SwapChain;
        RenderTargetView RenderTargetView;

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

            if (Result < 0) throw new Exception("No device could be created");

            // Create a render target view

            Unknown Surface;
            Result = SwapChain.GetBuffer(0, typeof(Texture2D), out Surface);
            if (Result < 0) throw new Exception("SwapChain.GetBuffer has failed");

            var BackBuffer = (Texture2D)Surface;

            Result = Device.CreateRenderTargetView(BackBuffer, out RenderTargetView);
            if (Result < 0) throw new Exception("Device.CreateRenderTargetView has failed");

            if (BackBuffer != null) BackBuffer.Release();

            Device.OM_SetRenderTargets(1, new[] { RenderTargetView }, null);

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

            return true;
        }

        void Render()
        {
            // Just clear the backbuffer
            var ClearColor = new Float4(new[] { 0.0f, 0.125f, 0.3f, 1.0f }); //red,green,blue,alpha
            Device.ClearRenderTargetView(RenderTargetView, ref ClearColor);
            SwapChain.Present(0, 0);
        }

        void CleanupDevice()
        {
            if (Device != null) Device.ClearState();

            if (RenderTargetView != null) RenderTargetView.Release();
            if (SwapChain != null) SwapChain.Release();
            if (Device != null) Device.Release();
        }
    }
}
// ReSharper restore CSharpWarnings::CS0197