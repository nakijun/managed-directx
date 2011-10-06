public value class SamplerDescription : IEquatable<SamplerDescription>
{
public:
    Filter Filter;
    TextureAddressMode AddressU;
    TextureAddressMode AddressV;
    TextureAddressMode AddressW;
    float MipLOD_Bias;
    unsigned int MaximumAnisotropy;
    ComparisonFunction ComparisonFunction;
    Float4 BorderColor;
    float MinimumLOD;
    float MaximumLOD;

	static bool operator == (SamplerDescription Left, SamplerDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SamplerDescription Left, SamplerDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Filter ^
			(int)AddressU ^
			(int)AddressV ^
			(int)AddressW ^
			(int)MipLOD_Bias ^
			MaximumAnisotropy ^
			(int)ComparisonFunction ^
			BorderColor.GetHashCode() ^
			(int)MinimumLOD ^
			(int)MaximumLOD;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SamplerDescription Value)
	{
		pin_ptr<Xtro::MDX::Direct3D10::Filter> PinnedThis = &Filter;
		pin_ptr<SamplerDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(SamplerDescription::typeid)) == 0;
	}

	static bool Equals(SamplerDescription% Value1, SamplerDescription% Value2)
	{
		pin_ptr<SamplerDescription> PinnedValue1 = &Value1;
		pin_ptr<SamplerDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SamplerDescription::typeid)) == 0;
	}
};