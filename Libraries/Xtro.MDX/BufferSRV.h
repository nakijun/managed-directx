public value class BufferSRV : IEquatable<BufferSRV>
{
public:
	unsigned int ElementOffset;
	unsigned int ElementWidth;

	static bool operator == (BufferSRV Left, BufferSRV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (BufferSRV Left, BufferSRV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			ElementOffset ^
			ElementWidth;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(BufferSRV Value)
	{
		pin_ptr<unsigned int> PinnedThis = &ElementOffset;
		pin_ptr<BufferSRV> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(BufferSRV::typeid)) == 0;
	}

	static bool Equals(BufferSRV% Value1, BufferSRV% Value2)
	{
		pin_ptr<BufferSRV> PinnedValue1 = &Value1;
		pin_ptr<BufferSRV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(BufferSRV::typeid)) == 0;
	}
};