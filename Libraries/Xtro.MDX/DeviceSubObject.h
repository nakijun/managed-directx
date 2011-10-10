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
	int GetDevice(Type^ Type, [Out] System::Object^% Device)
	{
		Guid RIID;
		try { RIID = (Guid)Type->GetField("IID", BindingFlags::DeclaredOnly | BindingFlags::NonPublic | BindingFlags::Static)->GetValue(nullptr); }
		catch (...) { RIID = Guid::Empty; }

		void* pDevice = 0;
		int Result = pDeviceSubObject->GetDevice(IID_Converter::ToNative(RIID), &pDevice);

		if (pDevice) 
		{
			try { Device = Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device = Activator::CreateInstance(Type, BindingFlags::NonPublic | BindingFlags::Instance, nullptr, gcnew array<System::Object^>(1) { IntPtr(pObject) }, CultureInfo::CurrentCulture); }
		}
		else Device = nullptr;

		return Result;
	}
};