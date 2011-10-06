public value class QueryDescription : IEquatable<QueryDescription>
{
public:
    Query Query;
    QueryMiscellaneousFlag MiscellaneousFlags;

	static bool operator == (QueryDescription Left, QueryDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (QueryDescription Left, QueryDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Query ^
			(int)MiscellaneousFlags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(QueryDescription Value)
	{
		pin_ptr<Xtro::MDX::Direct3D10::Query> PinnedThis = &Query;
		pin_ptr<QueryDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(QueryDescription::typeid)) == 0;
	}

	static bool Equals(QueryDescription% Value1, QueryDescription% Value2)
	{
		pin_ptr<QueryDescription> PinnedValue1 = &Value1;
		pin_ptr<QueryDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(QueryDescription::typeid)) == 0;
	}
};