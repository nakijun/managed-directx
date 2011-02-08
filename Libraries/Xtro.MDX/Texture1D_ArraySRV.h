[StructLayout(LayoutKind::Sequential)]
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
		return
			MostDetailedMip == Value.MostDetailedMip &&
			MipLevels == Value.MipLevels &&
			FirstArraySlice == Value.FirstArraySlice &&
			ArraySize == Value.ArraySize;
	}

	static bool Equals(Texture1D_ArraySRV% Value1, Texture1D_ArraySRV% Value2)
	{
		return
			Value1.MostDetailedMip == Value2.MostDetailedMip &&
			Value1.MipLevels == Value2.MipLevels &&
			Value1.FirstArraySlice == Value2.FirstArraySlice &&
			Value1.ArraySize == Value2.ArraySize;
	}
};