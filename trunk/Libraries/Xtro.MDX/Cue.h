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
};