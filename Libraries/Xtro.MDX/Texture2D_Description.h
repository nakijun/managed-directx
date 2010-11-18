[StructLayout(LayoutKind::Sequential)]
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
	CPU_AccessFlag CPU_AccessFlags;
	ResourceMiscFlag MiscFlags;

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
			(int)CPU_AccessFlags ^
			(int)MiscFlags;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Texture2D_Description Value)
	{
		return
			Width == Value.Width &&
			Height == Value.Height &&
			MipLevels == Value.MipLevels &&
			ArraySize == Value.ArraySize &&
			Format == Value.Format &&
			SampleDescription == Value.SampleDescription &&
			Usage == Value.Usage &&
			BindFlags == Value.BindFlags &&
			CPU_AccessFlags == Value.CPU_AccessFlags &&
			MiscFlags == Value.MiscFlags;
	}

	static bool Equals(Texture2D_Description% Value1, Texture2D_Description% Value2)
	{
		return
			Value1.Width == Value2.Width &&
			Value1.Height == Value2.Height &&
			Value1.MipLevels == Value2.MipLevels &&
			Value1.ArraySize == Value2.ArraySize &&
			Value1.Format == Value2.Format &&
			Value1.SampleDescription == Value2.SampleDescription &&
			Value1.Usage == Value2.Usage &&
			Value1.BindFlags == Value2.BindFlags &&
			Value1.CPU_AccessFlags == Value2.CPU_AccessFlags &&
			Value1.MiscFlags == Value2.MiscFlags;
	}
};