[StructLayout(LayoutKind::Sequential)]
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
		return
			FillMode == Value.FillMode &&
			CullMode == Value.CullMode &&
			FrontCounterClockwise == Value.FrontCounterClockwise &&
			DepthBias == Value.DepthBias &&
			DepthBiasClamp == Value.DepthBiasClamp &&
			SlopeScaledDepthBias == Value.SlopeScaledDepthBias &&
			DepthClipEnable == Value.DepthClipEnable &&
			ScissorEnable == Value.ScissorEnable &&
			MultisampleEnable == Value.MultisampleEnable &&
			AntialiasedLineEnable == Value.AntialiasedLineEnable;
	}

	static bool Equals(RasterizerDescription% Value1, RasterizerDescription% Value2)
	{
		return
			Value1.FillMode == Value2.FillMode &&
			Value1.CullMode == Value2.CullMode &&
			Value1.FrontCounterClockwise == Value2.FrontCounterClockwise &&
			Value1.DepthBias == Value2.DepthBias &&
			Value1.DepthBiasClamp == Value2.DepthBiasClamp &&
			Value1.SlopeScaledDepthBias == Value2.SlopeScaledDepthBias &&
			Value1.DepthClipEnable == Value2.DepthClipEnable &&
			Value1.ScissorEnable == Value2.ScissorEnable &&
			Value1.MultisampleEnable == Value2.MultisampleEnable &&
			Value1.AntialiasedLineEnable == Value2.AntialiasedLineEnable;
	}
};