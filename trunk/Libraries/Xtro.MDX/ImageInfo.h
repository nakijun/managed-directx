public value class ImageInfo : IEquatable<ImageInfo>
{
public:
	unsigned int Width;
	unsigned int Height;
	unsigned int Depth;
	unsigned int ArraySize;
	unsigned int MipLevels;
	ResourceMiscellaneousFlag MiscellaneousFlags;
	Format Format;
	ResourceDimension ResourceDimension;
	ImageFileFormat ImageFileFormat;

	static bool operator == (ImageInfo Left, ImageInfo Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ImageInfo Left, ImageInfo Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Width ^
			(int)Height ^
			(int)Depth ^
			(int)ArraySize ^
			(int)MipLevels ^
			(int)MiscellaneousFlags ^
			(int)Format ^
			(int)ResourceDimension ^
			(int)ImageFileFormat;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ImageInfo Value)
	{
		return
			Width == Value.Width &&
			Height == Value.Height &&
			Depth == Value.Depth &&
			ArraySize == Value.ArraySize &&
			MipLevels == Value.MipLevels &&
			MiscellaneousFlags == Value.MiscellaneousFlags &&
			Format == Value.Format &&
			ResourceDimension == Value.ResourceDimension &&
			ImageFileFormat == Value.ImageFileFormat;
	}

	static bool Equals(ImageInfo% Value1, ImageInfo% Value2)
	{
		return
			Value1.Width == Value2.Width &&
			Value1.Height == Value2.Height &&
			Value1.Depth == Value2.Depth &&
			Value1.ArraySize == Value2.ArraySize &&
			Value1.MipLevels == Value2.MipLevels &&
			Value1.MiscellaneousFlags == Value2.MiscellaneousFlags &&
			Value1.Format == Value2.Format &&
			Value1.ResourceDimension == Value2.ResourceDimension &&
			Value1.ImageFileFormat == Value2.ImageFileFormat;
	}
};