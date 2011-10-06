public value class FrameStatistics : IEquatable<FrameStatistics>
{
public:
	unsigned int PresentCount;
	unsigned int PresentRefreshCount;
	unsigned int SyncRefreshCount;
	long long SyncQpcTime;
    long long SyncGpuTime;

	static bool operator == (FrameStatistics Left, FrameStatistics Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (FrameStatistics Left, FrameStatistics Right)
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

	virtual bool Equals(FrameStatistics Value)
	{
		pin_ptr<unsigned int> PinnedThis = &PresentCount;
		pin_ptr<FrameStatistics> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(FrameStatistics::typeid)) == 0;
	}

	static bool Equals(FrameStatistics% Value1, FrameStatistics% Value2)
	{
		pin_ptr<FrameStatistics> PinnedValue1 = &Value1;
		pin_ptr<FrameStatistics> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(FrameStatistics::typeid)) == 0;
	}
};