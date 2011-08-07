public ref class SwitchToRef : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10SwitchToRef);

internal:
	ID3D10SwitchToRef* pSwitchToRef;

	SwitchToRef(IntPtr SwitchToRef) : DeviceChild(SwitchToRef)
	{	
		pSwitchToRef = (ID3D10SwitchToRef*)SwitchToRef.ToPointer();
	}

public:
	bool SetUseRef(bool UseRef)
	{
		return pSwitchToRef->SetUseRef(UseRef) != 0;
	}

	bool GetUseRef()
	{
		return pSwitchToRef->GetUseRef() != 0;
	}
};