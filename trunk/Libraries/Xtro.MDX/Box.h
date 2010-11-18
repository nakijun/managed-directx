[StructLayout(LayoutKind::Sequential)]
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
		return
			Left == Value.Left &&
			Top == Value.Top &&
			Front == Value.Front &&
			Right == Value.Right &&
			Bottom == Value.Bottom &&
			Back == Value.Back;
	}

	static bool Equals(Box% Value1, Box% Value2)
	{
		return
			Value1.Left == Value2.Left && 
			Value1.Top == Value2.Top && 
			Value1.Front == Value2.Front && 
			Value1.Right == Value2.Right && 
			Value1.Bottom == Value2.Bottom && 
			Value1.Back == Value2.Back;
	}
};