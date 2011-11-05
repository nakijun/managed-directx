public ref class Effect sealed : Unknown
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
	EffectConstantBuffer^ GetConstantBufferByIndex(unsigned int Index)
	{
		ID3D10EffectConstantBuffer* pResult = pEffect->GetConstantBufferByIndex(Index);

		if (pResult)
		{	
			try { return (EffectConstantBuffer^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectConstantBuffer(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	EffectConstantBuffer^ GetConstantBufferByName(String^ Name)
	{
		ID3D10EffectConstantBuffer* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffect->GetConstantBufferByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult)
		{	
			try { return (EffectConstantBuffer^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectConstantBuffer(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	int GetDescription([Out] EffectDescription% Description)
	{
		pin_ptr<EffectDescription> PinnedDescription = &Description;
		return pEffect->GetDesc((D3D10_EFFECT_DESC*)PinnedDescription);
	}

	int GetDevice([Out] Device^% Device)
	{
		ID3D10Device* pDevice = 0;
		int Result = pEffect->GetDevice(&pDevice);

		if (pDevice) 
		{
			try { Device = (Xtro::MDX::Direct3D10::Device^)Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }
		}
		else Device = nullptr;

		return Result;
	}

	EffectTechnique^ GetTechniqueByIndex(unsigned int Index)
	{
		ID3D10EffectTechnique* pResult = 0;

		pResult = pEffect->GetTechniqueByIndex(Index);

		if (pResult) 
		{
			try { return (EffectTechnique^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectTechnique(IntPtr(pResult)); }
		}

		return nullptr;
	}
	
	EffectTechnique^ GetTechniqueByName(String^ Name)
	{
		ID3D10EffectTechnique* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffect->GetTechniqueByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult) 
		{
			try { return (EffectTechnique^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectTechnique(IntPtr(pResult)); }
		}

		return nullptr;
	}
	
	EffectVariable^ GetVariableByIndex(unsigned int Index)
	{
		ID3D10EffectVariable* pResult = 0;

		pResult = pEffect->GetVariableByIndex(Index);

		if (pResult) 
		{
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}

		return nullptr;
	}
	
	EffectVariable^ GetVariableByName(String^ Name)
	{
		ID3D10EffectVariable* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffect->GetVariableByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult) 
		{
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}

		return nullptr;
	}
	
	EffectVariable^ GetVariableBySemantic(String^ Name)
	{
		ID3D10EffectVariable* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffect->GetVariableBySemantic((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult) 
		{
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}

		return nullptr;
	}

	bool IsOptimized()
	{
		return pEffect->IsOptimized() != 0;
	}

	bool IsPool()
	{
		return pEffect->IsPool() != 0;
	}

	bool IsValid()
	{
		return pEffect->IsValid() != 0;
	}

	int Optimize()
	{
		return pEffect->Optimize();
	}
};