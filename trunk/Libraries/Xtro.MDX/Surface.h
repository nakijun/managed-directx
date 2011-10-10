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
	void GetDescription([Out] SurfaceDescription% Description)
	{
		pin_ptr<SurfaceDescription> PinnedDescription = &Description;
		pSurface->GetDesc((DXGI_SURFACE_DESC*)PinnedDescription);
	}

	int Map([Out] MappedRectangle% LockedRectangle, MapFlag MapFlags)
	{
		DXGI_MAPPED_RECT NativeLockedRectangle;
		int Result = pSurface->Map(&NativeLockedRectangle, (unsigned int)MapFlags);

		LockedRectangle.FromNative(&NativeLockedRectangle);

		return Result;
	}

	void Unmap()
	{
		pSurface->Unmap();
	}
};