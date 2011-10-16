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

	int GetPrivateData(Guid Guid, [Out] unsigned int% DataSize)
	{
		pin_ptr<unsigned int> PinnedDataSize = &DataSize;

		return pDeviceChild->GetPrivateData(IID_Converter::ToNative(Guid), PinnedDataSize, 0);
	}

	int GetPrivateData(Guid Guid, unsigned int DataSize, UnmanagedMemory^ Data)
	{
		pin_ptr<unsigned int> PinnedDataSize = &DataSize;
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		
		return pDeviceChild->GetPrivateData(IID_Converter::ToNative(Guid), PinnedDataSize, pData);
	}

	int GetPrivateData(Guid Guid, [Out] Unknown^% Unknown)
	{
		void* pData = 0;
		
		unsigned int DataSize = Marshal::SizeOf(IntPtr::typeid);
		int Result = pDeviceChild->GetPrivateData(IID_Converter::ToNative(Guid), &DataSize, &pData);

		if (pData)
		{
			try	{ Unknown = (Xtro::MDX::Unknown^)Interfaces[IntPtr(pData)];	}
			catch (KeyNotFoundException^) { Unknown = nullptr; }
		}

		return Result;
	}

	int SetPrivateData(Guid Guid, unsigned int DataSize, UnmanagedMemory^ Data)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		int Result = pDeviceChild->SetPrivateData(IID_Converter::ToNative(Guid), DataSize, pData);

		return Result;
	}

	int SetPrivateDataInterface(Guid Guid, Unknown^ Data)
	{
		IUnknown* pData = Data == nullptr ? 0 : Data->pUnknown;

		int Result = pDeviceChild->SetPrivateDataInterface(IID_Converter::ToNative(Guid), pData);

		return Result;
	}
};