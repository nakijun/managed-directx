public ref class Cue sealed : Interface
{
internal:
	IXACT3Cue* pCue;

	Cue(IntPtr Cue) : Interface(Cue)
	{
		pCue = (IXACT3Cue*)Cue.ToPointer();
	}

public:
	int Destroy()
	{
		return pCue->Destroy();
	}

	int GetProperties([Out] CueInstanceProperties% Properties)
	{
		XACT_CUE_INSTANCE_PROPERTIES* pProperties = 0;
		int Result = pCue->GetProperties(&pProperties);

		try	{ Properties.FromNative(pProperties); }
		finally
		{
			if (pProperties) CoTaskMemFree(pProperties);
		}

		return Result;
	}
};