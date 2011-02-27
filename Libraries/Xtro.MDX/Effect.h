public ref class Effect : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Effect);

internal:
	ID3D10Effect* pEffect;

	Effect(IntPtr Effect) : Unknown(Effect)
	{	
		pEffect = (ID3D10Effect*)Effect.ToPointer();
	}

public:
	EffectTechnique^ GetTechniqueByName(String^ Name)
	{
		ID3D10EffectTechnique* pResult;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffect->GetTechniqueByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

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
		ID3D10EffectVariable* pResult;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffect->GetVariableByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		EffectVariable^ Result = nullptr;
		if (pResult) 
		{
			try { Result = (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { Result = gcnew EffectVariable(IntPtr(pResult)); }
		}

		return Result;
	}
};