public ref class EffectVectorVariable : EffectVariable
{
internal:
	ID3D10EffectVectorVariable* pEffectVectorVariable;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectVectorVariable);

	EffectVectorVariable(IntPtr EffectVectorVariable) : EffectVariable(EffectVectorVariable)
	{	
		pEffectVectorVariable = (ID3D10EffectVectorVariable*)EffectVectorVariable.ToPointer();
	}

public:
	int SetFloatVector(array<float>^ Data)
	{
		pin_ptr<float> PinnedData = Data != nullptr && Data->Length > 0 ? &Data[0] : nullptr;

		return pEffectVectorVariable->SetFloatVector(PinnedData);
	}

	int SetFloatVectorArray(UnmanagedMemory^ Data, unsigned int Offset, unsigned int Count)
	{
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectVectorVariable->SetFloatVectorArray(pData, Offset, Count);
	}
};