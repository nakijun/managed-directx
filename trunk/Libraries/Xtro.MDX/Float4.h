public value class Float4 : IEquatable<Float4>
{
private:
	float Value0;
	float Value1;
	float Value2;
	float Value3;

public:
	property float default[int]
	{
		float get(int Index)
		{
			return ((float*)&*this)[Index];
		}

		void set(int Index, float Value)
		{
			((float*)&*this)[Index] = Value;
		}
	}

	Float4(array<float>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 4) return;

		pin_ptr<float> PinnedFloats = &Floats[0];
		memcpy(&*this, PinnedFloats, Marshal::SizeOf(Float4::typeid));
	}

	static explicit operator array<float>^(Float4 Value)
	{
		array<float>^ Floats = gcnew array<float>(4);
		
		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<Float4> PinnedValue = &Value;
		memcpy(PinnedFloats, PinnedValue, Marshal::SizeOf(Float4::typeid));

		return Floats;
	}

	static bool operator == (Float4 Left, Float4 Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Float4 Left, Float4 Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Value0 ^
			(int)Value1 ^
			(int)Value2 ^
			(int)Value3;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Float4 Value)
	{
		pin_ptr<Float4> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Float4::typeid)) == 0;
	}

	static bool Equals(Float4% Value1, Float4% Value2)
	{
		pin_ptr<Float4> PinnedValue1 = &Value1;
		pin_ptr<Float4> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Float4::typeid)) == 0;
	}
};