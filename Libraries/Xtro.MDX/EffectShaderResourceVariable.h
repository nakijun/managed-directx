public ref class EffectShaderResourceVariable : EffectVariable
{
internal:
	ID3D10EffectShaderResourceVariable* pEffectShaderResourceVariable;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectShaderResourceVariable);

	EffectShaderResourceVariable(IntPtr EffectShaderResourceVariable) : EffectVariable(EffectShaderResourceVariable)
	{	
		pEffectShaderResourceVariable = (ID3D10EffectShaderResourceVariable*)EffectShaderResourceVariable.ToPointer();
	}

public:
	int SetResource(ShaderResourceView^ Resource)
	{
		ID3D10ShaderResourceView* pResource = Resource == nullptr ? 0 : Resource->pShaderResourceView;

		return pEffectShaderResourceVariable->SetResource(pResource);
	}
};