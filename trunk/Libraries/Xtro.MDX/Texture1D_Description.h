public value class Texture1D_Description : IEquatable<Texture1D_Description>
{
public:	   
	unsigned int Width;
	unsigned int MipLevels;
	unsigned int ArraySize;
	Format Format;
	Usage Usage;
	BindFlag BindFlags;
	CpuAccessFlag CpuAccessFlags;
	ResourceMiscellaneousFlag MiscellaneousFlags;

	static bool operator == (Texture1D_Description Left, Texture1D_Description Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture1D_Description Left, Texture1D_Description Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Width ^
			MipLevels ^
			ArraySize ^
			(int)Format ^
			(int)Usage ^
			(int)BindFlags ^
			(int)CpuAccessFlags ^
			(int)MiscellaneousFlags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture1D_Description Value)
	{
		pin_ptr<Texture1D_Description> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(Texture1D_Description::typeid)) == 0;
	}

	static bool Equals(Texture1D_Description% Value1, Texture1D_Description% Value2)
	{
		pin_ptr<Texture1D_Description> PinnedValue1 = &Value1;
		pin_ptr<Texture1D_Description> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture1D_Description::typeid)) == 0;
	}
};