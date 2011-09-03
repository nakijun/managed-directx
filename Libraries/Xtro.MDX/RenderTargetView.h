public ref class RenderTargetView sealed : View
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10RenderTargetView);

internal:
	ID3D10RenderTargetView* pRenderTargetView;

	RenderTargetView(IntPtr RenderTargetView) : View(RenderTargetView)
	{	
		pRenderTargetView = (ID3D10RenderTargetView*)RenderTargetView.ToPointer();
	}

public:
};