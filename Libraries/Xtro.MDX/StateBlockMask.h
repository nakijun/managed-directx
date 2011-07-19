public value class StateBlockMask : IEquatable<StateBlockMask>
{
internal:				 
	void FromNative(D3D10_STATE_BLOCK_MASK* Native)
	{
		VS = Native->VS;
		GS = Native->GS;
		PS = Native->PS;
		IA_IndexBuffer = Native->IAIndexBuffer;
		IA_InputLayout = Native->IAInputLayout;
		IA_PrimitiveTopology = Native->IAPrimitiveTopology;
		OM_RenderTargets = Native->OMRenderTargets;
		OM_DepthStencilState = Native->OMDepthStencilState;
		OM_BlendState = Native->OMBlendState;
		RS_Viewports = Native->RSViewports;
		RS_ScissorRectangles = Native->RSScissorRects;
		RS_RasterizerState = Native->RSRasterizerState;
		SO_Buffers = Native->SOBuffers;
		Predication = Native->Predication;

		VS_Samplers = gcnew UnmanagedMemory(Constants::CommonShaderSamplerSlotCount);
		memcpy(VS_Samplers->pMemory, Native->VSSamplers, VS_Samplers->Size);

		VS_ShaderResources = gcnew UnmanagedMemory(Constants::CommonShaderInputResourceSlotCount);
		memcpy(VS_ShaderResources->pMemory, Native->VSShaderResources, VS_ShaderResources->Size);

		VS_ConstantBuffers = gcnew UnmanagedMemory(Constants::CommonShaderConstantBufferApiSlotCount);
		memcpy(VS_ConstantBuffers->pMemory, Native->VSConstantBuffers, VS_ConstantBuffers->Size);

		GS_Samplers = gcnew UnmanagedMemory(Constants::CommonShaderSamplerSlotCount);
		memcpy(GS_Samplers->pMemory, Native->GSSamplers, GS_Samplers->Size);

		GS_ShaderResources = gcnew UnmanagedMemory(Constants::CommonShaderInputResourceSlotCount);
		memcpy(GS_ShaderResources->pMemory, Native->GSShaderResources, GS_ShaderResources->Size);

		GS_ConstantBuffers = gcnew UnmanagedMemory(Constants::CommonShaderConstantBufferApiSlotCount);
		memcpy(GS_ConstantBuffers->pMemory, Native->GSConstantBuffers, GS_ConstantBuffers->Size);

		PS_Samplers = gcnew UnmanagedMemory(Constants::CommonShaderSamplerSlotCount);
		memcpy(PS_Samplers->pMemory, Native->PSSamplers, PS_Samplers->Size);

		PS_ShaderResources = gcnew UnmanagedMemory(Constants::CommonShaderInputResourceSlotCount);
		memcpy(PS_ShaderResources->pMemory, Native->PSShaderResources, PS_ShaderResources->Size);

		PS_ConstantBuffers = gcnew UnmanagedMemory(Constants::CommonShaderConstantBufferApiSlotCount);
		memcpy(PS_ConstantBuffers->pMemory, Native->PSConstantBuffers, PS_ConstantBuffers->Size);

		IA_VertexBuffers = gcnew UnmanagedMemory(Constants::IA_VertexInputResourceSlotCount);
		memcpy(IA_VertexBuffers->pMemory, Native->IAVertexBuffers, IA_VertexBuffers->Size);
	}

	void ToNative(D3D10_STATE_BLOCK_MASK* Native)
	{
		Native->VS = VS;
		Native->GS = GS;
		Native->PS = PS;
		Native->IAIndexBuffer = IA_IndexBuffer;
		Native->IAInputLayout = IA_InputLayout;
		Native->IAPrimitiveTopology = IA_PrimitiveTopology;
		Native->OMRenderTargets = OM_RenderTargets;
		Native->OMDepthStencilState = OM_DepthStencilState;
		Native->OMBlendState = OM_BlendState;
		Native->RSViewports = RS_Viewports;
		Native->RSScissorRects = RS_ScissorRectangles;
		Native->RSRasterizerState = RS_RasterizerState;
		Native->SOBuffers = SO_Buffers;
		Native->Predication = Predication;

		memcpy(Native->VSSamplers, VS_Samplers->pMemory, VS_Samplers->Size);
		memcpy(Native->VSShaderResources, VS_ShaderResources->pMemory, VS_ShaderResources->Size);
		memcpy(Native->VSConstantBuffers, VS_ConstantBuffers->pMemory, VS_ConstantBuffers->Size);
		memcpy(Native->GSSamplers, GS_Samplers->pMemory, GS_Samplers->Size);
		memcpy(Native->GSShaderResources, GS_ShaderResources->pMemory, GS_ShaderResources->Size);
		memcpy(Native->GSConstantBuffers, GS_ConstantBuffers->pMemory, GS_ConstantBuffers->Size);
		memcpy(Native->PSSamplers, PS_Samplers->pMemory, PS_Samplers->Size);
		memcpy(Native->PSShaderResources, PS_ShaderResources->pMemory, PS_ShaderResources->Size);
		memcpy(Native->PSConstantBuffers, PS_ConstantBuffers->pMemory, PS_ConstantBuffers->Size);
		memcpy(Native->IAVertexBuffers, IA_VertexBuffers->pMemory, IA_VertexBuffers->Size);
	}

public:
	unsigned char VS;
	UnmanagedMemory^ VS_Samplers;
	UnmanagedMemory^ VS_ShaderResources;
	UnmanagedMemory^ VS_ConstantBuffers;
	unsigned char GS;
	UnmanagedMemory^ GS_Samplers;       
	UnmanagedMemory^ GS_ShaderResources;
	UnmanagedMemory^ GS_ConstantBuffers;
	unsigned char PS;
	UnmanagedMemory^ PS_Samplers;       
	UnmanagedMemory^ PS_ShaderResources;
	UnmanagedMemory^ PS_ConstantBuffers;
	UnmanagedMemory^ IA_VertexBuffers;  
	unsigned char IA_IndexBuffer;
	unsigned char IA_InputLayout;
	unsigned char IA_PrimitiveTopology;
	unsigned char OM_RenderTargets;
	unsigned char OM_DepthStencilState;
	unsigned char OM_BlendState;
	unsigned char RS_Viewports;
	unsigned char RS_ScissorRectangles;
	unsigned char RS_RasterizerState;
	unsigned char SO_Buffers;
	unsigned char Predication;
		
	static bool operator == (StateBlockMask Left, StateBlockMask Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (StateBlockMask Left, StateBlockMask Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			VS ^
			VS_Samplers->GetHashCode() ^
			VS_ShaderResources->GetHashCode() ^
			VS_ConstantBuffers->GetHashCode() ^
			GS ^
			GS_Samplers->GetHashCode() ^
			GS_ShaderResources->GetHashCode() ^
			GS_ConstantBuffers->GetHashCode() ^
			PS ^
			PS_Samplers->GetHashCode() ^
			PS_ShaderResources->GetHashCode() ^
			PS_ConstantBuffers->GetHashCode() ^
			IA_VertexBuffers->GetHashCode() ^
			IA_IndexBuffer ^
			IA_InputLayout ^
			IA_PrimitiveTopology ^
			OM_RenderTargets ^
			OM_DepthStencilState ^
			OM_BlendState ^
			RS_Viewports ^
			RS_ScissorRectangles ^
			RS_RasterizerState ^
			SO_Buffers ^
			Predication;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(StateBlockMask Value)
	{
		return
			VS == Value.VS &&
			GS == Value.GS &&
			PS == Value.PS &&
			IA_IndexBuffer == Value.IA_IndexBuffer &&
			IA_InputLayout == Value.IA_InputLayout &&
			IA_PrimitiveTopology == Value.IA_PrimitiveTopology &&
			OM_RenderTargets == Value.OM_RenderTargets &&
			OM_DepthStencilState == Value.OM_DepthStencilState &&
			OM_BlendState == Value.OM_BlendState &&
			RS_Viewports == Value.RS_Viewports &&
			RS_ScissorRectangles == Value.RS_ScissorRectangles &&
			RS_RasterizerState == Value.RS_RasterizerState &&
			SO_Buffers == Value.SO_Buffers &&
			Predication == Value.Predication &&
			VS_Samplers->Equals(Value.VS_Samplers) &&
			VS_ShaderResources->Equals(Value.VS_ShaderResources) &&
			VS_ConstantBuffers->Equals(Value.VS_ConstantBuffers) &&
			GS_Samplers->Equals(Value.GS_Samplers) &&
			GS_ShaderResources->Equals(Value.GS_ShaderResources) &&
			GS_ConstantBuffers->Equals(Value.GS_ConstantBuffers) &&
			PS_Samplers->Equals(Value.PS_Samplers) &&
			PS_ShaderResources->Equals(Value.PS_ShaderResources) &&
			PS_ConstantBuffers->Equals(Value.PS_ConstantBuffers) &&
			IA_VertexBuffers->Equals(Value.IA_VertexBuffers);
	}

	static bool Equals(StateBlockMask% Value1, StateBlockMask% Value2)
	{
		return
			Value1.VS == Value2.VS &&
			Value1.GS == Value2.GS &&
			Value1.PS == Value2.PS &&
			Value1.IA_IndexBuffer == Value2.IA_IndexBuffer &&
			Value1.IA_InputLayout == Value2.IA_InputLayout &&
			Value1.IA_PrimitiveTopology == Value2.IA_PrimitiveTopology &&
			Value1.OM_RenderTargets == Value2.OM_RenderTargets &&
			Value1.OM_DepthStencilState == Value2.OM_DepthStencilState &&
			Value1.OM_BlendState == Value2.OM_BlendState &&
			Value1.RS_Viewports == Value2.RS_Viewports &&
			Value1.RS_ScissorRectangles == Value2.RS_ScissorRectangles &&
			Value1.RS_RasterizerState == Value2.RS_RasterizerState &&
			Value1.SO_Buffers == Value2.SO_Buffers &&
			Value1.Predication == Value2.Predication &&
			Value1.VS_Samplers->Equals(Value2.VS_Samplers) &&
			Value1.VS_ShaderResources->Equals(Value2.VS_ShaderResources) &&
			Value1.VS_ConstantBuffers->Equals(Value2.VS_ConstantBuffers) &&
			Value1.GS_Samplers->Equals(Value2.GS_Samplers) &&
			Value1.GS_ShaderResources->Equals(Value2.GS_ShaderResources) &&
			Value1.GS_ConstantBuffers->Equals(Value2.GS_ConstantBuffers) &&
			Value1.PS_Samplers->Equals(Value2.PS_Samplers) &&
			Value1.PS_ShaderResources->Equals(Value2.PS_ShaderResources) &&
			Value1.PS_ConstantBuffers->Equals(Value2.PS_ConstantBuffers) &&
			Value1.IA_VertexBuffers->Equals(Value2.IA_VertexBuffers);
	}
};