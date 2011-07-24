public ref class DeviceChild : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10DeviceChild);

internal:
	ID3D10DeviceChild* pDeviceChild;

	DeviceChild(IntPtr DeviceChild) : Unknown(DeviceChild)
	{	
		pDeviceChild = (ID3D10DeviceChild*)DeviceChild.ToPointer();
	}

public:
	void GetDevice([Out] Device^% Device);
};