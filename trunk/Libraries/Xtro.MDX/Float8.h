public value class	Float8 : IEquatable<Float8>
{
private:
	float Value0;
	float Value1;
	float Value2;
	float Value3;
	float Value4;
	float Value5;
	float Value6;
	float Value7;

public:
	property float default[int]
	{
		float get(int Index)
		{
			pin_ptr<float> PinnedThis = &Value0;
			return ((float*)PinnedThis)[Index];
		}

		void set(int Index, float Value)
		{
			pin_ptr<float> PinnedThis = &Value0;
			((float*)PinnedThis)[Index] = Value;
		}
	}

	Float8(array<float>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 8) return;

		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<float> PinnedThis = &Value0;
		memcpy(PinnedThis, PinnedFloats, Marshal::SizeOf(Float8::typeid));
	}

	static explicit operator array<float>^(Float8 Value)
	{
		array<float>^ Floats = gcnew array<float>(8);
		
		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<Float8> PinnedValue = &Value;
		memcpy(PinnedFloats, PinnedValue, Marshal::SizeOf(Float8::typeid));

		return Floats;
	}

	static bool operator == (Float8 Left, Float8 Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Float8 Left, Float8 Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Value0 ^
			(int)Value1 ^
			(int)Value2 ^
			(int)Value3 ^
			(int)Value4 ^
			(int)Value5 ^
			(int)Value6 ^
			(int)Value7;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Float8 Value)
	{
		pin_ptr<float> PinnedThis = &Value0;
		pin_ptr<Float8> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Float8::typeid)) == 0;
	}

	static bool Equals(Float8% Value1, Float8% Value2)
	{
		pin_ptr<Float8> PinnedValue1 = &Value1;
		pin_ptr<Float8> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Float8::typeid)) == 0;
	}
};