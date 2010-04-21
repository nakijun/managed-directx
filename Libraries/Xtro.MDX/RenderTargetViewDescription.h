namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	[StructLayout(LayoutKind::Explicit)]
	public value class RenderTargetViewDescription : IEquatable<RenderTargetViewDescription>
	{
	public:
		[FieldOffset(0)]Format Format;
		[FieldOffset(4)]RTV_Dimension ViewDimension;
		[FieldOffset(8)]BufferRTV Buffer;
		[FieldOffset(8)]Texture1D_RTV Texture1D;
		[FieldOffset(8)]Texture1D_ArrayRTV Texture1DArray;
		[FieldOffset(8)]Texture2D_RTV Texture2D;
		[FieldOffset(8)]Texture2DArrayRTV Texture2DArray;
		[FieldOffset(8)]Texture2D_MS_RTV Texture2DMS;
		[FieldOffset(8)]Texture2D_MS_ArrayRTV Texture2DMSArray;
		[FieldOffset(8)]Texture3D_RTV Texture3D;
		 
		RenderTargetViewDescription(Xtro::MDX::DXGI::Format Format, RTV_Dimension ViewDimension)
		{
			this->Format = Format;
			this->ViewDimension = ViewDimension;
		}

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
			return
				Format == Value.Format &&
				ViewDimension == Value.ViewDimension &&
				Texture1DArray == Value.Texture1DArray;
		}

		static bool Equals(RenderTargetViewDescription% Value1, RenderTargetViewDescription% Value2)
		{
			return
				Value1.Format == Value2.Format && 
				Value1.ViewDimension == Value2.ViewDimension &&
				Value1.Texture1DArray == Value2.Texture1DArray;
		}
	};
}
}
}