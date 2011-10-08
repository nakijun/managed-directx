public value class ShaderTypeDescription : IEquatable<ShaderTypeDescription>
{
public:
    ShaderVariableClass Class;
    ShaderVariableType Type;
    unsigned int Rows;
    unsigned int Columns;
    unsigned int Elements;
    unsigned int Members;
    unsigned int Offset;

	static bool operator == (ShaderTypeDescription Left, ShaderTypeDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ShaderTypeDescription Left, ShaderTypeDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Class ^
			(int)Type ^
			Rows ^
			Columns ^
			Elements ^
			Members ^
			Offset;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ShaderTypeDescription Value)
	{
		pin_ptr<ShaderVariableClass> PinnedThis = &Class;
		pin_ptr<ShaderTypeDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(ShaderTypeDescription::typeid)) == 0;
	}

	static bool Equals(ShaderTypeDescription% Value1, ShaderTypeDescription% Value2)
	{
		pin_ptr<ShaderTypeDescription> PinnedValue1 = &Value1;
		pin_ptr<ShaderTypeDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(ShaderTypeDescription::typeid)) == 0;
	}
};