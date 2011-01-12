public ref class Texture2D : Resource
{
internal:
	ID3D10Texture2D* pTexture2D;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Texture2D);

	Texture2D(IntPtr Texture2D) : Resource(Texture2D)
	{	
		pTexture2D = (ID3D10Texture2D*)Texture2D.ToPointer();
	}

public:
	void GetDescription([Out] Texture2D_Description% Description)
	{
		pin_ptr<Texture2D_Description> PinnedDescription = &Description;
		pTexture2D->GetDesc((D3D10_TEXTURE2D_DESC*)PinnedDescription);
	}
};