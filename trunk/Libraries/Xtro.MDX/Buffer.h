public ref class Buffer : Resource
{
internal:
	ID3D10Buffer* pBuffer;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Buffer);

	Buffer(IntPtr Buffer) : Resource(Buffer)
	{	
		pBuffer = (ID3D10Buffer*)Buffer.ToPointer();
	}

public:
};