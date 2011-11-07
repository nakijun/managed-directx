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
	int GetBoolVector(Xtro::MDX::Generic::UnmanagedMemory<int>^ Data)
	{
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectVectorVariable->GetBoolVector(pData);
	}

	int GetBoolVectorArray(Xtro::MDX::Generic::UnmanagedMemory<int>^ Data, unsigned int Offset, unsigned int Count)
	{
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectVectorVariable->GetBoolVectorArray(pData, Offset, Count);
	}

	int GetFloatVector(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data)
	{
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectVectorVariable->GetFloatVector(pData);
	}

	int GetFloatVectorArray(Xtro::MDX::Generic::UnmanagedMemory<float>^ Data, unsigned int Offset, unsigned int Count)
	{
		float* pData = Data == nullptr ? 0 : (float*)Data->pMemory;

		return pEffectVectorVariable->GetFloatVectorArray(pData, Offset, Count);
	}

	int GetIntVector(Xtro::MDX::Generic::UnmanagedMemory<int>^ Data)
	{
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectVectorVariable->GetIntVector(pData);
	}

	int GetIntVectorArray(Xtro::MDX::Generic::UnmanagedMemory<int>^ Data, unsigned int Offset, unsigned int Count)
	{
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectVectorVariable->GetIntVectorArray(pData, Offset, Count);
	}

	int SetBoolVector(Xtro::MDX::Generic::UnmanagedMemory<int>^ Data)
	{
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectVectorVariable->SetBoolVector(pData);
	}

	int SetBoolVectorArray(Xtro::MDX::Generic::UnmanagedMemory<int>^ Data, unsigned int Offset, unsigned int Count)
	{
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectVectorVariable->SetBoolVectorArray(pData, Offset, Count);
	}

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

	int SetIntVector(Xtro::MDX::Generic::UnmanagedMemory<int>^ Data)
	{
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectVectorVariable->SetIntVector(pData);
	}

	int SetIntVectorArray(Xtro::MDX::Generic::UnmanagedMemory<int>^ Data, unsigned int Offset, unsigned int Count)
	{
		int* pData = Data == nullptr ? 0 : (int*)Data->pMemory;

		return pEffectVectorVariable->SetIntVectorArray(pData, Offset, Count);
	}
};