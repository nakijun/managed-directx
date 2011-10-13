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
	int CloneMesh(MeshFlag Flags, String^ PositionSemantic, array<InputElementDescription>^ Description, unsigned int DeclarationCount, [Out] Mesh^% CloneMesh)
	{
		int Result = 0;
		ID3DX10Mesh* pMesh = 0;

		D3D10_INPUT_ELEMENT_DESC* pDescription = 0;
		IntPtr pPositionSemantic = Marshal::StringToHGlobalAnsi(PositionSemantic);
		try
		{
			if (Description != nullptr && Description->Length > 0)
			{
				pDescription = new D3D10_INPUT_ELEMENT_DESC[Description->Length];
				for (int ElementNo = 0; ElementNo < Description->Length; ElementNo++)
				{
					Description[ElementNo].Marshal(&pDescription[ElementNo]);
				}
			}

			Result = pMesh->CloneMesh((unsigned int)Flags, (LPCSTR)pPositionSemantic.ToPointer(), pDescription, DeclarationCount, &pMesh);
		}
		finally
		{
			Marshal::FreeHGlobal(pPositionSemantic);

			if (Description != nullptr)
			{
				for (int ElementNo = 0; ElementNo < Description->Length; ElementNo++)
				{
					Description[ElementNo].Unmarshal();
				}
			}
			if (pDescription) delete[] pDescription;
		}

		if (pMesh)
		{
			try { CloneMesh = (Xtro::MDX::Direct3DX10::Mesh^)Interfaces[IntPtr(pMesh)]; }
			catch (KeyNotFoundException^) { CloneMesh = gcnew Xtro::MDX::Direct3DX10::Mesh(IntPtr(pMesh)); }					
		}
		else CloneMesh = nullptr;

		return Result;
	}

	int CommitToDevice()
	{
		return pMesh->CommitToDevice();
	}

	int Discard(MeshDiscardFlag Discard)
	{
		return pMesh->Discard((D3DX10_MESH_DISCARD_FLAGS)Discard);
	}

	int DrawSubset(unsigned int AttributeId)
	{
		return pMesh->DrawSubset(AttributeId);
	}

	int DrawSubsetInstanced(unsigned int AttributeId, unsigned int InstanceCount, unsigned int StartInstanceLocation)
	{
		return pMesh->DrawSubsetInstanced(AttributeId, InstanceCount, StartInstanceLocation);
	}

	int GenerateAdjacencyAndPointReps(float Epsilon)
	{
		return pMesh->GenerateAdjacencyAndPointReps(Epsilon);
	}

	int GenerateAttributeBufferFromTable()
	{
		return pMesh->GenerateAttributeBufferFromTable();
	}

	int GenerateGeometyShaderAdjacency()
	{
		return pMesh->GenerateGSAdjacency();
	}

	int GetAdjacencyBuffer([Out] MeshBuffer^% Adjacency)
	{
		ID3DX10MeshBuffer* pAdjacency = 0;
		int Result = pMesh->GetAdjacencyBuffer(&pAdjacency);

		if (pAdjacency)
		{	
			try { Adjacency = (MeshBuffer^)Interfaces[IntPtr(pAdjacency)]; }
			catch (KeyNotFoundException^) { Adjacency = gcnew MeshBuffer(IntPtr(pAdjacency)); }
		}
		else Adjacency = nullptr;

		return Result;
	}

	int GetAttributeBuffer([Out] MeshBuffer^% AttributeBuffer)
	{
		ID3DX10MeshBuffer* pAttributeBuffer = 0;
		int Result = pMesh->GetAttributeBuffer(&pAttributeBuffer);

		if (pAttributeBuffer)
		{	
			try { AttributeBuffer = (MeshBuffer^)Interfaces[IntPtr(pAttributeBuffer)]; }
			catch (KeyNotFoundException^) { AttributeBuffer = gcnew MeshBuffer(IntPtr(pAttributeBuffer)); }
		}
		else AttributeBuffer = nullptr;

		return Result;
	}

	int GetAttributeTable(array<AttributeRange>^ AttribTable, unsigned int% AttribTableSize)
	{
		pin_ptr<unsigned int> PinnedAttribTableSize = &AttribTableSize;
		pin_ptr<AttributeRange> PinnedAttribTable = AttribTable != nullptr && AttribTable->Length > 0 ? &AttribTable[0] : nullptr;

		return pMesh->GetAttributeTable((D3DX10_ATTRIBUTE_RANGE*)PinnedAttribTable, PinnedAttribTableSize);
	}

	int GetDeviceIndexBuffer([Out] Buffer^% IndexBuffer)
	{
		ID3D10Buffer* pIndexBuffer = 0;

		int Result = pMesh->GetDeviceIndexBuffer(&pIndexBuffer);

		if (pIndexBuffer)
		{
			try { IndexBuffer = (Buffer^)Interfaces[IntPtr(pIndexBuffer)]; }
			catch (KeyNotFoundException^) { IndexBuffer = gcnew Buffer(IntPtr(pIndexBuffer)); }					
		}
		else IndexBuffer = nullptr;

		return Result;
	}

	int GetDeviceVertexBuffer(unsigned int Buffer, [Out] Xtro::MDX::Direct3D10::Buffer^% VertexBuffer)
	{
		ID3D10Buffer* pVertexBuffer = 0;

		int Result = pMesh->GetDeviceVertexBuffer(Buffer, &pVertexBuffer);

		if (pVertexBuffer)
		{
			try { VertexBuffer = (Xtro::MDX::Direct3D10::Buffer^)Interfaces[IntPtr(pVertexBuffer)]; }
			catch (KeyNotFoundException^) { VertexBuffer = gcnew Xtro::MDX::Direct3D10::Buffer(IntPtr(pVertexBuffer)); }					
		}
		else VertexBuffer = nullptr;

		return Result;
	}

	unsigned int GetFaceCount()
	{
		return pMesh->GetFaceCount();
	}

	MeshFlag GetFlags()
	{
		return (MeshFlag)pMesh->GetFlags();
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

	int GetPointRepBuffer([Out] MeshBuffer^% PointReps)
	{
		ID3DX10MeshBuffer* pPointReps = 0;
		int Result = pMesh->GetPointRepBuffer(&pPointReps);

		if (pPointReps)
		{	
			try { PointReps = (MeshBuffer^)Interfaces[IntPtr(pPointReps)]; }
			catch (KeyNotFoundException^) { PointReps = gcnew MeshBuffer(IntPtr(pPointReps)); }
		}
		else PointReps = nullptr;

		return Result;
	}

	int GetVertexBuffer(unsigned int Buffer, [Out] MeshBuffer^% VertexBuffer)
	{
		ID3DX10MeshBuffer* pVertexBuffer = 0;

		int Result = pMesh->GetVertexBuffer(Buffer, &pVertexBuffer);

		if (pVertexBuffer)
		{
			try { VertexBuffer = (MeshBuffer^)Interfaces[IntPtr(pVertexBuffer)]; }
			catch (KeyNotFoundException^) { VertexBuffer = gcnew MeshBuffer(IntPtr(pVertexBuffer)); }					
		}
		else VertexBuffer = nullptr;

		return Result;
	}

	unsigned int GetVertexBufferCount()
	{
		return pMesh->GetVertexBufferCount();
	}

	unsigned int GetVertexCount()
	{
		return pMesh->GetVertexCount();
	}

	int GetVertexDescription([Out] Xtro::MDX::Generic::UnmanagedMemory<InputElementDescription>^% Description, unsigned int% DeclarationCount)
	{
		pin_ptr<unsigned int> PinnedDeclarationCount = &DeclarationCount;

		const D3D10_INPUT_ELEMENT_DESC* pDescription = 0;

		int Result = pMesh->GetVertexDescription(&pDescription, PinnedDeclarationCount);

		if (pDescription) Description = gcnew Xtro::MDX::Generic::UnmanagedMemory<InputElementDescription>(IntPtr((void*)pDescription), sizeof(D3D10_INPUT_ELEMENT_DESC) * DeclarationCount);
		else Description = nullptr;

		return Result;
	}

	int Intersect(Vector3% RayPosisiton, Vector3% RayDirection, [Out] unsigned int% HitCount, [Out] unsigned int% FaceIndex, [Out] float% U, [Out] float% V, [Out] float% Distance, [Out] Blob^% AllHits)
	{
		pin_ptr<Vector3> PinnedRayPosisiton = &RayPosisiton;
		pin_ptr<Vector3> PinnedRayDirection = &RayDirection;
		pin_ptr<unsigned int> PinnedHitCount = &HitCount;
		pin_ptr<unsigned int> PinnedFaceIndex = &FaceIndex;
		pin_ptr<float> PinnedU = &U;
		pin_ptr<float> PinnedV = &V;
		pin_ptr<float> PinnedDistance = &Distance;

		ID3D10Blob* pAllHits = 0;

		int Result = pMesh->Intersect((D3DXVECTOR3*)PinnedRayPosisiton, (D3DXVECTOR3*)PinnedRayDirection, PinnedHitCount, PinnedFaceIndex, PinnedU, PinnedV, PinnedDistance, &pAllHits);

		if (pAllHits)
		{
			try { AllHits = (Blob^)Interfaces[IntPtr(pAllHits)]; }
			catch (KeyNotFoundException^) { AllHits = gcnew Blob(IntPtr(pAllHits)); }					
		}
		else AllHits = nullptr;

		return Result;
	}

	int IntersectSubset(unsigned int AttributeId, Vector3% RayPosisiton, Vector3% RayDirection, [Out] unsigned int% HitCount, [Out] unsigned int% FaceIndex, [Out] float% U, [Out] float% V, [Out] float% Distance, [Out] Blob^% AllHits)
	{
		pin_ptr<Vector3> PinnedRayPosisiton = &RayPosisiton;
		pin_ptr<Vector3> PinnedRayDirection = &RayDirection;
		pin_ptr<unsigned int> PinnedHitCount = &HitCount;
		pin_ptr<unsigned int> PinnedFaceIndex = &FaceIndex;
		pin_ptr<float> PinnedU = &U;
		pin_ptr<float> PinnedV = &V;
		pin_ptr<float> PinnedDistance = &Distance;

		ID3D10Blob* pAllHits = 0;

		int Result = pMesh->IntersectSubset(AttributeId, (D3DXVECTOR3*)PinnedRayPosisiton, (D3DXVECTOR3*)PinnedRayDirection, PinnedHitCount, PinnedFaceIndex, PinnedU, PinnedV, PinnedDistance, &pAllHits);

		if (pAllHits)
		{
			try { AllHits = (Blob^)Interfaces[IntPtr(pAllHits)]; }
			catch (KeyNotFoundException^) { AllHits = gcnew Blob(IntPtr(pAllHits)); }					
		}
		else AllHits = nullptr;

		return Result;
	}

	int Optimize(unsigned int Flags, Xtro::MDX::Generic::UnmanagedMemory<unsigned int>^ FaceRemap, [Out] Blob^% VertexRemap)
	{
		unsigned int* pFaceRemap = FaceRemap == nullptr ? 0 : (unsigned int*)FaceRemap->pMemory;

		ID3D10Blob* pVertexRemap = 0;

		int Result = pMesh->Optimize(Flags,	pFaceRemap, &pVertexRemap);

		if (pVertexRemap)
		{
			try { VertexRemap = (Blob^)Interfaces[IntPtr(pVertexRemap)]; }
			catch (KeyNotFoundException^) { VertexRemap = gcnew Blob(IntPtr(pVertexRemap)); }					
		}
		else VertexRemap = nullptr;

		return Result;
	}

	int SetAdjacencyData(Xtro::MDX::Generic::UnmanagedMemory<unsigned int>^ Adjacency)
	{
		unsigned int* pAdjacency = Adjacency == nullptr ? 0 : (unsigned int*)Adjacency->pMemory;

		return pMesh->SetAdjacencyData(pAdjacency);
	}

	int SetAttributeData(Xtro::MDX::Generic::UnmanagedMemory<unsigned int>^ Data)
	{
		unsigned int* pData = Data == nullptr ? 0 : (unsigned int*)Data->pMemory;

		return pMesh->SetAttributeData(pData);
	}

	int SetAttributeTable(Xtro::MDX::Generic::UnmanagedMemory<AttributeRange>^ AttribTable, unsigned int AttributeTableSize)
	{
		D3DX10_ATTRIBUTE_RANGE* pAttribTable = AttribTable == nullptr ? 0 : (D3DX10_ATTRIBUTE_RANGE*)AttribTable->pMemory;

		return pMesh->SetAttributeTable(pAttribTable, AttributeTableSize);
	}

	int SetIndexData(UnmanagedMemory^ Data, unsigned int Indices)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		return pMesh->SetIndexData(pData, Indices);
	}

	int SetPointRepData(Xtro::MDX::Generic::UnmanagedMemory<unsigned int>^ PointReps)
	{
		unsigned int* pPointReps = PointReps == nullptr ? 0 : (unsigned int*)PointReps->pMemory;

		return pMesh->SetPointRepData(pPointReps);
	}
	
	int SetVertexData(unsigned int Buffer, UnmanagedMemory^ Data)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		return pMesh->SetVertexData(Buffer, pData);
	}
};