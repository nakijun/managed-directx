public value class WeldEpsilons : IEquatable<WeldEpsilons>
{
public:
    float Position;
    float BlendWeights;
    float Normal;
    float PointSize;
    float Specular;
    float Diffuse;
	Float8 TextureCoordinates;
    float Tangent;
    float Binormal;
    float TessellationFactor;

	static bool operator == (WeldEpsilons Left, WeldEpsilons Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (WeldEpsilons Left, WeldEpsilons Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Position ^
			(int)BlendWeights ^
			(int)Normal ^
			(int)PointSize ^
			(int)Specular ^
			(int)Diffuse ^
			TextureCoordinates.GetHashCode() ^
			(int)Tangent ^
			(int)Binormal ^
			(int)TessellationFactor;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(WeldEpsilons Value)
	{
		pin_ptr<WeldEpsilons> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(WeldEpsilons::typeid)) == 0;
	}

	static bool Equals(WeldEpsilons% Value1, WeldEpsilons% Value2)
	{
		pin_ptr<WeldEpsilons> PinnedValue1 = &Value1;
		pin_ptr<WeldEpsilons> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(WeldEpsilons::typeid)) == 0;
	}
};