namespace Xtro
{
namespace MDX
{
namespace DXGI
{
	public ref class Adapter : Object
	{
	internal:
		IDXGIAdapter* pAdapter;

		static Guid IID = IID_Converter::ToManaged(IID_IDXGIAdapter);

		Adapter(IntPtr Adapter) : Object(Adapter)
		{
			pAdapter = (IDXGIAdapter*)Adapter.ToPointer();
		}
	};
}
}
}