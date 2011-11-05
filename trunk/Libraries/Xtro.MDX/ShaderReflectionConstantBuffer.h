public ref class ShaderReflectionConstantBuffer sealed : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10ShaderReflectionConstantBuffer);

internal:
	ID3D10ShaderReflectionConstantBuffer* pShaderReflectionConstantBuffer;

	ShaderReflectionConstantBuffer(IntPtr ShaderReflectionConstantBuffer) : Interface(ShaderReflectionConstantBuffer)
	{	
		pShaderReflectionConstantBuffer = (ID3D10ShaderReflectionConstantBuffer*)ShaderReflectionConstantBuffer.ToPointer();
	}

public:
	int GetDescription([Out] ShaderBufferDescription% Description)
	{
		D3D10_SHADER_BUFFER_DESC NativeDescription;
		int Result = pShaderReflectionConstantBuffer->GetDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	ShaderReflectionVariable^ GetVariableByIndex(unsigned int Index)
	{
		ID3D10ShaderReflectionVariable* pResult = pShaderReflectionConstantBuffer->GetVariableByIndex(Index);

		if (pResult)
		{	
			try { return (ShaderReflectionVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew ShaderReflectionVariable(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	ShaderReflectionVariable^ GetVariableByName(String^ Name)
	{
		ID3D10ShaderReflectionVariable* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pShaderReflectionConstantBuffer->GetVariableByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult)
		{	
			try { return (ShaderReflectionVariable^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew ShaderReflectionVariable(IntPtr(pResult)); }
		}
		
		return nullptr;
	}
};