public value class Rgb : IEquatable<Rgb>
{
public:
	float Red;
    float Green;
    float Blue;	 

	static bool operator == (Rgb Left, Rgb Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Rgb Left, Rgb Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Red.GetHashCode() ^
			Green.GetHashCode() ^
			Blue.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Rgb Value)
	{
		pin_ptr<float> PinnedThis = &Red;
		pin_ptr<Rgb> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Rgb::typeid)) == 0;
	}

	static bool Equals(Rgb% Value1, Rgb% Value2)
	{
		pin_ptr<Rgb> PinnedValue1 = &Value1;
		pin_ptr<Rgb> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Rgb::typeid)) == 0;
	}
};