public value class SurfaceDescription : IEquatable<SurfaceDescription>
{
public:
	unsigned int Width;
	unsigned int Height;
	Format Format;
	SampleDescription SampleDescription;
		
	static bool operator == (SurfaceDescription Left, SurfaceDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SurfaceDescription Left, SurfaceDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Width ^
			Height ^
			(int)Format ^
			SampleDescription.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SurfaceDescription Value)
	{
		pin_ptr<unsigned int> PinnedThis = &Width;
		pin_ptr<SurfaceDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(SurfaceDescription::typeid)) == 0;
	}

	static bool Equals(SurfaceDescription% Value1, SurfaceDescription% Value2)
	{
		pin_ptr<SurfaceDescription> PinnedValue1 = &Value1;
		pin_ptr<SurfaceDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SurfaceDescription::typeid)) == 0;
	}
};