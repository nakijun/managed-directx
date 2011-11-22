public value class TextureLoadInfo : IEquatable<TextureLoadInfo>
{
private:
	D3D10_BOX* pSourceBox;
	D3D10_BOX* pDestinationBox;
					  
internal:
	inline void Marshal(D3DX10_TEXTURE_LOAD_INFO* Native)
	{
		Native->SrcFirstMip = SourceFirstMip;
		Native->DstFirstMip = DestinationFirstMip;
		Native->NumMips = NumberOfMips;
		Native->SrcFirstElement = SourceFirstElement;
		Native->DstFirstElement = DestinationFirstElement;
		Native->NumElements = NumberOfElements;
		Native->Filter = (unsigned int)Filter;
		Native->MipFilter = (unsigned int)MipFilter;

		if (SourceBox.HasValue) 
		{
			pSourceBox = new D3D10_BOX();
			pin_ptr<Box> PinnedSourceBox = &SourceBox.Value;
			memcpy(pSourceBox, PinnedSourceBox, sizeof(D3D10_BOX));
		}
		else pSourceBox = 0;

		Native->pSrcBox = pSourceBox;

		if (DestinationBox.HasValue) 
		{
			pDestinationBox = new D3D10_BOX();
			pin_ptr<Box> PinnedDestinationBox = &DestinationBox.Value;
			memcpy(pDestinationBox, PinnedDestinationBox, sizeof(D3D10_BOX));
		}
		else pDestinationBox = 0;

		Native->pDstBox = pDestinationBox;
	}

	inline void Unmarshal()
	{
		if (pSourceBox)
		{
			delete pSourceBox; 
			pSourceBox = 0;
		}

		if (pDestinationBox)
		{
			delete pDestinationBox; 
			pDestinationBox = 0;
		}
	}

public:
    Nullable<Box> SourceBox;
    Nullable<Box> DestinationBox;
    unsigned int SourceFirstMip;
    unsigned int DestinationFirstMip;
    unsigned int NumberOfMips;
    unsigned int SourceFirstElement;
    unsigned int DestinationFirstElement;
    unsigned int NumberOfElements;
    FilterFlag Filter;
    FilterFlag MipFilter;

	static bool operator == (TextureLoadInfo Left, TextureLoadInfo Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (TextureLoadInfo Left, TextureLoadInfo Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(SourceBox.HasValue ? SourceBox.Value.GetHashCode() : 1) ^
			(DestinationBox.HasValue ? DestinationBox.Value.GetHashCode() : 1) ^
			SourceFirstMip ^
			DestinationFirstMip ^
			NumberOfMips ^
			SourceFirstElement ^
			DestinationFirstElement ^
			NumberOfElements ^
			(int)Filter ^
			(int)MipFilter;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(TextureLoadInfo Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(TextureLoadInfo::typeid)) == 0;
	}

	static bool Equals(TextureLoadInfo% Value1, TextureLoadInfo% Value2)
	{
		pin_ptr<TextureLoadInfo> PinnedValue1 = &Value1;
		pin_ptr<TextureLoadInfo> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(TextureLoadInfo::typeid)) == 0;
	}
};