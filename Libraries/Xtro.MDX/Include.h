public ref class Include : Interface
{
internal:
	ID3D10Include* pInclude;

	//static Guid IID = IID_Converter::ToManaged(IID_ID3D10Include);

	Include(IntPtr Include) : Interface(Include)
	{	
		pInclude = (ID3D10Include*)Include.ToPointer();
	}

public:
};