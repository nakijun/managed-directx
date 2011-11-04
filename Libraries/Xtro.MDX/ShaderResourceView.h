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
	void GetDescription([Out] ShaderResourceViewDescription% Description)
	{
		pin_ptr<ShaderResourceViewDescription> PinnedDescription = &Description;
		pShaderResourceView->GetDesc((D3D10_SHADER_RESOURCE_VIEW_DESC*)PinnedDescription);
	}
};