public value class BufferDescription : IEquatable<BufferDescription>
{
public:
	unsigned int ByteWidth;
	Usage Usage;
	BindFlag BindFlags;
	CpuAccessFlag CpuAccessFlags;
	ResourceMiscellaneousFlag MiscellaneousFlags;

	static bool operator == (BufferDescription Left, BufferDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (BufferDescription Left, BufferDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			ByteWidth ^
			(int)Usage ^
			(int)BindFlags ^
			(int)CpuAccessFlags ^
			(int)MiscellaneousFlags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(BufferDescription Value)
	{
		pin_ptr<unsigned int> PinnedThis = &ByteWidth;
		pin_ptr<BufferDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(BufferDescription::typeid)) == 0;
	}

	static bool Equals(BufferDescription% Value1, BufferDescription% Value2)
	{
		pin_ptr<BufferDescription> PinnedValue1 = &Value1;
		pin_ptr<BufferDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(BufferDescription::typeid)) == 0;
	}
};