public value class TechniqueDescription : IEquatable<TechniqueDescription>
{
internal:	 
	inline void FromNative(D3D10_TECHNIQUE_DESC* Native)
	{
		Name = gcnew String(Native->Name);
		Passes = Native->Passes;
		Annotations = Native->Annotations;
	}
		
public:
	String^ Name;
	unsigned int Passes;
	unsigned int Annotations;
	
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