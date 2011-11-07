public ref class EffectType sealed : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectType);

internal:
	ID3D10EffectType* pEffectType;

	EffectType(IntPtr EffectType) : Interface(EffectType)
	{	
		pEffectType = (ID3D10EffectType*)EffectType.ToPointer();
	}

public:
	int GetDescription([Out] EffectTypeDescription% Description)
	{
		D3D10_EFFECT_TYPE_DESC NativeDescription;
		int Result = pEffectType->GetDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	String^ GetMemberName(unsigned int Index)
	{
		LPCSTR pResult = pEffectType->GetMemberName(Index);
		return pResult ? gcnew System::String(pResult) : nullptr;
	}

	String^ GetMemberSemantic(unsigned int Index)
	{
		LPCSTR pResult = pEffectType->GetMemberSemantic(Index);
		return pResult ? gcnew System::String(pResult) : nullptr;
	}

	EffectType^ GetMemberTypeByIndex(unsigned int Index)
	{
		ID3D10EffectType* pResult = pEffectType->GetMemberTypeByIndex(Index);

		if (pResult) 
		{
			try { return (EffectType^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectType(IntPtr(pResult)); }
		}

		return nullptr;
	}
	
	EffectType^ GetMemberTypeByName(String^ Name)
	{
		ID3D10EffectType* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffectType->GetMemberTypeByName((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult) 
		{
			try { return (EffectType^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectType(IntPtr(pResult)); }
		}

		return nullptr;
	}
	
	EffectType^ GetMemberTypeBySemantic(String^ Name)
	{
		ID3D10EffectType* pResult = 0;

		IntPtr pName = Marshal::StringToHGlobalAnsi(Name);
		try { pResult = pEffectType->GetMemberTypeBySemantic((LPCSTR)pName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pName); }

		if (pResult) 
		{
			try { return (EffectType^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew EffectType(IntPtr(pResult)); }
		}

		return nullptr;
	}

	bool IsValid()
	{
		return pEffectType->IsValid();
	}
};