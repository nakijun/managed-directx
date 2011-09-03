public ref class InputLayout sealed : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10InputLayout);

internal:
	ID3D10InputLayout* pInputLayout;

	InputLayout(IntPtr InputLayout) : DeviceChild(InputLayout)
	{	
		pInputLayout = (ID3D10InputLayout*)InputLayout.ToPointer();
	}

public:
};