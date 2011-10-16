public ref class StateBlock sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10StateBlock);

internal:
	ID3D10StateBlock* pStateBlock;

	StateBlock(IntPtr StateBlock) : Unknown(StateBlock)
	{	
		pStateBlock = (ID3D10StateBlock*)StateBlock.ToPointer();
	}

public:
	int Apply()
	{
		return pStateBlock->Apply();
	}

	int Capture()
	{
		return pStateBlock->Capture();
	}

	int GetDevice([Out] Device^% Device)
	{
		ID3D10Device* pDevice = 0;

		int Result = pStateBlock->GetDevice(&pDevice);

		if (pDevice) 
		{
			try { Device = (Xtro::MDX::Direct3D10::Device^)Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }
		}
		else Device = nullptr;

		return Result;
	}

	int ReleaseAllDeviceObjects()
	{
		return pStateBlock->ReleaseAllDeviceObjects();
	}
};