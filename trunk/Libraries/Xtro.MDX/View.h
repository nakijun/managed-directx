public ref class View : DeviceChild
{
internal:
	ID3D10View* pView;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10View);

	View(IntPtr View) : DeviceChild(View)
	{	
		pView = (ID3D10View*)View.ToPointer();
	}

public:
};