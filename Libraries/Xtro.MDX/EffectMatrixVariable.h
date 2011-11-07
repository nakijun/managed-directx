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
	int GetMatrix(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrix(pData);
	}

	int GetMatrixArray(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrixArray(pData, Offset, Count);
	}

	int GetMatrixTranspose(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrixTranspose(pData);
	}

	int GetMatrixTransposeArray(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrixTransposeArray(pData, Offset, Count);
	}

	int SetMatrix(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->SetMatrix(pData);
	}

	int SetMatrixArray(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->GetMatrixArray(pData, Offset, Count);
	}

	int SetMatrixTranspose(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->SetMatrixTranspose(pData);
	}

	int SetMatrixTransposeArray(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data, unsigned Offset, unsigned int Count)
	{											   
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectMatrixVariable->SetMatrixTransposeArray(pData, Offset, Count);
	}
};