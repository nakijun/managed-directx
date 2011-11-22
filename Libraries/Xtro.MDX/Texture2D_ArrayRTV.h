public value class Texture2DArrayRTV : IEquatable<Texture2DArrayRTV>
{
public:
	unsigned int MipSlice;
	unsigned int FirstArraySlice;
	unsigned int ArraySize;

	static bool operator == (Texture2DArrayRTV Left, Texture2DArrayRTV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture2DArrayRTV Left, Texture2DArrayRTV Right)
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

	virtual bool Equals(Texture2DArrayRTV Value)
	{
		pin_ptr<Texture2DArrayRTV> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Texture2DArrayRTV::typeid)) == 0;
	}

	static bool Equals(Texture2DArrayRTV% Value1, Texture2DArrayRTV% Value2)
	{
		pin_ptr<Texture2DArrayRTV> PinnedValue1 = &Value1;
		pin_ptr<Texture2DArrayRTV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture2DArrayRTV::typeid)) == 0;
	}
};