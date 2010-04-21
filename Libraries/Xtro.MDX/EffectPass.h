namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public ref class EffectPass : Interface
	{
	internal:
		ID3D10EffectPass* pEffectPass;

		static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectPass);

		EffectPass(IntPtr EffectPass) : Interface(EffectPass)
		{	
			pEffectPass = (ID3D10EffectPass*)EffectPass.ToPointer();
		}

	public:
		int GetDescription([Out] PassDescription% Description)
		{
			D3D10_PASS_DESC NativeDescription;
			int Result = pEffectPass->GetDesc(&NativeDescription);

			if (Result >= 0) Description = PassDescription(&NativeDescription);

			return Result;
		}

		int Apply(unsigned int Flags)
		{
			return pEffectPass->Apply(Flags);
		}
	};
}
}
}