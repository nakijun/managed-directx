public ref class EffectPass sealed : Interface
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
	int Apply(unsigned int Flags)
	{
		return pEffectPass->Apply(Flags);
	}

	int ComputeStateBlockMask([Out] StateBlockMask% StateBlockMask)
	{
		pin_ptr<Xtro::MDX::Direct3D10::StateBlockMask> PinnedStateBlockMask = &StateBlockMask;
		return pEffectPass->ComputeStateBlockMask((D3D10_STATE_BLOCK_MASK*)PinnedStateBlockMask);
	}

	EffectVariable^ GetAnnotationByIndex(unsigned int Index)
	{
		ID3D10EffectVariable* pResult = pEffectPass->GetAnnotationByIndex(Index);

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
		try { pResult = pEffectPass->GetAnnotationByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult)
		{	
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	int GetDescription([Out] PassDescription% Description)
	{
		D3D10_PASS_DESC NativeDescription;
		int Result = pEffectPass->GetDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetGeometryShaderDescription([Out] PassShaderDescription% Description)
	{
		D3D10_PASS_SHADER_DESC NativeDescription;
		int Result = pEffectPass->GetGeometryShaderDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetPixelShaderDescription([Out] PassShaderDescription% Description)
	{
		D3D10_PASS_SHADER_DESC NativeDescription;
		int Result = pEffectPass->GetPixelShaderDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetVertexShaderDescription([Out] PassShaderDescription% Description)
	{
		D3D10_PASS_SHADER_DESC NativeDescription;
		int Result = pEffectPass->GetVertexShaderDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	bool IsValid()
	{
		return pEffectPass->IsValid();
	}
};