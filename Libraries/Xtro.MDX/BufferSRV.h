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
		return
			ElementOffset == Value.ElementOffset &&
			ElementWidth == Value.ElementWidth;
	}

	static bool Equals(BufferSRV% Value1, BufferSRV% Value2)
	{
		return
			Value1.ElementOffset == Value2.ElementOffset && 
			Value1.ElementWidth == Value2.ElementWidth;
	}
};