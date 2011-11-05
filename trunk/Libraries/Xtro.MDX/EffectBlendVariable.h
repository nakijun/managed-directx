public ref class EffectBlendVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectBlendVariable);

internal:
	ID3D10EffectBlendVariable* pEffectBlendVariable;

	EffectBlendVariable(IntPtr EffectBlendVariable) : EffectVariable(EffectBlendVariable)
	{	
		pEffectBlendVariable = (ID3D10EffectBlendVariable*)EffectBlendVariable.ToPointer();
	}

public:
	int GetBackingStore(unsigned int Index, [Out] BlendDescription% BlendDescription)
	{
		pin_ptr<Xtro::MDX::Direct3D10::BlendDescription> PinnedBlendDescription = &BlendDescription;
		return pEffectBlendVariable->GetBackingStore(Index, (D3D10_BLEND_DESC*)PinnedBlendDescription);
	}

	int GetBlendState(unsigned int Index, [Out] BlendState^% BlendState)
	{
		ID3D10BlendState* pBlendState = 0;
		int Result = pEffectBlendVariable->GetBlendState(Index, &pBlendState);

		if (pBlendState) 
		{
			try { BlendState = (Xtro::MDX::Direct3D10::BlendState^)Interfaces[IntPtr(pBlendState)]; }
			catch (KeyNotFoundException^) { BlendState = gcnew Xtro::MDX::Direct3D10::BlendState(IntPtr(pBlendState)); }
		}
		else BlendState = nullptr;

		return Result;
	}
};