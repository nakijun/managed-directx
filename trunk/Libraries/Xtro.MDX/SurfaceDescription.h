namespace Xtro
{
namespace MDX
{
namespace DXGI
{
	[StructLayout(LayoutKind::Sequential)]
	public value class SurfaceDescription : IEquatable<SurfaceDescription>
	{
	public:
		unsigned int Width;
		unsigned int Height;
		Format Format;
		SampleDescription SampleDescription;
		
		SurfaceDescription(unsigned int Width, unsigned int Height, Xtro::MDX::DXGI::Format Format, Xtro::MDX::DXGI::SampleDescription SampleDescription)
		{
			this->Width = Width;
			this->Height = Height;
			this->Format = Format;
			this->SampleDescription = SampleDescription;
		}

		static bool operator == (SurfaceDescription Left, SurfaceDescription Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (SurfaceDescription Left, SurfaceDescription Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				Width ^
				Height ^
				(int)Format ^
				SampleDescription.GetHashCode();
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(SurfaceDescription Value)
		{
			return
				Width == Value.Width &&
				Height == Value.Height &&
				Format == Value.Format &&
				SampleDescription == Value.SampleDescription;
		}

		static bool Equals(SurfaceDescription% Value1, SurfaceDescription% Value2)
		{
			return
				Value1.Width == Value2.Width &&
				Value1.Height == Value2.Height &&
				Value1.Format == Value2.Format &&
				Value1.SampleDescription == Value2.SampleDescription;
		}
	};
}
}
}