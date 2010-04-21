namespace Xtro
{
namespace MDX
{
namespace DXGI
{
	public ref class Factory : Object
	{
	internal:
		IDXGIFactory* pFactory;

		static Guid IID = IID_Converter::ToManaged(IID_IDXGIFactory);

		Factory(IntPtr Factory) : Object(Factory)
		{	
			pFactory = (IDXGIFactory*)Factory.ToPointer();
		}

	public:
		int CreateSoftwareAdapter(Module^ Module, [Out] Adapter^% Adapter)
		{
			HMODULE hModule = 0;
			if (Module != nullptr) hModule = (HMODULE)Marshal::GetHINSTANCE(Module).ToPointer();

			IDXGIAdapter* pAdapter = 0;
			int Result = pFactory->CreateSoftwareAdapter(hModule, &pAdapter);

			if (pAdapter) 
			{
				try { Adapter = (Xtro::MDX::DXGI::Adapter^)Interfaces[IntPtr(pAdapter)]; }
				catch (KeyNotFoundException^) { Adapter = gcnew Xtro::MDX::DXGI::Adapter(IntPtr(pAdapter)); }
			}
			else Adapter = nullptr;

			return Result;
		}

		int CreateSwapChain(Unknown^ Device, SwapChainDescription% Description, [Out] SwapChain^% SwapChain)
		{
			IUnknown* pDevice = 0;
			if (Device != nullptr) pDevice = Device->pUnknown;

			pin_ptr<SwapChainDescription> PinnedDescription = &Description;

			IDXGISwapChain* pSwapChain = 0;
			int Result = pFactory->CreateSwapChain(pDevice, (DXGI_SWAP_CHAIN_DESC*)PinnedDescription, &pSwapChain);

			if (pSwapChain) 
			{
				try { SwapChain = (Xtro::MDX::DXGI::SwapChain^)Interfaces[IntPtr(pSwapChain)]; }
				catch (KeyNotFoundException^) { SwapChain = gcnew Xtro::MDX::DXGI::SwapChain(IntPtr(pSwapChain)); }
			}
			else SwapChain = nullptr;

			return Result;
		}

		int EnumAdapters(unsigned int AdapterIndex, [Out] Adapter^% Adapter)
		{
			IDXGIAdapter* pAdapter = 0;
			int Result = pFactory->EnumAdapters(AdapterIndex, &pAdapter);

			if (pAdapter) 
			{
				try { Adapter = (Xtro::MDX::DXGI::Adapter^)Interfaces[IntPtr(pAdapter)]; }
				catch (KeyNotFoundException^) { Adapter = gcnew Xtro::MDX::DXGI::Adapter(IntPtr(pAdapter)); }
			}
			else Adapter = nullptr;

			return Result;
		}

		int GetWindowAssociation([Out] IntPtr% WindowHandle)
		{
			HWND hWindow = 0;
			int Result = pFactory->GetWindowAssociation(&hWindow);

			WindowHandle = IntPtr(hWindow);

			return Result;
		}

		int SetWindowAssociation(IntPtr WindowHandle, MakeWindowAssociation Flags)
		{
			return pFactory->MakeWindowAssociation((HWND)WindowHandle.ToPointer(), (unsigned int)Flags);
		}
	};
}
}
}