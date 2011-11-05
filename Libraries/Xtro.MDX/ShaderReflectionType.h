public ref class ShaderReflectionType sealed : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10ShaderReflectionType);

internal:
	ID3D10ShaderReflectionType* pShaderReflectionType;

	ShaderReflectionType(IntPtr ShaderReflectionType) : Interface(ShaderReflectionType)
	{	
		pShaderReflectionType = (ID3D10ShaderReflectionType*)ShaderReflectionType.ToPointer();
	}

public:
	int GetDescription([Out] ShaderTypeDescription% Description)
	{
		pin_ptr<ShaderTypeDescription> PinnedDescription = &Description;
		return pShaderReflectionType->GetDesc((D3D10_SHADER_TYPE_DESC*)PinnedDescription);
	}

	String^ GetMemberTypeName(unsigned int Index)
	{
		LPCSTR Result = pShaderReflectionType->GetMemberTypeName(Index);
		return gcnew String(Result);
	}

	ShaderReflectionType^ GetMemberTypeByIndex(unsigned int Index)
	{
		ID3D10ShaderReflectionType* pResult = pShaderReflectionType->GetMemberTypeByIndex(Index);

		if (pResult)
		{	
			try { return (ShaderReflectionType^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew ShaderReflectionType(IntPtr(pResult)); }
		}
		
		return nullptr;
	}

	ShaderReflectionType^ GetMemberTypeByName(String^ Name)
	{
		ID3D10ShaderReflectionType* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pShaderReflectionType->GetMemberTypeByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult)
		{	
			try { return (ShaderReflectionType^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew ShaderReflectionType(IntPtr(pResult)); }
		}
		
		return nullptr;
	}
};