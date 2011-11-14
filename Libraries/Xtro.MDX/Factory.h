public ref class Factory sealed : Object
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGIFactory);

internal:
	IDXGIFactory* pFactory;

	Factory(IntPtr Factory) : Object(Factory)
	{	
		pFactory = (IDXGIFactory*)Factory.ToPointer();
	}

public:
	int CreateSoftwareAdapter(Module^ Module, [Out] Adapter^% Adapter)
	{
		HMODULE hModule = Module == nullptr ? 0 : (HMODULE)Marshal::GetHINSTANCE(Module).ToPointer();

		IDXGIAdapter* pAdapter = 0;
		int Result = pFactory->CreateSoftwareAdapter(hModule, &pAdapter);

		if (pAdapter) 
		{
			try { Adapter = (DXGI::Adapter^)Interfaces[IntPtr(pAdapter)]; }
			catch (KeyNotFoundException^) { Adapter = gcnew DXGI::Adapter(IntPtr(pAdapter)); }
		}
		else Adapter = nullptr;

		return Result;
	}

	int CreateSwapChain(Unknown^ Device, SwapChainDescription% Description, [Out] SwapChain^% SwapChain)
	{
		IUnknown* pDevice = Device == nullptr ? 0 : Device->pUnknown;

		pin_ptr<SwapChainDescription> PinnedDescription = &Description;

		IDXGISwapChain* pSwapChain = 0;
		int Result = pFactory->CreateSwapChain(pDevice, (DXGI_SWAP_CHAIN_DESC*)PinnedDescription, &pSwapChain);

		if (pSwapChain) 
		{
			try { SwapChain = (DXGI::SwapChain^)Interfaces[IntPtr(pSwapChain)]; }
			catch (KeyNotFoundException^) { SwapChain = gcnew DXGI::SwapChain(IntPtr(pSwapChain)); }
		}
		else SwapChain = nullptr;

		return Result;
	}

	int EnumerateAdapters(unsigned int AdapterIndex, [Out] Adapter^% Adapter)
	{
		IDXGIAdapter* pAdapter = 0;
		int Result = pFactory->EnumAdapters(AdapterIndex, &pAdapter);

		if (pAdapter) 
		{
			try { Adapter = (DXGI::Adapter^)Interfaces[IntPtr(pAdapter)]; }
			catch (KeyNotFoundException^) { Adapter = gcnew DXGI::Adapter(IntPtr(pAdapter)); }
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

	int MakeWindowAssociation(IntPtr WindowHandle, MakeWindowAssociationFlag Flags)
	{
		return pFactory->MakeWindowAssociation((HWND)WindowHandle.ToPointer(), (unsigned int)Flags);
	}
};