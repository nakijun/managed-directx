public value class SkinningChannel : IEquatable<SkinningChannel>
{
public:
    unsigned int SourceOffset;
    unsigned int DestinationOffset;
    bool IsNormal;

	static bool operator == (SkinningChannel Left, SkinningChannel Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SkinningChannel Left, SkinningChannel Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			SourceOffset ^
			DestinationOffset ^
			(int)IsNormal;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SkinningChannel Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(SkinningChannel::typeid)) == 0;
	}

	static bool Equals(SkinningChannel% Value1, SkinningChannel% Value2)
	{
		pin_ptr<SkinningChannel> PinnedValue1 = &Value1;
		pin_ptr<SkinningChannel> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SkinningChannel::typeid)) == 0;
	}
};