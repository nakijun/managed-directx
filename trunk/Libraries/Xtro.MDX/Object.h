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

	int GetPrivateData(Guid Name, [Out] array<Byte>^% Data, unsigned int% Size, [Out] Unknown^% Unknown)
	{
		pin_ptr<unsigned int> PinnedSize = &Size;

		Data = nullptr;
		Unknown = nullptr;
		
		int Result = pObject->GetPrivateData(IID_Converter::ToNative(Name), PinnedSize, 0);

		if (Size > 0)
		{
			Data = gcnew array<Byte>(Size);
			pin_ptr<unsigned char> PinnedData = &Data[0];
			Result = pObject->GetPrivateData(IID_Converter::ToNative(Name), PinnedSize, PinnedData);

			if (Size == 4)
			{
				void* pData = 0;
				memcpy(&pData, PinnedData, 4);

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

	int SetPrivateData(Guid Name, unsigned int Size, array<Byte>^ Data)
	{
		pin_ptr<unsigned char> PinnedData = nullptr;
		if (Data != nullptr && Data->Length > 0) PinnedData = &Data[0];

		int Result = pObject->SetPrivateData(IID_Converter::ToNative(Name), Size, PinnedData);

		return Result;
	}

	int SetPrivateDataInterface(Guid Name, Unknown^ Unknown)
	{
		IUnknown* pUnknown = Unknown == nullptr ? 0 : Unknown->pUnknown;

		int Result = pObject->SetPrivateDataInterface(IID_Converter::ToNative(Name), pUnknown);

		return Result;
	}
};