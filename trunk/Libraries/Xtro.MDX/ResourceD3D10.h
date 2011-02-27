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
};