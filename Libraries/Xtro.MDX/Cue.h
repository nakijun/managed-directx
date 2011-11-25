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

	int GetState([Out] CueState% State)
	{
		pin_ptr<CueState> PinnedState = &State;
		return pCue->GetState((unsigned long*)PinnedState);
	}

	int GetVariable(unsigned short Index, [Out] float% Value)
	{
		pin_ptr<float> PinnedValue = &Value;
		return pCue->GetVariable(Index, (float*)PinnedValue);
	}

	unsigned short GetVariableIndex(String^ FriendlyName)
	{
		IntPtr pFriendlyName = Marshal::StringToHGlobalAnsi(FriendlyName);
		try { return pCue->GetVariableIndex((PCSTR)pFriendlyName.ToPointer()); }
		finally { Marshal::FreeHGlobal(pFriendlyName); }
	}

	int Pause(bool Pause)
	{
		return pCue->Pause(Pause);
	}

	int Play()
	{
		return pCue->Play();
	}

	int SetMatrixCoefficients(unsigned int SourceChannelCount, unsigned int DestinationChannelCount, [Out] float% MatrixCoefficients)
	{
		pin_ptr<float> PinnedMatrixCoefficients = &MatrixCoefficients;
		return pCue->SetMatrixCoefficients(SourceChannelCount, DestinationChannelCount, (float*)PinnedMatrixCoefficients);
	}

	int SetVariable(unsigned short Index, float Value)
	{
		return pCue->SetVariable(Index, Value);
	}

	int Stop(CueStopFlag Flags)
	{					 
		return pCue->Stop((unsigned int)Flags);
	}
};