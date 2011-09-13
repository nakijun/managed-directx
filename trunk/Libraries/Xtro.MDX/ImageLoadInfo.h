public value class ImageLoadInfo : IEquatable<ImageLoadInfo>
{
internal:
	D3DX10_IMAGE_INFO* pSourceInfo;

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
		if (pSourceInfo) delete pSourceInfo; 
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
		if (SourceInfo.HasValue != Value.SourceInfo.HasValue) return false;
		bool SourceInfoEquals = SourceInfo.HasValue ? SourceInfo.Value == Value.SourceInfo.Value : true;
																										 
		return
			Width == Value.Width &&
			Height == Value.Height &&
			Depth == Value.Depth &&
			FirstMipLevel == Value.FirstMipLevel &&
			MipLevels == Value.MipLevels &&
			Usage == Value.Usage &&
			BindFlags == Value.BindFlags &&
			CpuAccessFlags == Value.CpuAccessFlags &&
			MiscellaneousFlags == Value.MiscellaneousFlags &&
			Format == Value.Format &&
			Filter == Value.Filter &&
			MipFilter == Value.MipFilter;
	}

	static bool Equals(ImageLoadInfo% Value1, ImageLoadInfo% Value2)
	{
		if (Value1.SourceInfo.HasValue != Value2.SourceInfo.HasValue) return false;
		bool SourceInfoEquals = Value1.SourceInfo.HasValue ? Value1.SourceInfo.Value == Value2.SourceInfo.Value : true;
																										 
		return
			Value1.Width == Value2.Width &&
			Value1.Height == Value2.Height &&
			Value1.Depth == Value2.Depth &&
			Value1.FirstMipLevel == Value2.FirstMipLevel &&
			Value1.MipLevels == Value2.MipLevels &&
			Value1.Usage == Value2.Usage &&
			Value1.BindFlags == Value2.BindFlags &&
			Value1.CpuAccessFlags == Value2.CpuAccessFlags &&
			Value1.MiscellaneousFlags == Value2.MiscellaneousFlags &&
			Value1.Format == Value2.Format &&
			Value1.Filter == Value2.Filter &&
			Value1.MipFilter == Value2.MipFilter;
	}
};