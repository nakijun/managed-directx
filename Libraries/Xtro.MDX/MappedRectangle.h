public value class MappedRectangle : IEquatable<MappedRectangle>
{
public:
	unsigned int Pitch;
	UnmanagedMemory^ Bits;

	static bool operator == (MappedRectangle Left, MappedRectangle Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (MappedRectangle Left, MappedRectangle Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Pitch ^
			(Bits == nullptr ? 1 : Bits->GetHashCode());
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(MappedRectangle Value)
	{
		return
			Pitch == Value.Pitch &&
			Bits == Value.Bits;
	}

	static bool Equals(MappedRectangle% Value1, MappedRectangle% Value2)
	{
		return
			Value1.Pitch == Value2.Pitch &&
			Value1.Bits == Value2.Bits;
	}
};