public value class BufferRTV : IEquatable<BufferRTV>
{
public:
	unsigned int ElementOffset;
	unsigned int ElementWidth;

	static bool operator == (BufferRTV Left, BufferRTV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (BufferRTV Left, BufferRTV Right)
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

	virtual bool Equals(BufferRTV Value)
	{
		pin_ptr<unsigned int> PinnedThis = &ElementOffset;
		pin_ptr<BufferRTV> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(BufferRTV::typeid)) == 0;
	}

	static bool Equals(BufferRTV% Value1, BufferRTV% Value2)
	{
		pin_ptr<BufferRTV> PinnedValue1 = &Value1;
		pin_ptr<BufferRTV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(BufferRTV::typeid)) == 0;
	}
};