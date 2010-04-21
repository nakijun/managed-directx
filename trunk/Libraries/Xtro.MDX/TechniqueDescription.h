namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public value class TechniqueDescription : IEquatable<TechniqueDescription>
	{
	internal:
		TechniqueDescription(D3D10_TECHNIQUE_DESC* Native)
		{
			Name = gcnew String(Native->Name);
			Passes = Native->Passes;
			Annotations = Native->Annotations;
		}
		
		D3D10_TECHNIQUE_DESC ToNative()
		{
			D3D10_TECHNIQUE_DESC Result;
			
			Result.Name = (LPCSTR)Marshal::StringToHGlobalAnsi(Name).ToPointer();
			Result.Passes = Passes;
			Result.Annotations = Annotations;
			
			return Result;
		}

	public:
		String^ Name;
		unsigned int Passes;
		unsigned int Annotations;
	
		TechniqueDescription(String^ Name, unsigned int Passes, unsigned int Annotations)
		{
			this->Name = Name;
			this->Passes = Passes;
			this->Annotations = Annotations;
		}
	
		static bool operator == (TechniqueDescription Left, TechniqueDescription Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (TechniqueDescription Left, TechniqueDescription Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				Name->GetHashCode() ^
				(int)Passes ^
				(int)Annotations;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(TechniqueDescription Value)
		{
			return
				Name == Value.Name &&
				Passes == Value.Passes &&
				Annotations == Value.Annotations;
		}

		static bool Equals(TechniqueDescription% Value1, TechniqueDescription% Value2)
		{
			return
				Value1.Name == Value2.Name && 
				Value1.Passes == Value2.Passes &&
				Value1.Annotations == Value2.Annotations;
		}
	};
}
}
}