public value class Texture2D_MS_ArrayRTV : IEquatable<Texture2D_MS_ArrayRTV>
{
public:
	unsigned int FirstArraySlice;
	unsigned int ArraySize;

	static bool operator == (Texture2D_MS_ArrayRTV Left, Texture2D_MS_ArrayRTV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture2D_MS_ArrayRTV Left, Texture2D_MS_ArrayRTV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			FirstArraySlice ^
			ArraySize;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture2D_MS_ArrayRTV Value)
	{
		pin_ptr<Texture2D_MS_ArrayRTV> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Texture2D_MS_ArrayRTV::typeid)) == 0;
	}

	static bool Equals(Texture2D_MS_ArrayRTV% Value1, Texture2D_MS_ArrayRTV% Value2)
	{
		pin_ptr<Texture2D_MS_ArrayRTV> PinnedValue1 = &Value1;
		pin_ptr<Texture2D_MS_ArrayRTV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture2D_MS_ArrayRTV::typeid)) == 0;
	}
};