public ref class Device sealed : Object
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGIDevice);

internal:
	IDXGIDevice* pDevice;

	Device(IntPtr Device) : Object(Device)
	{	
		pDevice = (IDXGIDevice*)Device.ToPointer();
	}

public:
	int CreateSurface(SurfaceDescription% Description, unsigned int NumberOfSurfaces, UsageFlag Usage, SharedResource% SharedResource, [Out] Surface^% Surface)
	{
		pin_ptr<SurfaceDescription> PinnedDescription = &Description;
		pin_ptr<Xtro::MDX::DXGI::SharedResource> PinnedSharedResource = &SharedResource;

		IDXGISurface* pSurface = 0;
		int Result = pDevice->CreateSurface((DXGI_SURFACE_DESC*)PinnedDescription, NumberOfSurfaces, (DXGI_USAGE)Usage, (DXGI_SHARED_RESOURCE*)PinnedSharedResource, &pSurface);

		if (pSurface) 
		{
			try { Surface = (Xtro::MDX::DXGI::Surface^)Interfaces[IntPtr(pSurface)]; }
			catch (KeyNotFoundException^) { Surface = gcnew Xtro::MDX::DXGI::Surface(IntPtr(pSurface)); }
		}
		else Surface = nullptr;

		return Result;
	}

	int GetAdapter([Out] Adapter^% Adapter)
	{
		IDXGIAdapter* pAdapter = 0;
		int Result = pDevice->GetAdapter(&pAdapter);

		if (pAdapter) 
		{
			try { Adapter = (Xtro::MDX::DXGI::Adapter^)Interfaces[IntPtr(pAdapter)]; }
			catch (KeyNotFoundException^) { Adapter = gcnew Xtro::MDX::DXGI::Adapter(IntPtr(pAdapter)); }
		}
		else Adapter = nullptr;

		return Result;
	}

	int GetGpuThreadPriority([Out] int% Priority)
	{
		pin_ptr<int> PinnedPriority = &Priority;

		return pDevice->GetGPUThreadPriority((int*)PinnedPriority);
	}

	int QueryResourceResidency(array<Unknown^>^ Resources, [Out] ResidencyFlag% ResidencyStatus, unsigned int NumberOfResources)
	{
		pin_ptr<ResidencyFlag> PinnedResidencyStatus = &ResidencyStatus;

		IUnknown** pResources = 0;
		try
		{
			if (Resources != nullptr && Resources->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfResources, (unsigned int)Resources->Length);
				pResources = new IUnknown*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					pResources[No] = Resources[No] == nullptr ? 0 : Resources[No]->pUnknown;
				}
			}

			return pDevice->QueryResourceResidency(pResources, (DXGI_RESIDENCY*)PinnedResidencyStatus, NumberOfResources);
		}
		finally
		{
			if (pResources) delete[] pResources;
		}
	}

	int SetGpuThreadPriority(int Priority)
	{
		return pDevice->SetGPUThreadPriority(Priority);
	}
};