public ref class EffectConstantBuffer sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectConstantBuffer);

internal:
	ID3D10EffectConstantBuffer* pEffectConstantBuffer;

	EffectConstantBuffer(IntPtr EffectConstantBuffer) : EffectVariable(EffectConstantBuffer)
	{	
		pEffectConstantBuffer = (ID3D10EffectConstantBuffer*)EffectConstantBuffer.ToPointer();
	}

public:
	int GetConstantBuffer([Out] Buffer^% ConstantBuffer)
	{
		ID3D10Buffer* pConstantBuffer = 0;
		int Result = pEffectConstantBuffer->GetConstantBuffer(&pConstantBuffer);

		if (pConstantBuffer) 
		{
			try { ConstantBuffer = (Buffer^)Interfaces[IntPtr(pConstantBuffer)]; }
			catch (KeyNotFoundException^) { ConstantBuffer = gcnew Buffer(IntPtr(pConstantBuffer)); }
		}
		else ConstantBuffer = nullptr;

		return Result;
	}

	int GetTextureBuffer([Out] ShaderResourceView^% TextureBuffer)
	{
		ID3D10ShaderResourceView* pTextureBuffer = 0;
		int Result = pEffectConstantBuffer->GetTextureBuffer(&pTextureBuffer);

		if (pTextureBuffer) 
		{
			try { TextureBuffer = (ShaderResourceView^)Interfaces[IntPtr(pTextureBuffer)]; }
			catch (KeyNotFoundException^) { TextureBuffer = gcnew ShaderResourceView(IntPtr(pTextureBuffer)); }
		}
		else TextureBuffer = nullptr;

		return Result;
	}

	int SetConstantBuffer(Buffer^ ConstantBuffer)
	{
		ID3D10Buffer *pConstantBuffer = ConstantBuffer == nullptr ? 0 : ConstantBuffer->pBuffer;
		return pEffectConstantBuffer->SetConstantBuffer(pConstantBuffer);
	}

	int SetTextureBuffer(ShaderResourceView^ TextureBuffer)
	{
		ID3D10ShaderResourceView *pTextureBuffer = TextureBuffer == nullptr ? 0 : TextureBuffer->pShaderResourceView;
		return pEffectConstantBuffer->SetTextureBuffer(pTextureBuffer);
	}
};