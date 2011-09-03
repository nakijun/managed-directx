public ref class BlendState sealed : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10BlendState);

internal:
	ID3D10BlendState* pBlendState;

	BlendState(IntPtr BlendState) : DeviceChild(BlendState)
	{	
		pBlendState = (ID3D10BlendState*)BlendState.ToPointer();
	}

public:
};