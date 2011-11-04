public ref class DepthStencilView sealed : View
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10DepthStencilView);

internal:
	ID3D10DepthStencilView* pDepthStencilView;

	DepthStencilView(IntPtr DepthStencilView) : View(DepthStencilView)
	{	
		pDepthStencilView = (ID3D10DepthStencilView*)DepthStencilView.ToPointer();
	}

public:
	void GetDescription([Out] DepthStencilViewDescription% Description)
	{
		pin_ptr<DepthStencilViewDescription> PinnedDescription = &Description;
		pDepthStencilView->GetDesc((D3D10_DEPTH_STENCIL_VIEW_DESC*)PinnedDescription);
	}
};