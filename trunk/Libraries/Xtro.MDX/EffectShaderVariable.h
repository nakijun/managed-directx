public ref class EffectShaderVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectShaderVariable);

internal:
	ID3D10EffectShaderVariable* pEffectShaderVariable;

	EffectShaderVariable(IntPtr EffectShaderVariable) : EffectVariable(EffectShaderVariable)
	{	
		pEffectShaderVariable = (ID3D10EffectShaderVariable*)EffectShaderVariable.ToPointer();
	}

public:
	int GetGeometryShader(unsigned int ShaderIndex, [Out] GeometryShader^% GeometryShader)
	{
		ID3D10GeometryShader* pGeometryShader = 0;
		int Result = pEffectShaderVariable->GetGeometryShader(ShaderIndex, &pGeometryShader);

		if (pGeometryShader) 
		{
			try { GeometryShader = (Xtro::MDX::Direct3D10::GeometryShader^)Interfaces[IntPtr(pGeometryShader)]; }
			catch (KeyNotFoundException^) { GeometryShader = gcnew Xtro::MDX::Direct3D10::GeometryShader(IntPtr(pGeometryShader)); }
		}
		else GeometryShader = nullptr;

		return Result;
	}

	int GetInputSignatureElementDescription(unsigned int ShaderIndex, unsigned int Element, [Out] SignatureParameterDescription% Description)
	{
		D3D10_SIGNATURE_PARAMETER_DESC NativeDescription;
		int Result = pEffectShaderVariable->GetInputSignatureElementDesc(ShaderIndex, Element, &NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetOutputSignatureElementDescription(unsigned int ShaderIndex, unsigned int Element, [Out] SignatureParameterDescription% Description)
	{
		D3D10_SIGNATURE_PARAMETER_DESC NativeDescription;
		int Result = pEffectShaderVariable->GetOutputSignatureElementDesc(ShaderIndex, Element, &NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetPixelShader(unsigned int ShaderIndex, [Out] PixelShader^% PixelShader)
	{
		ID3D10PixelShader* pPixelShader = 0;
		int Result = pEffectShaderVariable->GetPixelShader(ShaderIndex, &pPixelShader);

		if (pPixelShader) 
		{
			try { PixelShader = (Xtro::MDX::Direct3D10::PixelShader^)Interfaces[IntPtr(pPixelShader)]; }
			catch (KeyNotFoundException^) { PixelShader = gcnew Xtro::MDX::Direct3D10::PixelShader(IntPtr(pPixelShader)); }
		}
		else PixelShader = nullptr;

		return Result;
	}

	int GetShaderDescription(unsigned int ShaderIndex, [Out] EffectShaderDescription% Description)
	{
		D3D10_EFFECT_SHADER_DESC NativeDescription;
		int Result = pEffectShaderVariable->GetShaderDesc(ShaderIndex, &NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetVertexShader(unsigned int ShaderIndex, [Out] VertexShader^% VertexShader)
	{
		ID3D10VertexShader* pVertexShader = 0;
		int Result = pEffectShaderVariable->GetVertexShader(ShaderIndex, &pVertexShader);

		if (pVertexShader) 
		{
			try { VertexShader = (Xtro::MDX::Direct3D10::VertexShader^)Interfaces[IntPtr(pVertexShader)]; }
			catch (KeyNotFoundException^) { VertexShader = gcnew Xtro::MDX::Direct3D10::VertexShader(IntPtr(pVertexShader)); }
		}
		else VertexShader = nullptr;

		return Result;
	}
};