public ref class Device sealed : Object
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGIDevice);

internal:
	IDXGIDevice* pDevice;

	Device(IntPtr Device) : Object(Device)
	{	
		pDevice = (IDXGIDevice*)Device.ToPointer();
	}

public:
	int GetAdapter([Out] Adapter^% Adapter)
	{
		IDXGIAdapter* pAdapter = 0;
		int Result = pDevice->GetAdapter(&pAdapter);

		if (pAdapter) 
		{
			try { Adapter = (Xtro::MDX::DXGI::Adapter^)Interfaces[IntPtr(pAdapter)]; }
			catch (KeyNotFoundException^) { Adapter = gcnew Xtro::MDX::DXGI::Adapter(IntPtr(pAdapter)); }
		}
		else Adapter = nullptr;

		return Result;
	}
};