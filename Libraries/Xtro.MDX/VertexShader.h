public ref class VertexShader sealed : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10VertexShader);

internal:
	ID3D10VertexShader* pVertexShader;

	VertexShader(IntPtr VertexShader) : DeviceChild(VertexShader)
	{	
		pVertexShader = (ID3D10VertexShader*)VertexShader.ToPointer();
	}

public:
};