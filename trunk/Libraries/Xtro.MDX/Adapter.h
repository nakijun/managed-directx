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
	int CheckInterfaceSupport(Type^ InterfaceName, [Out] long long% UserModeDriverVersion)
	{
		Guid RIID;
		try { RIID = (Guid)InterfaceName->GetField("IID", BindingFlags::DeclaredOnly | BindingFlags::NonPublic | BindingFlags::Static)->GetValue(nullptr); }
		catch (...) { RIID = Guid::Empty; }

		pin_ptr<long long> PinnedUserModeDriverVersion = &UserModeDriverVersion;

		return pAdapter->CheckInterfaceSupport(IID_Converter::ToNative(RIID), (LARGE_INTEGER*)PinnedUserModeDriverVersion);
	}

	int EnumerateOutputs(unsigned int OutputNo, [Out] Output^% Output)
	{
		IDXGIOutput* pOutput = 0;
		int Result = pAdapter->EnumOutputs(OutputNo, &pOutput);

		if (pOutput)
		{
			try { Output = (DXGI::Output^)Interfaces[IntPtr(pOutput)]; }
			catch (KeyNotFoundException^) { Output = gcnew DXGI::Output(IntPtr(pOutput)); }					
		}
		else Output = nullptr;

		return Result;
	}

	int GetDescription([Out] AdapterDescription% Description)
	{
		DXGI_ADAPTER_DESC NativeDescription;
		int Result = pAdapter->GetDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}
};