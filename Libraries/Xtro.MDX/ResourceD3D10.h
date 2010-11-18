public ref class Resource : DeviceChild
{
internal:
	ID3D10Resource* pResource;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Resource);

	Resource(IntPtr Resource) : DeviceChild(Resource)
	{	
		pResource = (ID3D10Resource*)Resource.ToPointer();
	}

public:
};