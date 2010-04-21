namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
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
			pin_ptr<float> PinnedData = nullptr;
			if (Data != nullptr) PinnedData = &Data[0];

			return pEffectVectorVariable->SetFloatVector(PinnedData);
		}

		int SetFloatVectorArray(UnmanagedMemory^ Data, unsigned int Offset, unsigned int Count)
		{
			float* pData = 0;
			if (Data != nullptr) pData = (float*)Data->Memory;

			return pEffectVectorVariable->SetFloatVectorArray(pData, Offset, Count);
		}
	};
}
}
}