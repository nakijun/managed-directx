public value class Box : IEquatable<Box>
{
public:
	unsigned int Left;
	unsigned int Top;
	unsigned int Front;
	unsigned int Right;
	unsigned int Bottom;
	unsigned int Back;

	static bool operator == (Box Left, Box Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Box Left, Box Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Left ^
			Top ^
			Front ^
			Right ^
			Bottom ^
			Back;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Box Value)
	{
		pin_ptr<unsigned int> PinnedThis = &Left;
		pin_ptr<Box> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Box::typeid)) == 0;
	}

	static bool Equals(Box% Value1, Box% Value2)
	{
		pin_ptr<Box> PinnedValue1 = &Value1;
		pin_ptr<Box> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Box::typeid)) == 0;
	}
};