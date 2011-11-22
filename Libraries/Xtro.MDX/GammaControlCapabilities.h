public value class GammaControlCapabilities : IEquatable<GammaControlCapabilities>
{
public:
    bool ScaleAndOffsetSupported;
    float MaximumConvertedValue;
    float MinimumConvertedValue;
    unsigned int NumberOfGammaControlPoints;
    Float1025 ControlPointPositions;

	static bool operator == (GammaControlCapabilities Left, GammaControlCapabilities Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (GammaControlCapabilities Left, GammaControlCapabilities Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)ScaleAndOffsetSupported ^
			MaximumConvertedValue.GetHashCode() ^
			MinimumConvertedValue.GetHashCode() ^
			NumberOfGammaControlPoints ^
			ControlPointPositions.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(GammaControlCapabilities Value)
	{
		pin_ptr<GammaControlCapabilities> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(GammaControlCapabilities::typeid)) == 0;
	}

	static bool Equals(GammaControlCapabilities% Value1, GammaControlCapabilities% Value2)
	{
		pin_ptr<GammaControlCapabilities> PinnedValue1 = &Value1;
		pin_ptr<GammaControlCapabilities> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(GammaControlCapabilities::typeid)) == 0;
	}
};