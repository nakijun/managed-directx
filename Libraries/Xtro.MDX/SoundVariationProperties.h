public value class SoundVariationProperties : IEquatable<SoundVariationProperties>
{
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
		return memcmp(&*this, &Value, Marshal::SizeOf(SoundVariationProperties::typeid)) == 0;
	}

	static bool Equals(SoundVariationProperties% Value1, SoundVariationProperties% Value2)
	{
		pin_ptr<SoundVariationProperties> PinnedValue1 = &Value1;
		pin_ptr<SoundVariationProperties> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SoundVariationProperties::typeid)) == 0;
	}
};