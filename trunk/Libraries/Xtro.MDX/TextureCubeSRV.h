public value class TextureCubeSRV : IEquatable<TextureCubeSRV>
{
public:
	unsigned int MostDetailedMip;
	unsigned int MipLevels;

	static bool operator == (TextureCubeSRV Left, TextureCubeSRV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (TextureCubeSRV Left, TextureCubeSRV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			MipLevels;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(TextureCubeSRV Value)
	{
		return
			MostDetailedMip == Value.MostDetailedMip &&
			MipLevels == Value.MipLevels;
	}

	static bool Equals(TextureCubeSRV% Value1, TextureCubeSRV% Value2)
	{
		return
			Value1.MostDetailedMip == Value2.MostDetailedMip &&
			Value1.MipLevels == Value2.MipLevels;
	}
};