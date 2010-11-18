[StructLayout(LayoutKind::Sequential)]
public value class ModeDescription : IEquatable<ModeDescription>
{
public:
	int Width;
	int Height;
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
		return
			Width == Value.Width &&
			Height == Value.Height &&
			RefreshRate == Value.RefreshRate &&
			Format == Value.Format &&
			ScanlineOrdering == Value.ScanlineOrdering &&
			Scaling == Value.Scaling;
	}

	static bool Equals(ModeDescription% Value1, ModeDescription% Value2)
	{
		return
			Value1.Width == Value2.Width &&
			Value1.Height == Value2.Height &&
			Value1.RefreshRate == Value2.RefreshRate &&
			Value1.Format == Value2.Format &&
			Value1.ScanlineOrdering == Value2.ScanlineOrdering &&
			Value1.Scaling == Value2.Scaling;
	}
};