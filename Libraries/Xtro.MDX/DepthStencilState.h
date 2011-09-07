public ref class DepthStencilState sealed : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10DepthStencilState);

internal:
	ID3D10DepthStencilState* pDepthStencilState;

	DepthStencilState(IntPtr DepthStencilState) : DeviceChild(DepthStencilState)
	{
		pDepthStencilState = (ID3D10DepthStencilState*)DepthStencilState.ToPointer();
	}

public:
	void GetDescription([Out] DepthStencilDescription% Description)
	{
		pin_ptr<DepthStencilDescription> PinnedDescription = &Description;
		pDepthStencilState->GetDesc((D3D10_DEPTH_STENCIL_DESC*)PinnedDescription);
	}
};