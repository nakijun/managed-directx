public value class ShaderVariableDescription : IEquatable<ShaderVariableDescription>
{
internal:				 
	inline void FromNative(D3D10_SHADER_VARIABLE_DESC* Native)
	{
		Name = Native->Name ? gcnew String(Native->Name) : nullptr;
		StartOffset = Native->StartOffset;
		Size = Native->Size;
		Flags = (ShaderVariableFlag)Native->uFlags;
		DefaultValue = Native->DefaultValue ? gcnew UnmanagedMemory(IntPtr(Native->DefaultValue), Size) : nullptr;
	}

public:
	String^ Name;
    unsigned int StartOffset;
    unsigned int Size;
    ShaderVariableFlag Flags;
    UnmanagedMemory^ DefaultValue;
		
	static bool operator == (ShaderVariableDescription Left, ShaderVariableDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ShaderVariableDescription Left, ShaderVariableDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Name == nullptr ? 1 : Name->GetHashCode() ^
			StartOffset ^
			Size ^
			(int)Flags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ShaderVariableDescription Value)
	{
		return
			Name == Value.Name &&
			StartOffset == Value.StartOffset &&
			Size == Value.Size &&
			Flags == Value.Flags &&
			DefaultValue == Value.DefaultValue;
	}

	static bool Equals(ShaderVariableDescription% Value1, ShaderVariableDescription% Value2)
	{
		return
			Value1.Name == Value2.Name &&
			Value1.StartOffset == Value2.StartOffset &&
			Value1.Size == Value2.Size &&
			Value1.Flags == Value2.Flags &&
			Value1.DefaultValue == Value2.DefaultValue;
	}
};