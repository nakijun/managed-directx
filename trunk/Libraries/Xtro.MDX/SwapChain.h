public ref class SwapChain sealed : DeviceSubObject
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGISwapChain);

internal:
	IDXGISwapChain* pSwapChain;

	SwapChain(IntPtr SwapChain) : DeviceSubObject(SwapChain)
	{
		pSwapChain = (IDXGISwapChain*)SwapChain.ToPointer();
	}

public:
	int GetBuffer(unsigned int Buffer, Type^ Type, [Out] Unknown^% Surface)
	{
		Guid RIID;
		try { RIID = (Guid)Type->GetField("IID", BindingFlags::DeclaredOnly | BindingFlags::NonPublic | BindingFlags::Static)->GetValue(nullptr); }
		catch (...) { RIID = Guid::Empty; }

		IUnknown* pUnknown = 0;
		int Result = pSwapChain->GetBuffer(Buffer, IID_Converter::ToNative(RIID), (void**)&pUnknown);

		if (pUnknown) 
		{
			try { Surface = (Unknown^)Interfaces[IntPtr(pUnknown)]; }
			catch (KeyNotFoundException^) { Surface = (Unknown^)Activator::CreateInstance(Type, BindingFlags::NonPublic | BindingFlags::Instance, nullptr, gcnew array<System::Object^>(1) { IntPtr(pUnknown) }, CultureInfo::CurrentCulture); }
		}
		else Surface = nullptr;

		return Result;
	}

	int GetContainingOutput([Out] Output^% Output)
	{
		IDXGIOutput* pOutput = 0;
		return pSwapChain->GetContainingOutput(&pOutput);

		if (pOutput)
		{	
			try { Output = (DXGI::Output^)Interfaces[IntPtr(pOutput)]; }
			catch (KeyNotFoundException^) { Output = gcnew DXGI::Output(IntPtr(pOutput)); }
		}
		else Output = nullptr;
	}

	int GetDescription([Out] SwapChainDescription% Description)
	{
		pin_ptr<SwapChainDescription> PinnedDescription = &Description;
		return pSwapChain->GetDesc((DXGI_SWAP_CHAIN_DESC*)PinnedDescription);
	}

	int GetFrameStatistics([Out] FrameStatistics% Stats)
	{
		pin_ptr<FrameStatistics> PinnedStats = &Stats;

		return pSwapChain->GetFrameStatistics((DXGI_FRAME_STATISTICS*)PinnedStats);
	}

	int GetFullscreenState([Out] bool% Fullscreen, [Out] Output^% Target)
	{
		pin_ptr<bool> PinnedFullscreen = &Fullscreen;

		IDXGIOutput* pTarget = 0;
		return pSwapChain->GetFullscreenState((int*)PinnedFullscreen, &pTarget);

		if (pTarget)
		{	
			try { Target = (Output^)Interfaces[IntPtr(pTarget)]; }
			catch (KeyNotFoundException^) { Target = gcnew Output(IntPtr(pTarget)); }
		}
		else Target = nullptr;
	}

	int GetFullscreenState([Out] bool% Fullscreen)
	{
		pin_ptr<bool> PinnedFullscreen = &Fullscreen;

		return pSwapChain->GetFullscreenState((int*)PinnedFullscreen, 0);
	}

	int GetLastPresentCount([Out] unsigned int% LastPresentCount)
	{
		pin_ptr<unsigned int> PinnedLastPresentCount = &LastPresentCount;

		return pSwapChain->GetLastPresentCount(PinnedLastPresentCount);
	}

	int Present(unsigned int SyncInterval, PresentFlag Flags)
	{
		return pSwapChain->Present(SyncInterval, (unsigned int)Flags);
	}

	int ResizeBuffers(unsigned int BufferCount, unsigned int Width, unsigned int Height, Format NewFormat, SwapChainFlag SwapChainFlags)
	{
		return pSwapChain->ResizeBuffers(BufferCount, Width, Height, (DXGI_FORMAT)NewFormat, (unsigned int)SwapChainFlags);
	}

	int ResizeTarget(ModeDescription% NewTargetParameters)
	{
		pin_ptr<ModeDescription> PinnedNewTargetParameters = &NewTargetParameters;
		return pSwapChain->ResizeTarget((DXGI_MODE_DESC*)PinnedNewTargetParameters);
	}

	int SetFullscreenState(bool Fullscreen, Output^ Target)
	{
		IDXGIOutput* pTarget = Target == nullptr ? 0 : Target->pOutput;

		return pSwapChain->SetFullscreenState(Fullscreen, pTarget);
	}
};