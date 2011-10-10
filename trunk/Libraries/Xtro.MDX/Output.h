public ref class Output sealed : Object
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGIOutput);

internal:
	IDXGIOutput* pOutput;

	Output(IntPtr Output) : Object(Output)
	{
		pOutput = (IDXGIOutput*)Output.ToPointer();
	}

public:
	int FindClosestMatchingMode(ModeDescription% ModeToMatch, [Out] ModeDescription% ClosestMatch, Unknown^ ConcernedDevice)
	{
		pin_ptr<ModeDescription> PinnedModeToMatch = &ModeToMatch;
		pin_ptr<ModeDescription> PinnedClosestMatch = &ClosestMatch;
		IUnknown* pConcernedDevice = ConcernedDevice == nullptr ? 0 : ConcernedDevice->pUnknown;

		return pOutput->FindClosestMatchingMode((DXGI_MODE_DESC*)PinnedModeToMatch, (DXGI_MODE_DESC*)PinnedClosestMatch, pConcernedDevice);
	}

	int GetDescription([Out] OutputDescription% Description)
	{
		DXGI_OUTPUT_DESC NativeDescription;
		int Result = pOutput->GetDesc(&NativeDescription);

		if (Result >= 0) Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetDisplayModeList(Format EnumerationFormat, EnumerationModeFlag Flags, unsigned int% NumberOfModes, array<ModeDescription>^ Descriptions)
	{
		pin_ptr<unsigned int> PinnedNumberOfModes = &NumberOfModes;
		pin_ptr<ModeDescription> PinnedDescriptions = Descriptions != nullptr && Descriptions->Length > 0 ? PinnedDescriptions = &Descriptions[0] : nullptr;

		int Result = pOutput->GetDisplayModeList((DXGI_FORMAT)EnumerationFormat, (unsigned int)Flags, (unsigned int*)PinnedNumberOfModes, (DXGI_MODE_DESC*)PinnedDescriptions);

		return Result;
	}

	int GetDisplaySurfaceData(Surface^ Destination)
	{
		IDXGISurface* pDestination = Destination == nullptr ? 0 : Destination->pSurface;

		return pOutput->GetDisplaySurfaceData(pDestination);
	}

	int GetFrameStatistics([Out] FrameStatistics% Stats)
	{
		pin_ptr<FrameStatistics> PinnedStats = &Stats;

		return pOutput->GetFrameStatistics((DXGI_FRAME_STATISTICS*)PinnedStats);
	}

	int GetGammaControl(array<GammaControl>^ Array)
	{
		pin_ptr<GammaControl> PinnedArray = Array != nullptr && Array->Length > 0 ? &Array[0] : nullptr;

		return pOutput->GetGammaControl((DXGI_GAMMA_CONTROL*)PinnedArray);
	}

	int GetGammaControlCapabilities([Out] GammaControlCapabilities% GammaCapabilities)
	{
		pin_ptr<GammaControlCapabilities> PinnedGammaCapabilities = &GammaCapabilities;

		return pOutput->GetGammaControlCapabilities((DXGI_GAMMA_CONTROL_CAPABILITIES*)PinnedGammaCapabilities);
	}

	void ReleaseOwnership()
	{
		pOutput->ReleaseOwnership();
	}

	int SetDisplaySurface(Surface^ ScanoutSurface)
	{
		IDXGISurface* pScanoutSurface = ScanoutSurface == nullptr ? 0 : ScanoutSurface->pSurface;

		return pOutput->SetDisplaySurface(pScanoutSurface);
	}

	int SetGammaControl(array<GammaControl>^ Array)
	{
		pin_ptr<GammaControl> PinnedArray = Array != nullptr && Array->Length > 0 ? &Array[0] : nullptr;

		return pOutput->SetGammaControl((DXGI_GAMMA_CONTROL*)PinnedArray);
	}

	int TakeOwnership(Unknown^ Device, bool Exclusive)
	{
		IUnknown *pDevice = Device == nullptr ? 0 : Device->pUnknown;

		return pOutput->TakeOwnership(pDevice, Exclusive);
	}

	int WaitForVerticalBlank()
	{
		return pOutput->WaitForVBlank();
	}
};