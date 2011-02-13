public ref class View : DeviceChild
{
internal:
	ID3D10View* pView;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10View);

	View(IntPtr View) : DeviceChild(View)
	{	
		pView = (ID3D10View*)View.ToPointer();
	}

public:
	void GetResource([Out] Resource^% Resource)
	{
		ID3D10Resource* pResource = 0;
		pView->GetResource(&pResource);

		if (pResource)
		{	
			try { Resource = (Direct3D10::Resource^)Interfaces[IntPtr(pResource)]; }
			catch (KeyNotFoundException^)
			{
				D3D10_RESOURCE_DIMENSION Type;
				pResource->GetType(&Type);

				switch(Type)
				{
				case D3D10_RESOURCE_DIMENSION_BUFFER : Resource = gcnew Buffer(IntPtr(pResource)); break;
				case D3D10_RESOURCE_DIMENSION_TEXTURE1D : Resource = gcnew Texture1D(IntPtr(pResource)); break;
				case D3D10_RESOURCE_DIMENSION_TEXTURE2D : Resource = gcnew Texture2D(IntPtr(pResource)); break;
				case D3D10_RESOURCE_DIMENSION_TEXTURE3D : Resource = gcnew Texture3D(IntPtr(pResource)); break;
				default : Resource = gcnew Direct3D10::Resource(IntPtr(pResource));
				}
			}
		}
		else Resource = nullptr;
	}
};