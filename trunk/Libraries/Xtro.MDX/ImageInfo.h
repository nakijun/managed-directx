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
			Width ^
			Height ^
			Depth ^
			ArraySize ^
			MipLevels ^
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
		pin_ptr<unsigned int> PinnedThis = &Width;
		pin_ptr<ImageInfo> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(ImageInfo::typeid)) == 0;
	}

	static bool Equals(ImageInfo% Value1, ImageInfo% Value2)
	{
		pin_ptr<ImageInfo> PinnedValue1 = &Value1;
		pin_ptr<ImageInfo> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(ImageInfo::typeid)) == 0;
	}
};