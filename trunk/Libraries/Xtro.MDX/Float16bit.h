public value class Float16bit : IEquatable<Float16bit>
{
internal:
	unsigned short Value;

public:
	Float16bit(float Float)
	{
		Value = (unsigned short)Float;
	}

	static explicit operator float(Float16bit Value)
	{
		return Value.Value;
	}

	static bool operator == (Float16bit Left, Float16bit Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Float16bit Left, Float16bit Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return (int)Value;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Float16bit Value)
	{
		return this->Value == Value.Value;
	}

	static bool Equals(Float16bit% Value1, Float16bit% Value2)
	{
		return Value1.Value == Value2.Value;
	}
};