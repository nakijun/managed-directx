public value class QueryDataSO_Statistics : IEquatable<QueryDataSO_Statistics>
{
public:
    unsigned long long NumberOfPrimitivesWritten;
    unsigned long long PrimitivesStorageNeeded;

	static bool operator == (QueryDataSO_Statistics Left, QueryDataSO_Statistics Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (QueryDataSO_Statistics Left, QueryDataSO_Statistics Right)
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

	virtual bool Equals(QueryDataSO_Statistics Value)
	{
		pin_ptr<unsigned long long> PinnedThis = &NumberOfPrimitivesWritten;
		pin_ptr<QueryDataSO_Statistics> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(QueryDataSO_Statistics::typeid)) == 0;
	}

	static bool Equals(QueryDataSO_Statistics% Value1, QueryDataSO_Statistics% Value2)
	{
		pin_ptr<QueryDataSO_Statistics> PinnedValue1 = &Value1;
		pin_ptr<QueryDataSO_Statistics> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(QueryDataSO_Statistics::typeid)) == 0;
	}
};