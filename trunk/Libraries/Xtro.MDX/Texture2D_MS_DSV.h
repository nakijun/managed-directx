namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	[StructLayout(LayoutKind::Sequential)]
	public value class Texture2D_MS_DSV : IEquatable<Texture2D_MS_DSV>
	{
	public:
		unsigned int UnusedField_NothingToDefine;

		Texture2D_MS_DSV(unsigned int UnusedField_NothingToDefine)
		{
			this->UnusedField_NothingToDefine = UnusedField_NothingToDefine;
		}

		static bool operator == (Texture2D_MS_DSV Left, Texture2D_MS_DSV Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (Texture2D_MS_DSV Left, Texture2D_MS_DSV Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return UnusedField_NothingToDefine;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(Texture2D_MS_DSV Value)
		{
			return UnusedField_NothingToDefine == Value.UnusedField_NothingToDefine;
		}

		static bool Equals(Texture2D_MS_DSV% Value1, Texture2D_MS_DSV% Value2)
		{
			return Value1.UnusedField_NothingToDefine == Value2.UnusedField_NothingToDefine;
		}
	};
}
}
}