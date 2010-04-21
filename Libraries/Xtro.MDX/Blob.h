namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public ref class Blob : Unknown
	{
	internal:
		ID3D10Blob* pBlob;

		static Guid IID = IID_Converter::ToManaged(IID_ID3D10Blob);

		Blob(IntPtr Blob) : Unknown(Blob)
		{	
			pBlob = (ID3D10Blob*)Blob.ToPointer();
		}

	public:
	};
}
}
}