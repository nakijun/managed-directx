public ref class EffectShaderResourceVariable sealed : EffectVariable
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectShaderResourceVariable);

internal:
	ID3D10EffectShaderResourceVariable* pEffectShaderResourceVariable;

	EffectShaderResourceVariable(IntPtr EffectShaderResourceVariable) : EffectVariable(EffectShaderResourceVariable)
	{	
		pEffectShaderResourceVariable = (ID3D10EffectShaderResourceVariable*)EffectShaderResourceVariable.ToPointer();
	}

public:
	int GetResource([Out] ShaderResourceView^% Resource)
	{
		ID3D10ShaderResourceView* pResource = 0;
		int Result = pEffectShaderResourceVariable->GetResource(&pResource);

		if (pResource) 
		{
			try { Resource = (ShaderResourceView^)Interfaces[IntPtr(pResource)]; }
			catch (KeyNotFoundException^) { Resource = gcnew ShaderResourceView(IntPtr(pResource)); }
		}
		else Resource = nullptr;

		return Result;
	}

	int GetResourceArray(array<ShaderResourceView^>^ Resources, unsigned int Offset, unsigned int Count)
	{
		ID3D10ShaderResourceView** pResources = Resources != nullptr && Resources->Length > 0 ? new ID3D10ShaderResourceView*[Resources->Length] : 0;
		try
		{
			int Result = pEffectShaderResourceVariable->GetResourceArray(pResources, Offset, Count);

			Count = Math::Min(Offset + Count, (unsigned int)Resources->Length);
			for (unsigned int No = Offset; No < Count; No++)
			{
				if (pResources[No])
				{
					try { Resources[No] = (ShaderResourceView^)Interfaces[IntPtr(pResources[No])]; }
					catch (KeyNotFoundException^) { Resources[No] = gcnew ShaderResourceView(IntPtr(pResources[No])); }
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

	int SetResource(ShaderResourceView^ Resource)
	{
		ID3D10ShaderResourceView* pResource = Resource == nullptr ? 0 : Resource->pShaderResourceView;
		return pEffectShaderResourceVariable->SetResource(pResource);
	}

	int SetResourceArray(array<ShaderResourceView^>^ Resources, unsigned int Offset, unsigned int Count)
	{
		ID3D10ShaderResourceView** pResources = 0;
		try
		{
			if (Resources != nullptr && Resources->Length > 0)
			{
				pResources = new ID3D10ShaderResourceView*[Resources->Length];

				unsigned int LoopCount = Math::Min(Offset + Count, (unsigned int)Resources->Length);
				for (unsigned int No = Offset; No < LoopCount; No++)
				{
					pResources[No] = Resources[No] == nullptr ? 0 : Resources[No]->pShaderResourceView;
				}
			}

			return pEffectShaderResourceVariable->SetResourceArray(pResources, Offset, Count);
		}
		finally
		{
			if (pResources) delete[] pResources;
		}
	}
};