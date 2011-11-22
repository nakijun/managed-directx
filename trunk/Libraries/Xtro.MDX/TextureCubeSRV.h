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
		pin_ptr<TextureCubeSRV> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(TextureCubeSRV::typeid)) == 0;
	}

	static bool Equals(TextureCubeSRV% Value1, TextureCubeSRV% Value2)
	{
		pin_ptr<TextureCubeSRV> PinnedValue1 = &Value1;
		pin_ptr<TextureCubeSRV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(TextureCubeSRV::typeid)) == 0;
	}
};