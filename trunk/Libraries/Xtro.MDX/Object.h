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
			try { Object = (Xtro::MDX::DXGI::Object^)Interfaces[IntPtr(pParent)]; }
			catch (KeyNotFoundException^) { Object = (Xtro::MDX::DXGI::Object^)Activator::CreateInstance(Type, BindingFlags::NonPublic | BindingFlags::Instance, nullptr, gcnew array<System::Object^>(1) { IntPtr(pParent) }, CultureInfo::CurrentCulture); }
		}
		else Object = nullptr;

		return Result;
	}

	int GetPrivateData(Guid Name, [Out] unsigned int% DataSize)
	{
		pin_ptr<unsigned int> PinnedDataSize = &DataSize;

		return pObject->GetPrivateData(IID_Converter::ToNative(Name), PinnedDataSize, 0);
	}

	int GetPrivateData(Guid Name, unsigned int DataSize, [Out] UnmanagedMemory^% Data, [Out] Unknown^% Unknown)
	{
		pin_ptr<unsigned int> PinnedDataSize = &DataSize;

		Data = nullptr;
		Unknown = nullptr;
		
		int Result = pObject->GetPrivateData(IID_Converter::ToNative(Name), PinnedDataSize, 0);

		if (DataSize > 0)
		{
			Data = gcnew UnmanagedMemory(DataSize);
			Result = pObject->GetPrivateData(IID_Converter::ToNative(Name), PinnedDataSize, Data->pMemory);

			if (DataSize == sizeof(IntPtr))
			{
				void* pData = 0;
				memcpy(&pData, Data->pMemory, sizeof(IntPtr));

				try
				{
					Unknown = (Xtro::MDX::Unknown^)Interfaces[IntPtr(pData)];
					Data = nullptr;
				}
				catch (KeyNotFoundException^) { }
			}
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