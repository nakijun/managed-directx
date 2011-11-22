[StructLayout(LayoutKind::Explicit)]
public value class DepthStencilViewDescription : IEquatable<DepthStencilViewDescription>
{
public:
	[FieldOffset(0)]Format Format;
	[FieldOffset(4)]DepthStencilViewDimension ViewDimension;
	[FieldOffset(8)]Texture1D_DSV Texture1D;
	[FieldOffset(8)]Texture1D_ArrayDSV Texture1DArray;
	[FieldOffset(8)]Texture2D_DSV Texture2D;
	[FieldOffset(8)]Texture2DArrayDSV Texture2DArray;
	[FieldOffset(8)]Texture2D_MS_DSV Texture2DMS;
	[FieldOffset(8)]Texture2D_MS_ArrayDSV Texture2DMSArray;
		 
	static bool operator == (DepthStencilViewDescription Left, DepthStencilViewDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (DepthStencilViewDescription Left, DepthStencilViewDescription Right)
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

	virtual bool Equals(DepthStencilViewDescription Value)
	{
		pin_ptr<DepthStencilViewDescription> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(DepthStencilViewDescription::typeid)) == 0;
	}

	static bool Equals(DepthStencilViewDescription% Value1, DepthStencilViewDescription% Value2)
	{
		pin_ptr<DepthStencilViewDescription> PinnedValue1 = &Value1;
		pin_ptr<DepthStencilViewDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(DepthStencilViewDescription::typeid)) == 0;
	}
};