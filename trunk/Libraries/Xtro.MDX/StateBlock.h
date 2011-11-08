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

	int GetDevice([Out] Device^% Device);

	int ReleaseAllDeviceObjects()
	{
		return pStateBlock->ReleaseAllDeviceObjects();
	}
};