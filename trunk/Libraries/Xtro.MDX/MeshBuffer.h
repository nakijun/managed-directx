public ref class MeshBuffer : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3DX10MeshBuffer);

internal:
	ID3DX10MeshBuffer* pMeshBuffer;

	MeshBuffer(IntPtr MeshBuffer) : Unknown(MeshBuffer)
	{	
		pMeshBuffer = (ID3DX10MeshBuffer*)MeshBuffer.ToPointer();
	}

public:
	int Map([Out] UnmanagedMemory^% Data)
	{
		SIZE_T Size;
		void* pData = 0;
		int Result = pMeshBuffer->Map(&pData, &Size);

		if (pData) Data = gcnew UnmanagedMemory(IntPtr(pData), (unsigned int)Size);
		else Data = nullptr;

		return Result;
	}

	int Unmap()
	{
		return pMeshBuffer->Unmap();
	}
};