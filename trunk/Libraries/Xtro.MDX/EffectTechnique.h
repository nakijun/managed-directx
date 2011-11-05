public ref class EffectTechnique sealed : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectTechnique);

internal:
	ID3D10EffectTechnique* pEffectTechnique;

	EffectTechnique(IntPtr EffectTechnique) : Interface(EffectTechnique)
	{	
		pEffectTechnique = (ID3D10EffectTechnique*)EffectTechnique.ToPointer();
	}

public:
	EffectPass^ GetPassByIndex(unsigned int Index)
	{
		ID3D10EffectPass* pEffectPass = pEffectTechnique->GetPassByIndex(Index);

		if (pEffectPass)
		{
			try { return (EffectPass^)Interfaces[IntPtr(pEffectPass)]; }
			catch (KeyNotFoundException^) { return gcnew EffectPass(IntPtr(pEffectPass)); }
		}
		
		return nullptr;
	}

	int GetDescription([Out] TechniqueDescription% Description)
	{
		D3D10_TECHNIQUE_DESC NativeDescription;
		int Result = pEffectTechnique->GetDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}
};