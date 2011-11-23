public value class VariationProperties : IEquatable<VariationProperties>
{
public:
	unsigned int PresentCount;
	unsigned int PresentRefreshCount;
	unsigned int SyncRefreshCount;
	long long SyncQpcTime;
    long long SyncGpuTime;

	static bool operator == (VariationProperties Left, VariationProperties Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (VariationProperties Left, VariationProperties Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			PresentCount ^
			PresentRefreshCount ^
			SyncRefreshCount ^
			SyncQpcTime.GetHashCode() ^
			SyncGpuTime.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(VariationProperties Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(VariationProperties::typeid)) == 0;
	}

	static bool Equals(VariationProperties% Value1, VariationProperties% Value2)
	{
		pin_ptr<VariationProperties> PinnedValue1 = &Value1;
		pin_ptr<VariationProperties> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(VariationProperties::typeid)) == 0;
	}
};