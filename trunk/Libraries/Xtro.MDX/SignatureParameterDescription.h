public value class SignatureParameterDescription : IEquatable<SignatureParameterDescription>
{
internal:				 
	inline void FromNative(D3D10_SIGNATURE_PARAMETER_DESC* Native)
	{
		SemanticName = gcnew String(Native->SemanticName);
		SemanticIndex = Native->SemanticIndex;
		Register = Native->Register;
		SystemValueType = (Name)Native->SystemValueType;
		ComponentType = (RegisterComponentType)Native->ComponentType;
		Mask = Native->Mask;
		ReadWriteMask = Native->ReadWriteMask;
	}

public:
	String^ SemanticName;
    unsigned int SemanticIndex;
    unsigned int Register;
    Name SystemValueType;
    RegisterComponentType ComponentType;
    unsigned char Mask;
    unsigned char ReadWriteMask;		

	static bool operator == (SignatureParameterDescription Left, SignatureParameterDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SignatureParameterDescription Left, SignatureParameterDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			SemanticName == nullptr ? 1 : SemanticName->GetHashCode() ^
			SemanticIndex ^
			Register ^
			(int)SystemValueType ^
			(int)ComponentType ^
			Mask ^
			ReadWriteMask;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SignatureParameterDescription Value)
	{
		return
			SemanticName == Value.SemanticName &&
			SemanticIndex == Value.SemanticIndex &&
			Register == Value.Register &&
			SystemValueType == Value.SystemValueType &&
			ComponentType == Value.ComponentType &&
			Mask == Value.Mask &&
			ReadWriteMask == Value.ReadWriteMask;
	}

	static bool Equals(SignatureParameterDescription% Value1, SignatureParameterDescription% Value2)
	{
		return
			Value1.SemanticName == Value2.SemanticName && 
			Value1.SemanticIndex == Value2.SemanticIndex && 
			Value1.Register == Value2.Register &&
			Value1.SystemValueType == Value2.SystemValueType && 
			Value1.ComponentType == Value2.ComponentType && 
			Value1.Mask == Value2.Mask && 
			Value1.ReadWriteMask == Value2.ReadWriteMask;
	}
};