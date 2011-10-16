public ref class Multithread sealed : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Multithread);

internal:
	ID3D10Multithread* pMultithread;

	Multithread(IntPtr Multithread) : DeviceChild(Multithread)
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
		return pMultithread->GetMultithreadProtected() != 0;
	}

	void Leave()
	{
		pMultithread->Leave();
	}

	bool SetMultithreadProtected(bool MTProtect)
	{
		return pMultithread->SetMultithreadProtected(MTProtect) != 0;
	}
};