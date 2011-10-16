public ref class Include sealed : Interface
{
private:
	//static Guid IID = IID_Converter::ToManaged(IID_ID3D10Include);

internal:
	ID3D10Include* pInclude;

	Include(IntPtr Include) : Interface(Include)
	{	
		pInclude = (ID3D10Include*)Include.ToPointer();
	}

public:
	int Close(UnmanagedMemory^ Data)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		
		return pInclude->Close(pData);
	}

	int Open(IncludeType IncludeType, String^ FileName, UnmanagedMemory^ ParentData, [Out] UnmanagedMemory^% Data, [Out] unsigned int% Bytes)
	{
		void* pParentData = ParentData == nullptr ? 0 : ParentData->pMemory;
		pin_ptr<unsigned int> PinnedBytes = &Bytes;

		int Result = 0;
		void* pData = 0;

		IntPtr pFileName = Marshal::StringToHGlobalAnsi(FileName);
		try { Result = pInclude->Open((D3D10_INCLUDE_TYPE)IncludeType, (LPCSTR)pFileName.ToPointer(), pParentData, (const void**)&pData, PinnedBytes); }
		finally { Marshal::FreeHGlobal(pFileName); }

		Data = pData ? gcnew UnmanagedMemory(IntPtr(pData), Bytes) : nullptr;

		return Result;
	}
};