public ref class EffectRenderTargetViewVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectRenderTargetViewVariable);

internal:
	ID3D10EffectRenderTargetViewVariable* pEffectRenderTargetViewVariable;

	EffectRenderTargetViewVariable(IntPtr EffectRenderTargetViewVariable) : EffectVariable(EffectRenderTargetViewVariable)
	{	
		pEffectRenderTargetViewVariable = (ID3D10EffectRenderTargetViewVariable*)EffectRenderTargetViewVariable.ToPointer();
	}

public:
	int GetRenderTarget([Out] RenderTargetView^% Resource)
	{
		ID3D10RenderTargetView* pResource = 0;
		int Result = pEffectRenderTargetViewVariable->GetRenderTarget(&pResource);

		if (pResource) 
		{
			try { Resource = (RenderTargetView^)Interfaces[IntPtr(pResource)]; }
			catch (KeyNotFoundException^) { Resource = gcnew RenderTargetView(IntPtr(pResource)); }
		}
		else Resource = nullptr;

		return Result;
	}

	int GetRenderTargetArray(array<RenderTargetView^>^ Resources, unsigned int Offset, unsigned int Count)
	{
		ID3D10RenderTargetView** pResources = Resources != nullptr && Resources->Length > 0 ? new ID3D10RenderTargetView*[Resources->Length] : 0;
		try
		{
			int Result = pEffectRenderTargetViewVariable->GetRenderTargetArray(pResources, Offset, Count);

			Count = Math::Min(Offset + Count, (unsigned int)Resources->Length);
			for (unsigned int No = Offset; No < Count; No++)
			{
				if (pResources[No])
				{
					try { Resources[No] = (RenderTargetView^)Interfaces[IntPtr(pResources[No])]; }
					catch (KeyNotFoundException^) { Resources[No] = gcnew RenderTargetView(IntPtr(pResources[No])); }
				}
				else Resources[No] = nullptr;
			}

			return Result;
		}
		finally
		{
			if (pResources) delete[] pResources;
		}
	}

	int SetRenderTarget(RenderTargetView^ Resource)
	{
		ID3D10RenderTargetView* pResource = Resource == nullptr ? 0 : Resource->pRenderTargetView;
		return pEffectRenderTargetViewVariable->SetRenderTarget(pResource);
	}

	int SetRenderTargetArray(array<RenderTargetView^>^ Resources, unsigned int Offset, unsigned int Count)
	{
		ID3D10RenderTargetView** pResources = 0;
		try
		{
			if (Resources != nullptr && Resources->Length > 0)
			{
				pResources = new ID3D10RenderTargetView*[Resources->Length];

				unsigned int LoopCount = Math::Min(Offset + Count, (unsigned int)Resources->Length);
				for (unsigned int No = Offset; No < LoopCount; No++)
				{
					pResources[No] = Resources[No] == nullptr ? 0 : Resources[No]->pRenderTargetView;
				}
			}

			return pEffectRenderTargetViewVariable->SetRenderTargetArray(pResources, Offset, Count);
		}
		finally
		{
			if (pResources) delete[] pResources;
		}
	}
};