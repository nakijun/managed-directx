public ref class Resource : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Resource);

internal:
	ID3D10Resource* pResource;

	Resource(IntPtr Resource) : DeviceChild(Resource)
	{	
		pResource = (ID3D10Resource*)Resource.ToPointer();
	}

public:
	ResourcePriority GetEvictionPriority()
	{
		return (ResourcePriority)pResource->GetEvictionPriority();
	}

	void GetType([Out] ResourceDimension% Type)
	{
		pin_ptr<ResourceDimension> PinnedType = &Type;

		pResource->GetType((D3D10_RESOURCE_DIMENSION*)PinnedType);
	}  

	void SetEvictionPriority(ResourcePriority EvictionPriority)
	{
		pResource->SetEvictionPriority((unsigned int)EvictionPriority);
	}
};