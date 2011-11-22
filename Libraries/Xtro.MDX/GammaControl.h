public value class GammaControl : IEquatable<GammaControl>
{
public:
    Rgb Scale;
    Rgb Offset;
    Rgb1025 GammaCurve;

	static bool operator == (GammaControl Left, GammaControl Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (GammaControl Left, GammaControl Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Scale.GetHashCode() ^
			Offset.GetHashCode() ^
			GammaCurve.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(GammaControl Value)
	{
		pin_ptr<GammaControl> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(GammaControl::typeid)) == 0;
	}

	static bool Equals(GammaControl% Value1, GammaControl% Value2)
	{
		pin_ptr<GammaControl> PinnedValue1 = &Value1;
		pin_ptr<GammaControl> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(GammaControl::typeid)) == 0;
	}
};