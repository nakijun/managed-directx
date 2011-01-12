public ref class Asynchronous : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Asynchronous);

internal:
	ID3D10Asynchronous* pAsynchronous;

	Asynchronous(IntPtr Asynchronous) : Unknown(Asynchronous)
	{	
		pAsynchronous = (ID3D10Asynchronous*)Asynchronous.ToPointer();
	}

public:
};