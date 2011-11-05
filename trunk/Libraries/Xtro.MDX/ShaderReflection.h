public ref class ShaderReflection sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10ShaderReflection);

internal:
	ID3D10ShaderReflection* pShaderReflection;

	ShaderReflection(IntPtr ShaderReflection) : Unknown(ShaderReflection)
	{	
		pShaderReflection = (ID3D10ShaderReflection*)ShaderReflection.ToPointer();
	}

public:
	ShaderReflectionConstantBuffer^ GetConstantBufferByIndex(unsigned int Index)
	{
		ID3D10ShaderReflectionConstantBuffer* pResult = pShaderReflection->GetConstantBufferByIndex(Index);

		if (pResult)
		{	
			try { return (ShaderReflectionConstantBuffer^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew ShaderReflectionConstantBuffer(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	ShaderReflectionConstantBuffer^ GetConstantBufferByName(String^ Name)
	{
		ID3D10ShaderReflectionConstantBuffer* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pShaderReflection->GetConstantBufferByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult)
		{	
			try { return (ShaderReflectionConstantBuffer^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew ShaderReflectionConstantBuffer(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	int GetDescription([Out] ShaderDescription% Description)
	{
		D3D10_SHADER_DESC NativeDescription;
		int Result = pShaderReflection->GetDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetInputParameterDescription(unsigned int ParameterIndex, [Out] SignatureParameterDescription% Description)
	{
		D3D10_SIGNATURE_PARAMETER_DESC NativeDescription;
		int Result = pShaderReflection->GetInputParameterDesc(ParameterIndex, &NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetOutputParameterDescription(unsigned int ParameterIndex, [Out] SignatureParameterDescription% Description)
	{
		D3D10_SIGNATURE_PARAMETER_DESC NativeDescription;
		int Result = pShaderReflection->GetOutputParameterDesc(ParameterIndex, &NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetResourceBindingDescription(unsigned int ResourceIndex, [Out] ShaderInputBindDescription% Description)
	{
		D3D10_SHADER_INPUT_BIND_DESC NativeDescription;
		int Result = pShaderReflection->GetResourceBindingDesc(ResourceIndex, &NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}
};