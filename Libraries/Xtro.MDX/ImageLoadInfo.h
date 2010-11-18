[StructLayout(LayoutKind::Sequential)]
public value class ImageLoadInfo : IEquatable<ImageLoadInfo>
{
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