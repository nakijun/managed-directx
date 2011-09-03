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
	int SetMatrix(array<float>^ Data)
	{											   
		pin_ptr<float> PinnedData = Data != nullptr && Data->Length > 0 ? &Data[0] : nullptr;

		return pEffectMatrixVariable->SetMatrix(PinnedData);
	}
};