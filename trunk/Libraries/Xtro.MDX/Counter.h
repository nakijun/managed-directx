public ref class Counter : Asynchronous
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Counter);

internal:
	ID3D10Counter* pCounter;

	Counter(IntPtr Counter) : Asynchronous(Counter)
	{	
		pCounter = (ID3D10Counter*)Counter.ToPointer();
	}

public:
};