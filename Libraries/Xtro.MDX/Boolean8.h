public value class Boolean8 : IEquatable<Boolean8>
{
private:
	int Value0;
	int Value1;
	int Value2;
	int Value3;
	int Value4;
	int Value5;
	int Value6;
	int Value7;

public:
	property bool default[int]
	{
		bool get(int Index)
		{
			return ((int*)&*this)[Index];
		}

		void set(int Index, bool Value)
		{
			((int*)&*this)[Index] = Value;
		}
	}

	Boolean8(array<bool>^ Booleans)
	{
		if (Booleans == nullptr || Booleans->Length < 8) return;

		pin_ptr<bool> PinnedBooleans = &Booleans[0];
		memcpy(&*this, PinnedBooleans, Marshal::SizeOf(Boolean8::typeid));
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
			Value0 ^
			Value1 ^
			Value2 ^
			Value3 ^
			Value4 ^
			Value5 ^
			Value6 ^
			Value7;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Boolean8 Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(Boolean8::typeid)) == 0;
	}

	static bool Equals(Boolean8% Value1, Boolean8% Value2)
	{
		pin_ptr<Boolean8> PinnedValue1 = &Value1;
		pin_ptr<Boolean8> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Boolean8::typeid)) == 0;
	}
};