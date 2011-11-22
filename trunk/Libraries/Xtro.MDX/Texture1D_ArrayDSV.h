public value class Texture1D_ArrayDSV : IEquatable<Texture1D_ArrayDSV>
{
public:
	unsigned int MipSlice;
	unsigned int FirstArraySlice;
	unsigned int ArraySize;

	static bool operator == (Texture1D_ArrayDSV Left, Texture1D_ArrayDSV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture1D_ArrayDSV Left, Texture1D_ArrayDSV Right)
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

	virtual bool Equals(Texture1D_ArrayDSV Value)
	{
		pin_ptr<Texture1D_ArrayDSV> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Texture1D_ArrayDSV::typeid)) == 0;
	}

	static bool Equals(Texture1D_ArrayDSV% Value1, Texture1D_ArrayDSV% Value2)
	{
		pin_ptr<Texture1D_ArrayDSV> PinnedValue1 = &Value1;
		pin_ptr<Texture1D_ArrayDSV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture1D_ArrayDSV::typeid)) == 0;
	}
};