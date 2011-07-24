public value class Texture3D_RTV : IEquatable<Texture3D_RTV>
{
public:
	unsigned int MipSlice;
	unsigned int FirstWSlice;
	unsigned int WSize;

	static bool operator == (Texture3D_RTV Left, Texture3D_RTV Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture3D_RTV Left, Texture3D_RTV Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			MipSlice ^
			FirstWSlice ^
			WSize;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture3D_RTV Value)
	{
		pin_ptr<unsigned int> PinnedThis = &MipSlice;
		pin_ptr<Texture3D_RTV> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Texture3D_RTV::typeid)) == 0;
	}

	static bool Equals(Texture3D_RTV% Value1, Texture3D_RTV% Value2)
	{
		pin_ptr<Texture3D_RTV> PinnedValue1 = &Value1;
		pin_ptr<Texture3D_RTV> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture3D_RTV::typeid)) == 0;
	}
};