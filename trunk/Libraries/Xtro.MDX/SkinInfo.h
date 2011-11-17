public ref class SkinInfo sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3DX10SkinInfo);

internal:
	ID3DX10SkinInfo* pSkinInfo;

	SkinInfo(IntPtr SkinInfo) : Unknown(SkinInfo)
	{	
		pSkinInfo = (ID3DX10SkinInfo*)SkinInfo.ToPointer();
	}

public:
	int AddBoneInfluences(unsigned int BoneIndex, unsigned int InfluenceCount, Generic::UnmanagedMemory<unsigned int>^ Indices, Generic::UnmanagedMemory<float>^ Weights)
	{
		unsigned int* pIndices = Indices == nullptr ? 0 : (unsigned int*)Indices->pMemory;
		float* pWeights = Weights == nullptr ? 0 : (float*)Weights->pMemory;

		return pSkinInfo->AddBoneInfluences(BoneIndex, InfluenceCount, pIndices, pWeights);
	}

	int AddBones(unsigned int Count)
	{
		return pSkinInfo->AddBones(Count);
	}

	int AddVertices(unsigned int Count)
	{
		return pSkinInfo->AddVertices(Count);
	}

	int ClearBoneInfluences(unsigned int BoneIndex)
	{
		return pSkinInfo->ClearBoneInfluences(BoneIndex);
	}

	int Compact(unsigned int MaximumPerVertexInfluences, SkinInfoScale ScaleMode, float MinimumWeight)
	{
		return pSkinInfo->Compact(MaximumPerVertexInfluences, (unsigned int)ScaleMode, MinimumWeight);
	}

	int DoSoftwareSkinning(unsigned int StartVertex, unsigned int VertexCount, UnmanagedMemory^ SourceVertices, unsigned int SourceStride, UnmanagedMemory^ DestinationVertices, unsigned int DestinationStride, Generic::UnmanagedMemory<Matrix>^ BoneMatrices, Generic::UnmanagedMemory<Matrix>^ InverseTransposeBoneMatrices, Generic::UnmanagedMemory<SkinningChannel>^ ChannelDescriptions, unsigned int NumberOfChannels)
	{
		void* pSourceVertices = SourceVertices == nullptr ? 0 : (void*)SourceVertices->pMemory;
		void* pDestinationVertices = DestinationVertices == nullptr ? 0 : (void*)DestinationVertices->pMemory;
		D3DXMATRIX* pBoneMatrices = BoneMatrices == nullptr ? 0 : (D3DXMATRIX*)BoneMatrices->pMemory;
		D3DXMATRIX* pInverseTransposeBoneMatrices = InverseTransposeBoneMatrices == nullptr ? 0 : (D3DXMATRIX*)InverseTransposeBoneMatrices->pMemory;
		D3DX10_SKINNING_CHANNEL* pChannelDescriptions = ChannelDescriptions == nullptr ? 0 : (D3DX10_SKINNING_CHANNEL*)ChannelDescriptions->pMemory;
	
		return pSkinInfo->DoSoftwareSkinning(StartVertex, VertexCount, pSourceVertices, SourceStride, pDestinationVertices, DestinationStride, pBoneMatrices, pInverseTransposeBoneMatrices, pChannelDescriptions, NumberOfChannels);
	}

	int FindBoneInfluenceIndex(unsigned int BoneIndex, unsigned int VertexIndex, [Out] unsigned int% InfluenceIndex)
	{
		pin_ptr<unsigned int> PinnedInfluenceIndex = &InfluenceIndex;

		return pSkinInfo->FindBoneInfluenceIndex(BoneIndex, VertexIndex, PinnedInfluenceIndex);
	}

	int GetBoneInfluence(unsigned int BoneIndex, unsigned int InfluenceIndex, [Out] float% Weight)
	{
		pin_ptr<float> PinnedWeight = &Weight;

		return pSkinInfo->GetBoneInfluence(BoneIndex, InfluenceIndex, PinnedWeight);
	}

	int GetBoneInfluenceCount(unsigned int BoneIndex)
	{
		return pSkinInfo->GetBoneInfluenceCount(BoneIndex);
	}

	int GetBoneInfluences(unsigned int BoneIndex, unsigned int Offset, unsigned int Count, Generic::UnmanagedMemory<unsigned int>^ DestinationIndices, Generic::UnmanagedMemory<float>^ DestinationWeights)
	{
		unsigned int* pDestinationIndices = DestinationIndices == nullptr ? 0 : (unsigned int*)DestinationIndices->pMemory;
		float* pDestinationWeights = DestinationWeights == nullptr ? 0 : (float*)DestinationWeights->pMemory;

		return pSkinInfo->GetBoneInfluences(BoneIndex, Offset, Count, pDestinationIndices, pDestinationWeights);
	}

	int GetMaximumBoneInfluences()
	{
		return pSkinInfo->GetMaxBoneInfluences();
	}

	int GetNumberOfBones()
	{
		return pSkinInfo->GetNumBones();
	}

	int GetNumberOfVertices()
	{
		return pSkinInfo->GetNumVertices();
	}

	int RemapBones(unsigned int NewBoneCount, Generic::UnmanagedMemory<unsigned int>^ BoneRemap)
	{
		unsigned int* pBoneRemap = BoneRemap == nullptr ? 0 : (unsigned int*)BoneRemap->pMemory;

		return pSkinInfo->RemapBones(NewBoneCount, pBoneRemap);
	}

	int RemapVertices(unsigned int NewVertexCount, Generic::UnmanagedMemory<unsigned int>^ VertexRemap)
	{
		unsigned int* pVertexRemap = VertexRemap == nullptr ? 0 : (unsigned int*)VertexRemap->pMemory;

		return pSkinInfo->RemapVertices(NewVertexCount, pVertexRemap);
	}

	int RemoveBone(unsigned int Index)
	{
		return pSkinInfo->RemoveBone(Index);
	}

	int SetBoneInfluence(unsigned int BoneIndex, unsigned int InfluenceIndex, float Weight)
	{
		return pSkinInfo->SetBoneInfluence(BoneIndex, InfluenceIndex, Weight);
	}
};