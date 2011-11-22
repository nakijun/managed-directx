public value class Texture2DArraySRV : IEquatable<Texture2DArraySRV>
{
public:
	unsigned int MostDetailedMip;
	unsigned int MipLevels;
	unsigned int FirstArraySlice;
	unsigned int ArraySize;

	static bool operator == (Texture2DArraySRV Left, Texture2DArraySRV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture2DArraySRV Left, Texture2DArraySRV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			MostDetailedMip ^
			MipLevels ^
			FirstArraySlice ^
			ArraySize;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture2DArraySRV Value)
	{
		pin_ptr<Texture2DArraySRV> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Texture2DArraySRV::typeid)) == 0;
	}

	static bool Equals(Texture2DArraySRV% Value1, Texture2DArraySRV% Value2)
	{
		pin_ptr<Texture2DArraySRV> PinnedValue1 = &Value1;
		pin_ptr<Texture2DArraySRV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture2DArraySRV::typeid)) == 0;
	}
};