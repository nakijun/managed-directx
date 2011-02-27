public ref class Blob : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Blob);

internal:
	ID3D10Blob* pBlob;

	
	Blob(IntPtr Blob) : Unknown(Blob)
	{	
		pBlob = (ID3D10Blob*)Blob.ToPointer();
	}

public:
};