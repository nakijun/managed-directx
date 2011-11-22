public value class CounterInfo : IEquatable<CounterInfo>
{
public:
    CounterEnum LastDeviceDependentCounter;
    unsigned int NumberOfSimultaneousCounters;
    unsigned char NumberOfDetectableParallelUnits;

	static bool operator == (CounterInfo Left, CounterInfo Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (CounterInfo Left, CounterInfo Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)LastDeviceDependentCounter ^
			NumberOfSimultaneousCounters ^
			NumberOfDetectableParallelUnits;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(CounterInfo Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(CounterInfo::typeid)) == 0;
	}

	static bool Equals(CounterInfo% Value1, CounterInfo% Value2)
	{
		pin_ptr<CounterInfo> PinnedValue1 = &Value1;
		pin_ptr<CounterInfo> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(CounterInfo::typeid)) == 0;
	}
};