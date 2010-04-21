namespace Xtro
{
namespace MDX
{
namespace Direct3DX10
{
	[StructLayout(LayoutKind::Sequential)]
	public value class Float16 : IEquatable<Float16>
	{
	internal:
		WORD Value;

	public:
		Float16(float Float)
		{
			Value = (WORD)Float;
		}

		static explicit operator float(Float16 Value)
		{
			return Value.Value;
		}

		static bool operator == (Float16 Left, Float16 Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (Float16 Left, Float16 Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return (int)Value;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(Float16 Value)
		{
			return this->Value == Value.Value;
		}

		static bool Equals(Float16% Value1, Float16% Value2)
		{
			return Value1.Value == Value2.Value;
		}
	};
}
}
}