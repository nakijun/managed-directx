ref class EffectMatrixVariable;
ref class EffectVectorVariable;
ref class EffectShaderResourceVariable;

public ref class EffectVariable : Interface
{
internal:
	ID3D10EffectVariable* pEffectVariable;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectVariable);

	EffectVariable(IntPtr EffectVariable) : Interface(EffectVariable)
	{	
		pEffectVariable = (ID3D10EffectVariable*)EffectVariable.ToPointer();
	}

public:
	EffectMatrixVariable^ AsMatrix();
	EffectVectorVariable^ AsVector();
	EffectShaderResourceVariable^ AsShaderResource();
};