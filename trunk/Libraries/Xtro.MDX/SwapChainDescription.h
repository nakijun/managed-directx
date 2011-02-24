[StructLayout(LayoutKind::Sequential, Size = 60)]
public value class SwapChainDescription : IEquatable<SwapChainDescription>
{
public:
	ModeDescription BufferDescription;
	SampleDescription SampleDescription;
	UsageFlag BufferUsage;
	unsigned int BufferCount;
	IntPtr OutputWindow;
	bool Windowed;
	SwapEffect SwapEffect;
	SwapChainFlag Flags;

	static bool operator == (SwapChainDescription Left, SwapChainDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SwapChainDescription Left, SwapChainDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			BufferDescription.GetHashCode() ^
			SampleDescription.GetHashCode() ^
			(int)BufferUsage ^
			BufferCount ^
			OutputWindow.GetHashCode() ^
			Windowed.GetHashCode() ^
			(int)SwapEffect ^
			(int)Flags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SwapChainDescription Value)
	{
		return
			BufferDescription == Value.BufferDescription &&
			SampleDescription == Value.SampleDescription &&
			BufferUsage == Value.BufferUsage &&
			BufferCount == Value.BufferCount &&
			OutputWindow == Value.OutputWindow &&
			Windowed == Value.Windowed &&
			SwapEffect == Value.SwapEffect &&
			Flags == Value.Flags;
	}

	static bool Equals(SwapChainDescription% Value1, SwapChainDescription% Value2)
	{
		return
			Value1.BufferDescription == Value2.BufferDescription &&
			Value1.SampleDescription == Value2.SampleDescription &&
			Value1.BufferUsage == Value2.BufferUsage &&
			Value1.BufferCount == Value2.BufferCount &&
			Value1.OutputWindow == Value2.OutputWindow &&
			Value1.Windowed == Value2.Windowed &&
			Value1.SwapEffect == Value2.SwapEffect &&
			Value1.Flags == Value2.Flags;
	}
};