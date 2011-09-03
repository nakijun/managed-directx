public value class ShaderMacro : IEquatable<ShaderMacro>
{
internal: 
	IntPtr pName;
	IntPtr pDefinition;

	inline void Marshal(D3D10_SHADER_MACRO* Native)
	{
		pName = Marshal::StringToHGlobalAnsi(Name);
		pDefinition = Marshal::StringToHGlobalAnsi(Definition);

		Native->Name = (LPCSTR)pName.ToPointer();
		Native->Definition = (LPCSTR)pDefinition.ToPointer();
	}

	inline void Unmarshal()
	{
		Marshal::FreeHGlobal(pName); 
		Marshal::FreeHGlobal(pDefinition); 
	}

public:
	String^ Name;
	String^ Definition;

	static bool operator == (ShaderMacro Left, ShaderMacro Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ShaderMacro Left, ShaderMacro Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Name == nullptr ? 1 : Name->GetHashCode() ^
			(Definition == nullptr ? 1 : Definition->GetHashCode());
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ShaderMacro Value)
	{
		return
			Name == Value.Name &&
			Definition == Value.Definition;
	}

	static bool Equals(ShaderMacro% Value1, ShaderMacro% Value2)
	{
		return
			Value1.Name == Value2.Name && 
			Value1.Definition == Value2.Definition;
	}
};