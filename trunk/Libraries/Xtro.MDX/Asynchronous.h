public ref class Asynchronous : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Asynchronous);

internal:
	ID3D10Asynchronous* pAsynchronous;

	Asynchronous(IntPtr Asynchronous) : DeviceChild(Asynchronous)
	{	
		pAsynchronous = (ID3D10Asynchronous*)Asynchronous.ToPointer();
	}

public:
	void Begin()
	{
		pAsynchronous->Begin();
	}

	void End()
	{
		pAsynchronous->End();
	}

	int GetData(UnmanagedMemory^ Data, unsigned int DataSize, AsyncGetDataFlag GetDataFlags)
	{
		void* pData = Data == nullptr ? 0 : (void*)Data->pMemory;

		return pAsynchronous->GetData(pData, DataSize, (unsigned int)GetDataFlags);
	}

	unsigned int GetDataSize()
	{
		return pAsynchronous->GetDataSize();
	}
};