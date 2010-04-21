namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public ref class InputLayout : DeviceChild
	{
	internal:
		ID3D10InputLayout* pInputLayout;

		static Guid IID = IID_Converter::ToManaged(IID_ID3D10InputLayout);

		InputLayout(IntPtr InputLayout) : DeviceChild(InputLayout)
		{	
			pInputLayout = (ID3D10InputLayout*)InputLayout.ToPointer();
		}

	public:
	};
}
}
}