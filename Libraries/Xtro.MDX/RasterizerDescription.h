[StructLayout(LayoutKind::Sequential, Size = 40)]
public value class RasterizerDescription : IEquatable<RasterizerDescription>
{
public:
	FillMode FillMode;
    CullMode CullMode;
    bool FrontCounterClockwise;
    int DepthBias;
    float DepthBiasClamp;
    float SlopeScaledDepthBias;
    bool DepthClipEnable;
    bool ScissorEnable;
    bool MultisampleEnable;
    bool AntialiasedLineEnable;

	static bool operator == (RasterizerDescription Left, RasterizerDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (RasterizerDescription Left, RasterizerDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)FillMode ^
			(int)CullMode ^
			FrontCounterClockwise.GetHashCode() ^
			DepthBias ^
			(int)DepthBiasClamp ^
			(int)SlopeScaledDepthBias ^
			DepthClipEnable.GetHashCode() ^
			ScissorEnable.GetHashCode() ^
			MultisampleEnable.GetHashCode() ^
			AntialiasedLineEnable.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(RasterizerDescription Value)
	{
		pin_ptr<Xtro::MDX::Direct3D10::FillMode> PinnedThis = &FillMode;
		pin_ptr<RasterizerDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(RasterizerDescription::typeid)) == 0;
	}

	static bool Equals(RasterizerDescription% Value1, RasterizerDescription% Value2)
	{
		pin_ptr<RasterizerDescription> PinnedValue1 = &Value1;
		pin_ptr<RasterizerDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(RasterizerDescription::typeid)) == 0;
	}
};