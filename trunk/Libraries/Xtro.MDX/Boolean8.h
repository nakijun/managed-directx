[StructLayout(LayoutKind::Sequential, Size = 32)]
public value class Boolean8 : IEquatable<Boolean8>
{
private:
	bool Value0;
	bool Value1;
	bool Value2;
	bool Value3;
	bool Value4;
	bool Value5;
	bool Value6;
	bool Value7;

public:
	property bool default[int]
	{
		bool get(int Index)
		{
			switch (Index)
			{
			case 0 : return Value0;
			case 1 : return Value1;
			case 2 : return Value2;
			case 3 : return Value3;
			case 4 : return Value4;
			case 5 : return Value5;
			case 6 : return Value6;
			case 7 : return Value7;
			default : return 0;
			}
		}

		void set(int Index, bool Value)
		{
			switch (Index)
			{
			case 0 : Value0 = Value; break;
			case 1 : Value1 = Value; break;
			case 2 : Value2 = Value; break;
			case 3 : Value3 = Value; break;
			case 4 : Value4 = Value; break;
			case 5 : Value5 = Value; break;
			case 6 : Value6 = Value; break;
			case 7 : Value7 = Value; break;
			default : return;
			}
		}
	}

	Boolean8(array<bool>^ Booleans)
	{
		if (Booleans == nullptr || Booleans->Length < 8) return;

		pin_ptr<bool> PinnedBooleans = &Booleans[0];
		pin_ptr<bool> PinnedValue0 = &Value0;
		memcpy(PinnedValue0, PinnedBooleans, Marshal::SizeOf(Boolean8::typeid));
	}

	static explicit operator array<bool>^(Boolean8 Value)
	{
		array<bool>^ Booleans = gcnew array<bool>(8);
		
		pin_ptr<bool> PinnedBooleans = &Booleans[0];
		pin_ptr<Boolean8> PinnedValue = &Value;
		memcpy(PinnedBooleans, PinnedValue, Marshal::SizeOf(Boolean8::typeid));

		return Booleans;
	}

	static bool operator == (Boolean8 Left, Boolean8 Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Boolean8 Left, Boolean8 Right)
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

	virtual bool Equals(Boolean8 Value)
	{
		pin_ptr<bool> PinnedThis = &Value0;
		pin_ptr<Boolean8> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Boolean8::typeid)) == 0;
	}

	static bool Equals(Boolean8% Value1, Boolean8% Value2)
	{
		pin_ptr<Boolean8> PinnedValue1 = &Value1;
		pin_ptr<Boolean8> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Boolean8::typeid)) == 0;
	}
};