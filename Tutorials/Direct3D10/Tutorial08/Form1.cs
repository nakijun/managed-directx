using System;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
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
            UtilitiesFunctions.SetWindow(this);
            UtilitiesFunctions.SetCursorSettings(true, true);
            UtilitiesFunctions.CreateDevice(true, 640, 480);

            if (UtilitiesFunctions.PreMainLoop() < 0) Close();
                
            Application.Idle += Application_Idle;
        }

        private static void Form1_FormClosing(object Sender, FormClosingEventArgs E)
        {
            // Application.exitcode = DXUTGetExitCode();
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

        void Application_Exception(object Sender, ThreadExceptionEventArgs E)
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
