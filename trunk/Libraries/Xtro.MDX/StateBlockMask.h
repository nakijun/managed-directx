public value class StateBlockMask : IEquatable<StateBlockMask>
{
public:
	unsigned char VS;
	Byte2 VS_Samplers;
	Byte16 VS_ShaderResources;
	Byte2 VS_ConstantBuffers;
	unsigned char GS;
	Byte2 GS_Samplers;       
	Byte16 GS_ShaderResources;
	Byte2 GS_ConstantBuffers;
	unsigned char PS;
	Byte2 PS_Samplers;       
	Byte16 PS_ShaderResources;
	Byte2 PS_ConstantBuffers;
	Byte2 IA_VertexBuffers;  
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
			VS_Samplers.GetHashCode() ^
			VS_ShaderResources.GetHashCode() ^
			VS_ConstantBuffers.GetHashCode() ^
			GS ^
			GS_Samplers.GetHashCode() ^
			GS_ShaderResources.GetHashCode() ^
			GS_ConstantBuffers.GetHashCode() ^
			PS ^
			PS_Samplers.GetHashCode() ^
			PS_ShaderResources.GetHashCode() ^
			PS_ConstantBuffers.GetHashCode() ^
			IA_VertexBuffers.GetHashCode() ^
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
		pin_ptr<unsigned char> PinnedThis = &VS;
		pin_ptr<StateBlockMask> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(StateBlockMask::typeid)) == 0;
	}

	static bool Equals(StateBlockMask% Value1, StateBlockMask% Value2)
	{
		pin_ptr<StateBlockMask> PinnedValue1 = &Value1;
		pin_ptr<StateBlockMask> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(StateBlockMask::typeid)) == 0;
	}
};