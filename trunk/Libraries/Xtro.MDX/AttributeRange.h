public value class AttributeRange : IEquatable<AttributeRange>
{
public:
    unsigned int AttributeId;
    unsigned int FaceStart;
    unsigned int FaceCount;
    unsigned int VertexStart;
    unsigned int VertexCount;

	static bool operator == (AttributeRange Left, AttributeRange Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (AttributeRange Left, AttributeRange Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			AttributeId ^
			FaceStart ^
			FaceCount ^
			VertexStart ^
			VertexCount;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(AttributeRange Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(AttributeRange::typeid)) == 0;
	}

	static bool Equals(AttributeRange% Value1, AttributeRange% Value2)
	{
		pin_ptr<AttributeRange> PinnedValue1 = &Value1;
		pin_ptr<AttributeRange> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(AttributeRange::typeid)) == 0;
	}
};