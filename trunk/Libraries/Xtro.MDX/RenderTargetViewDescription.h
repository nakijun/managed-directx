[StructLayout(LayoutKind::Explicit)]
public value class RenderTargetViewDescription : IEquatable<RenderTargetViewDescription>
{
public:
	[FieldOffset(0)]Format Format;
	[FieldOffset(4)]RenderTargetViewDimension ViewDimension;
	[FieldOffset(8)]BufferRTV Buffer;
	[FieldOffset(8)]Texture1D_RTV Texture1D;
	[FieldOffset(8)]Texture1D_ArrayRTV Texture1DArray;
	[FieldOffset(8)]Texture2D_RTV Texture2D;
	[FieldOffset(8)]Texture2DArrayRTV Texture2DArray;
	[FieldOffset(8)]Texture2D_MS_RTV Texture2DMS;
	[FieldOffset(8)]Texture2D_MS_ArrayRTV Texture2DMSArray;
	[FieldOffset(8)]Texture3D_RTV Texture3D;
		 
	static bool operator == (RenderTargetViewDescription Left, RenderTargetViewDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (RenderTargetViewDescription Left, RenderTargetViewDescription Right)
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

	virtual bool Equals(RenderTargetViewDescription Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(RenderTargetViewDescription::typeid)) == 0;
	}

	static bool Equals(RenderTargetViewDescription% Value1, RenderTargetViewDescription% Value2)
	{
		pin_ptr<RenderTargetViewDescription> PinnedValue1 = &Value1;
		pin_ptr<RenderTargetViewDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(RenderTargetViewDescription::typeid)) == 0;
	}
};