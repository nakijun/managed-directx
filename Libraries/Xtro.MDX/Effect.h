namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public ref class Effect : Unknown
	{
	internal:
		ID3D10Effect* pEffect;

		static Guid IID = IID_Converter::ToManaged(IID_ID3D10Effect);

		Effect(IntPtr Effect) : Unknown(Effect)
		{	
			pEffect = (ID3D10Effect*)Effect.ToPointer();
		}

	public:
		EffectTechnique^ GetTechniqueByName(String^ Name)
		{
			LPCSTR pName = 0;
			if (Name != nullptr) pName = (LPCSTR)Marshal::StringToHGlobalAnsi(Name).ToPointer();

			ID3D10EffectTechnique* pResult = pEffect->GetTechniqueByName(pName);

			EffectTechnique^ Result = nullptr;
			if (pResult) 
			{
				try { Result = (EffectTechnique^)Interfaces[IntPtr(pResult)]; }
				catch (KeyNotFoundException^) { Result = gcnew EffectTechnique(IntPtr(pResult)); }
			}

			return Result;
		}
	
		EffectVariable^ GetVariableByName(String^ Name)
		{
			LPCSTR pName = 0;
			if (Name != nullptr) pName = (LPCSTR)Marshal::StringToHGlobalAnsi(Name).ToPointer();

			ID3D10EffectVariable* pResult = pEffect->GetVariableByName(pName);

			EffectVariable^ Result = nullptr;
			if (pResult) 
			{
				try { Result = (EffectVariable^)Interfaces[IntPtr(pResult)]; }
				catch (KeyNotFoundException^) { Result = gcnew EffectVariable(IntPtr(pResult)); }
			}

			return Result;
		}
	};
}
}
}