using System;
using System.Windows.Forms;
using Xtro.MDX;
using Xtro.MDX.DXGI;
using Functions = Xtro.MDX.DXGI.Functions;
using Object = Xtro.MDX.DXGI.Object;
using Xtro.MDX.Direct3DX10;
using D3D10Functions = Xtro.MDX.Direct3D10.Functions;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Xtro.MDX.Direct3D10;
using System.Runtime.InteropServices;
using System.IO;
using System.Linq;

namespace TestMDX
{
    public sealed partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Shown(object Sender, EventArgs E)
        {
            var at = new TextureLoadInfo { Filter = FilterFlag.Box, DestinationBox = new Box { Back=1} };
            var bt = new TextureLoadInfo { Filter = FilterFlag.Box, DestinationBox = new Box() };
            bt.DestinationBox = new Box { Back = 1 };
            var tt = at == bt;

            var b1 = new byte[] { 1, 2, 3 };
            var b2 = new byte[] { 1, 2, 3 };
            var c = b1.SequenceEqual(b2);
            var d = b1 == b2;

            var OD = new OutputDescription()
                         {
                             AttachedToDesktop = false
                         };

            var V = new Vector3(1, 2, 3);
            V.Add(new Vector3(1, 2, 3));

            var VV = V - new Vector3(0, 1, 2);
            VV = +V;
            VV = -V;
            VV.Multiply(3);
            VV.Add(V);
            Vector3 ZZ = VV + V;

            Matrix M = new Matrix(new float[16] { 11, 12, 13, 14, 21, 22, 23, 24, 31, 32, 33, 34, 41, 42, 43, 44 });
            M.Multiply(3);
            float[] fs = (float[])M;

            SwapChainDescription SwapChainDescription = new SwapChainDescription();
            SwapChainDescription.BufferCount = 1;
            SwapChainDescription.BufferDescription.Width = (uint)ClientSize.Width;
            SwapChainDescription.BufferDescription.Height = (uint)ClientSize.Height;
            SwapChainDescription.BufferDescription.Format = Format.R8G8B8A8_UNorm;
            SwapChainDescription.BufferDescription.RefreshRate.Numerator = 60;
            SwapChainDescription.BufferDescription.RefreshRate.Denominator = 1;
            SwapChainDescription.BufferUsage = UsageFlag.RenderTargetOutput;
            SwapChainDescription.OutputWindow = Handle;
            SwapChainDescription.SampleDescription.Count = 1;
            SwapChainDescription.SampleDescription.Quality = 0;
            SwapChainDescription.Windowed = true;

            int Result = 0;
            SwapChain SwapChain;
            Xtro.MDX.Direct3D10.Device Device;            
            Result = D3D10Functions.CreateDeviceAndSwapChain(null, DriverType.Hardware, null, CreateDeviceFlag.Debug, ref SwapChainDescription, out SwapChain, out Device);

            Effect Effect;
            Result = D3DX10Functions.CreateEffectFromFile("Tutorial02.fx", null, null, "fx_4_0", ShaderFlag.EnableStrictness | ShaderFlag.Debug, 0, Device, null, out Effect);

            EffectTechnique Technique = Effect.GetTechniqueByName("Render");

            // Create the input layout
            PassDescription PassDescription;
            Result = Technique.GetPassByIndex(0).GetDescription(out PassDescription);
            if (Result < 0) throw new Exception("GetDescription has failed : " + Result);

            // factory test

            uint a, b;

            Factory Factory;
            Functions.CreateFactory(typeof(Factory), out Factory);

            Object Parent;
            Factory.GetParent(typeof(Factory), out Parent);

            Guid Name = Guid.NewGuid();

            Unknown InterfaceData;
            UnmanagedMemory MemoryData;

            uint Size = 0;
            Factory.GetPrivateData(Name, out Size);
            Factory.GetPrivateData(Name, out InterfaceData);
            MemoryData = new UnmanagedMemory(1000);
            Factory.GetPrivateData(Name, Size, MemoryData);

            Factory.SetPrivateDataInterface(Name, Factory);
            Factory.GetPrivateData(Name, out InterfaceData);
            a = InterfaceData.Release();

            MemoryData = new UnmanagedMemory(64);
            MemoryData.Write(0, new[] { 1, 2, 3, 4 });
            Factory.SetPrivateData(Name, MemoryData.Size, MemoryData);
            Size = 0;
            Factory.GetPrivateData(Name, out Size);
            MemoryData = new UnmanagedMemory(Size);
            Factory.GetPrivateData(Name, Size, MemoryData);
            MemoryData.Get<uint>(0, out a);

            Adapter Adapter;
            Factory.EnumerateAdapters(0, out Adapter);

            Output Output;
            Adapter.EnumerateOutputs(0, out Output);

            OutputDescription D;
            Output.GetDescription(out D);

            b = Adapter.Release();

            a = Factory.Release();
        }
    }
}
