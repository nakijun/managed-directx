public value class Vector : IEquatable<Vector>
{
public:
	float X;
	float Y;
	float Z;

	static bool operator == (Vector Left, Vector Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Vector Left, Vector Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)X ^
			(int)Y ^
			(int)Z;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Vector Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(Vector::typeid)) == 0;
	}

	static bool Equals(Vector% Value1, Vector% Value2)
	{
		pin_ptr<Vector> PinnedValue1 = &Value1;
		pin_ptr<Vector> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Vector::typeid)) == 0;
	}
};