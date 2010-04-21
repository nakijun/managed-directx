namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public ref class Functions
	{
	public:
		static int CreateDevice()
		{
		//	ID3D10Device* Device = 0;
		//	return D3D10CreateDevice( 0, D3D10_DRIVER_TYPE_REFERENCE, 0, 0, D3D10_SDK_VERSION, &Device );             
			return 0;
		}

		static int CreateDeviceAndSwapChain(Adapter^ Adapter, DriverType DriverType, Module^ Software, CreateDeviceFlag Flags, SwapChainDescription% SwapChainDescription, [Out] SwapChain^% SwapChain, [Out] Device^% Device)
		{
			IDXGIAdapter* pAdapter = 0;
			if (Adapter != nullptr) pAdapter = Adapter->pAdapter;

			HMODULE hSoftware = 0;
			if (Software != nullptr) hSoftware = (HMODULE)Marshal::GetHINSTANCE(Software).ToPointer();

			pin_ptr<Xtro::MDX::DXGI::SwapChainDescription> PinnedSwapChainDescription = &SwapChainDescription;

			ID3D10Device* pDevice = 0;
			IDXGISwapChain* pSwapChain = 0;
			int Result = D3D10CreateDeviceAndSwapChain(pAdapter, (D3D10_DRIVER_TYPE)DriverType, hSoftware, (unsigned int)Flags, D3D10_SDK_VERSION, (DXGI_SWAP_CHAIN_DESC*)PinnedSwapChainDescription, &pSwapChain, &pDevice);

			if (pDevice)
			{
				try { Device = (Xtro::MDX::Direct3D10::Device^)Interface::Interfaces[IntPtr(pDevice)]; }
				catch (KeyNotFoundException^) { Device = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }					
			}
			else Device = nullptr;

			if (pSwapChain)
			{
				try { SwapChain = (Xtro::MDX::DXGI::SwapChain^)Interface::Interfaces[IntPtr(pSwapChain)]; }
				catch (KeyNotFoundException^) { SwapChain = gcnew Xtro::MDX::DXGI::SwapChain(IntPtr(pSwapChain)); }
			}
			else SwapChain = nullptr;

			return Result;
		}
	};
}
}
}