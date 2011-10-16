public ref class Texture1D sealed : Resource
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Texture1D);

internal:
	ID3D10Texture1D* pTexture1D;

	Texture1D(IntPtr Texture1D) : Resource(Texture1D)
	{	
		pTexture1D = (ID3D10Texture1D*)Texture1D.ToPointer();
	}

public:
	void GetDescription([Out] Texture1D_Description% Description)
	{
		pin_ptr<Texture1D_Description> PinnedDescription = &Description;
		pTexture1D->GetDesc((D3D10_TEXTURE1D_DESC*)PinnedDescription);
	}

	int Map(unsigned int Subresource, Map MapType, MapFlag MapFlags, [Out] UnmanagedMemory^% Data)
	{
		void* pData = 0;
		int Result = pTexture1D->Map(Subresource, (D3D10_MAP)MapType, (unsigned int)MapFlags, &pData);

		Data = pData ? gcnew UnmanagedMemory(IntPtr(pData), 0) : nullptr;

		return Result;
	}

	void Unmap(unsigned int Subresource)
	{
		pTexture1D->Unmap(Subresource);
	}
};