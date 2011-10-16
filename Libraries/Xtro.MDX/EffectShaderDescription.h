public value class EffectShaderDescription : IEquatable<EffectShaderDescription>
{
internal:				 
	inline void FromNative(D3D10_EFFECT_SHADER_DESC* Native)
	{
		InputSignature = gcnew UnmanagedMemory(IntPtr((void*)Native->pInputSignature), 0);
		IsInline = Native->IsInline != 0;
		Bytecode = gcnew UnmanagedMemory(IntPtr((void*)Native->pBytecode), Native->BytecodeLength);
		StreamOutputDeclaration = gcnew String(Native->SODecl);
		NumberOfInputSignatureEntries = Native->NumInputSignatureEntries;
		NumberOfOutputSignatureEntries = Native->NumOutputSignatureEntries;
	}

public:
    UnmanagedMemory^ InputSignature;
    bool IsInline;
    UnmanagedMemory^ Bytecode;
    String^ StreamOutputDeclaration;
    unsigned int NumberOfInputSignatureEntries;
    unsigned int NumberOfOutputSignatureEntries;
		
	static bool operator == (EffectShaderDescription Left, EffectShaderDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (EffectShaderDescription Left, EffectShaderDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)IsInline ^
			(StreamOutputDeclaration == nullptr ? 1 : StreamOutputDeclaration->GetHashCode()) ^
			NumberOfInputSignatureEntries ^
			NumberOfOutputSignatureEntries;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(EffectShaderDescription Value)
	{
		return
			InputSignature == Value.InputSignature &&
			IsInline == Value.IsInline &&
			Bytecode == Value.Bytecode &&
			StreamOutputDeclaration == Value.StreamOutputDeclaration &&
			NumberOfInputSignatureEntries == Value.NumberOfInputSignatureEntries &&
			NumberOfOutputSignatureEntries == Value.NumberOfOutputSignatureEntries;
	}

	static bool Equals(EffectShaderDescription% Value1, EffectShaderDescription% Value2)
	{
		return
			Value1.InputSignature == Value2.InputSignature &&
			Value1.IsInline == Value2.IsInline &&
			Value1.Bytecode == Value2.Bytecode &&
			Value1.StreamOutputDeclaration == Value2.StreamOutputDeclaration &&
			Value1.NumberOfInputSignatureEntries == Value2.NumberOfInputSignatureEntries &&
			Value1.NumberOfOutputSignatureEntries == Value2.NumberOfOutputSignatureEntries;
	}
};