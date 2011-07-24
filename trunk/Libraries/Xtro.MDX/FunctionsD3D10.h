public ref class Functions abstract sealed
{
public:
	static int CreateDevice(Adapter^ Adapter, DriverType DriverType, Module^ Software, CreateDeviceFlag Flags, [Out] Device^% Device)
	{
		IDXGIAdapter* pAdapter = Adapter == nullptr ? 0 : Adapter->pAdapter;
		HMODULE hSoftware = Software == nullptr ? 0 : (HMODULE)Marshal::GetHINSTANCE(Software).ToPointer();

		ID3D10Device* pDevice = 0;
		int Result = D3D10CreateDevice(pAdapter, (D3D10_DRIVER_TYPE)DriverType, hSoftware, (unsigned int)Flags, D3D10_SDK_VERSION, &pDevice);

		if (pDevice)
		{
			try { Device = (Xtro::MDX::Direct3D10::Device^)Interface::Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }					
		}
		else Device = nullptr;

		return Result;
	}

	static int CreateDeviceAndSwapChain(Adapter^ Adapter, DriverType DriverType, Module^ Software, CreateDeviceFlag Flags, SwapChainDescription% SwapChainDescription, [Out] SwapChain^% SwapChain, [Out] Device^% Device)
	{
		IDXGIAdapter* pAdapter = Adapter == nullptr ? 0 : Adapter->pAdapter;
		HMODULE hSoftware = Software == nullptr ? 0 : (HMODULE)Marshal::GetHINSTANCE(Software).ToPointer();
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

	static int StateBlockMaskEnableAll([Out] StateBlockMask% Mask)
	{
		pin_ptr<StateBlockMask> PinnedMask = &Mask;
		int Result = D3D10StateBlockMaskEnableAll((D3D10_STATE_BLOCK_MASK*)PinnedMask);

		return Result;
	}

	static int StateBlockMaskDisableCapture(StateBlockMask% Mask, DeviceStateType StateType, unsigned int RangeStart, unsigned int RangeLength)
	{
		pin_ptr<StateBlockMask> PinnedMask = &Mask;

		int Result = D3D10StateBlockMaskDisableCapture((D3D10_STATE_BLOCK_MASK*)PinnedMask, (D3D10_DEVICE_STATE_TYPES)StateType, RangeStart, RangeLength);

		return Result;
	}

	static int CreateStateBlock(Device^ Device, StateBlockMask% StateBlockMask, [Out] StateBlock^% StateBlock)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		pin_ptr<Xtro::MDX::Direct3D10::StateBlockMask> PinnedStateBlockMask = &StateBlockMask;

		ID3D10StateBlock* pStateBlock = 0;
		int Result = D3D10CreateStateBlock(pDevice, (D3D10_STATE_BLOCK_MASK*)PinnedStateBlockMask, &pStateBlock);

		if (pStateBlock)
		{
			try { StateBlock = (Xtro::MDX::Direct3D10::StateBlock^)Interface::Interfaces[IntPtr(pStateBlock)]; }
			catch (KeyNotFoundException^) { StateBlock = gcnew Xtro::MDX::Direct3D10::StateBlock(IntPtr(pStateBlock)); }					
		}
		else StateBlock = nullptr;

		return Result;
	}
};