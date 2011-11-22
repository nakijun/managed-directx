public value class Texture1D_ArrayRTV : IEquatable<Texture1D_ArrayRTV>
{
public:
	unsigned int MipSlice;
	unsigned int FirstArraySlice;
	unsigned int ArraySize;

	static bool operator == (Texture1D_ArrayRTV Left, Texture1D_ArrayRTV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture1D_ArrayRTV Left, Texture1D_ArrayRTV Right)
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

	virtual bool Equals(Texture1D_ArrayRTV Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(Texture1D_ArrayRTV::typeid)) == 0;
	}

	static bool Equals(Texture1D_ArrayRTV% Value1, Texture1D_ArrayRTV% Value2)
	{
		pin_ptr<Texture1D_ArrayRTV> PinnedValue1 = &Value1;
		pin_ptr<Texture1D_ArrayRTV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture1D_ArrayRTV::typeid)) == 0;
	}
};