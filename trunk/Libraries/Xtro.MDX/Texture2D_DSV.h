namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	[StructLayout(LayoutKind::Sequential)]
	public value class Texture2D_DSV : IEquatable<Texture2D_DSV>
	{
	public:
		unsigned int MipSlice;

		Texture2D_DSV(unsigned int MipSlice)
		{
			this->MipSlice = MipSlice;
		}

		static bool operator == (Texture2D_DSV Left, Texture2D_DSV Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (Texture2D_DSV Left, Texture2D_DSV Right)
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

		virtual bool Equals(Texture2D_DSV Value)
		{
			return MipSlice == Value.MipSlice;
		}

		static bool Equals(Texture2D_DSV% Value1, Texture2D_DSV% Value2)
		{
			return Value1.MipSlice == Value2.MipSlice;
		}
	};
}
}
}