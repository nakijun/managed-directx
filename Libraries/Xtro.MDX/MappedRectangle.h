public value class MappedRectangle : IEquatable<MappedRectangle>
{
internal:
	inline void FromNative(DXGI_MAPPED_RECT* Native)
	{
		Bits = Native->pBits ? gcnew UnmanagedMemory(IntPtr(Native->pBits), 0) : nullptr;
		Pitch = Native->Pitch;
	}	
		
public:
	unsigned int Pitch;
	UnmanagedMemory^ Bits;

	static bool operator == (MappedRectangle Left, MappedRectangle Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (MappedRectangle Left, MappedRectangle Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Pitch ^
			(Bits == nullptr ? 1 : Bits->GetHashCode());
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(MappedRectangle Value)
	{
		pin_ptr<unsigned int> PinnedThis = &Pitch;
		pin_ptr<MappedRectangle> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(MappedRectangle::typeid)) == 0;
	}

	static bool Equals(MappedRectangle% Value1, MappedRectangle% Value2)
	{
		pin_ptr<MappedRectangle> PinnedValue1 = &Value1;
		pin_ptr<MappedRectangle> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(MappedRectangle::typeid)) == 0;
	}
};