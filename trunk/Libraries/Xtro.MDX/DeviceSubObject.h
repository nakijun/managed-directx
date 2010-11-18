public ref class DeviceSubObject : Object
{
internal:
	IDXGIDeviceSubObject* pDeviceSubObject;

	static Guid IID = IID_Converter::ToManaged(IID_IDXGIDeviceSubObject);

	DeviceSubObject(IntPtr DeviceSubObject) : Object(DeviceSubObject)
	{	
		pDeviceSubObject = (IDXGIDeviceSubObject*)DeviceSubObject.ToPointer();
	}

public:
};