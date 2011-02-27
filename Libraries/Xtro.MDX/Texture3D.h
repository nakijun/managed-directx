public ref class Texture3D : Resource
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Texture3D);

internal:
	ID3D10Texture3D* pTexture3D;

	Texture3D(IntPtr Texture3D) : Resource(Texture3D)
	{	
		pTexture3D = (ID3D10Texture3D*)Texture3D.ToPointer();
	}

public:
};