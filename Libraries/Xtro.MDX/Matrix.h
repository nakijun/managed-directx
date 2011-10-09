public value class Matrix : IEquatable<Matrix>
{
public:
	float Value11;
	float Value12;
	float Value13;
	float Value14;
	float Value21;
	float Value22;
	float Value23;
	float Value24;
	float Value31;
	float Value32;
	float Value33;
	float Value34;
	float Value41;
	float Value42;
	float Value43;
	float Value44;

	property float default[unsigned int, unsigned int]
	{
		float get(unsigned int Index1, unsigned int Index2)
		{
			pin_ptr<float> PinnedThis = &Value11;
			return ((float**)PinnedThis)[Index1][Index2];
		}

		void set(unsigned int Index1, unsigned int Index2, float Value)
		{
			pin_ptr<float> PinnedThis = &Value11;
			((float**)PinnedThis)[Index1][Index2] = Value;
		}
	}

	Matrix(array<float>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 16) return;

		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<float> PinnedThis = &Value11;
		memcpy(PinnedThis, PinnedFloats, Marshal::SizeOf(Matrix::typeid));
	}
		
	Matrix(array<Float16bit>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 16) return;

		Value11 = Floats[0].Value;
		Value12 = Floats[1].Value;
		Value13 = Floats[2].Value;
		Value14 = Floats[3].Value;
		Value21 = Floats[4].Value;
		Value22 = Floats[5].Value;
		Value23 = Floats[6].Value;
		Value24 = Floats[7].Value;
		Value31 = Floats[8].Value;
		Value32 = Floats[9].Value;
		Value33 = Floats[10].Value;
		Value34 = Floats[11].Value;
		Value41 = Floats[12].Value;
		Value42 = Floats[13].Value;
		Value43 = Floats[14].Value;
		Value44 = Floats[15].Value;
	}

	Matrix(float F11, float F12, float F13, float F14,
			float F21, float F22, float F23, float F24,
			float F31, float F32, float F33, float F34,
			float F41, float F42, float F43, float F44)
	{
			Value11 = F11;
			Value12 = F12;
			Value13 = F13;
			Value14 = F14;
			Value11 = F21;
			Value12 = F22;
			Value13 = F23;
			Value14 = F24;
			Value11 = F31;
			Value12 = F32;
			Value13 = F33;
			Value14 = F34;
			Value11 = F41;
			Value12 = F42;
			Value13 = F43;
			Value44 = F44;
	}

	static explicit operator array<float>^(Matrix Value)
	{
		return gcnew array<float>(16) { Value.Value11, Value.Value12, Value.Value13, Value.Value14,
										Value.Value21, Value.Value22, Value.Value23, Value.Value24,
										Value.Value31, Value.Value32, Value.Value33, Value.Value34,
										Value.Value41, Value.Value42, Value.Value43, Value.Value44 };
	}

	void Multiply(Matrix Value)
	{
		pin_ptr<Matrix> PinnedThis = this;
		pin_ptr<Matrix> PinnedValue = &Value;
		D3DXMatrixMultiply((D3DXMATRIX*)PinnedThis, (D3DXMATRIX*)PinnedThis, (D3DXMATRIX*)PinnedValue);
	}

	void Add(Matrix Value)
	{
		Value11 += Value.Value11;
		Value12 += Value.Value12;
		Value13 += Value.Value13;
		Value14 += Value.Value14;
		Value21 += Value.Value21;
		Value22 += Value.Value22;
		Value23 += Value.Value23;
		Value24 += Value.Value24;
		Value31 += Value.Value31;
		Value32 += Value.Value32;
		Value33 += Value.Value33;
		Value34 += Value.Value34;
		Value41 += Value.Value41;
		Value42 += Value.Value42;
		Value43 += Value.Value43;
		Value44 += Value.Value44;
	}

	void Subtract(Matrix Value)
	{
		Value11 -= Value.Value11;
		Value12 -= Value.Value12;
		Value13 -= Value.Value13;
		Value14 -= Value.Value14;
		Value21 -= Value.Value21;
		Value22 -= Value.Value22;
		Value23 -= Value.Value23;
		Value24 -= Value.Value24;
		Value31 -= Value.Value31;
		Value32 -= Value.Value32;
		Value33 -= Value.Value33;
		Value34 -= Value.Value34;
		Value41 -= Value.Value41;
		Value42 -= Value.Value42;
		Value43 -= Value.Value43;
		Value44 -= Value.Value44;
	}

	void Multiply(float Value)
	{
		Value11 *= Value;
		Value12 *= Value;
		Value13 *= Value;
		Value14 *= Value;
		Value21 *= Value;
		Value22 *= Value;
		Value23 *= Value;
		Value24 *= Value;
		Value31 *= Value;
		Value32 *= Value;
		Value33 *= Value;
		Value34 *= Value;
		Value41 *= Value;
		Value42 *= Value;
		Value43 *= Value;
		Value44 *= Value;
	}

	void Divide(float Value)
	{
		float InvertedValue = 1.0f / Value;
		Value11 *= InvertedValue;
		Value12 *= InvertedValue;
		Value13 *= InvertedValue;
		Value14 *= InvertedValue;
		Value21 *= InvertedValue;
		Value22 *= InvertedValue;
		Value23 *= InvertedValue;
		Value24 *= InvertedValue;
		Value31 *= InvertedValue;
		Value32 *= InvertedValue;
		Value33 *= InvertedValue;
		Value34 *= InvertedValue;
		Value41 *= InvertedValue;
		Value42 *= InvertedValue;
		Value43 *= InvertedValue;
		Value44 *= InvertedValue;
	}

	static Matrix operator + (Matrix Value)
	{
		return Value;
	}

	static Matrix operator - (Matrix Value)
	{
		return Matrix(-Value.Value11, -Value.Value12, -Value.Value13, -Value.Value14,
						-Value.Value21, -Value.Value22, -Value.Value23, -Value.Value24,
						-Value.Value31, -Value.Value32, -Value.Value33, -Value.Value34,
						-Value.Value41, -Value.Value42, -Value.Value43, -Value.Value44);
	}

	static Matrix operator * (Matrix Value1, Matrix Value2)
	{
		Matrix Result;
		pin_ptr<Matrix> PinnedResult = &Result;

		pin_ptr<Matrix> PinnedValue1 = &Value1;
		pin_ptr<Matrix> PinnedValue2 = &Value2;
		D3DXMatrixMultiply((D3DXMATRIX*)PinnedResult, (D3DXMATRIX*)PinnedValue1, (D3DXMATRIX*)PinnedValue2);

		return Result;
	}

	static Matrix operator + (Matrix Value1, Matrix Value2)
	{
		return Matrix(Value1.Value11 + Value2.Value11, Value1.Value12 + Value2.Value12, Value1.Value13 + Value2.Value13, Value1.Value14 + Value2.Value14,
						Value1.Value21 + Value2.Value21, Value1.Value22 + Value2.Value22, Value1.Value23 + Value2.Value23, Value1.Value24 + Value2.Value24,
						Value1.Value31 + Value2.Value31, Value1.Value32 + Value2.Value32, Value1.Value33 + Value2.Value33, Value1.Value34 + Value2.Value34,
						Value1.Value41 + Value2.Value41, Value1.Value42 + Value2.Value42, Value1.Value43 + Value2.Value43, Value1.Value44 + Value2.Value44);
	}

	static Matrix operator - (Matrix Value1, Matrix Value2)
	{
		return Matrix(Value1.Value11 - Value2.Value11, Value1.Value12 - Value2.Value12, Value1.Value13 - Value2.Value13, Value1.Value14 - Value2.Value14,
						Value1.Value21 - Value2.Value21, Value1.Value22 - Value2.Value22, Value1.Value23 - Value2.Value23, Value1.Value24 - Value2.Value24,
						Value1.Value31 - Value2.Value31, Value1.Value32 - Value2.Value32, Value1.Value33 - Value2.Value33, Value1.Value34 - Value2.Value34,
						Value1.Value41 - Value2.Value41, Value1.Value42 - Value2.Value42, Value1.Value43 - Value2.Value43, Value1.Value44 - Value2.Value44);
	}

	static Matrix operator * (Matrix Value1, float Value2)
	{
		return Matrix(Value1.Value11 * Value2, Value1.Value12 * Value2, Value1.Value13 * Value2, Value1.Value14 * Value2,
						Value1.Value21 * Value2, Value1.Value22 * Value2, Value1.Value23 * Value2, Value1.Value24 * Value2,
						Value1.Value31 * Value2, Value1.Value32 * Value2, Value1.Value33 * Value2, Value1.Value34 * Value2,
						Value1.Value41 * Value2, Value1.Value42 * Value2, Value1.Value43 * Value2, Value1.Value44 * Value2);
	}

	static Matrix operator / (Matrix Value1, float Value2)
	{
		float InvertedValue = 1.0f / Value2;
		return Matrix(Value1.Value11 * InvertedValue, Value1.Value12 * InvertedValue, Value1.Value13 * InvertedValue, Value1.Value14 * InvertedValue,
						Value1.Value21 * InvertedValue, Value1.Value22 * InvertedValue, Value1.Value23 * InvertedValue, Value1.Value24 * InvertedValue,
						Value1.Value31 * InvertedValue, Value1.Value32 * InvertedValue, Value1.Value33 * InvertedValue, Value1.Value34 * InvertedValue,
						Value1.Value41 * InvertedValue, Value1.Value42 * InvertedValue, Value1.Value43 * InvertedValue, Value1.Value44 * InvertedValue);
	}

	static bool operator == (Matrix Left, Matrix Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Matrix Left, Matrix Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Value11 ^
			(int)Value12 ^
			(int)Value13 ^
			(int)Value14 ^
			(int)Value21 ^
			(int)Value22 ^
			(int)Value23 ^
			(int)Value24 ^
			(int)Value31 ^
			(int)Value32 ^
			(int)Value33 ^
			(int)Value34 ^
			(int)Value41 ^
			(int)Value42 ^
			(int)Value43 ^
			(int)Value44;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Matrix Value)
	{
		pin_ptr<float> PinnedThis = &Value11;
		pin_ptr<Matrix> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Matrix::typeid)) == 0;
	}

	static bool Equals(Matrix% Value1, Matrix% Value2)
	{
		pin_ptr<Matrix> PinnedValue1 = &Value1;
		pin_ptr<Matrix> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Matrix::typeid)) == 0;
	}
};