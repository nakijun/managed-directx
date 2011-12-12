public value class Viewport : IEquatable<Viewport>
{
public:
	unsigned int TopLeftX;
	unsigned int TopLeftY;
	unsigned int Width;
	unsigned int Height;
	float MinimumDepth;
	float MaximumDepth;

	static bool operator == (Viewport Left, Viewport Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Viewport Left, Viewport Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			TopLeftX ^
			TopLeftY ^
			Width ^
			Height ^
			(int)MinimumDepth ^
			(int)MaximumDepth;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Viewport Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(Viewport::typeid)) == 0;
	}

	static bool Equals(Viewport% Value1, Viewport% Value2)
	{
		pin_ptr<Viewport> PinnedValue1 = &Value1;
		pin_ptr<Viewport> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Viewport::typeid)) == 0;
	}
};