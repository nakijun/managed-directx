public ref class EffectPass : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectPass);

internal:
	ID3D10EffectPass* pEffectPass;

	EffectPass(IntPtr EffectPass) : Interface(EffectPass)
	{	
		pEffectPass = (ID3D10EffectPass*)EffectPass.ToPointer();
	}

public:
	int GetDescription([Out] PassDescription% Description)
	{
		D3D10_PASS_DESC NativeDescription;
		int Result = pEffectPass->GetDesc(&NativeDescription);

		if (Result >= 0) Description.FromNative(&NativeDescription);

		return Result;
	}

	int Apply(unsigned int Flags)
	{
		return pEffectPass->Apply(Flags);
	}
};