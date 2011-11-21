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
			return ((unsigned char*)&*this)[Index];
		}

		void set(int Index, unsigned char Value)
		{
			((unsigned char*)&*this)[Index] = Value;
		}
	}

	Byte2(array<unsigned char>^ Bytes)
	{
		if (Bytes == nullptr || Bytes->Length < 2) return;

		pin_ptr<unsigned char> PinnedBytes = &Bytes[0];
		memcpy(&*this, PinnedBytes, Marshal::SizeOf(Byte2::typeid));
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
			Value0 ^
			Value1;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Byte2 Value)
	{
		pin_ptr<Byte2> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Byte2::typeid)) == 0;
	}

	static bool Equals(Byte2% Value1, Byte2% Value2)
	{
		pin_ptr<Byte2> PinnedValue1 = &Value1;
		pin_ptr<Byte2> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Byte2::typeid)) == 0;
	}
};