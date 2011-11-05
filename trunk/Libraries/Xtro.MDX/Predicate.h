public ref class Predicate sealed : Query
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Predicate);

internal:
	ID3D10Predicate* pPredicate;

	Predicate(IntPtr Predicate) : Query(Predicate)
	{	
		pPredicate = (ID3D10Predicate*)Predicate.ToPointer();
	}

public:
};