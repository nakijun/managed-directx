public value class ModeDescription : IEquatable<ModeDescription>
{
public:
	unsigned int Width;
	unsigned int Height;
	Rational RefreshRate;
	Format Format;
	ModeScanlineOrder ScanlineOrdering;
	ModeScaling Scaling;

	static bool operator == (ModeDescription Left, ModeDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ModeDescription Left, ModeDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Width ^
			Height ^
			RefreshRate.GetHashCode() ^
			(int)Format ^
			(int)ScanlineOrdering ^
			(int)Scaling;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ModeDescription Value)
	{
		pin_ptr<unsigned int> PinnedThis = &Width;
		pin_ptr<ModeDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(ModeDescription::typeid)) == 0;
	}

	static bool Equals(ModeDescription% Value1, ModeDescription% Value2)
	{
		pin_ptr<ModeDescription> PinnedValue1 = &Value1;
		pin_ptr<ModeDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(ModeDescription::typeid)) == 0;
	}
};