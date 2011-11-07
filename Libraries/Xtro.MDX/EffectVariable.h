public ref class EffectVariable : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectVariable);

internal:
	ID3D10EffectVariable* pEffectVariable;

	EffectVariable(IntPtr EffectVariable) : Interface(EffectVariable)
	{	
		pEffectVariable = (ID3D10EffectVariable*)EffectVariable.ToPointer();
	}

public:
	EffectBlendVariable^ AsBlend();
	EffectConstantBuffer^ AsConstantBuffer();
	EffectDepthStencilVariable^ AsDepthStencil();
	EffectDepthStencilViewVariable^ AsDepthStencilView();
	EffectMatrixVariable^ AsMatrix();
	EffectRasterizerVariable^ AsRasterizer();
	EffectRenderTargetViewVariable^ AsRenderTargetView();
	EffectSamplerVariable^ AsSampler();
	EffectScalarVariable^ AsScalar();
	EffectShaderVariable^ AsShader();
	EffectShaderResourceVariable^ AsShaderResource();
	EffectStringVariable^ AsString();
	EffectVectorVariable^ AsVector();

	EffectVariable^ GetAnnotationByIndex(unsigned int Index)
	{
		ID3D10EffectVariable* pResult = pEffectVariable->GetAnnotationByIndex(Index);

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
		try { pResult = pEffectVariable->GetAnnotationByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult)
		{	
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	int GetDescription([Out] EffectVariableDescription% Description)
	{
		D3D10_EFFECT_VARIABLE_DESC NativeDescription;
		int Result = pEffectVariable->GetDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	EffectVariable^ GetElement(unsigned int Index)
	{
		ID3D10EffectVariable* pResult = pEffectVariable->GetElement(Index);

		if (pResult)
		{	
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	EffectVariable^ GetMemberByIndex(unsigned int Index)
	{
		ID3D10EffectVariable* pResult = pEffectVariable->GetMemberByIndex(Index);

		if (pResult)
		{	
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	EffectVariable^ GetMemberByName(String^ Name)
	{
		ID3D10EffectVariable* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffectVariable->GetMemberByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult)
		{	
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	EffectVariable^ GetMemberBySemantic(String^ Semantic)
	{
		ID3D10EffectVariable* pResult = 0;

		IntPtr pSemantic = Marshal::StringToHGlobalAnsi(Semantic);
		try { pResult = pEffectVariable->GetMemberBySemantic((LPCSTR)pSemantic.ToPointer()); }
		finally { Marshal::FreeHGlobal(pSemantic); }

		if (pResult)
		{	
			try { return (EffectVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectVariable(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	EffectConstantBuffer^ GetParentConstantBuffer();

	int GetRawValue(UnmanagedMemory^ Data, unsigned int Offset, unsigned int Count)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		return pEffectVariable->GetRawValue(pData, Offset, Count);
	}

	EffectType^ GetType()
	{
		ID3D10EffectType* pResult = pEffectVariable->GetType();

		if (pResult)
		{	
			try { return (EffectType^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectType(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	bool IsValid()
	{
		return pEffectVariable->IsValid();
	}

	int SetRawValue(UnmanagedMemory^ Data, unsigned int Offset, unsigned int Count)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		return pEffectVariable->SetRawValue(pData, Offset, Count);
	}
};