public ref class EffectShaderVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectShaderVariable);

internal:
	ID3D10EffectShaderVariable* pEffectShaderVariable;

	EffectShaderVariable(IntPtr EffectShaderVariable) : EffectVariable(EffectShaderVariable)
	{	
		pEffectShaderVariable = (ID3D10EffectShaderVariable*)EffectShaderVariable.ToPointer();
	}

public:
};