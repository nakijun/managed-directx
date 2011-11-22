public value class DepthStencilOperationDescription : IEquatable<DepthStencilOperationDescription>
{
public:
    StencilOperation StencilFailOperation;
    StencilOperation StencilDepthFailOperation;
    StencilOperation StencilPassOperation;
    ComparisonFunction StencilFunction;

	static bool operator == (DepthStencilOperationDescription Left, DepthStencilOperationDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (DepthStencilOperationDescription Left, DepthStencilOperationDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)StencilFailOperation ^
			(int)StencilDepthFailOperation ^
			(int)StencilPassOperation ^
			(int)StencilFunction;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(DepthStencilOperationDescription Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(DepthStencilOperationDescription::typeid)) == 0;
	}

	static bool Equals(DepthStencilOperationDescription% Value1, DepthStencilOperationDescription% Value2)
	{
		pin_ptr<DepthStencilOperationDescription> PinnedValue1 = &Value1;
		pin_ptr<DepthStencilOperationDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(DepthStencilOperationDescription::typeid)) == 0;
	}
};