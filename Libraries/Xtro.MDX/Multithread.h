public ref class Multithread sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Multithread);

internal:
	ID3D10Multithread* pMultithread;

	Multithread(IntPtr Multithread) : Unknown(Multithread)
	{	
		pMultithread = (ID3D10Multithread*)Multithread.ToPointer();
	}

public:
	void Enter()
	{
		pMultithread->Enter();
	}

	bool GetMultithreadProtected()
	{
		return pMultithread->GetMultithreadProtected();
	}

	void Leave()
	{
		pMultithread->Leave();
	}

	bool SetMultithreadProtected(bool MTProtect)
	{
		return pMultithread->SetMultithreadProtected(MTProtect);
	}
};