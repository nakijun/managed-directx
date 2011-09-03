public ref class ShaderResourceView sealed : View
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10ShaderResourceView);

internal:
	ID3D10ShaderResourceView* pShaderResourceView;

	ShaderResourceView(IntPtr ShaderResourceView) : View(ShaderResourceView)
	{	
		pShaderResourceView = (ID3D10ShaderResourceView*)ShaderResourceView.ToPointer();
	}

public:
};