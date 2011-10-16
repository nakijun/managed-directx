public ref class Blob sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Blob);

internal:
	ID3D10Blob* pBlob;

	
	Blob(IntPtr Blob) : Unknown(Blob)
	{	
		pBlob = (ID3D10Blob*)Blob.ToPointer();
	}

public:
	UnmanagedMemory^ GetBufferPointer()
	{
		void* pResult = pBlob->GetBufferPointer();

		UnmanagedMemory^ Result;

		Result = pResult ? gcnew UnmanagedMemory(IntPtr(pResult), 0) : nullptr;

		return Result;
	}

	SIZE_T GetBufferSize()
	{
		return pBlob->GetBufferSize();
	}
};