public value class PassShaderDescription : IEquatable<PassShaderDescription>
{
internal:
	inline void FromNative(D3D10_PASS_SHADER_DESC* Native)
	{
		if (Native->pShaderVariable)
		{
			try { ShaderVariable = (EffectShaderVariable^)Interface::Interfaces[IntPtr(Native->pShaderVariable)]; }
			catch (KeyNotFoundException^) { ShaderVariable = gcnew EffectShaderVariable(IntPtr(Native->pShaderVariable)); }					
		}
		else ShaderVariable = nullptr;

		ShaderIndex = Native->ShaderIndex;
	}

public:
	EffectShaderVariable^ ShaderVariable;
    unsigned int ShaderIndex;

	static bool operator == (PassShaderDescription Left, PassShaderDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (PassShaderDescription Left, PassShaderDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			ShaderIndex;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(PassShaderDescription Value)
	{
		pin_ptr<EffectShaderVariable^> PinnedThis = &ShaderVariable;
		pin_ptr<PassShaderDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(PassShaderDescription::typeid)) == 0;
	}

	static bool Equals(PassShaderDescription% Value1, PassShaderDescription% Value2)
	{
		pin_ptr<PassShaderDescription> PinnedValue1 = &Value1;
		pin_ptr<PassShaderDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(PassShaderDescription::typeid)) == 0;
	}
};