namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	[StructLayout(LayoutKind::Sequential)]
	public value class Texture3D_RTV : IEquatable<Texture3D_RTV>
	{
	public:
		unsigned int MipSlice;
		unsigned int FirstWSlice;
		unsigned int WSize;

		Texture3D_RTV(unsigned int MipSlice, unsigned int FirstWSlice, unsigned int WSize)
		{
			this->MipSlice = MipSlice;
			this->FirstWSlice = FirstWSlice;
			this->WSize = WSize;
		}

		static bool operator == (Texture3D_RTV Left, Texture3D_RTV Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (Texture3D_RTV Left, Texture3D_RTV Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				MipSlice ^
				FirstWSlice ^
				WSize;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(Texture3D_RTV Value)
		{
			return
				MipSlice == Value.MipSlice &&
				FirstWSlice == Value.FirstWSlice &&
				WSize == Value.WSize;
		}

		static bool Equals(Texture3D_RTV% Value1, Texture3D_RTV% Value2)
		{
			return
				Value1.MipSlice == Value2.MipSlice && 
				Value1.FirstWSlice == Value2.FirstWSlice && 
				Value1.WSize == Value2.WSize;
		}
	};
}
}
}