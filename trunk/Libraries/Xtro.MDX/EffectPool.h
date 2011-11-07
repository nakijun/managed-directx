public ref class EffectPool sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectPool);

internal:
	ID3D10EffectPool* pEffectPool;

	EffectPool(IntPtr EffectPool) : Unknown(EffectPool)
	{	
		pEffectPool = (ID3D10EffectPool*)EffectPool.ToPointer();
	}

public:
	Effect^ AsEffect()
	{
		ID3D10Effect* pResult = pEffectPool->AsEffect();

		if (pResult)
		{	
			try { return (Effect^)Interfaces[IntPtr(pResult)]; }
			catch (KeyNotFoundException^) { return gcnew Effect(IntPtr(pResult)); }
		}
		
		return nullptr;
	}
};