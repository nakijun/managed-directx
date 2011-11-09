public ref class EffectStringVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectStringVariable);

internal:
	ID3D10EffectStringVariable* pEffectStringVariable;

	EffectStringVariable(IntPtr EffectStringVariable) : EffectVariable(EffectStringVariable)
	{	
		pEffectStringVariable = (ID3D10EffectStringVariable*)EffectStringVariable.ToPointer();
	}

public:
	int GetString([Out] String^ String)
	{
		LPCSTR pString;

		int Result = pEffectStringVariable->GetString(&pString);
		String = pString ? gcnew System::String(pString) : nullptr;

		return Result;
	}

	int GetStringArray(array<String^>^ Strings, unsigned int Offset, unsigned int Count)
	{
		unsigned int Length = Strings == nullptr ? 0 : Math::Min(Offset + Count, (unsigned int)Strings->Length);
		LPCSTR* pStrings = Strings != nullptr && Strings->Length > 0 ? new LPCSTR[Length] : 0;
		try
		{
			int Result = pEffectStringVariable->GetStringArray(pStrings, Offset, Count);

			if (pStrings)
			{
				for (unsigned int No = Offset; No < Length; No++)
				{
					Strings[No] = pStrings[No] ? gcnew System::String(pStrings[No]) : nullptr;
				}
			}

			return Result;
		}
		finally
		{
			if (pStrings) delete[] pStrings;
		}
	}
};