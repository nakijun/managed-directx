[StructLayout(LayoutKind::Sequential)]
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
		return
			ElementOffset == Value.ElementOffset &&
			ElementWidth == Value.ElementWidth;
	}

	static bool Equals(BufferRTV% Value1, BufferRTV% Value2)
	{
		return
			Value1.ElementOffset == Value2.ElementOffset && 
			Value1.ElementWidth == Value2.ElementWidth;
	}
};