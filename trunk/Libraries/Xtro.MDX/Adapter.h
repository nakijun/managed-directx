public ref class Adapter : Object
{
internal:
	IDXGIAdapter* pAdapter;

	static Guid IID = IID_Converter::ToManaged(IID_IDXGIAdapter);

	Adapter(IntPtr Adapter) : Object(Adapter)
	{
		pAdapter = (IDXGIAdapter*)Adapter.ToPointer();
	}

public:
	int EnumerateOutputs(unsigned int OutputNo, [Out] Output^% Output)
	{
		IDXGIOutput* pOutput = 0;
		int Result = pAdapter->EnumOutputs(OutputNo, &pOutput);

		if (pOutput)
		{
			try { Output = (Xtro::MDX::DXGI::Output^)Interfaces[IntPtr(pOutput)]; }
			catch (KeyNotFoundException^) { Output = gcnew Xtro::MDX::DXGI::Output(IntPtr(pOutput)); }					
		}
		else Output = nullptr;

		return Result;
	}
};