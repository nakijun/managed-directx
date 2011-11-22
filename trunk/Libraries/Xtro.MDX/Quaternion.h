public value class Quaternion : IEquatable<Quaternion>
{
public:
	float X;
	float Y;
	float Z;
	float W;

	Quaternion(array<float>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 4) return;

		pin_ptr<float> PinnedFloats = &Floats[0];
		memcpy(&*this, PinnedFloats, Marshal::SizeOf(Quaternion::typeid));
	}

	Quaternion(array<Float16bit>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 4) return;

		X = Floats[0].Value;
		Y = Floats[1].Value;
		Z = Floats[2].Value;
		W = Floats[3].Value;
	}

	Quaternion(float X, float Y, float Z, float W)
	{
		this->X = X;
		this->Y = Y;
		this->Z = Z;
		this->W = W;
	}

	static explicit operator array<float>^(Quaternion Value)
	{
		array<float>^ Floats = gcnew array<float>(4);
		
		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<Quaternion> PinnedValue = &Value;
		memcpy(PinnedFloats, PinnedValue, Marshal::SizeOf(Quaternion::typeid));

		return Floats;
	}

	void Add(Quaternion% Value)
	{
		X += Value.X;
		Y += Value.Y;
		Z += Value.Z;
		W += Value.W;
	}

	void Subtract(Quaternion% Value)
	{
		X -= Value.X;
		Y -= Value.Y;
		Z -= Value.Z;
		W -= Value.W;
	}

	void Multiply(Quaternion% Value)
	{
		pin_ptr<Quaternion> PinnedValue = &Value;
		D3DXQuaternionMultiply((D3DXQUATERNION*)&*this, (D3DXQUATERNION*)&*this, (D3DXQUATERNION*)PinnedValue);
	}


	void Multiply(float Value)
	{
		X *= Value;
		Y *= Value;
		Z *= Value;
		W *= Value;
	}

	void Divide(float Value)
	{
		X /= Value;
		Y /= Value;
		Z /= Value;
		W /= Value;
	}

	static Quaternion operator + (Quaternion Value)
	{
		return Value;
	}

	static Quaternion operator - (Quaternion Value)
	{
		return Quaternion(-Value.X, -Value.Y, -Value.Z, -Value.W);
	}

	static Quaternion operator + (Quaternion Value1, Quaternion Value2)
	{
		return Quaternion(Value1.X + Value2.X, Value1.Y + Value2.Y, Value1.Z + Value2.Z, Value1.W + Value2.W);
	}

	static Quaternion operator - (Quaternion Value1, Quaternion Value2)
	{
		return Quaternion(Value1.X - Value2.X, Value1.Y - Value2.Y, Value1.Z - Value2.Z, Value1.W - Value2.W);
	}

	static Quaternion operator * (Quaternion Value1, Quaternion Value2)
	{
		Quaternion Result;
		D3DXQuaternionMultiply((D3DXQUATERNION*)&Result, (D3DXQUATERNION*)&Value1, (D3DXQUATERNION*)&Value2);

		return Result;
	}

	static Quaternion operator * (Quaternion Value1, float Value2)
	{
		return Quaternion(Value1.X * Value2, Value1.Y * Value2, Value1.Z * Value2, Value1.W * Value2);
	}

	static Quaternion operator / (Quaternion Value1, float Value2)
	{
		float InvertedValue = 1.0f / Value2;
		return Quaternion(Value1.X * InvertedValue, Value1.Y * InvertedValue, Value1.Z * InvertedValue, Value1.W * InvertedValue);
	}

	static bool operator == (Quaternion Left, Quaternion Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Quaternion Left, Quaternion Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)X ^
			(int)Y ^
			(int)Z ^
			(int)W;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Quaternion Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(Quaternion::typeid)) == 0;
	}

	static bool Equals(Quaternion% Value1, Quaternion% Value2)
	{
		pin_ptr<Quaternion> PinnedValue1 = &Value1;
		pin_ptr<Quaternion> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Quaternion::typeid)) == 0;
	}
};