public value class ShaderInputBindDescription : IEquatable<ShaderInputBindDescription>
{
internal:				 
	inline void FromNative(D3D10_SHADER_INPUT_BIND_DESC* Native)
	{
		Name = Native->Name ? gcnew String(Native->Name) : nullptr;
		Type = (ShaderInputType)Native->Type;
		BindPoint = Native->BindPoint;
		BindCount = Native->BindCount;
		Flags = (ShaderInputFlag)Native->uFlags;
		ReturnType = (ResourceReturnType)Native->ReturnType;
		Dimension = (ShaderResourceViewDimension)Native->Dimension;
		NumberOfSamples = Native->NumSamples;
	}

public:
	String^ Name;
	ShaderInputType Type;
    unsigned int BindPoint;
    unsigned int BindCount;
    ShaderInputFlag Flags;
    ResourceReturnType ReturnType;
    ShaderResourceViewDimension Dimension;
    unsigned int NumberOfSamples;
		
	static bool operator == (ShaderInputBindDescription Left, ShaderInputBindDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ShaderInputBindDescription Left, ShaderInputBindDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Name == nullptr ? 1 : Name->GetHashCode() ^
			(int)Type ^
			BindPoint ^
			BindCount ^
			(int)Flags ^
			(int)ReturnType ^
			(int)Dimension ^
			NumberOfSamples;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ShaderInputBindDescription Value)
	{
		return
			Name == Value.Name &&
			Type == Value.Type &&
			BindPoint == Value.BindPoint &&
			BindCount == Value.BindCount &&
			Flags == Value.Flags &&
			ReturnType == Value.ReturnType &&
			Dimension == Value.Dimension &&
			NumberOfSamples == Value.NumberOfSamples;
	}

	static bool Equals(ShaderInputBindDescription% Value1, ShaderInputBindDescription% Value2)
	{
		return
			Value1.Name == Value2.Name && 
			Value1.Type == Value2.Type && 
			Value1.BindPoint == Value2.BindPoint && 
			Value1.BindCount == Value2.BindCount && 
			Value1.Flags == Value2.Flags &&
			Value1.ReturnType == Value2.ReturnType && 
			Value1.Dimension == Value2.Dimension && 
			Value1.NumberOfSamples == Value2.NumberOfSamples;
	}
};