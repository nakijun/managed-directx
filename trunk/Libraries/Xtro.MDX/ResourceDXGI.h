public ref class Resource sealed : DeviceSubObject
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGIResource);

internal:
	IDXGIResource* pResource;

	Resource(IntPtr Resource) : DeviceSubObject(Resource)
	{	
		pResource = (IDXGIResource*)Resource.ToPointer();
	}

public:
	int GetEvictionPriority([Out] ResourcePriority% EvictionPriority)
	{
		pin_ptr<ResourcePriority> PinnedEvictionPriority = &EvictionPriority;

		return pResource->GetEvictionPriority((unsigned int*)PinnedEvictionPriority);
	}

	int GetSharedHandle([Out] IntPtr% SharedHandle)
	{
		HANDLE hSharedHandle = 0;
		int Result = pResource->GetSharedHandle(&hSharedHandle);

		SharedHandle = IntPtr(hSharedHandle);

		return Result;
	}

	int GetUsage(UsageFlag Usage)
	{
		pin_ptr<UsageFlag> PinnedUsage = &Usage;

		return pResource->GetUsage((DXGI_USAGE*)PinnedUsage);
	}
	   
	int SetEvictionPriority(ResourcePriority EvictionPriority)
	{
		return pResource->SetEvictionPriority((unsigned int)EvictionPriority);
	}
};