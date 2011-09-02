public value class Byte2 : IEquatable<Byte2>
{
private:
	unsigned char Value0;
	unsigned char Value1;

public:
	property unsigned char default[int]
	{
		unsigned char get(int Index)
		{
			switch (Index)
			{
			case 0 : return Value0;
			case 1 : return Value1;
			default : return 0;
			}
		}

		void set(int Index, unsigned char Value)
		{
			switch (Index)
			{
			case 0 : Value0 = Value; break;
			case 1 : Value1 = Value; break;
			default : return;
			}
		}
	}

	Byte2(array<unsigned char>^ Bytes)
	{
		if (Bytes == nullptr || Bytes->Length < 2) return;

		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];
		pin_ptr<unsigned char> PinnedValue0 = &Value0;
		memcpy(PinnedValue0, PinnedBytes, Marshal::SizeOf(Byte2::typeid));
	}

	static explicit operator array<unsigned char>^(Byte2 Value)
	{
		array<unsigned char>^ Bytes = gcnew array<unsigned char>(2);
		
		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];
		pin_ptr<Byte2> PinnedValue = &Value;
		memcpy(PinnedBytes, PinnedValue, Marshal::SizeOf(Byte2::typeid));

		return Bytes;
	}

	static unsigned char operator == (Byte2 Left, Byte2 Right)
	{
		return Equals(Left, Right);
	}

	static unsigned char operator != (Byte2 Left, Byte2 Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Value0 ^
			(int)Value1;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Byte2 Value)
	{
		pin_ptr<unsigned char> PinnedThis = &Value0;
		pin_ptr<Byte2> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Byte2::typeid)) == 0;
	}

	static bool Equals(Byte2% Value1, Byte2% Value2)
	{
		pin_ptr<Byte2> PinnedValue1 = &Value1;
		pin_ptr<Byte2> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Byte2::typeid)) == 0;
	}
};