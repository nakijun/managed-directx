namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public ref class EffectTechnique : Interface
	{
	internal:
		ID3D10EffectTechnique* pEffectTechnique;

		static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectTechnique);

		EffectTechnique(IntPtr EffectTechnique) : Interface(EffectTechnique)
		{	
			pEffectTechnique = (ID3D10EffectTechnique*)EffectTechnique.ToPointer();
		}

	public:
		EffectPass^ GetPassByIndex(unsigned int Index)
		{
			ID3D10EffectPass* pEffectPass = pEffectTechnique->GetPassByIndex(Index);

			EffectPass^ Result = nullptr;
			if (pEffectPass)
			{
				try { Result = (EffectPass^)Interfaces[IntPtr(pEffectPass)]; }
				catch (KeyNotFoundException^) { Result = gcnew EffectPass(IntPtr(pEffectPass)); }
			}

			return Result;
		}

		int GetDescription([Out] TechniqueDescription% Description)
		{
			D3D10_TECHNIQUE_DESC NativeDescription;
			int Result = pEffectTechnique->GetDesc(&NativeDescription);

			if (Result >= 0) Description = TechniqueDescription(&NativeDescription);

			return Result;
		}
	};
}
}
}