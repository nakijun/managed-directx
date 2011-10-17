public value class ShaderBufferDescription : IEquatable<ShaderBufferDescription>
{
internal:				 
	inline void FromNative(D3D10_SHADER_BUFFER_DESC* Native)
	{
		Name = Native->Name ? gcnew String(Native->Name) : nullptr;
		Type = (ConstantBufferType)Native->Type;
		Variables = Native->Variables;
		Size = Native->Size;
		Flags = (ShaderConstantBufferFlag)Native->uFlags;
	}

public:
	String^ Name;
    ConstantBufferType Type;
    unsigned int Variables;
    unsigned int Size;
    ShaderConstantBufferFlag Flags;
		
	static bool operator == (ShaderBufferDescription Left, ShaderBufferDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ShaderBufferDescription Left, ShaderBufferDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Name == nullptr ? 1 : Name->GetHashCode() ^
			(int)Type ^
			Variables ^
			Size ^
			(int)Flags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ShaderBufferDescription Value)
	{
		return
			Name == Value.Name &&
			Type == Value.Type &&
			Variables == Value.Variables &&
			Size == Value.Size &&
			Flags == Value.Flags;
	}

	static bool Equals(ShaderBufferDescription% Value1, ShaderBufferDescription% Value2)
	{
		return
			Value1.Name == Value2.Name &&
			Value1.Type == Value2.Type &&
			Value1.Variables == Value2.Variables &&
			Value1.Size == Value2.Size &&
			Value1.Flags == Value2.Flags;
	}
};