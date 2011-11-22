public value class Texture3D_SRV : IEquatable<Texture3D_SRV>
{
public:
	unsigned int MostDetailedMip;
	unsigned int MipLevels;

	static bool operator == (Texture3D_SRV Left, Texture3D_SRV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture3D_SRV Left, Texture3D_SRV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			MipLevels;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture3D_SRV Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(Texture3D_SRV::typeid)) == 0;
	}

	static bool Equals(Texture3D_SRV% Value1, Texture3D_SRV% Value2)
	{
		pin_ptr<Texture3D_SRV> PinnedValue1 = &Value1;
		pin_ptr<Texture3D_SRV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture3D_SRV::typeid)) == 0;
	}
};