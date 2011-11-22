public value class SharedResource : IEquatable<SharedResource>
{
public:
	IntPtr Handle;

	static bool operator == (SharedResource Left, SharedResource Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SharedResource Left, SharedResource Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Handle.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SharedResource Value)
	{
		pin_ptr<SharedResource> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(SharedResource::typeid)) == 0;
	}

	static bool Equals(SharedResource% Value1, SharedResource% Value2)
	{
		pin_ptr<SharedResource> PinnedValue1 = &Value1;
		pin_ptr<SharedResource> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SharedResource::typeid)) == 0;
	}
};