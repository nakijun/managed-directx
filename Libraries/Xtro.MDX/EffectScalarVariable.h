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
	int GetBool([Out] bool% Value)
	{
		pin_ptr<bool> PinnedValue = &Value;
		return pEffectScalarVariable->GetBool((int*)PinnedValue);
	}

	int GetBoolArray(Generic::UnmanagedMemory<bool>^ Data, unsigned Offset, unsigned int Count)
	{											   
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectScalarVariable->GetBoolArray(pData, Offset, Count);
	}

	int GetFloat([Out] float% Value)
	{
		pin_ptr<float> PinnedValue = &Value;
		return pEffectScalarVariable->GetFloat(PinnedValue);
	}

	int GetFloatArray(Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectScalarVariable->GetFloatArray(pData, Offset, Count);
	}

	int GetInt(int Value)
	{
		pin_ptr<int> PinnedValue = &Value;
		return pEffectScalarVariable->GetInt(PinnedValue);
	}

	int GetIntArray(Generic::UnmanagedMemory<int>^ Data, unsigned Offset, unsigned int Count)
	{											   
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectScalarVariable->GetIntArray(pData, Offset, Count);
	}

	int SetBool(bool Value)
	{
		return pEffectScalarVariable->SetBool(Value);
	}

	int SetBoolArray(Generic::UnmanagedMemory<bool>^ Data, unsigned Offset, unsigned int Count)
	{											   
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectScalarVariable->GetBoolArray(pData, Offset, Count);
	}

	int SetFloat(float Value)
	{
		return pEffectScalarVariable->SetFloat(Value);
	}

	int SetFloatArray(Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectScalarVariable->GetFloatArray(pData, Offset, Count);
	}

	int SetInt(int Value)
	{
		return pEffectScalarVariable->SetInt(Value);
	}

	int SetIntArray(Generic::UnmanagedMemory<int>^ Data, unsigned Offset, unsigned int Count)
	{											   
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectScalarVariable->GetIntArray(pData, Offset, Count);
	}
};