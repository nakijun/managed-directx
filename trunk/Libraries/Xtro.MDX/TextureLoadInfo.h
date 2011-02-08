public value class TextureLoadInfo : IEquatable<TextureLoadInfo>
{
internal:
	D3D10_BOX* pSourceBox;
	D3D10_BOX* pDestinationBox;
					  
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

			Native->pSrcBox = pSourceBox;
		}
		else Native->pSrcBox = 0;

		if (DestinationBox.HasValue) 
		{
			pDestinationBox = new D3D10_BOX();
			pin_ptr<Box> PinnedDestinationBox = &DestinationBox.Value;
			memcpy(pDestinationBox, PinnedDestinationBox, sizeof(D3D10_BOX));

			Native->pDstBox = pDestinationBox;
		}
		else Native->pDstBox = 0;
	}

	inline void Unmarshal()
	{
		if (pSourceBox) delete pSourceBox; 
		if (pDestinationBox) delete pDestinationBox; 
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
			SourceBox.GetHashCode() ^
			DestinationBox.GetHashCode() ^
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
		if (SourceBox.HasValue != Value.SourceBox.HasValue) return false;
		bool SourceBoxEquals = SourceBox.HasValue ? SourceBox.Value == Value.SourceBox.Value : true;
																										 
		if (DestinationBox.HasValue != Value.DestinationBox.HasValue) return false;
		bool DestinationBoxEquals = DestinationBox.HasValue ? DestinationBox.Value == Value.DestinationBox.Value : true;
																										 
		return
			SourceFirstMip == Value.SourceFirstMip &&
			DestinationFirstMip == Value.DestinationFirstMip &&
			NumberOfMips == Value.NumberOfMips &&
			SourceFirstElement == Value.SourceFirstElement &&
			DestinationFirstElement == Value.DestinationFirstElement &&
			NumberOfElements == Value.NumberOfElements &&
			Filter == Value.Filter &&
			MipFilter == Value.MipFilter &&
			SourceBoxEquals &&
			DestinationBoxEquals;
	}

	static bool Equals(TextureLoadInfo% Value1, TextureLoadInfo% Value2)
	{
		if (Value1.SourceBox.HasValue != Value2.SourceBox.HasValue) return false;
		bool SourceBoxEquals = Value1.SourceBox.HasValue ? Value1.SourceBox.Value == Value2.SourceBox.Value : true;
																										 
		if (Value1.DestinationBox.HasValue != Value2.DestinationBox.HasValue) return false;
		bool DestinationBoxEquals = Value1.DestinationBox.HasValue ? Value1.DestinationBox.Value == Value2.DestinationBox.Value : true;
																										 
		return
			Value1.SourceFirstMip == Value2.SourceFirstMip &&
			Value1.DestinationFirstMip == Value2.DestinationFirstMip &&
			Value1.NumberOfMips == Value2.NumberOfMips &&
			Value1.SourceFirstElement == Value2.SourceFirstElement &&
			Value1.DestinationFirstElement == Value2.DestinationFirstElement &&
			Value1.NumberOfElements == Value2.NumberOfElements &&
			Value1.Filter == Value2.Filter &&
			Value1.MipFilter == Value2.MipFilter &&
			SourceBoxEquals &&
			DestinationBoxEquals;
	}
};