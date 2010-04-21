namespace Xtro
{
namespace MDX
{
namespace Direct3DX10
{
	[StructLayout(LayoutKind::Sequential)]
	public value class Vector4 : IEquatable<Vector4>
	{
	public:
		float X;
		float Y;
		float Z;
		float W;

		Vector4(array<float>^ Floats)
		{
			if (Floats == nullptr || Floats->Length < 4) return;

			pin_ptr<float> PinnedFloats = &Floats[0];
			pin_ptr<float> PinnedX = &X;
			memcpy(PinnedX, PinnedFloats, sizeof(float) * 4);
		}

		Vector4(array<Float16>^ Floats)
		{
			if (Floats == nullptr || Floats->Length < 4) return;

			X = Floats[0].Value;
			Y = Floats[1].Value;
			Z = Floats[2].Value;
			W = Floats[3].Value;
		}

		Vector4(Vector XYZ, float W)
		{
			this->X = XYZ.X;
			this->Y = XYZ.Y;
			this->Z = XYZ.Z;
			this->W = W;
		}

		Vector4(float X, float Y, float Z, float W)
		{
			this->X = X;
			this->Y = Y;
			this->Z = Z;
			this->W = W;
		}

		static explicit operator array<float>^(Vector4 Value)
		{
			return gcnew array<float>(4) { Value.X, Value.Y, Value.Z, Value.W };
		}

		void Add(Vector4 Value)
		{
			X += Value.X;
			Y += Value.Y;
			Z += Value.Z;
			W += Value.W;
		}

		void Subtract(Vector4 Value)
		{
			X -= Value.X;
			Y -= Value.Y;
			Z -= Value.Z;
			W -= Value.W;
		}

		void Multiply(float Value)
		{
			X *= Value;
			Y *= Value;
			Z *= Value;
			W *= Value;
		}

		void Divide(float Value)
		{
			X /= Value;
			Y /= Value;
			Z /= Value;
			W /= Value;
		}

		static Vector4 operator + (Vector4 Value)
		{
			return Value;
		}

		static Vector4 operator - (Vector4 Value)
		{
			return Vector4(-Value.X, -Value.Y, -Value.Z, -Value.W);
		}

		static Vector4 operator + (Vector4 Value1, Vector4 Value2)
		{
			return Vector4(Value1.X + Value2.X, Value1.Y + Value2.Y, Value1.Z + Value2.Z, Value1.W + Value2.W);
		}

		static Vector4 operator - (Vector4 Value1, Vector4 Value2)
		{
			return Vector4(Value1.X - Value2.X, Value1.Y - Value2.Y, Value1.Z - Value2.Z, Value1.W - Value2.W);
		}

		static Vector4 operator * (Vector4 Value1, float Value2)
		{
			return Vector4(Value1.X * Value2, Value1.Y * Value2, Value1.Z * Value2, Value1.W * Value2);
		}

		static Vector4 operator / (Vector4 Value1, float Value2)
		{
			float InvertedValue = 1.0f / Value2;
			return Vector4(Value1.X * InvertedValue, Value1.Y * InvertedValue, Value1.Z * InvertedValue, Value1.W * InvertedValue);
		}

		static bool operator == (Vector4 Left, Vector4 Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (Vector4 Left, Vector4 Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				(int)X ^
				(int)Y ^
				(int)Z ^
				(int)W;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(Vector4 Value)
		{
			return
				X == Value.X &&
				Y == Value.Y &&
				Y == Value.Z &&
				Z == Value.W;
		}

		static bool Equals(Vector4% Value1, Vector4% Value2)
		{
			return
				Value1.X == Value2.X && 
				Value1.Y == Value2.Y && 
				Value1.Z == Value2.Z && 
				Value1.W == Value2.W;
		}
	};
}
}
}