public value class Color : IEquatable<Color>
{
public:
	float R;
	float G;
	float B;
	float A;

	Color(unsigned int ARGB)
	{
		const float F = 1.0f / 255.0f;
		R = F * (float) (unsigned char) (ARGB >> 16);
		G = F * (float) (unsigned char) (ARGB >>  8);
		B = F * (float) (unsigned char) (ARGB >>  0);
		A = F * (float) (unsigned char) (ARGB >> 24);
	}

	Color(array<float>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 4) return;

		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<float> PinnedThis = &R;
		memcpy(PinnedThis, PinnedFloats, Marshal::SizeOf(Color::typeid));
	}

	Color(array<Float16bit>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 4) return;

		R = Floats[0].Value;
		G = Floats[1].Value;
		B = Floats[2].Value;
		A = Floats[3].Value;
	}

	Color(float R, float G, float B, float A)
	{
		this->R = R;
		this->G = G;
		this->B = B;
		this->A = A;
	}

	static explicit operator array<float>^(Color Value)
	{
		array<float>^ Floats = gcnew array<float>(4);
		
		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<Color> PinnedValue = &Value;
		memcpy(PinnedFloats, PinnedValue, Marshal::SizeOf(Color::typeid));

		return Floats;
	}

	void Add(Color Value)
	{
		R += Value.R;
		G += Value.G;
		B += Value.B;
		A += Value.A;
	}

	void Subtract(Color Value)
	{
		R -= Value.R;
		G -= Value.G;
		B -= Value.B;
		A -= Value.A;
	}

	void Multiply(float Value)
	{
		R *= Value;
		G *= Value;
		B *= Value;
		A *= Value;
	}

	void Divide(float Value)
	{
		R /= Value;
		G /= Value;
		B /= Value;
		A /= Value;
	}

	static Color operator + (Color Value)
	{
		return Value;
	}

	static Color operator - (Color Value)
	{
		return Color(-Value.R, -Value.G, -Value.B, -Value.A);
	}

	static Color operator + (Color Value1, Color Value2)
	{
		return Color(Value1.R + Value2.R, Value1.G + Value2.G, Value1.B + Value2.B, Value1.A + Value2.A);
	}

	static Color operator - (Color Value1, Color Value2)
	{
		return Color(Value1.R - Value2.R, Value1.G - Value2.G, Value1.B - Value2.B, Value1.A - Value2.A);
	}

	static Color operator * (Color Value1, float Value2)
	{
		return Color(Value1.R * Value2, Value1.G * Value2, Value1.B * Value2, Value1.A * Value2);
	}

	static Color operator / (Color Value1, float Value2)
	{
		float InvertedValue = 1.0f / Value2;
		return Color(Value1.R * InvertedValue, Value1.G * InvertedValue, Value1.B * InvertedValue, Value1.A * InvertedValue);
	}

	static bool operator == (Color Left, Color Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Color Left, Color Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)R ^
			(int)G ^
			(int)B ^
			(int)A;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Color Value)
	{
		pin_ptr<float> PinnedThis = &R;
		pin_ptr<Color> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Color::typeid)) == 0;
	}

	static bool Equals(Color% Value1, Color% Value2)
	{
		pin_ptr<Color> PinnedValue1 = &Value1;
		pin_ptr<Color> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Color::typeid)) == 0;
	}
};