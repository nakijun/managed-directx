public ref class ShaderReflectionVariable sealed : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10ShaderReflectionVariable);

internal:
	ID3D10ShaderReflectionVariable* pShaderReflectionVariable;

	ShaderReflectionVariable(IntPtr ShaderReflectionVariable) : Interface(ShaderReflectionVariable)
	{	
		pShaderReflectionVariable = (ID3D10ShaderReflectionVariable*)ShaderReflectionVariable.ToPointer();
	}

public:
	int GetDescription([Out] ShaderVariableDescription% Description)
	{
		D3D10_SHADER_VARIABLE_DESC NativeDescription;
		int Result = pShaderReflectionVariable->GetDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	ShaderReflectionType^ GetType()
	{
		ID3D10ShaderReflectionType* pResult = pShaderReflectionVariable->GetType();

		if (pResult)
		{	
			try { return (ShaderReflectionType^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew ShaderReflectionType(IntPtr(pResult)); }
		}
		
		return nullptr;
	}
};