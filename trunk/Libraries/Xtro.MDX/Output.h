namespace Xtro
{
namespace MDX
{
namespace DXGI
{
	public ref class Output : Object
	{
	internal:
		IDXGIOutput* pOutput;

		static Guid IID = IID_Converter::ToManaged(IID_IDXGIOutput);

		Output(IntPtr Output) : Object(Output)
		{
			pOutput = (IDXGIOutput*)Output.ToPointer();
		}
	};
}
}
}