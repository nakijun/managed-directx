public value class Texture1D_SRV : IEquatable<Texture1D_SRV>
{
public:
	unsigned int MostDetailedMip;
	unsigned int MipLevels;

	static bool operator == (Texture1D_SRV Left, Texture1D_SRV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture1D_SRV Left, Texture1D_SRV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return 
			MostDetailedMip ^
			MipLevels;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture1D_SRV Value)
	{
		pin_ptr<Texture1D_SRV> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Texture1D_SRV::typeid)) == 0;
	}

	static bool Equals(Texture1D_SRV% Value1, Texture1D_SRV% Value2)
	{
		pin_ptr<Texture1D_SRV> PinnedValue1 = &Value1;
		pin_ptr<Texture1D_SRV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture1D_SRV::typeid)) == 0;
	}
};