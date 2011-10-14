public ref class Counter sealed : Asynchronous
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
	void GetDescription([Out] CounterDescription% Description)
	{
		pin_ptr<CounterDescription> PinnedDescription = &Description;
		pCounter->GetDesc((D3D10_COUNTER_DESC*)PinnedDescription);
	}
};