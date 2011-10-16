public value class QueryDataStreamOutputStatistics : IEquatable<QueryDataStreamOutputStatistics>
{
public:
    unsigned long long NumberOfPrimitivesWritten;
    unsigned long long PrimitivesStorageNeeded;

	static bool operator == (QueryDataStreamOutputStatistics Left, QueryDataStreamOutputStatistics Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (QueryDataStreamOutputStatistics Left, QueryDataStreamOutputStatistics Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			NumberOfPrimitivesWritten.GetHashCode() ^
			PrimitivesStorageNeeded.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(QueryDataStreamOutputStatistics Value)
	{
		pin_ptr<unsigned long long> PinnedThis = &NumberOfPrimitivesWritten;
		pin_ptr<QueryDataStreamOutputStatistics> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(QueryDataStreamOutputStatistics::typeid)) == 0;
	}

	static bool Equals(QueryDataStreamOutputStatistics% Value1, QueryDataStreamOutputStatistics% Value2)
	{
		pin_ptr<QueryDataStreamOutputStatistics> PinnedValue1 = &Value1;
		pin_ptr<QueryDataStreamOutputStatistics> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(QueryDataStreamOutputStatistics::typeid)) == 0;
	}
};