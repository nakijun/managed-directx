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
		pin_ptr<ModeDescription> PinnedThis = &BufferDescription;
		pin_ptr<SwapChainDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(SwapChainDescription::typeid)) == 0;
	}

	static bool Equals(SwapChainDescription% Value1, SwapChainDescription% Value2)
	{
		pin_ptr<SwapChainDescription> PinnedValue1 = &Value1;
		pin_ptr<SwapChainDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SwapChainDescription::typeid)) == 0;
	}
};