public ref class PixelShader sealed : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10PixelShader);

	int Size;

internal:
	ID3D10PixelShader* pPixelShader;

	PixelShader(IntPtr PixelShader) : DeviceChild(PixelShader)
	{	
		Size = -1;

		pPixelShader = (ID3D10PixelShader*)PixelShader.ToPointer();
	}

public:
};