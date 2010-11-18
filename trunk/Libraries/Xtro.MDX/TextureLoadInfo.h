[StructLayout(LayoutKind::Sequential)]
public value class TextureLoadInfo : IEquatable<TextureLoadInfo>
{
public:
    Box SourceBox;
    Box DestinationBox;
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
		return
			SourceBox == Value.SourceBox &&
			DestinationBox == Value.DestinationBox &&
			SourceFirstMip == Value.SourceFirstMip &&
			DestinationFirstMip == Value.DestinationFirstMip &&
			NumberOfMips == Value.NumberOfMips &&
			SourceFirstElement == Value.SourceFirstElement &&
			DestinationFirstElement == Value.DestinationFirstElement &&
			NumberOfElements == Value.NumberOfElements &&
			Filter == Value.Filter &&
			MipFilter == Value.MipFilter;
	}

	static bool Equals(TextureLoadInfo% Value1, TextureLoadInfo% Value2)
	{
		return
			Value1.SourceBox == Value2.SourceBox &&
			Value1.DestinationBox == Value2.DestinationBox &&
			Value1.SourceFirstMip == Value2.SourceFirstMip &&
			Value1.DestinationFirstMip == Value2.DestinationFirstMip &&
			Value1.NumberOfMips == Value2.NumberOfMips &&
			Value1.SourceFirstElement == Value2.SourceFirstElement &&
			Value1.DestinationFirstElement == Value2.DestinationFirstElement &&
			Value1.NumberOfElements == Value2.NumberOfElements &&
			Value1.Filter == Value2.Filter &&
			Value1.MipFilter == Value2.MipFilter;
	}
};