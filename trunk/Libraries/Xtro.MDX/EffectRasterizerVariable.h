public ref class EffectRasterizerVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectRasterizerVariable);

internal:
	ID3D10EffectRasterizerVariable* pEffectRasterizerVariable;

	EffectRasterizerVariable(IntPtr EffectRasterizerVariable) : EffectVariable(EffectRasterizerVariable)
	{	
		pEffectRasterizerVariable = (ID3D10EffectRasterizerVariable*)EffectRasterizerVariable.ToPointer();
	}

public:
	int GetBackingStore(unsigned int Index, [Out] RasterizerDescription% RasterizerDescription)
	{
		pin_ptr<Direct3D10::RasterizerDescription> PinnedRasterizerDescription = &RasterizerDescription;
		return pEffectRasterizerVariable->GetBackingStore(Index, (D3D10_RASTERIZER_DESC*)PinnedRasterizerDescription);
	}

	int GetRasterizerState(unsigned int Index, [Out] RasterizerState^% RasterizerState)
	{
		ID3D10RasterizerState* pRasterizerState = 0;
		int Result = pEffectRasterizerVariable->GetRasterizerState(Index, &pRasterizerState);

		if (pRasterizerState) 
		{
			try { RasterizerState = (Direct3D10::RasterizerState^)Interfaces[IntPtr(pRasterizerState)]; }
			catch (KeyNotFoundException^) { RasterizerState = gcnew Direct3D10::RasterizerState(IntPtr(pRasterizerState)); }
		}
		else RasterizerState = nullptr;

		return Result;
	}
};