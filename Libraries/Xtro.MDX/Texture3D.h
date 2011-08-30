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
	void GetDescription([Out] Texture3D_Description% Description)
	{
		pin_ptr<Texture3D_Description> PinnedDescription = &Description;
		pTexture3D->GetDesc((D3D10_TEXTURE3D_DESC*)PinnedDescription);
	}

	int Map(unsigned int Subresource, Map MapType, MapFlag MapFlags, [Out] MappedTexture3D% MappedTexture3D, unsigned int DataSize)
	{
		D3D10_MAPPED_TEXTURE3D NativeMappedTexture3D;
		int Result = pTexture3D->Map(Subresource, (D3D10_MAP)MapType, (unsigned int)MapFlags, &NativeMappedTexture3D);

		MappedTexture3D.FromNative(&NativeMappedTexture3D);

		return Result;
	}
};