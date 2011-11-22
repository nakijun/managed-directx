public value class Texture2DArrayDSV : IEquatable<Texture2DArrayDSV>
{
public:
	unsigned int MipSlice;
	unsigned int FirstArraySlice;
	unsigned int ArraySize;

	static bool operator == (Texture2DArrayDSV Left, Texture2DArrayDSV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture2DArrayDSV Left, Texture2DArrayDSV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			MipSlice ^
			FirstArraySlice ^
			ArraySize;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture2DArrayDSV Value)
	{
		pin_ptr<Texture2DArrayDSV> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Texture2DArrayDSV::typeid)) == 0;
	}

	static bool Equals(Texture2DArrayDSV% Value1, Texture2DArrayDSV% Value2)
	{
		pin_ptr<Texture2DArrayDSV> PinnedValue1 = &Value1;
		pin_ptr<Texture2DArrayDSV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture2DArrayDSV::typeid)) == 0;
	}
};