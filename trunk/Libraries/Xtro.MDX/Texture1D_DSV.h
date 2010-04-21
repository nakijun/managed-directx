namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	[StructLayout(LayoutKind::Sequential)]
	public value class Texture1D_DSV : IEquatable<Texture1D_DSV>
	{
	public:
		unsigned int MipSlice;

		Texture1D_DSV(unsigned int MipSlice)
		{
			this->MipSlice = MipSlice;
		}

		static bool operator == (Texture1D_DSV Left, Texture1D_DSV Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (Texture1D_DSV Left, Texture1D_DSV Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return MipSlice;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(Texture1D_DSV Value)
		{
			return MipSlice == Value.MipSlice;
		}

		static bool Equals(Texture1D_DSV% Value1, Texture1D_DSV% Value2)
		{
			return Value1.MipSlice == Value2.MipSlice;
		}
	};
}
}
}