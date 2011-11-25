public value class SendDescriptor : IEquatable<SendDescriptor>
{
internal:
	inline void ToNative(XAUDIO2_SEND_DESCRIPTOR* Native)
	{
		Native->Flags = Flags;
		Native->pOutputVoice = OutputVoice == nullptr ? 0 : OutputVoice->pVoice;
	}

public:
	unsigned int Flags;
	Voice^ OutputVoice;

	static bool operator == (SendDescriptor Left, SendDescriptor Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SendDescriptor Left, SendDescriptor Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return Flags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SendDescriptor Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(SendDescriptor::typeid)) == 0;
	}

	static bool Equals(SendDescriptor% Value1, SendDescriptor% Value2)
	{
		pin_ptr<SendDescriptor> PinnedValue1 = &Value1;
		pin_ptr<SendDescriptor> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SendDescriptor::typeid)) == 0;
	}
};