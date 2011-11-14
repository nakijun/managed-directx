public value class BlendDescription : IEquatable<BlendDescription>
{
public:
	bool AlphaToCoverageEnable;
    Boolean8 BlendEnable;
    Blend SourceBlend;
    Blend DestinationBlend;
    BlendOperation BlendOperation;
    Blend SourceBlendAlpha;
    Blend DestinationBlendAlpha;
    Direct3D10::BlendOperation BlendOperationAlpha;
	Byte8 RenderTargetWriteMask;

	static bool operator == (BlendDescription Left, BlendDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (BlendDescription Left, BlendDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)AlphaToCoverageEnable ^
			BlendEnable.GetHashCode() ^
			(int)SourceBlend ^
			(int)DestinationBlend ^
			(int)BlendOperation ^
			(int)SourceBlendAlpha ^
			(int)DestinationBlendAlpha ^
			(int)BlendOperationAlpha ^
			RenderTargetWriteMask.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(BlendDescription Value)
	{
		pin_ptr<bool> PinnedThis = &AlphaToCoverageEnable;
		pin_ptr<BlendDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(BlendDescription::typeid)) == 0;
	}

	static bool Equals(BlendDescription% Value1, BlendDescription% Value2)
	{
		pin_ptr<BlendDescription> PinnedValue1 = &Value1;
		pin_ptr<BlendDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(BlendDescription::typeid)) == 0;
	}
};