public ref class EffectScalarVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectScalarVariable);

internal:
	ID3D10EffectScalarVariable* pEffectScalarVariable;

	EffectScalarVariable(IntPtr EffectScalarVariable) : EffectVariable(EffectScalarVariable)
	{	
		pEffectScalarVariable = (ID3D10EffectScalarVariable*)EffectScalarVariable.ToPointer();
	}

public:
	int SetFloat(float Value)
	{
		return pEffectScalarVariable->SetFloat(Value);
	}

	int SetInt(unsigned int Value)
	{
		return pEffectScalarVariable->SetInt(Value);
	}

	int SetBool(bool Value)
	{
		return pEffectScalarVariable->SetInt(Value);
	}
};