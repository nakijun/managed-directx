public ref class DepthStencilView : View
{
internal:
	ID3D10DepthStencilView* pDepthStencilView;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10DepthStencilView);

	DepthStencilView(IntPtr DepthStencilView) : View(DepthStencilView)
	{	
		pDepthStencilView = (ID3D10DepthStencilView*)DepthStencilView.ToPointer();
	}

public:
};