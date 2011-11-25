public value class SoundVariationProperties : IEquatable<SoundVariationProperties>
{
internal:
	inline void FromNative(XACT_SOUND_VARIATION_PROPERTIES* Native)
	{
		pin_ptr<XACT3::VariationProperties> PinnedVariationProperties = &VariationProperties;
		memcpy(PinnedVariationProperties, &Native->variationProperties, sizeof(XACT_VARIATION_PROPERTIES));

		SoundProperties.FromNative(&Native->soundProperties);
	}

public:
	VariationProperties VariationProperties;
	SoundProperties SoundProperties;

	static bool operator == (SoundVariationProperties Left, SoundVariationProperties Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SoundVariationProperties Left, SoundVariationProperties Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			VariationProperties.GetHashCode() ^
			SoundProperties.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SoundVariationProperties Value)
	{
		return
			VariationProperties == Value.VariationProperties &&
			SoundProperties == Value.SoundProperties;
	}

	static bool Equals(SoundVariationProperties% Value1, SoundVariationProperties% Value2)
	{
		return
			Value1.VariationProperties == Value2.VariationProperties &&
			Value1.SoundProperties == Value2.SoundProperties;
	}
};