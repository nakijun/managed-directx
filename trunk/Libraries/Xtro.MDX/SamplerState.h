public ref class SamplerState sealed : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10SamplerState);

internal:
	ID3D10SamplerState* pSamplerState;

	SamplerState(IntPtr SamplerState) : DeviceChild(SamplerState)
	{	
		pSamplerState = (ID3D10SamplerState*)SamplerState.ToPointer();
	}

public:
	void GetDescription([Out] SamplerDescription% Description)
	{
		pin_ptr<SamplerDescription> PinnedDescription = &Description;
		pSamplerState->GetDesc((D3D10_SAMPLER_DESC*)PinnedDescription);
	}
};