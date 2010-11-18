public ref class ShaderResourceView : View
{
internal:
	ID3D10ShaderResourceView* pShaderResourceView;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10ShaderResourceView);

	ShaderResourceView(IntPtr ShaderResourceView) : View(ShaderResourceView)
	{	
		pShaderResourceView = (ID3D10ShaderResourceView*)ShaderResourceView.ToPointer();
	}

public:
};