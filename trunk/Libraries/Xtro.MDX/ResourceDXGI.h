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
};