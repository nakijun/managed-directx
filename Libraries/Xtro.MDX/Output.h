public ref class Output : Object
{
internal:
	IDXGIOutput* pOutput;

	static Guid IID = IID_Converter::ToManaged(IID_IDXGIOutput);

	Output(IntPtr Output) : Object(Output)
	{
		pOutput = (IDXGIOutput*)Output.ToPointer();
	}

public:
	int GetDescription([Out] OutputDescription% Description)
	{
		DXGI_OUTPUT_DESC NativeDescription;
		int Result = pOutput->GetDesc(&NativeDescription);

		if (Result >= 0) Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetDisplayModeList(Format EnumerationFormat, EnumerationMode Flags, unsigned int% NumberOfModes, array<ModeDescription>^ Descriptions)
	{
		pin_ptr<unsigned int> PinnedNumberOfModes = &NumberOfModes;
		pin_ptr<ModeDescription> PinnedDescriptions = Descriptions != nullptr && Descriptions->Length > 0 ? PinnedDescriptions = &Descriptions[0] : nullptr;

		int Result = pOutput->GetDisplayModeList((DXGI_FORMAT)EnumerationFormat, (unsigned int)Flags, (unsigned int*)PinnedNumberOfModes, (DXGI_MODE_DESC*)PinnedDescriptions);

		return Result;
	}
};