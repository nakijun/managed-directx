public ref class Voice sealed : Interface
{
internal:
	IXAudio2Voice* pVoice;

	Voice(IntPtr Voice) : Interface(Voice)
	{
		pVoice = (IXAudio2Voice*)Voice.ToPointer();
	}

public:
};