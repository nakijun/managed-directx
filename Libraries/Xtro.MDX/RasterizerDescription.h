public value class RasterizerDescription : IEquatable<RasterizerDescription>
{
public:
	FillMode FillMode;
    CullMode CullMode;
    bool FrontCounterClockwise;
    int DepthBias;
    float DepthBiasClamp;
    float SlopeScaledDepthBias;
private:
    int BDepthClipEnable;
    int BScissorEnable;
    int BMultisampleEnable;
    int BAntialiasedLineEnable;

public:
	property bool DepthClipEnable
	{
		bool get()
		{
			return BDepthClipEnable;
		}

		void set(bool Value)
		{
			BDepthClipEnable = Value;
		}
	}

	property bool ScissorEnable
	{
		bool get()
		{
			return BScissorEnable;
		}

		void set(bool Value)
		{
			BScissorEnable = Value;
		}
	}

	property bool MultisampleEnable
	{
		bool get()
		{
			return BMultisampleEnable;
		}

		void set(bool Value)
		{
			BMultisampleEnable = Value;
		}
	}

	property bool AntialiasedLineEnable
	{
		bool get()
		{
			return BAntialiasedLineEnable;
		}

		void set(bool Value)
		{
			BAntialiasedLineEnable = Value;
		}
	}

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
			(int)FrontCounterClockwise ^
			DepthBias ^
			(int)DepthBiasClamp ^
			(int)SlopeScaledDepthBias ^
			BDepthClipEnable ^
			BScissorEnable ^
			BMultisampleEnable ^
			BAntialiasedLineEnable;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(RasterizerDescription Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(RasterizerDescription::typeid)) == 0;
	}

	static bool Equals(RasterizerDescription% Value1, RasterizerDescription% Value2)
	{
		pin_ptr<RasterizerDescription> PinnedValue1 = &Value1;
		pin_ptr<RasterizerDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(RasterizerDescription::typeid)) == 0;
	}
};