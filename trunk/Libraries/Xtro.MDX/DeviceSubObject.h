public ref class DeviceSubObject : Object
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGIDeviceSubObject);

internal:
	IDXGIDeviceSubObject* pDeviceSubObject;

	DeviceSubObject(IntPtr DeviceSubObject) : Object(DeviceSubObject)
	{	
		pDeviceSubObject = (IDXGIDeviceSubObject*)DeviceSubObject.ToPointer();
	}

public:
};