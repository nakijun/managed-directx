public value class Rational : IEquatable<Rational>
{
public:
	unsigned int Numerator;
	unsigned int Denominator;

	static bool operator == (Rational Left, Rational Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Rational Left, Rational Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Numerator ^
			Denominator;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Rational Value)
	{
		pin_ptr<unsigned int> PinnedThis = &Numerator;
		pin_ptr<Rational> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Rational::typeid)) == 0;
	}

	static bool Equals(Rational% Value1, Rational% Value2)
	{
		pin_ptr<Rational> PinnedValue1 = &Value1;
		pin_ptr<Rational> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Rational::typeid)) == 0;
	}
};