[StructLayout(LayoutKind::Sequential)]
public value class Float4 : IEquatable<Float4>
{
internal:
	float Value1;
	float Value2;
	float Value3;
	float Value4;

public:
	property float default[int]
	{
		float get(int Index)
		{
			switch (Index)
			{
			case 0 : return Value1;
			case 1 : return Value2;
			case 2 : return Value3;
			case 3 : return Value4;
			default : return 0;
			}
		}

		void set(int Index, float Value)
		{
			switch (Index)
			{
			case 0 : Value1 = Value; break;
			case 1 : Value2 = Value; break;
			case 2 : Value3 = Value; break;
			case 3 : Value4 = Value; break;
			default : return;
			}
		}
	}

	Float4(array<float>^ Floats)
	{
		if (Floats == nullptr || Floats->Length < 4) return;

		pin_ptr<float> PinnedFloats = &Floats[0];
		pin_ptr<float> PinnedValue1 = &Value1;
		memcpy(PinnedValue1, PinnedFloats, sizeof(float) * 4);
	}

	Float4(float Value1, float Value2, float Value3, float Value4)
	{
		this->Value1 = Value1;
		this->Value2 = Value2;
		this->Value3 = Value3;
		this->Value4 = Value4;
	}

	static explicit operator array<float>^(Float4 Value)
	{
		return gcnew array<float>(4) { Value.Value1, Value.Value2, Value.Value3, Value.Value4 };
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
			(int)Value1 ^
			(int)Value2 ^
			(int)Value3 ^
			(int)Value4;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Float4 Value)
	{
		return
			Value1 == Value.Value1 &&
			Value2 == Value.Value2 &&
			Value3 == Value.Value3 &&
			Value4 == Value.Value4;
	}

	static bool Equals(Float4% Value1, Float4% Value2)
	{
		return
			Value1.Value1 == Value2.Value1 && 
			Value1.Value2 == Value2.Value2 && 
			Value1.Value3 == Value2.Value3 && 
			Value1.Value4 == Value2.Value4;
	}
};