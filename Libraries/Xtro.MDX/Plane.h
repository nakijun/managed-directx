public value class Plane : IEquatable<Plane>
{
public:
	float A;
	float B;
	float C;
	float D;

	Plane(array<float>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 4) return;

		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<float> PinnedThis = &A;
		memcpy(PinnedThis, PinnedFloats, Marshal::SizeOf(Plane::typeid));
	}

	Plane(array<Float16bit>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 4) return;

		A = Floats[0].Value;
		B = Floats[1].Value;
		C = Floats[2].Value;
		D = Floats[3].Value;
	}

	Plane(float A, float B, float C, float D)
	{
		this->A = A;
		this->B = B;
		this->C = C;
		this->D = D;
	}

	static explicit operator array<float>^(Plane Value)
	{
		array<float>^ Floats = gcnew array<float>(4);
		
		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<Plane> PinnedValue = &Value;
		memcpy(PinnedFloats, PinnedValue, Marshal::SizeOf(Plane::typeid));

		return Floats;
	}

	void Add(Plane Value)
	{
		A += Value.A;
		B += Value.B;
		C += Value.C;
		D += Value.D;
	}

	void Subtract(Plane Value)
	{
		A -= Value.A;
		B -= Value.B;
		C -= Value.C;
		D -= Value.D;
	}

	void Multiply(float Value)
	{
		A *= Value;
		B *= Value;
		C *= Value;
		D *= Value;
	}

	void Divide(float Value)
	{
		A /= Value;
		B /= Value;
		C /= Value;
		D /= Value;
	}

	static Plane operator + (Plane Value)
	{
		return Value;
	}

	static Plane operator - (Plane Value)
	{
		return Plane(-Value.A, -Value.B, -Value.C, -Value.D);
	}

	static Plane operator + (Plane Value1, Plane Value2)
	{
		return Plane(Value1.A + Value2.A, Value1.B + Value2.B, Value1.C + Value2.C, Value1.D + Value2.D);
	}

	static Plane operator - (Plane Value1, Plane Value2)
	{
		return Plane(Value1.A - Value2.A, Value1.B - Value2.B, Value1.C - Value2.C, Value1.D - Value2.D);
	}

	static Plane operator * (Plane Value1, float Value2)
	{
		return Plane(Value1.A * Value2, Value1.B * Value2, Value1.C * Value2, Value1.D * Value2);
	}

	static Plane operator / (Plane Value1, float Value2)
	{
		float InvertedValue = 1.0f / Value2;
		return Plane(Value1.A * InvertedValue, Value1.B * InvertedValue, Value1.C * InvertedValue, Value1.D * InvertedValue);
	}

	static bool operator == (Plane Left, Plane Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Plane Left, Plane Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)A ^
			(int)B ^
			(int)C ^
			(int)D;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Plane Value)
	{
		pin_ptr<float> PinnedThis = &A;
		pin_ptr<Plane> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Plane::typeid)) == 0;
	}

	static bool Equals(Plane% Value1, Plane% Value2)
	{
		pin_ptr<Plane> PinnedValue1 = &Value1;
		pin_ptr<Plane> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Plane::typeid)) == 0;
	}
};