public ref class Buffer : Resource
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Buffer);


internal:
	ID3D10Buffer* pBuffer;


	Buffer(IntPtr Buffer) : Resource(Buffer)
	{	
		pBuffer = (ID3D10Buffer*)Buffer.ToPointer();
	}

public:
};