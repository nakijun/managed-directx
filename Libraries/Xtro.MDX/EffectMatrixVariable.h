public ref class EffectMatrixVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectMatrixVariable);

internal:
	ID3D10EffectMatrixVariable* pEffectMatrixVariable;

	EffectMatrixVariable(IntPtr EffectMatrixVariable) : EffectVariable(EffectMatrixVariable)
	{	
		pEffectMatrixVariable = (ID3D10EffectMatrixVariable*)EffectMatrixVariable.ToPointer();
	}

public:
	int SetMatrix(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->SetMatrix(pData);
	}
};