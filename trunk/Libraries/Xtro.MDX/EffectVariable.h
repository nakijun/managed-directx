ref class EffectMatrixVariable;
ref class EffectVectorVariable;
ref class EffectScalarVariable;
ref class EffectShaderResourceVariable;

public ref class EffectVariable : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectVariable);

internal:
	ID3D10EffectVariable* pEffectVariable;

	EffectVariable(IntPtr EffectVariable) : Interface(EffectVariable)
	{	
		pEffectVariable = (ID3D10EffectVariable*)EffectVariable.ToPointer();
	}

public:
	EffectMatrixVariable^ AsMatrix();
	EffectScalarVariable^ AsScalar();
	EffectVectorVariable^ AsVector();
	EffectShaderResourceVariable^ AsShaderResource();

	int SetRawValue(UnmanagedMemory^ Data, unsigned int Offset, unsigned int Count)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		return pEffectVariable->SetRawValue(pData, Offset, Count);
	}
};