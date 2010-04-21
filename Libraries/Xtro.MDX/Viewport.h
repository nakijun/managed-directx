namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	[StructLayout(LayoutKind::Sequential)]
	public value class Viewport : IEquatable<Viewport>
	{
	public:
		unsigned int TopLeftX;
		unsigned int TopLeftY;
		unsigned int Width;
		unsigned int Height;
		float MinDepth;
		float MaxDepth;

		Viewport(unsigned int TopLeftX, unsigned int TopLeftY, unsigned int Width, unsigned int Height, float MinDepth, float MaxDepth)
		{
			this->TopLeftX = TopLeftX;
			this->TopLeftY = TopLeftY;
			this->Width = Width;
			this->Height = Height;
			this->MinDepth = MinDepth;
			this->MaxDepth = MaxDepth;
		}

		static bool operator == (Viewport Left, Viewport Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (Viewport Left, Viewport Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				TopLeftX ^
				TopLeftY ^
				Width ^
				Height ^
				(int)MinDepth ^
				(int)MaxDepth;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(Viewport Value)
		{
			return
				TopLeftX == Value.TopLeftX &&
				TopLeftY == Value.TopLeftY &&
				Width == Value.Width &&
				Height == Value.Height &&
				MinDepth == Value.MinDepth &&
				MaxDepth == Value.MaxDepth;
		}

		static bool Equals(Viewport% Value1, Viewport% Value2)
		{
			return
				Value1.TopLeftX == Value2.TopLeftX && 
				Value1.TopLeftY == Value2.TopLeftY && 
				Value1.Width == Value2.Width && 
				Value1.Height == Value2.Height && 
				Value1.MinDepth == Value2.MinDepth && 
				Value1.MaxDepth == Value2.MaxDepth;
		}
	};
}
}
}