namespace Xtro
{
namespace MDX
{
namespace DXGI
{
	public ref class Surface : DeviceSubObject
	{
	internal:
		IDXGISurface* pSurface;

		static Guid IID = IID_Converter::ToManaged(IID_IDXGISurface);

		Surface(IntPtr Surface) : DeviceSubObject(Surface)
		{	
			pSurface = (IDXGISurface*)Surface.ToPointer();
		}

	public:
	};
}
}
}