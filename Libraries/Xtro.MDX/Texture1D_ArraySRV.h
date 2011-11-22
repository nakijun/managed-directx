public value class Texture1D_ArraySRV : IEquatable<Texture1D_ArraySRV>
{
public:
	unsigned int MostDetailedMip;
	unsigned int MipLevels;
	unsigned int FirstArraySlice;
	unsigned int ArraySize;

	static bool operator == (Texture1D_ArraySRV Left, Texture1D_ArraySRV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture1D_ArraySRV Left, Texture1D_ArraySRV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			MostDetailedMip ^
			MipLevels ^
			FirstArraySlice ^
			ArraySize;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture1D_ArraySRV Value)
	{
		pin_ptr<Texture1D_ArraySRV> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Texture1D_ArraySRV::typeid)) == 0;
	}

	static bool Equals(Texture1D_ArraySRV% Value1, Texture1D_ArraySRV% Value2)
	{
		pin_ptr<Texture1D_ArraySRV> PinnedValue1 = &Value1;
		pin_ptr<Texture1D_ArraySRV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture1D_ArraySRV::typeid)) == 0;
	}
};