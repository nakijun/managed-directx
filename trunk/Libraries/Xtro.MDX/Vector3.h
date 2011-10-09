public value class Vector3 : IEquatable<Vector3>
{
public:
	float X;
	float Y;
	float Z;

	Vector3(array<float>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 3) return;

		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<float> PinnedThis = &X;
		memcpy(PinnedThis, PinnedFloats, Marshal::SizeOf(Vector3::typeid));
	}

	Vector3(Vector Vector)
	{
		this->X = Vector.X;
		this->Y = Vector.Y;
		this->Z = Vector.Z;
	}

	Vector3(array<Float16bit>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 3) return;

		X = Floats[0].Value;
		Y = Floats[1].Value;
		Z = Floats[2].Value;
	}

	Vector3(float X, float Y, float Z)
	{
		this->X = X;
		this->Y = Y;
		this->Z = Z;
	}

	static explicit operator array<float>^(Vector3 Value)
	{
		return gcnew array<float>(3) { Value.X, Value.Y, Value.Z };
	}

	void Add(Vector3 Value)
	{
		X += Value.X;
		Y += Value.Y;
		Z += Value.Z;
	}

	void Subtract(Vector3 Value)
	{
		X -= Value.X;
		Y -= Value.Y;
		Z -= Value.Z;
	}

	void Multiply(float Value)
	{
		X *= Value;
		Y *= Value;
		Z *= Value;
	}

	void Divide(float Value)
	{
		X /= Value;
		Y /= Value;
		Z /= Value;
	}

	static Vector3 operator + (Vector3 Value)
	{
		return Value;
	}

	static Vector3 operator - (Vector3 Value)
	{
		return Vector3(-Value.X, -Value.Y, -Value.Z);
	}

	static Vector3 operator + (Vector3 Value1, Vector3 Value2)
	{
		return Vector3(Value1.X + Value2.X, Value1.Y + Value2.Y, Value1.Z + Value2.Z);
	}

	static Vector3 operator - (Vector3 Value1, Vector3 Value2)
	{
		return Vector3(Value1.X - Value2.X, Value1.Y - Value2.Y, Value1.Z - Value2.Z);
	}

	static Vector3 operator * (Vector3 Value1, float Value2)
	{
		return Vector3(Value1.X * Value2, Value1.Y * Value2, Value1.Z * Value2);
	}

	static Vector3 operator / (Vector3 Value1, float Value2)
	{
		float InvertedValue = 1.0f / Value2;
		return Vector3(Value1.X * InvertedValue, Value1.Y * InvertedValue, Value1.Z * InvertedValue);
	}

	static bool operator == (Vector3 Left, Vector3 Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Vector3 Left, Vector3 Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)X ^
			(int)Y ^
			(int)Z;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Vector3 Value)
	{
		pin_ptr<float> PinnedThis = &X;
		pin_ptr<Vector3> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Vector3::typeid)) == 0;
	}

	static bool Equals(Vector3% Value1, Vector3% Value2)
	{
		pin_ptr<Vector3> PinnedValue1 = &Value1;
		pin_ptr<Vector3> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Vector3::typeid)) == 0;
	}
};