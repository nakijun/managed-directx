public ref class StateBlock : Unknown
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
	int Capture()
	{
		return pStateBlock->Capture();
	}

	int Apply()
	{
		return pStateBlock->Apply();
	}
};