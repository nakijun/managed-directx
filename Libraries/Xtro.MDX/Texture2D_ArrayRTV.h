[StructLayout(LayoutKind::Sequential)]
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
		return
			MipSlice == Value.MipSlice &&
			FirstArraySlice == Value.FirstArraySlice &&
			ArraySize == Value.ArraySize;
	}

	static bool Equals(Texture2DArrayRTV% Value1, Texture2DArrayRTV% Value2)
	{
		return
			Value1.MipSlice == Value2.MipSlice &&
			Value1.FirstArraySlice == Value2.FirstArraySlice &&
			Value1.ArraySize == Value2.ArraySize;
	}
};