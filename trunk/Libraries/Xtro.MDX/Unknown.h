public ref class Unknown : Interface
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IUnknown);

protected private:
	Unknown(IntPtr Unknown) : Interface(Unknown)
	{
		pUnknown = (IUnknown*)Unknown.ToPointer();
	}

internal:
	IUnknown* pUnknown;

public:
	unsigned int AddRef()
	{
		return pUnknown->AddRef();
	}

	unsigned int Release()
	{
		unsigned int Result = pUnknown->Release();

		if(Result == 0) Interfaces.Remove(InterfacePointer);

		return Result;
	}
};