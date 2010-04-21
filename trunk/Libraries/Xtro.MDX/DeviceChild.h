namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public ref class DeviceChild : Unknown
	{
	internal:
		ID3D10DeviceChild* pDeviceChild;

		static Guid IID = IID_Converter::ToManaged(IID_ID3D10DeviceChild);

		DeviceChild(IntPtr DeviceChild) : Unknown(DeviceChild)
		{	
			pDeviceChild = (ID3D10DeviceChild*)DeviceChild.ToPointer();
		}

	public:
	};
}
}
}