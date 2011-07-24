[StructLayout(LayoutKind::Explicit)]
public value class ShaderResourceViewDescription : IEquatable<ShaderResourceViewDescription>
{
public:
	[FieldOffset(0)]Format Format;
	[FieldOffset(4)]ShaderResourceViewDimension ViewDimension;
	[FieldOffset(8)]BufferSRV Buffer;
	[FieldOffset(8)]Texture1D_SRV Texture1D;
	[FieldOffset(8)]Texture1D_ArraySRV Texture1DArray;
	[FieldOffset(8)]Texture2D_SRV Texture2D;
	[FieldOffset(8)]Texture2DArraySRV Texture2DArray;
	[FieldOffset(8)]Texture2D_MS_SRV Texture2DMS;
	[FieldOffset(8)]Texture2D_MS_ArraySRV Texture2DMSArray;
	[FieldOffset(8)]Texture3D_SRV Texture3D;
	[FieldOffset(8)]TextureCubeSRV TextureCube;
		 
	static bool operator == (ShaderResourceViewDescription Left, ShaderResourceViewDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (ShaderResourceViewDescription Left, ShaderResourceViewDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Format ^
			(int)ViewDimension ^
			Texture1DArray.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(ShaderResourceViewDescription Value)
	{
		pin_ptr<Xtro::MDX::Direct3D10::Format> PinnedThis = &Format;
		pin_ptr<ShaderResourceViewDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(ShaderResourceViewDescription::typeid)) == 0;
	}

	static bool Equals(ShaderResourceViewDescription% Value1, ShaderResourceViewDescription% Value2)
	{
		pin_ptr<ShaderResourceViewDescription> PinnedValue1 = &Value1;
		pin_ptr<ShaderResourceViewDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(ShaderResourceViewDescription::typeid)) == 0;
	}
};