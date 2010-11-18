[StructLayout(LayoutKind::Sequential)]
public value class BufferDescription : IEquatable<BufferDescription>
{
public:
	unsigned int ByteWidth;
	Usage Usage;
	BindFlag BindFlags;
	CPU_AccessFlag CPU_AccessFlags;
	ResourceMiscFlag MiscFlags;

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
			(int)CPU_AccessFlags ^
			(int)MiscFlags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(BufferDescription Value)
	{
		return
			ByteWidth == Value.ByteWidth &&
			Usage == Value.Usage &&
			BindFlags == Value.BindFlags &&
			CPU_AccessFlags == Value.CPU_AccessFlags &&
			MiscFlags == Value.MiscFlags;
	}

	static bool Equals(BufferDescription% Value1, BufferDescription% Value2)
	{
		return
			Value1.ByteWidth == Value2.ByteWidth && 
			Value1.Usage == Value2.Usage &&
			Value1.BindFlags == Value2.BindFlags &&
			Value1.CPU_AccessFlags == Value2.CPU_AccessFlags &&
			Value1.MiscFlags == Value2.MiscFlags;
	}
};