namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	[StructLayout(LayoutKind::Sequential)]
	public value class Texture1D_RTV : IEquatable<Texture1D_RTV>
	{
	public:
		unsigned int MipSlice;

		Texture1D_RTV(unsigned int MipSlice)
		{
			this->MipSlice = MipSlice;
		}

		static bool operator == (Texture1D_RTV Left, Texture1D_RTV Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (Texture1D_RTV Left, Texture1D_RTV Right)
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

		virtual bool Equals(Texture1D_RTV Value)
		{
			return MipSlice == Value.MipSlice;
		}

		static bool Equals(Texture1D_RTV% Value1, Texture1D_RTV% Value2)
		{
			return Value1.MipSlice == Value2.MipSlice;
		}
	};
}
}
}