using System;
using System.Windows.Forms;
using UtilitiesFunctions = Xtro.MDX.Utilities.Functions;

using Xtro.MDX.DXGI;

namespace Tutorial10
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            try { TestDirectX(); }
            catch
            {
#if DEBUG
                MessageBox.Show("Xtro.MDX (debug runtime) can not be loaded. Possibly, VC++ 2010 SP1 redistributable or DirectX 10 (August 2009) (debug runtime) is not installed.");
#else
                MessageBox.Show("Xtro.MDX can not be loaded. Possibly, VC++ 2010 SP1 redistributable or DirectX 10 (August 2009) is not installed.");
#endif     
                return;
            }

            RunApplication();
        }

        static void TestDirectX()
        {
            Factory Factory;
            Functions.CreateFactory(null, out Factory);
        }

        static void RunApplication()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
            UtilitiesFunctions.DestroyState();
        }
    }
}
