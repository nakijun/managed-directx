public value class Vector2 : IEquatable<Vector2>
{
public:
	float X;
	float Y;

	Vector2(array<float>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 2) return;

		X = Floats[0];
		Y = Floats[1];
	}

	Vector2(Vector Vector)
	{
		this->X = Vector.X;
		this->Y = Vector.Y;
	}

	Vector2(array<Float16>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 2) return;

		X = Floats[0].Value;
		Y = Floats[1].Value;
	}

	Vector2(float X, float Y)
	{
		this->X = X;
		this->Y = Y;
	}

	static explicit operator array<float>^(Vector2 Value)
	{
		return gcnew array<float>(2) { Value.X, Value.Y };
	}

	void Add(Vector2 Value)
	{
		X += Value.X;
		Y += Value.Y;
	}

	void Subtract(Vector2 Value)
	{
		X -= Value.X;
		Y -= Value.Y;
	}

	void Multiply(float Value)
	{
		X *= Value;
		Y *= Value;
	}

	void Divide(float Value)
	{
		X /= Value;
		Y /= Value;
	}

	static Vector2 operator + (Vector2 Value)
	{
		return Value;
	}

	static Vector2 operator - (Vector2 Value)
	{
		return Vector2(-Value.X, -Value.Y);
	}

	static Vector2 operator + (Vector2 Value1, Vector2 Value2)
	{
		return Vector2(Value1.X + Value2.X, Value1.Y + Value2.Y);
	}

	static Vector2 operator - (Vector2 Value1, Vector2 Value2)
	{
		return Vector2(Value1.X - Value2.X, Value1.Y - Value2.Y);
	}

	static Vector2 operator * (Vector2 Value1, float Value2)
	{
		return Vector2(Value1.X * Value2, Value1.Y * Value2);
	}

	static Vector2 operator / (Vector2 Value1, float Value2)
	{
		float InvertedValue = 1.0f / Value2;
		return Vector2(Value1.X * InvertedValue, Value1.Y * InvertedValue);
	}

	static bool operator == (Vector2 Left, Vector2 Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Vector2 Left, Vector2 Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)X ^
			(int)Y;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Vector2 Value)
	{
		return
			X == Value.X &&
			Y == Value.Y;
	}

	static bool Equals(Vector2% Value1, Vector2% Value2)
	{
		return
			Value1.X == Value2.X && 
			Value1.Y == Value2.Y;
	}
};