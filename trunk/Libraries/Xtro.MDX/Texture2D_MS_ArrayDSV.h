public value class Texture2D_MS_ArrayDSV : IEquatable<Texture2D_MS_ArrayDSV>
{
public:
	unsigned int FirstArraySlice;
	unsigned int ArraySize;

	static bool operator == (Texture2D_MS_ArrayDSV Left, Texture2D_MS_ArrayDSV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture2D_MS_ArrayDSV Left, Texture2D_MS_ArrayDSV Right)
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

	virtual bool Equals(Texture2D_MS_ArrayDSV Value)
	{
		pin_ptr<unsigned int> PinnedThis = &FirstArraySlice;
		pin_ptr<Texture2D_MS_ArrayDSV> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Texture2D_MS_ArrayDSV::typeid)) == 0;
	}

	static bool Equals(Texture2D_MS_ArrayDSV% Value1, Texture2D_MS_ArrayDSV% Value2)
	{
		pin_ptr<Texture2D_MS_ArrayDSV> PinnedValue1 = &Value1;
		pin_ptr<Texture2D_MS_ArrayDSV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture2D_MS_ArrayDSV::typeid)) == 0;
	}
};