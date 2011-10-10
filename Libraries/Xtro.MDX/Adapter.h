public ref class Adapter sealed : Object
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_IDXGIAdapter);

internal:
	IDXGIAdapter* pAdapter;

	Adapter(IntPtr Adapter) : Object(Adapter)
	{
		pAdapter = (IDXGIAdapter*)Adapter.ToPointer();
	}

public:
	int CheckInterfaceSupport(Guid InterfaceName, [Out] long long% UserModeDriverVersion)
	{
		pin_ptr<long long> PinnedUserModeDriverVersion = &UserModeDriverVersion;

		return pAdapter->CheckInterfaceSupport(IID_Converter::ToNative(InterfaceName), (LARGE_INTEGER*)PinnedUserModeDriverVersion);
	}

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

	int GetDescription([Out] AdapterDescription% Description)
	{
		DXGI_ADAPTER_DESC NativeDescription;
		int Result = pAdapter->GetDesc(&NativeDescription);

		if (Result >= 0) Description.FromNative(&NativeDescription);

		return Result;
	}
};