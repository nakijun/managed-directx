ref class IID_Converter abstract sealed
{
public:
	static Guid ToManaged(const IID &IID)
	{
		if (IID == GUID_NULL) return Guid::Empty;

		Guid Result(
			IID.Data1,
			IID.Data2,
			IID.Data3,
			IID.Data4[0],
			IID.Data4[1],
			IID.Data4[2], 
			IID.Data4[3],
			IID.Data4[4],
			IID.Data4[5],
			IID.Data4[6],
			IID.Data4[7]
		);

		return Result;
	}

	static IID ToNative(Guid Guid)
	{
		if (Guid == Guid::Empty) return GUID_NULL;

		IID Result;
		array<Byte>^ Bytes = Guid.ToByteArray();
		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];

		memcpy(&Result, PinnedBytes, sizeof(GUID));

		return Result;
	}
};