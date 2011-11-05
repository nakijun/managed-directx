public ref class ShaderReflectionConstantBuffer sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10ShaderReflectionConstantBuffer);

internal:
	ID3D10ShaderReflectionConstantBuffer* pShaderReflectionConstantBuffer;

	ShaderReflectionConstantBuffer(IntPtr ShaderReflectionConstantBuffer) : Unknown(ShaderReflectionConstantBuffer)
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
};