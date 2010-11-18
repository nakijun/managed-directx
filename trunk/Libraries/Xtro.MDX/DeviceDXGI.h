public ref class Device : Object
{
internal:
	IDXGIDevice* pDevice;

	static Guid IID = IID_Converter::ToManaged(IID_IDXGIDevice);

	Device(IntPtr Device) : Object(Device)
	{	
		pDevice = (IDXGIDevice*)Device.ToPointer();
	}

public:
};