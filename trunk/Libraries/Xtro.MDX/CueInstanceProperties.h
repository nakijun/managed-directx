public value class CueInstanceProperties : IEquatable<CueInstanceProperties>
{
internal:
	inline void FromNative(XACT_CUE_INSTANCE_PROPERTIES* Native)
	{
		AllocationAttributes = Native->allocAttributes;
		CueProperties.FromNative(&Native->cueProperties);
		ActiveVariationProperties.FromNative(&Native->activeVariationProperties);
	}

public:
	unsigned long AllocationAttributes;
	CueProperties CueProperties;
	SoundVariationProperties ActiveVariationProperties;

	static bool operator == (CueInstanceProperties Left, CueInstanceProperties Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (CueInstanceProperties Left, CueInstanceProperties Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			AllocationAttributes ^
			CueProperties.GetHashCode() ^
			ActiveVariationProperties.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(CueInstanceProperties Value)
	{
		return
			AllocationAttributes == Value.AllocationAttributes &&
			CueProperties == Value.CueProperties &&
			ActiveVariationProperties == Value.ActiveVariationProperties;
	}

	static bool Equals(CueInstanceProperties% Value1, CueInstanceProperties% Value2)
	{
		return
			Value1.AllocationAttributes == Value2.AllocationAttributes &&
			Value1.CueProperties == Value2.CueProperties &&
			Value1.ActiveVariationProperties == Value2.ActiveVariationProperties;
	}
};