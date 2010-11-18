public ref class RenderTargetView : View
{
internal:
	ID3D10RenderTargetView* pRenderTargetView;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10RenderTargetView);

	RenderTargetView(IntPtr RenderTargetView) : View(RenderTargetView)
	{	
		pRenderTargetView = (ID3D10RenderTargetView*)RenderTargetView.ToPointer();
	}

public:
};