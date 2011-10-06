public value class QueryDataPipelineStatistics : IEquatable<QueryDataPipelineStatistics>
{
public:
    unsigned long long IA_Vertices;
    unsigned long long IA_Primitives;
    unsigned long long VS_Invocations;
    unsigned long long GS_Invocations;
    unsigned long long GS_Primitives;
    unsigned long long C_Invocations;
    unsigned long long C_Primitives;
    unsigned long long PS_Invocations;

	static bool operator == (QueryDataPipelineStatistics Left, QueryDataPipelineStatistics Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (QueryDataPipelineStatistics Left, QueryDataPipelineStatistics Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			IA_Vertices.GetHashCode() ^
			IA_Primitives.GetHashCode() ^ 
			VS_Invocations.GetHashCode() ^
			GS_Invocations.GetHashCode() ^
			GS_Primitives.GetHashCode() ^
			C_Invocations.GetHashCode() ^
			C_Primitives.GetHashCode() ^
			PS_Invocations.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(QueryDataPipelineStatistics Value)
	{
		pin_ptr<unsigned long long> PinnedThis = &IA_Vertices;
		pin_ptr<QueryDataPipelineStatistics> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(QueryDataPipelineStatistics::typeid)) == 0;
	}

	static bool Equals(QueryDataPipelineStatistics% Value1, QueryDataPipelineStatistics% Value2)
	{
		pin_ptr<QueryDataPipelineStatistics> PinnedValue1 = &Value1;
		pin_ptr<QueryDataPipelineStatistics> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(QueryDataPipelineStatistics::typeid)) == 0;
	}
};