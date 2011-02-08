[StructLayout(LayoutKind::Sequential)]
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
		return
			MostDetailedMip == Value.MostDetailedMip &&
			MipLevels == Value.MipLevels;
	}

	static bool Equals(Texture1D_SRV% Value1, Texture1D_SRV% Value2)
	{
		return
			Value1.MostDetailedMip == Value2.MostDetailedMip &&
			Value1.MipLevels == Value2.MipLevels;
	}
};