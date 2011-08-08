public value class BlendDescription : IEquatable<BlendDescription>
{
private:
	int BAlphaToCoverageEnable;
public:
    Boolean8 BlendEnable;
    Blend SourceBlend;
    Blend DestinationBlend;
    BlendOperation BlendOperation;
    Blend SourceBlendAlpha;
    Blend DestinationBlendAlpha;
    Xtro::MDX::Direct3D10::BlendOperation BlendOperationAlpha;
	Byte8 RenderTargetWriteMask;

	property bool AlphaToCoverageEnable
	{
		bool get()
		{
			return BAlphaToCoverageEnable != 0;
		}

		void set(bool Value)
		{
			BAlphaToCoverageEnable = Value;
		}
	}

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
			BAlphaToCoverageEnable ^
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
		pin_ptr<int> PinnedAlphaToCoverageEnable = &BAlphaToCoverageEnable;
		pin_ptr<BlendDescription> PinnedValue = &Value;

		return memcmp(PinnedAlphaToCoverageEnable, PinnedValue, Marshal::SizeOf(BlendDescription::typeid)) == 0;
	}

	static bool Equals(BlendDescription% Value1, BlendDescription% Value2)
	{
		pin_ptr<BlendDescription> PinnedValue1 = &Value1;
		pin_ptr<BlendDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(BlendDescription::typeid)) == 0;
	}
};