namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
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
	};
}
}
}