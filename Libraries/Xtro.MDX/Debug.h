public ref class Debug sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Debug);

internal:
	ID3D10Debug* pDebug;

	
	Debug(IntPtr Debug) : Unknown(Debug)
	{	
		pDebug = (ID3D10Debug*)Debug.ToPointer();
	}

public:
	DebugFeatureFlag GetFeatureMask()
	{
		return (DebugFeatureFlag)pDebug->GetFeatureMask();
	}

	unsigned int GetPresentPerRenderOperationDelay()
	{
		return pDebug->GetPresentPerRenderOpDelay();
	}

	int GetSwapChain([Out] SwapChain^% SwapChain)
	{
		IDXGISwapChain* pSwapChain = 0;

		int Result = pDebug->GetSwapChain(&pSwapChain);

		if (pSwapChain)
		{	
			try { SwapChain = (Direct3D10::SwapChain^)Interfaces[IntPtr(pSwapChain)]; }
			catch (KeyNotFoundException^) { SwapChain = gcnew Direct3D10::SwapChain(IntPtr(pSwapChain)); }
		}
		else SwapChain = nullptr;

		return Result;
	}
							
	int SetFeatureMask(DebugFeatureFlag Mask)
	{
		return pDebug->SetFeatureMask((unsigned int)Mask);
	}
							
	int SetPresentPerRenderOperationDelay(unsigned int Milliseconds)
	{
		return pDebug->SetPresentPerRenderOpDelay(Milliseconds);
	}

	int SetSwapChain(SwapChain^ SwapChain)
	{
		IDXGISwapChain* pSwapChain = SwapChain == nullptr ? 0 : SwapChain->pSwapChain;

		return pDebug->SetSwapChain(pSwapChain);
	}

	unsigned int Validate()
	{
		return pDebug->Validate();
	}
};