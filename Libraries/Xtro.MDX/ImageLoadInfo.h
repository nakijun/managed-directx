namespace Xtro
{
namespace MDX
{
namespace Direct3DX10
{
	public value class ImageLoadInfo : IEquatable<ImageLoadInfo>
	{
	internal:
		ImageLoadInfo(D3DX10_IMAGE_LOAD_INFO* Native)
		{
			Width = Native->Width;
			Height = Native->Height;
			Depth = Native->Depth;
			FirstMipLevel = Native->FirstMipLevel;
			MipLevels = Native->MipLevels;
			Usage = (Xtro::MDX::Direct3D10::Usage)Native->Usage;
			BindFlags = (Xtro::MDX::Direct3D10::BindFlag)Native->BindFlags;
			CPU_AccessFlags = (Xtro::MDX::Direct3D10::CPU_AccessFlag)Native->CpuAccessFlags;
			MiscFlags = (Xtro::MDX::Direct3D10::ResourceMiscFlag)Native->MiscFlags;
			Format = (Xtro::MDX::DXGI::Format)Native->Format;
			Filter = (Xtro::MDX::Direct3DX10::FilterFlag)Native->Filter;
			MipFilter = (Xtro::MDX::Direct3DX10::FilterFlag)Native->MipFilter;
			
			pin_ptr<ImageInfo> PinnedSourceInfo = &SourceInfo;
			if (Native->pSrcInfo && ((D3DX10_IMAGE_INFO*)PinnedSourceInfo != Native->pSrcInfo)) memcpy(PinnedSourceInfo, Native->pSrcInfo, sizeof(PinnedSourceInfo));
		}
		
		D3DX10_IMAGE_LOAD_INFO ToNative()
		{
			D3DX10_IMAGE_LOAD_INFO Result;
			
			Result.Width = Width;
			Result.Height = Height;
			Result.Depth = Depth;
			Result.FirstMipLevel = FirstMipLevel;
			Result.MipLevels = MipLevels;
			Result.Usage = (D3D10_USAGE)Usage;
			Result.BindFlags = (D3D10_BIND_FLAG)BindFlags;
			Result.CpuAccessFlags = (D3D10_CPU_ACCESS_FLAG)CPU_AccessFlags;
			Result.MiscFlags = (D3D10_RESOURCE_MISC_FLAG)MiscFlags;
			Result.Format = (DXGI_FORMAT)Format;
			Result.Filter = (D3DX10_FILTER_FLAG)Filter;
			Result.MipFilter = (D3DX10_FILTER_FLAG)MipFilter;

			pin_ptr<ImageInfo> PinnedSourceInfo = &SourceInfo;
			Result.pSrcInfo = (D3DX10_IMAGE_INFO*)PinnedSourceInfo;

			return Result;
		}

	public:
		unsigned int Width;
		unsigned int Height;
		unsigned int Depth;
		unsigned int FirstMipLevel;
		unsigned int MipLevels;
		Xtro::MDX::Direct3D10::Usage Usage;
		BindFlag BindFlags;
		CPU_AccessFlag CPU_AccessFlags;
		ResourceMiscFlag MiscFlags;
		Format Format;
		FilterFlag Filter;
		FilterFlag MipFilter;
		ImageInfo SourceInfo;

		ImageLoadInfo(unsigned int Width, unsigned int Height, unsigned int Depth, unsigned int FirstMipLevel, unsigned int MipLevels, Xtro::MDX::Direct3D10::Usage Usage, BindFlag BindFlags, CPU_AccessFlag CPU_AccessFlags, ResourceMiscFlag MiscFlags, Xtro::MDX::DXGI::Format Format, FilterFlag Filter, FilterFlag MipFilter, ImageInfo SourceInfo)
		{
			this->Width = Width;
			this->Height = Height;
			this->Depth = Depth;
			this->FirstMipLevel = FirstMipLevel;
			this->MipLevels = MipLevels;
			this->Usage = Usage;
			this->BindFlags = BindFlags;
			this->CPU_AccessFlags = CPU_AccessFlags;
			this->MiscFlags = MiscFlags;
			this->Format = Format;
			this->Filter = Filter;
			this->MipFilter = MipFilter;
			this->SourceInfo = SourceInfo;
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
				(int)Width ^
				(int)Height ^
				(int)Depth ^
				(int)FirstMipLevel ^
				(int)MipLevels ^
				(int)Usage ^
				(int)BindFlags ^
				(int)CPU_AccessFlags ^
				(int)MiscFlags ^
				(int)Format ^
				(int)Filter ^
				(int)MipFilter ^
				SourceInfo.GetHashCode();
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(ImageLoadInfo Value)
		{
			return
				Width == Value.Width &&
				Height == Value.Height &&
				Depth == Value.Depth &&
				FirstMipLevel == Value.FirstMipLevel &&
				MipLevels == Value.MipLevels &&
				Usage == Value.Usage &&
				BindFlags == Value.BindFlags &&
				CPU_AccessFlags == Value.CPU_AccessFlags &&
				MiscFlags == Value.MiscFlags &&
				Format == Value.Format &&
				Filter == Value.Filter &&
				MipFilter == Value.MipFilter &&
				SourceInfo == Value.SourceInfo;
		}

		static bool Equals(ImageLoadInfo% Value1, ImageLoadInfo% Value2)
		{
			return
				Value1.Width == Value2.Width &&
				Value1.Height == Value2.Height &&
				Value1.Depth == Value2.Depth &&
				Value1.FirstMipLevel == Value2.FirstMipLevel &&
				Value1.MipLevels == Value2.MipLevels &&
				Value1.Usage == Value2.Usage &&
				Value1.BindFlags == Value2.BindFlags &&
				Value1.CPU_AccessFlags == Value2.CPU_AccessFlags &&
				Value1.MiscFlags == Value2.MiscFlags &&
				Value1.Format == Value2.Format &&
				Value1.Filter == Value2.Filter &&
				Value1.MipFilter == Value2.MipFilter &&
				Value1.SourceInfo == Value2.SourceInfo;
		}
	};
}
}
}