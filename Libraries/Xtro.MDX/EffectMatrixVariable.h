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
	int GetMatrix(Generic::UnmanagedMemory<float>^ Data)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrix(pData);
	}

	int GetMatrixArray(Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrixArray(pData, Offset, Count);
	}

	int GetMatrixTranspose(Generic::UnmanagedMemory<float>^ Data)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrixTranspose(pData);
	}

	int GetMatrixTransposeArray(Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrixTransposeArray(pData, Offset, Count);
	}

	int SetMatrix(Generic::UnmanagedMemory<float>^ Data)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->SetMatrix(pData);
	}

	int SetMatrixArray(Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrixArray(pData, Offset, Count);
	}

	int SetMatrixTranspose(Generic::UnmanagedMemory<float>^ Data)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->SetMatrixTranspose(pData);
	}

	int SetMatrixTransposeArray(Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->SetMatrixTransposeArray(pData, Offset, Count);
	}
};