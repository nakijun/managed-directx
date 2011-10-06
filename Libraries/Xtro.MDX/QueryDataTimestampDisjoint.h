public value class QueryDataTimestampDisjoint : IEquatable<QueryDataTimestampDisjoint>
{
public:
    unsigned long long Frequency;
    bool Disjoint;

	static bool operator == (QueryDataTimestampDisjoint Left, QueryDataTimestampDisjoint Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (QueryDataTimestampDisjoint Left, QueryDataTimestampDisjoint Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Frequency.GetHashCode() ^
			(int)Disjoint;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(QueryDataTimestampDisjoint Value)
	{
		pin_ptr<unsigned long long> PinnedThis = &Frequency;
		pin_ptr<QueryDataTimestampDisjoint> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(QueryDataTimestampDisjoint::typeid)) == 0;
	}

	static bool Equals(QueryDataTimestampDisjoint% Value1, QueryDataTimestampDisjoint% Value2)
	{
		pin_ptr<QueryDataTimestampDisjoint> PinnedValue1 = &Value1;
		pin_ptr<QueryDataTimestampDisjoint> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(QueryDataTimestampDisjoint::typeid)) == 0;
	}
};