public value class Byte8 : IEquatable<Byte8>
{
private:
	unsigned char Value0;
	unsigned char Value1;
	unsigned char Value2;
	unsigned char Value3;
	unsigned char Value4;
	unsigned char Value5;
	unsigned char Value6;
	unsigned char Value7;

public:
	property unsigned char default[int]
	{
		unsigned char get(int Index)
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

		void set(int Index, unsigned char Value)
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

	Byte8(array<unsigned char>^ Bytes)
	{
		if (Bytes == nullptr || Bytes->Length < 8) return;

		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];
		pin_ptr<unsigned char> PinnedValue0 = &Value0;
		memcpy(PinnedValue0, PinnedBytes, Marshal::SizeOf(Byte8::typeid));
	}

	static explicit operator array<unsigned char>^(Byte8 Value)
	{
		array<unsigned char>^ Bytes = gcnew array<unsigned char>(8);
		
		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];
		pin_ptr<Byte8> PinnedValue = &Value;
		memcpy(PinnedBytes, PinnedValue, Marshal::SizeOf(Byte8::typeid));

		return Bytes;
	}

	static unsigned char operator == (Byte8 Left, Byte8 Right)
	{
		return Equals(Left, Right);
	}

	static unsigned char operator != (Byte8 Left, Byte8 Right)
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

	virtual bool Equals(Byte8 Value)
	{
		pin_ptr<unsigned char> PinnedThis = &Value0;
		pin_ptr<Byte8> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Byte8::typeid)) == 0;
	}

	static bool Equals(Byte8% Value1, Byte8% Value2)
	{
		pin_ptr<Byte8> PinnedValue1 = &Value1;
		pin_ptr<Byte8> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Byte8::typeid)) == 0;
	}
};