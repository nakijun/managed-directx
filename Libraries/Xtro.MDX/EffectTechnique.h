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
	int ComputeStateBlockMask([Out] StateBlockMask% StateBlockMask)
	{
		pin_ptr<Xtro::MDX::Direct3D10::StateBlockMask> PinnedStateBlockMask = &StateBlockMask;
		return pEffectTechnique->ComputeStateBlockMask((D3D10_STATE_BLOCK_MASK*)PinnedStateBlockMask);
	}

	EffectVariable^ GetAnnotationByIndex(unsigned int Index)
	{
		ID3D10EffectVariable* pResult = pEffectTechnique->GetAnnotationByIndex(Index);

		if (pResult)
		{	
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	EffectVariable^ GetAnnotationByName(String^ Name)
	{
		ID3D10EffectVariable* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffectTechnique->GetAnnotationByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult)
		{	
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
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

	EffectPass^ GetPassByIndex(unsigned int Index)
	{
		ID3D10EffectPass* pResult = pEffectTechnique->GetPassByIndex(Index);

		if (pResult)
		{	
			try { return (EffectPass^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectPass(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	EffectPass^ GetPassByName(String^ Name)
	{
		ID3D10EffectPass* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffectTechnique->GetPassByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult)
		{	
			try { return (EffectPass^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectPass(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	bool IsValid()
	{
		return pEffectTechnique->IsValid();
	}
};