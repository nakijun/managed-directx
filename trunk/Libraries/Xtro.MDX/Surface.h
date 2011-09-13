public ref class Surface sealed : DeviceSubObject
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGISurface);

internal:
	IDXGISurface* pSurface;

	Surface(IntPtr Surface) : DeviceSubObject(Surface)
	{	
		pSurface = (IDXGISurface*)Surface.ToPointer();
	}

public:
};