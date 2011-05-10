public ref class Buffer : Resource
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Buffer);

	int Size;

internal:
	ID3D10Buffer* pBuffer;

	Buffer(IntPtr Buffer) : Resource(Buffer)
	{	
		Size = -1;

		pBuffer = (ID3D10Buffer*)Buffer.ToPointer();
	}

public:
	void GetDescription([Out] BufferDescription% Description)
	{
		pin_ptr<BufferDescription> PinnedDescription = &Description;
		pBuffer->GetDesc((D3D10_BUFFER_DESC*)PinnedDescription);

		Size = PinnedDescription->ByteWidth;
	}

	int Map(Map MapType, unsigned int MapFlags, [Out] UnmanagedMemory^% Data)
	{
		void* pData = 0;
		int Result = pBuffer->Map((D3D10_MAP)MapType, MapFlags, &pData);

		if (Size < 0)
		{
			D3D10_BUFFER_DESC Description;
			pBuffer->GetDesc(&Description);

			Size = Description.ByteWidth;
		}

		if (pData) Data = gcnew UnmanagedMemory(IntPtr(pData), Size);
		else Data = nullptr;

		return Result;
	}

	void Unmap()
	{
		pBuffer->Unmap();
	}
};