public value class CueProperties : IEquatable<CueProperties>
{
internal:
	inline void FromNative(XACT_CUE_PROPERTIES* Native)
	{
		FriendlyName = Native->friendlyName ? gcnew String(Native->friendlyName) : nullptr;
			
		Interactive = Native->interactive;
		VariableIndex = Native->iaVariableIndex;
		NumberOfVariations = Native->numVariations;
		MaximumInstances = Native->maxInstances;
		CurrentInstances = Native->currentInstances;
	}

public:
	String^ FriendlyName;
	bool Interactive;                       
	unsigned short VariableIndex;                   
	unsigned short NumberOfVariations;                     
	unsigned char MaximumInstances;                      
	unsigned char CurrentInstances;                  

	static bool operator == (CueProperties Left, CueProperties Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (CueProperties Left, CueProperties Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			FriendlyName == nullptr ? 1 : FriendlyName->GetHashCode() ^
			(int)Interactive ^
			VariableIndex ^
			NumberOfVariations ^
			MaximumInstances ^
			CurrentInstances;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(CueProperties Value)
	{
		return
			FriendlyName == Value.FriendlyName &&
			Interactive == Value.Interactive &&
			VariableIndex == Value.VariableIndex &&
			NumberOfVariations == Value.NumberOfVariations &&
			MaximumInstances == Value.MaximumInstances &&
			CurrentInstances == Value.CurrentInstances;
	}

	static bool Equals(CueProperties% Value1, CueProperties% Value2)
	{
		return
			Value1.FriendlyName == Value2.FriendlyName &&
			Value1.Interactive == Value2.Interactive &&
			Value1.VariableIndex == Value2.VariableIndex &&
			Value1.NumberOfVariations == Value2.NumberOfVariations &&
			Value1.MaximumInstances == Value2.MaximumInstances &&
			Value1.CurrentInstances == Value2.CurrentInstances;
	}
};