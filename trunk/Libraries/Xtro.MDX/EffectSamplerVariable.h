public ref class EffectSamplerVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectSamplerVariable);

internal:
	ID3D10EffectSamplerVariable* pEffectSamplerVariable;

	EffectSamplerVariable(IntPtr EffectSamplerVariable) : EffectVariable(EffectSamplerVariable)
	{	
		pEffectSamplerVariable = (ID3D10EffectSamplerVariable*)EffectSamplerVariable.ToPointer();
	}

public:
	int GetBackingStore(unsigned int Index, [Out] SamplerDescription% SamplerDescription)
	{
		pin_ptr<Direct3D10::SamplerDescription> PinnedSamplerDescription = &SamplerDescription;
		return pEffectSamplerVariable->GetBackingStore(Index, (D3D10_SAMPLER_DESC*)PinnedSamplerDescription);
	}

	int GetSampler(unsigned int Index, [Out] SamplerState^% Sampler)
	{
		ID3D10SamplerState* pSampler = 0;
		int Result = pEffectSamplerVariable->GetSampler(Index, &pSampler);

		if (pSampler) 
		{
			try { Sampler = (SamplerState^)Interfaces[IntPtr(pSampler)]; }
			catch (KeyNotFoundException^) { Sampler = gcnew SamplerState(IntPtr(pSampler)); }
		}
		else Sampler = nullptr;

		return Result;
	}
};