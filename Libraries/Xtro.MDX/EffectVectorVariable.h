public ref class EffectVectorVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectVectorVariable);

internal:
	ID3D10EffectVectorVariable* pEffectVectorVariable;

	EffectVectorVariable(IntPtr EffectVectorVariable) : EffectVariable(EffectVectorVariable)
	{	
		pEffectVectorVariable = (ID3D10EffectVectorVariable*)EffectVectorVariable.ToPointer();
	}

public:
	int SetFloatVector(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data)
	{
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectVectorVariable->SetFloatVector(pData);
	}

	int SetFloatVectorArray(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data, unsigned int Offset, unsigned int Count)
	{
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectVectorVariable->SetFloatVectorArray(pData, Offset, Count);
	}
};