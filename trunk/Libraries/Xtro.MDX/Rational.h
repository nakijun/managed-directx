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
		return
			Numerator == Value.Numerator &&
			Denominator == Value.Denominator;
	}

	static bool Equals(Rational% Value1, Rational% Value2)
	{
		return
			Value1.Numerator == Value2.Numerator && 
			Value1.Denominator == Value2.Denominator;
	}
};