[StructLayout(LayoutKind::Sequential, Size = 16)]
public value class Byte16 : IEquatable<Byte16>
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
	unsigned char Value8;
	unsigned char Value9;
	unsigned char Value10;
	unsigned char Value11;
	unsigned char Value12;
	unsigned char Value13;
	unsigned char Value14;
	unsigned char Value15;

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
			case 8 : return Value8;
			case 9 : return Value9;
			case 10 : return Value10;
			case 11 : return Value11;
			case 12 : return Value12;
			case 13 : return Value13;
			case 14 : return Value14;
			case 15 : return Value15;
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
			case 8 : Value8 = Value; break;
			case 9 : Value9 = Value; break;
			case 10 : Value10 = Value; break;
			case 11 : Value11 = Value; break;
			case 12 : Value12 = Value; break;
			case 13 : Value13 = Value; break;
			case 14 : Value14 = Value; break;
			case 15 : Value15 = Value; break;
			default : return;
			}
		}
	}

	Byte16(array<unsigned char>^ Bytes)
	{
		if (Bytes == nullptr || Bytes->Length < 16) return;

		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];
		pin_ptr<unsigned char> PinnedValue0 = &Value0;
		memcpy(PinnedValue0, PinnedBytes, Marshal::SizeOf(Byte16::typeid));
	}

	static explicit operator array<unsigned char>^(Byte16 Value)
	{
		array<unsigned char>^ Bytes = gcnew array<unsigned char>(16);
		
		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];
		pin_ptr<Byte16> PinnedValue = &Value;
		memcpy(PinnedBytes, PinnedValue, Marshal::SizeOf(Byte16::typeid));

		return Bytes;
	}

	static unsigned char operator == (Byte16 Left, Byte16 Right)
	{
		return Equals(Left, Right);
	}

	static unsigned char operator != (Byte16 Left, Byte16 Right)
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
			(int)Value7 ^
			(int)Value8 ^
			(int)Value9 ^
			(int)Value10 ^
			(int)Value11 ^
			(int)Value12 ^
			(int)Value13 ^
			(int)Value14 ^
			(int)Value15;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Byte16 Value)
	{
		pin_ptr<unsigned char> PinnedThis = &Value0;
		pin_ptr<Byte16> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Byte16::typeid)) == 0;
	}

	static bool Equals(Byte16% Value1, Byte16% Value2)
	{
		pin_ptr<Byte16> PinnedValue1 = &Value1;
		pin_ptr<Byte16> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Byte16::typeid)) == 0;
	}
};