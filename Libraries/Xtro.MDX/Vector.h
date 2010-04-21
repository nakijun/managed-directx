namespace Xtro
{
namespace MDX
{
namespace Direct3DX10
{
	[StructLayout(LayoutKind::Sequential)]
	public value class Vector : IEquatable<Vector>
	{
	public:
		float X;
		float Y;
		float Z;

		Vector(float X, float Y, float Z)
		{
			this->X = X;
			this->Y = Y;
			this->Z = Z;
		}

		static bool operator == (Vector Left, Vector Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (Vector Left, Vector Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				(int)X ^
				(int)Y ^
				(int)Z;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(Vector Value)
		{
			return
				X == Value.X &&
				Y == Value.Y &&
				Z == Value.Z;
		}

		static bool Equals(Vector% Value1, Vector% Value2)
		{
			return
				Value1.X == Value2.X && 
				Value1.Y == Value2.Y && 
				Value1.Z == Value2.Z;
		}
	};
}
}
}