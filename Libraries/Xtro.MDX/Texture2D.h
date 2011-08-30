public ref class Texture2D : Resource
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Texture2D);

internal:
	ID3D10Texture2D* pTexture2D;

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

	int Map(unsigned int Subresource, Map MapType, MapFlag MapFlags, [Out] MappedTexture2D% MappedTexture2D, unsigned int DataSize)
	{
		D3D10_MAPPED_TEXTURE2D NativeMappedTexture2D;
		int Result = pTexture2D->Map(Subresource, (D3D10_MAP)MapType, (unsigned int)MapFlags, &NativeMappedTexture2D);

		MappedTexture2D.FromNative(&NativeMappedTexture2D);

		return Result;
	}
};