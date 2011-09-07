public value class Texture2D_RTV : IEquatable<Texture2D_RTV>
{
public:
	unsigned int MipSlice;

	static bool operator == (Texture2D_RTV Left, Texture2D_RTV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture2D_RTV Left, Texture2D_RTV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			MipSlice;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture2D_RTV Value)
	{
		return MipSlice == Value.MipSlice;
	}

	static bool Equals(Texture2D_RTV% Value1, Texture2D_RTV% Value2)
	{
		return Value1.MipSlice == Value2.MipSlice;
	}
};