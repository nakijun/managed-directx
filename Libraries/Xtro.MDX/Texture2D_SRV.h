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
		return MipLevels;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture2D_SRV Value)
	{
		return 
			MostDetailedMip == Value.MostDetailedMip &&
			MipLevels == Value.MipLevels;
	}

	static bool Equals(Texture2D_SRV% Value1, Texture2D_SRV% Value2)
	{
		return 
			Value1.MostDetailedMip == Value2.MostDetailedMip &&
			Value1.MipLevels == Value2.MipLevels;
	}
};