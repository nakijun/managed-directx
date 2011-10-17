public value class ShaderDescription : IEquatable<ShaderDescription>
{
internal:				 
	inline void FromNative(D3D10_SHADER_DESC* Native)
	{
		Version = Native->Version;
		Creator = Native->Creator ? gcnew String(Native->Creator) : nullptr;
		Flags = Native->Flags;
		ConstantBuffers = Native->ConstantBuffers;
		BoundResources = Native->BoundResources;
		InputParameters = Native->InputParameters;
		OutputParameters = Native->OutputParameters;
		InstructionCount = Native->InstructionCount;
		TemporaryRegisterCount = Native->TempRegisterCount;
		TemporaryArrayCount = Native->TempArrayCount;
		DefineCount = Native->DefCount;
		DeclarationCount = Native->DclCount;
		TextureNormalInstructions = Native->TextureNormalInstructions;
		TextureLoadInstructions = Native->TextureLoadInstructions;
		TextureComparisonInstructions = Native->TextureCompInstructions;
		TextureBiasInstructions = Native->TextureBiasInstructions;
		TextureGradientInstructions = Native->TextureGradientInstructions;
		FloatInstructionCount = Native->FloatInstructionCount;
		IntInstructionCount = Native->IntInstructionCount;
		UintInstructionCount = Native->UintInstructionCount;
		StaticFlowControlCount = Native->StaticFlowControlCount;
		DynamicFlowControlCount = Native->DynamicFlowControlCount;
		MacroInstructionCount = Native->MacroInstructionCount;
		ArrayInstructionCount = Native->ArrayInstructionCount;
		CutInstructionCount = Native->CutInstructionCount;
		EmitInstructionCount = Native->EmitInstructionCount;
		GeometryShaderOutputTopology = (PrimitiveTopology)Native->GSOutputTopology;
		GeometryShaderMaximumOutputVertexCount = Native->GSMaxOutputVertexCount;
	}

public:
    unsigned int Version;
    String^ Creator;
    unsigned int Flags;
    unsigned int ConstantBuffers;
    unsigned int BoundResources;
    unsigned int InputParameters;
    unsigned int OutputParameters;
    unsigned int InstructionCount;
    unsigned int TemporaryRegisterCount;
    unsigned int TemporaryArrayCount;
    unsigned int DefineCount;
    unsigned int DeclarationCount;
    unsigned int TextureNormalInstructions;
    unsigned int TextureLoadInstructions;
    unsigned int TextureComparisonInstructions;
    unsigned int TextureBiasInstructions;
    unsigned int TextureGradientInstructions;
    unsigned int FloatInstructionCount;
    unsigned int IntInstructionCount;
    unsigned int UintInstructionCount;
    unsigned int StaticFlowControlCount;
    unsigned int DynamicFlowControlCount;
    unsigned int MacroInstructionCount;
    unsigned int ArrayInstructionCount;
    unsigned int CutInstructionCount;
    unsigned int EmitInstructionCount;
    PrimitiveTopology GeometryShaderOutputTopology;
    unsigned int GeometryShaderMaximumOutputVertexCount;

	static bool operator == (ShaderDescription Left, ShaderDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ShaderDescription Left, ShaderDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Version ^
			(Creator == nullptr ? 1 : Creator->GetHashCode()) ^
			Flags ^
			ConstantBuffers ^
			BoundResources ^
			InputParameters ^
			OutputParameters ^
			InstructionCount ^
			TemporaryRegisterCount ^
			TemporaryArrayCount ^
			DefineCount ^
			DeclarationCount ^
			TextureNormalInstructions ^
			TextureLoadInstructions ^
			TextureComparisonInstructions ^
			TextureBiasInstructions ^
			TextureGradientInstructions ^
			FloatInstructionCount ^
			IntInstructionCount ^
			UintInstructionCount ^
			StaticFlowControlCount ^
			DynamicFlowControlCount ^
			MacroInstructionCount ^
			ArrayInstructionCount ^
			CutInstructionCount ^
			EmitInstructionCount ^
			(int)GeometryShaderOutputTopology ^
			GeometryShaderMaximumOutputVertexCount;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ShaderDescription Value)
	{
		return
			Version = Value.Version &&
			Creator == Value.Creator &&
			Flags == Value.Flags &&
			ConstantBuffers == Value.ConstantBuffers &&
			BoundResources == Value.BoundResources &&
			InputParameters == Value.InputParameters &&
			OutputParameters == Value.OutputParameters &&
			InstructionCount == Value.InstructionCount &&
			TemporaryRegisterCount == Value.TemporaryRegisterCount &&
			TemporaryArrayCount == Value.TemporaryArrayCount &&
			DefineCount == Value.DefineCount &&
			DeclarationCount == Value.DeclarationCount &&
			TextureNormalInstructions == Value.TextureNormalInstructions &&
			TextureLoadInstructions == Value.TextureLoadInstructions &&
			TextureComparisonInstructions == Value.TextureComparisonInstructions &&
			TextureBiasInstructions == Value.TextureBiasInstructions &&
			TextureGradientInstructions == Value.TextureGradientInstructions &&
			FloatInstructionCount == Value.FloatInstructionCount &&
			IntInstructionCount == Value.IntInstructionCount &&
			UintInstructionCount == Value.UintInstructionCount &&
			StaticFlowControlCount == Value.StaticFlowControlCount &&
			DynamicFlowControlCount == Value.DynamicFlowControlCount &&
			MacroInstructionCount == Value.MacroInstructionCount &&
			ArrayInstructionCount == Value.ArrayInstructionCount &&
			CutInstructionCount == Value.CutInstructionCount &&
			EmitInstructionCount == Value.EmitInstructionCount &&
			GeometryShaderOutputTopology == Value.GeometryShaderOutputTopology &&
			GeometryShaderMaximumOutputVertexCount == Value.GeometryShaderMaximumOutputVertexCount;
	}

	static bool Equals(ShaderDescription% Value1, ShaderDescription% Value2)
	{
		return
			Value1.Version = Value2.Version &&
			Value1.Creator == Value2.Creator &&
			Value1.Flags == Value2.Flags &&
			Value1.ConstantBuffers == Value2.ConstantBuffers &&
			Value1.BoundResources == Value2.BoundResources &&
			Value1.InputParameters == Value2.InputParameters &&
			Value1.OutputParameters == Value2.OutputParameters &&
			Value1.InstructionCount == Value2.InstructionCount &&
			Value1.TemporaryRegisterCount == Value2.TemporaryRegisterCount &&
			Value1.TemporaryArrayCount == Value2.TemporaryArrayCount &&
			Value1.DefineCount == Value2.DefineCount &&
			Value1.DeclarationCount == Value2.DeclarationCount &&
			Value1.TextureNormalInstructions == Value2.TextureNormalInstructions &&
			Value1.TextureLoadInstructions == Value2.TextureLoadInstructions &&
			Value1.TextureComparisonInstructions == Value2.TextureComparisonInstructions &&
			Value1.TextureBiasInstructions == Value2.TextureBiasInstructions &&
			Value1.TextureGradientInstructions == Value2.TextureGradientInstructions &&
			Value1.FloatInstructionCount == Value2.FloatInstructionCount &&
			Value1.IntInstructionCount == Value2.IntInstructionCount &&
			Value1.UintInstructionCount == Value2.UintInstructionCount &&
			Value1.StaticFlowControlCount == Value2.StaticFlowControlCount &&
			Value1.DynamicFlowControlCount == Value2.DynamicFlowControlCount &&
			Value1.MacroInstructionCount == Value2.MacroInstructionCount &&
			Value1.ArrayInstructionCount == Value2.ArrayInstructionCount &&
			Value1.CutInstructionCount == Value2.CutInstructionCount &&
			Value1.EmitInstructionCount == Value2.EmitInstructionCount &&
			Value1.GeometryShaderOutputTopology == Value2.GeometryShaderOutputTopology &&
			Value1.GeometryShaderMaximumOutputVertexCount == Value2.GeometryShaderMaximumOutputVertexCount;
	}
};