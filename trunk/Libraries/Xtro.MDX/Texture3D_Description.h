public value class Texture3D_Description : IEquatable<Texture3D_Description>
{
public:	   
	unsigned int Width;
	unsigned int Height;
	unsigned int Depth;
	unsigned int MipLevels;
	Format Format;
	Usage Usage;
	BindFlag BindFlags;
	CpuAccessFlag CpuAccessFlags;
	ResourceMiscellaneousFlag MiscellaneousFlags;

	static bool operator == (Texture3D_Description Left, Texture3D_Description Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Texture3D_Description Left, Texture3D_Description Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Width ^
			Height ^
			Depth ^
			MipLevels ^
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

	virtual bool Equals(Texture3D_Description Value)
	{
		pin_ptr<unsigned int> PinnedThis = &Width;
		pin_ptr<Texture3D_Description> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(Texture3D_Description::typeid)) == 0;
	}

	static bool Equals(Texture3D_Description% Value1, Texture3D_Description% Value2)
	{
		pin_ptr<Texture3D_Description> PinnedValue1 = &Value1;
		pin_ptr<Texture3D_Description> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(Texture3D_Description::typeid)) == 0;
	}
};