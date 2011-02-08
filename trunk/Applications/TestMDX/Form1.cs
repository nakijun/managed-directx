﻿using System;
using System.Windows.Forms;
using Xtro.MDX;
using Xtro.MDX.DXGI;
using Usage = Xtro.MDX.DXGI.Usage;
using Functions = Xtro.MDX.DXGI.Functions;
using Object = Xtro.MDX.DXGI.Object;
using Xtro.MDX.Direct3DX10;
using D3D10Functions = Xtro.MDX.Direct3D10.Functions;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Xtro.MDX.Direct3D10;
using System.Runtime.InteropServices;
using System.IO;

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
            SwapChainDescription.BufferUsage = Usage.RenderTargetOutput;
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
            Factory.GetPrivateData(Name, out MemoryData, ref Size, out InterfaceData);
            Factory.SetPrivateDataInterface(Name, Factory);
            Size = 0;
            Factory.GetPrivateData(Name, out MemoryData, ref Size, out InterfaceData);
            a = InterfaceData.Release();
        //    Factory.SetPrivateData(Name, 4, new byte[4] { 1, 2, 3, 4 });
            Size = 0;
            Factory.GetPrivateData(Name, out MemoryData, ref Size, out InterfaceData);

            Adapter Adapter;
            Factory.EnumAdapters(0, out Adapter);

            Output Output;
            Adapter.EnumerateOutputs(0, out Output);

            OutputDescription D;
            Output.GetDescription(out D);

            b = Adapter.Release();

            a = Factory.Release();
        }
    }
}
