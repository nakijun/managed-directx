public ref class RasterizerState : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10RasterizerState);

internal:
	ID3D10RasterizerState* pRasterizerState;

	RasterizerState(IntPtr RasterizerState) : DeviceChild(RasterizerState)
	{
		pRasterizerState = (ID3D10RasterizerState*)RasterizerState.ToPointer();
	}

public:
	void GetDescription([Out] RasterizerDescription% Description)
	{
		pin_ptr<RasterizerDescription> PinnedDescription = &Description;
		pRasterizerState->GetDesc((D3D10_RASTERIZER_DESC*)PinnedDescription);
	}
};