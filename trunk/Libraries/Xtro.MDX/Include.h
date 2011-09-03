public ref class Include sealed : Interface
{
private:
	//static Guid IID = IID_Converter::ToManaged(IID_ID3D10Include);

internal:
	ID3D10Include* pInclude;

	Include(IntPtr Include) : Interface(Include)
	{	
		pInclude = (ID3D10Include*)Include.ToPointer();
	}

public:
};