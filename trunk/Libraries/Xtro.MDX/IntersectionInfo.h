public value class IntersectionInfo : IEquatable<IntersectionInfo>
{
public:
    unsigned int FaceIndex;
	float U;
	float V;
	float Distance;

	static bool operator == (IntersectionInfo Left, IntersectionInfo Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (IntersectionInfo Left, IntersectionInfo Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			FaceIndex ^
			(int)U ^
			(int)V ^
			(int)Distance;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(IntersectionInfo Value)
	{
		pin_ptr<IntersectionInfo> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(IntersectionInfo::typeid)) == 0;
	}

	static bool Equals(IntersectionInfo% Value1, IntersectionInfo% Value2)
	{
		pin_ptr<IntersectionInfo> PinnedValue1 = &Value1;
		pin_ptr<IntersectionInfo> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(IntersectionInfo::typeid)) == 0;
	}
};