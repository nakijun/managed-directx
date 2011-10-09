public value class AttributeWeights : IEquatable<AttributeWeights>
{
public:
    float Position;
    float Boundary;
    float Normal;
    float Diffuse;
    float Specular;
    Float8 TextureCoordinates;
    float Tangent;
    float Binormal;

	static bool operator == (AttributeWeights Left, AttributeWeights Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (AttributeWeights Left, AttributeWeights Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Position ^
			(int)Boundary ^
			(int)Normal ^
			(int)Diffuse ^
			(int)Specular ^
			TextureCoordinates.GetHashCode() ^
			(int)Tangent ^
			(int)Binormal;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(AttributeWeights Value)
	{
		pin_ptr<float> PinnedThis = &Position;
		pin_ptr<AttributeWeights> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(AttributeWeights::typeid)) == 0;
	}

	static bool Equals(AttributeWeights% Value1, AttributeWeights% Value2)
	{
		pin_ptr<AttributeWeights> PinnedValue1 = &Value1;
		pin_ptr<AttributeWeights> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(AttributeWeights::typeid)) == 0;
	}
};