public value class Texture2D_SRV : IEquatable<Texture2D_SRV>
{
public:
	unsigned int MostDetailedMip;
	unsigned int MipLevels;

	static bool operator == (Texture2D_SRV Left, Texture2D_SRV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture2D_SRV Left, Texture2D_SRV Right)
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

	virtual bool Equals(Texture2D_SRV Value)
	{
		pin_ptr<unsigned int> PinnedThis = &MostDetailedMip;
		pin_ptr<Texture2D_SRV> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Texture2D_SRV::typeid)) == 0;
	}

	static bool Equals(Texture2D_SRV% Value1, Texture2D_SRV% Value2)
	{
		pin_ptr<Texture2D_SRV> PinnedValue1 = &Value1;
		pin_ptr<Texture2D_SRV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture2D_SRV::typeid)) == 0;
	}
};