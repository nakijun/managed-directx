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
			return ((unsigned char*)&*this)[Index];
		}

		void set(int Index, unsigned char Value)
		{
			((unsigned char*)&*this)[Index] = Value;
		}
	}

	Byte16(array<unsigned char>^ Bytes)
	{
		if (Bytes == nullptr || Bytes->Length < 16) return;

		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];
		memcpy(&*this, PinnedBytes, Marshal::SizeOf(Byte16::typeid));
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
			Value0 ^
			Value1 ^
			Value2 ^
			Value3 ^
			Value4 ^
			Value5 ^
			Value6 ^
			Value7 ^
			Value8 ^
			Value9 ^
			Value10 ^
			Value11 ^
			Value12 ^
			Value13 ^
			Value14 ^
			Value15;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Byte16 Value)
	{
		pin_ptr<Byte16> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Byte16::typeid)) == 0;
	}

	static bool Equals(Byte16% Value1, Byte16% Value2)
	{
		pin_ptr<Byte16> PinnedValue1 = &Value1;
		pin_ptr<Byte16> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Byte16::typeid)) == 0;
	}
};