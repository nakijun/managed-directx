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
		return
			FirstArraySlice == Value.FirstArraySlice &&
			ArraySize == Value.ArraySize;
	}

	static bool Equals(Texture2D_MS_ArrayDSV% Value1, Texture2D_MS_ArrayDSV% Value2)
	{
		return
			Value1.FirstArraySlice == Value2.FirstArraySlice && 
			Value1.ArraySize == Value2.ArraySize;
	}
};