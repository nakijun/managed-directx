public value class ImageLoadInfo : IEquatable<ImageLoadInfo>
{
private:
	D3DX10_IMAGE_INFO* pSourceInfo;

internal:
	inline void Marshal(D3DX10_IMAGE_LOAD_INFO* Native)
	{
		Native->Width = Width;
		Native->Height = Height;
		Native->Depth = Depth;
		Native->FirstMipLevel = FirstMipLevel;
		Native->MipLevels = MipLevels;
		Native->Usage = (D3D10_USAGE)Usage;
		Native->BindFlags = (unsigned int)BindFlags;
		Native->CpuAccessFlags = (unsigned int)CpuAccessFlags;
		Native->MiscFlags = (unsigned int)MiscellaneousFlags;
		Native->Format = (DXGI_FORMAT)Format;
		Native->Filter = (unsigned int)Filter;
		Native->MipFilter = (unsigned int)MipFilter;

		if (SourceInfo.HasValue) 
		{
			pSourceInfo = new D3DX10_IMAGE_INFO();
			pin_ptr<ImageInfo> PinnedSourceInfo = &SourceInfo.Value;
			memcpy(pSourceInfo, PinnedSourceInfo, sizeof(D3DX10_IMAGE_INFO));

			Native->pSrcInfo = pSourceInfo;
		}
		else Native->pSrcInfo = 0;
	}

	inline void Unmarshal()
	{
		if (pSourceInfo)
		{
			delete pSourceInfo; 
			pSourceInfo = 0;
		}
	}

public:
	unsigned int Width;
	unsigned int Height;
	unsigned int Depth;
	unsigned int FirstMipLevel;
	unsigned int MipLevels;
	Direct3D10::Usage Usage;
	BindFlag BindFlags;
	CpuAccessFlag CpuAccessFlags;
	ResourceMiscellaneousFlag MiscellaneousFlags;
	Format Format;
	FilterFlag Filter;
	FilterFlag MipFilter;
	Nullable<ImageInfo> SourceInfo;

	static property ImageLoadInfo Default
	{
		ImageLoadInfo get()
		{
			ImageLoadInfo Result;
			Result.Width = D3DX10_DEFAULT;
			Result.Height = D3DX10_DEFAULT;
			Result.Depth = D3DX10_DEFAULT;
			Result.FirstMipLevel = D3DX10_DEFAULT;
			Result.MipLevels = D3DX10_DEFAULT;
			Result.Usage = (Direct3D10::Usage) D3DX10_DEFAULT;
			Result.BindFlags = (BindFlag)D3DX10_DEFAULT;
			Result.CpuAccessFlags = (CpuAccessFlag)D3DX10_DEFAULT;
			Result.MiscellaneousFlags = (ResourceMiscellaneousFlag)D3DX10_DEFAULT;
			Result.Format = (Direct3D10::Format)DXGI_FORMAT_FROM_FILE;
			Result.Filter = (FilterFlag)D3DX10_DEFAULT;
			Result.MipFilter = (FilterFlag)D3DX10_DEFAULT;	

			return Result;
		}
	}

	static bool operator == (ImageLoadInfo Left, ImageLoadInfo Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ImageLoadInfo Left, ImageLoadInfo Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Width ^
			Height ^
			Depth ^
			FirstMipLevel ^
			MipLevels ^
			(int)Usage ^
			(int)BindFlags ^
			(int)CpuAccessFlags ^
			(int)MiscellaneousFlags ^
			(int)Format ^
			(int)Filter ^
			(int)MipFilter ^
			(SourceInfo.HasValue ? SourceInfo.Value.GetHashCode() : 1);
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ImageLoadInfo Value)
	{
		pin_ptr<D3DX10_IMAGE_INFO*> PinnedThis = &pSourceInfo;
		pin_ptr<ImageLoadInfo> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(ImageLoadInfo::typeid)) == 0;
	}

	static bool Equals(ImageLoadInfo% Value1, ImageLoadInfo% Value2)
	{
		pin_ptr<ImageLoadInfo> PinnedValue1 = &Value1;
		pin_ptr<ImageLoadInfo> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(ImageLoadInfo::typeid)) == 0;
	}
};