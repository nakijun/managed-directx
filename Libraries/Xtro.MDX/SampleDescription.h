public value class SampleDescription : IEquatable<SampleDescription>
{
public:
	unsigned int Count;
	unsigned int Quality;

	static bool operator == (SampleDescription Left, SampleDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SampleDescription Left, SampleDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Count ^
			Quality;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SampleDescription Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(SampleDescription::typeid)) == 0;
	}

	static bool Equals(SampleDescription% Value1, SampleDescription% Value2)
	{
		pin_ptr<SampleDescription> PinnedValue1 = &Value1;
		pin_ptr<SampleDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SampleDescription::typeid)) == 0;
	}
};