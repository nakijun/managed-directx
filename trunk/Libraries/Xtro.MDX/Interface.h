public ref class Interface
{
protected private:
	IntPtr InterfacePointer;

	Interface(IntPtr Interface)
	{
		InterfacePointer = Interface;

		Interfaces.Add(Interface, this);
	}

internal:
	static Dictionary<IntPtr, Interface^> Interfaces;
};