namespace Xtro
{
namespace MDX
{
namespace DXGI
{
	[StructLayout(LayoutKind::Sequential)]
	public value class SampleDescription : IEquatable<SampleDescription>
	{
	public:
		int Count;
		int Quality;

		SampleDescription(int Count, int Quality)
		{
			this->Count = Count;
			this->Quality = Quality;
		}

		static bool operator == (SampleDescription Left, SampleDescription Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (SampleDescription Left, SampleDescription Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				Count ^
				Quality;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(SampleDescription Value)
		{
			return
				Count == Value.Count &&
				Quality == Value.Quality;
		}

		static bool Equals(SampleDescription% Value1, SampleDescription% Value2)
		{
			return
				Value1.Count == Value2.Count && 
				Value1.Quality == Value2.Quality;
		}
	};
}
}
}