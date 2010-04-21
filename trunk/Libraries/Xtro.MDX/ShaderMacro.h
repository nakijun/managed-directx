namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public value class ShaderMacro : IEquatable<ShaderMacro>
	{
	internal:
		ShaderMacro(D3D10_SHADER_MACRO* Native)
		{
			Name = gcnew String(Native->Name);
			Definition = gcnew String(Native->Definition);
		}
		
		D3D10_SHADER_MACRO ToNative()
		{
			D3D10_SHADER_MACRO Result;
			
			Result.Name = (LPCSTR)Marshal::StringToHGlobalAnsi(Name).ToPointer();
			Result.Definition = (LPCSTR)Marshal::StringToHGlobalAnsi(Definition).ToPointer();
			
			return Result;
		}

	public:
		String^ Name;
		String^ Definition;

		ShaderMacro(String^ Name, String^ Definition)
		{
			this->Name = Name;
			this->Definition = Definition;
		}

		static bool operator == (ShaderMacro Left, ShaderMacro Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (ShaderMacro Left, ShaderMacro Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				Name->GetHashCode() ^
				Definition->GetHashCode();
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(ShaderMacro Value)
		{
			return
				Name == Value.Name &&
				Definition == Value.Definition;
		}

		static bool Equals(ShaderMacro% Value1, ShaderMacro% Value2)
		{
			return
				Value1.Name == Value2.Name && 
				Value1.Definition == Value2.Definition;
		}
	};
}
}
}