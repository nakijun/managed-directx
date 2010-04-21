namespace Xtro
{
namespace MDX
{
	public ref class Interface
	{
	internal:
		IntPtr InterfacePointer;

		static Dictionary<IntPtr, Interface^> Interfaces;

	protected:
		Interface(IntPtr Interface)
		{
			InterfacePointer = Interface;

			Interfaces.Add(Interface, this);
		}
	};
}
}