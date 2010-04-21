namespace Xtro
{
namespace MDX
{
namespace DXGI
{
	public ref class Resource : DeviceSubObject
	{
	internal:
		IDXGIResource* pResource;

		static Guid IID = IID_Converter::ToManaged(IID_IDXGIResource);

		Resource(IntPtr Resource) : DeviceSubObject(Resource)
		{	
			pResource = (IDXGIResource*)Resource.ToPointer();
		}

	public:
	};
}
}
}