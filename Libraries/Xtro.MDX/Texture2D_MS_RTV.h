public value class Texture2D_MS_RTV : IEquatable<Texture2D_MS_RTV>
{
public:
	unsigned int UnusedField_NothingToDefine;

	static bool operator == (Texture2D_MS_RTV Left, Texture2D_MS_RTV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture2D_MS_RTV Left, Texture2D_MS_RTV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return UnusedField_NothingToDefine;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture2D_MS_RTV Value)
	{
		return UnusedField_NothingToDefine == Value.UnusedField_NothingToDefine;
	}

	static bool Equals(Texture2D_MS_RTV% Value1, Texture2D_MS_RTV% Value2)
	{
		return Value1.UnusedField_NothingToDefine == Value2.UnusedField_NothingToDefine;
	}
};