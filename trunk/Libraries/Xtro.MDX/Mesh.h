public ref class Mesh sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3DX10Mesh);

internal:
	ID3DX10Mesh* pMesh;

	Mesh(IntPtr Mesh) : Unknown(Mesh)
	{	
		pMesh = (ID3DX10Mesh*)Mesh.ToPointer();
	}

public:
	int SetVertexData(unsigned int Buffer, UnmanagedMemory^ Data)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		return pMesh->SetVertexData(Buffer, pData);
	}

	int SetIndexData(UnmanagedMemory^ Data, unsigned int Indices)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		return pMesh->SetIndexData(pData, Indices);
	}

	int GenerateAdjacencyAndPointReps(float Epsilon)
	{
		return pMesh->GenerateAdjacencyAndPointReps(Epsilon);
	}

	int GenerateGeometyShaderAdjacency()
	{
		return pMesh->GenerateGSAdjacency();
	}

	int GetIndexBuffer([Out] MeshBuffer^% IndexBuffer)
	{
		ID3DX10MeshBuffer* pIndexBuffer = 0;
		int Result = pMesh->GetIndexBuffer(&pIndexBuffer);

		if (pIndexBuffer)
		{	
			try { IndexBuffer = (MeshBuffer^)Interfaces[IntPtr(pIndexBuffer)]; }
			catch (KeyNotFoundException^) { IndexBuffer = gcnew MeshBuffer(IntPtr(pIndexBuffer)); }
		}
		else IndexBuffer = nullptr;

		return Result;
	}
};