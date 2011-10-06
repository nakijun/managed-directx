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
			pin_ptr<unsigned char> PinnedThis = &Value0;
			return ((unsigned char*)PinnedThis)[Index];
		}

		void set(int Index, unsigned char Value)
		{
			pin_ptr<unsigned char> PinnedThis = &Value0;
			((unsigned char*)PinnedThis)[Index] = Value;
		}
	}

	Byte8(array<unsigned char>^ Bytes)
	{
		if (Bytes == nullptr || Bytes->Length < 8) return;

		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];
		pin_ptr<unsigned char> PinnedThis = &Value0;
		memcpy(PinnedThis, PinnedBytes, Marshal::SizeOf(Byte8::typeid));
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