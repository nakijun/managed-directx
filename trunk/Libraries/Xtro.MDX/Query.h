public ref class Query : Asynchronous
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Query);

internal:
	ID3D10Query* pQuery;

	Query(IntPtr Query) : Asynchronous(Query)
	{	
		pQuery = (ID3D10Query*)Query.ToPointer();
	}

public:
	void GetDescription([Out] QueryDescription% Description)
	{
		pin_ptr<QueryDescription> PinnedDescription = &Description;
		pQuery->GetDesc((D3D10_QUERY_DESC*)PinnedDescription);
	}
};