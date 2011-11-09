public ref class EffectDepthStencilViewVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectDepthStencilViewVariable);

internal:
	ID3D10EffectDepthStencilViewVariable* pEffectDepthStencilViewVariable;

	EffectDepthStencilViewVariable(IntPtr EffectDepthStencilViewVariable) : EffectVariable(EffectDepthStencilViewVariable)
	{	
		pEffectDepthStencilViewVariable = (ID3D10EffectDepthStencilViewVariable*)EffectDepthStencilViewVariable.ToPointer();
	}

public:
	int GetDepthStencil([Out] DepthStencilView^% Resource)
	{
		ID3D10DepthStencilView* pResource = 0;
		int Result = pEffectDepthStencilViewVariable->GetDepthStencil(&pResource);

		if (pResource) 
		{
			try { Resource = (DepthStencilView^)Interfaces[IntPtr(pResource)]; }
			catch (KeyNotFoundException^) { Resource = gcnew DepthStencilView(IntPtr(pResource)); }
		}
		else Resource = nullptr;

		return Result;
	}

	int GetDepthStencilArray(array<DepthStencilView^>^ Resources, unsigned int Offset, unsigned int Count)
	{
		unsigned int Length = Resources == nullptr ? 0 : Math::Min(Offset + Count, (unsigned int)Resources->Length);
		ID3D10DepthStencilView** pResources = Resources != nullptr && Resources->Length > 0 ? new ID3D10DepthStencilView*[Length] : 0;
		try
		{
			int Result = pEffectDepthStencilViewVariable->GetDepthStencilArray(pResources, Offset, Count);

			if (pResources)
			{
				for (unsigned int No = Offset; No < Length; No++)
				{
					if (pResources[No])
					{
						try { Resources[No] = (DepthStencilView^)Interfaces[IntPtr(pResources[No])]; }
						catch (KeyNotFoundException^) { Resources[No] = gcnew DepthStencilView(IntPtr(pResources[No])); }
					}
					else Resources[No] = nullptr;
				}
			}

			return Result;
		}
		finally
		{
			if (pResources) delete[] pResources;
		}
	}

	int SetDepthStencil(DepthStencilView^ Resource)
	{
		ID3D10DepthStencilView* pResource = Resource == nullptr ? 0 : Resource->pDepthStencilView;
		return pEffectDepthStencilViewVariable->SetDepthStencil(pResource);
	}

	int SetDepthStencilArray(array<DepthStencilView^>^ Resources, unsigned int Offset, unsigned int Count)
	{
		ID3D10DepthStencilView** pResources = 0;
		try
		{
			if (Resources != nullptr && Resources->Length > 0)
			{
				unsigned int Length = Math::Min(Offset + Count, (unsigned int)Resources->Length);
				pResources = new ID3D10DepthStencilView*[Length];
				for (unsigned int No = Offset; No < Length; No++)
				{
					pResources[No] = Resources[No] == nullptr ? 0 : Resources[No]->pDepthStencilView;
				}
			}

			return pEffectDepthStencilViewVariable->SetDepthStencilArray(pResources, Offset, Count);
		}
		finally
		{
			if (pResources) delete[] pResources;
		}
	}
};