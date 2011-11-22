public value class Texture2D_Description : IEquatable<Texture2D_Description>
{
public:	   
	unsigned int Width;
	unsigned int Height;
	unsigned int MipLevels;
	unsigned int ArraySize;
	Format Format;
	SampleDescription SampleDescription;
	Usage Usage;
	BindFlag BindFlags;
	CpuAccessFlag CpuAccessFlags;
	ResourceMiscellaneousFlag MiscellaneousFlags;

	static bool operator == (Texture2D_Description Left, Texture2D_Description Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture2D_Description Left, Texture2D_Description Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Width ^
			Height ^
			MipLevels ^
			ArraySize ^
			(int)Format ^
			SampleDescription.GetHashCode() ^
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

	virtual bool Equals(Texture2D_Description Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(Texture2D_Description::typeid)) == 0;
	}

	static bool Equals(Texture2D_Description% Value1, Texture2D_Description% Value2)
	{
		pin_ptr<Texture2D_Description> PinnedValue1 = &Value1;
		pin_ptr<Texture2D_Description> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture2D_Description::typeid)) == 0;
	}
};