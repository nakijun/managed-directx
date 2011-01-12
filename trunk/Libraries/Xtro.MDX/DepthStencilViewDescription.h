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
		return
			Format == Value.Format &&
			ViewDimension == Value.ViewDimension &&
			Texture1DArray == Value.Texture1DArray;
	}

	static bool Equals(DepthStencilViewDescription% Value1, DepthStencilViewDescription% Value2)
	{
		return
			Value1.Format == Value2.Format && 
			Value1.ViewDimension == Value2.ViewDimension &&
			Value1.Texture1DArray == Value2.Texture1DArray;
	}
};