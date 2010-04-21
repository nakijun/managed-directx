namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public ref class EffectMatrixVariable : EffectVariable
	{
	internal:
		ID3D10EffectMatrixVariable* pEffectMatrixVariable;

		static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectMatrixVariable);

		EffectMatrixVariable(IntPtr EffectMatrixVariable) : EffectVariable(EffectMatrixVariable)
		{	
			pEffectMatrixVariable = (ID3D10EffectMatrixVariable*)EffectMatrixVariable.ToPointer();
		}

	public:
		int SetMatrix(array<float>^ Data)
		{
			pin_ptr<float> PinnedData = nullptr;
			if (Data != nullptr) PinnedData = &Data[0];

			return pEffectMatrixVariable->SetMatrix(PinnedData);
		}
	};
}
}
}