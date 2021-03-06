public ref class Object : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGIObject);

protected private:
	IDXGIObject* pObject;

	Object(IntPtr Object) : Unknown(Object)
	{
		pObject = (IDXGIObject*)Object.ToPointer();
	}

public:
	int GetParent(Type^ Type, [Out] Object^% Object)
	{
		Guid RIID;
		try { RIID = (Guid)Type->GetField("IID", BindingFlags::DeclaredOnly | BindingFlags::NonPublic | BindingFlags::Static)->GetValue(nullptr); }
		catch (...) { RIID = Guid::Empty; }

		IDXGIObject* pParent = 0;
		int Result = pObject->GetParent(IID_Converter::ToNative(RIID), (void**)&pParent);

		if (pParent) 
		{				
			try { Object = (DXGI::Object^)Interfaces[IntPtr(pParent)]; }
			catch (KeyNotFoundException^) { Object = (DXGI::Object^)Activator::CreateInstance(Type, BindingFlags::NonPublic | BindingFlags::Instance, nullptr, gcnew array<System::Object^>(1) { IntPtr(pParent) }, CultureInfo::CurrentCulture); }
		}
		else Object = nullptr;

		return Result;
	}

	int GetPrivateData(Guid Name, [Out] unsigned int% DataSize)
	{
		pin_ptr<unsigned int> PinnedDataSize = &DataSize;

		return pObject->GetPrivateData(IID_Converter::ToNative(Name), PinnedDataSize, 0);
	}

	int GetPrivateData(Guid Name, unsigned int DataSize, UnmanagedMemory^ Data)
	{
		pin_ptr<unsigned int> PinnedDataSize = &DataSize;
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		
		return pObject->GetPrivateData(IID_Converter::ToNative(Name), PinnedDataSize, pData);
	}

	int GetPrivateData(Guid Name, [Out] Unknown^% Unknown)
	{
		void* pData = 0;
		
		unsigned int DataSize = Marshal::SizeOf(IntPtr::typeid);
		int Result = pObject->GetPrivateData(IID_Converter::ToNative(Name), &DataSize, &pData);

		if (pData)
		{
			try	{ Unknown = (MDX::Unknown^)Interfaces[IntPtr(pData)];	}
			catch (KeyNotFoundException^) { Unknown = nullptr; }
		}

		return Result;
	}

	int SetPrivateData(Guid Name, unsigned int DataSize, UnmanagedMemory^ Data)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		int Result = pObject->SetPrivateData(IID_Converter::ToNative(Name), DataSize, pData);

		return Result;
	}

	int SetPrivateDataInterface(Guid Name, Unknown^ Unknown)
	{
		IUnknown* pUnknown = Unknown == nullptr ? 0 : Unknown->pUnknown;

		int Result = pObject->SetPrivateDataInterface(IID_Converter::ToNative(Name), pUnknown);

		return Result;
	}
};