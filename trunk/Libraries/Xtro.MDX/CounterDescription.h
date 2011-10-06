public value class CounterDescription : IEquatable<CounterDescription>
{
public:
	CounterEnum Counter;
	unsigned int MiscellaneousFlags;

	static bool operator == (CounterDescription Left, CounterDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (CounterDescription Left, CounterDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Counter ^
			MiscellaneousFlags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(CounterDescription Value)
	{
		pin_ptr<CounterEnum> PinnedThis = &Counter;
		pin_ptr<CounterDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(CounterDescription::typeid)) == 0;
	}

	static bool Equals(CounterDescription% Value1, CounterDescription% Value2)
	{
		pin_ptr<CounterDescription> PinnedValue1 = &Value1;
		pin_ptr<CounterDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(CounterDescription::typeid)) == 0;
	}
};