public ref class Functions abstract sealed
{
public:
	static int CreateFactory(Type^ Type, [Out] Factory^% Factory)
	{
		Guid RIID;
		try { RIID = (Guid)Type->GetField("IID", BindingFlags::DeclaredOnly | BindingFlags::NonPublic | BindingFlags::Static)->GetValue(nullptr); }
		catch (...) { RIID = Guid::Empty; }

		IDXGIFactory* pFactory = 0;
		int Result = CreateDXGIFactory(IID_Converter::ToNative(RIID), (void**)(&pFactory));

		if (pFactory) 
		{
			try { Factory = (DXGI::Factory^)Interface::Interfaces[IntPtr(pFactory)]; }
			catch (KeyNotFoundException^) { Factory = gcnew DXGI::Factory(IntPtr(pFactory)); }
		}
		else Factory = nullptr;

		return Result;
	}
};