public value class DepthStencilDescription : IEquatable<DepthStencilDescription>
{
public:
    bool DepthEnable;
    DepthWriteMask DepthWriteMask;
    ComparisonFunction DepthFunction;
private:
    int BStencilEnable;

public:
	property bool StencilEnable
	{
		bool get()
		{
			return BStencilEnable;
		}

		void set(bool Value)
		{
			BStencilEnable = Value;
		}
	}

    unsigned char StencilReadMask;
    unsigned char StencilWriteMask;
    DepthStencilOperationDescription FrontFace;
    DepthStencilOperationDescription BackFace;

	static bool operator == (DepthStencilDescription Left, DepthStencilDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (DepthStencilDescription Left, DepthStencilDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)DepthEnable ^
			(int)DepthWriteMask ^
			(int)DepthFunction ^
			BStencilEnable ^
			StencilReadMask ^
			StencilWriteMask ^
			FrontFace.GetHashCode() ^
			BackFace.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(DepthStencilDescription Value)
	{
		pin_ptr<bool> PinnedThis = &DepthEnable;
		pin_ptr<DepthStencilDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(DepthStencilDescription::typeid)) == 0;
	}

	static bool Equals(DepthStencilDescription% Value1, DepthStencilDescription% Value2)
	{
		pin_ptr<DepthStencilDescription> PinnedValue1 = &Value1;
		pin_ptr<DepthStencilDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(DepthStencilDescription::typeid)) == 0;
	}
};