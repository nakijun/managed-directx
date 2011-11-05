public ref class EffectDepthStencilVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectDepthStencilVariable);

internal:
	ID3D10EffectDepthStencilVariable* pEffectDepthStencilVariable;

	EffectDepthStencilVariable(IntPtr EffectDepthStencilVariable) : EffectVariable(EffectDepthStencilVariable)
	{	
		pEffectDepthStencilVariable = (ID3D10EffectDepthStencilVariable*)EffectDepthStencilVariable.ToPointer();
	}

public:
	int GetBackingStore(unsigned int Index, [Out] DepthStencilDescription% DepthStencilDescription)
	{
		pin_ptr<Xtro::MDX::Direct3D10::DepthStencilDescription> PinnedDepthStencilDescription = &DepthStencilDescription;
		return pEffectDepthStencilVariable->GetBackingStore(Index, (D3D10_DEPTH_STENCIL_DESC*)PinnedDepthStencilDescription);
	}

	int GetDepthStencilState(unsigned int Index, [Out] DepthStencilState^% DepthStencilState)
	{
		ID3D10DepthStencilState* pDepthStencilState = 0;
		int Result = pEffectDepthStencilVariable->GetDepthStencilState(Index, &pDepthStencilState);

		if (pDepthStencilState) 
		{
			try { DepthStencilState = (Xtro::MDX::Direct3D10::DepthStencilState^)Interfaces[IntPtr(pDepthStencilState)]; }
			catch (KeyNotFoundException^) { DepthStencilState = gcnew Xtro::MDX::Direct3D10::DepthStencilState(IntPtr(pDepthStencilState)); }
		}
		else DepthStencilState = nullptr;

		return Result;
	}
};