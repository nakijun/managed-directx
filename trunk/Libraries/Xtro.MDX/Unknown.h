public ref class Unknown : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IUnknown);

protected private:
	Unknown(IntPtr Unknown) : Interface(Unknown)
	{
		pUnknown = (IUnknown*)Unknown.ToPointer();
	}

internal:
	IUnknown* pUnknown;

public:
	unsigned int AddRef()
	{
		return pUnknown->AddRef();
	}

	unsigned int Release()
	{
		unsigned int Result = pUnknown->Release();

		if (Result == 0) Interfaces.Remove(InterfacePointer);

		return Result;
	}

	int QueryInterface(Type^ Type, [Out] Object^% Object)
	{
		Guid RIID;
		try { RIID = (Guid)Type->GetField("IID", BindingFlags::DeclaredOnly | BindingFlags::NonPublic | BindingFlags::Static)->GetValue(nullptr); }
		catch (...) { RIID = Guid::Empty; }

		void* pObject = 0;
		int Result = pUnknown->QueryInterface(IID_Converter::ToNative(RIID), &pObject);

		if (pObject) 
		{				
			try { Object = Interfaces[IntPtr(pObject)]; }
			catch (KeyNotFoundException^) { Object = Activator::CreateInstance(Type, BindingFlags::NonPublic | BindingFlags::Instance, nullptr, gcnew array<System::Object^>(1) { IntPtr(pObject) }, CultureInfo::CurrentCulture); }
		}
		else Object = nullptr;

		return Result;
	}
};