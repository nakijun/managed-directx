public value class EffectVariableDescription : IEquatable<EffectVariableDescription>
{
internal:				 
	inline void FromNative(D3D10_EFFECT_VARIABLE_DESC* Native)
	{
		Name = Native->Name ? gcnew String(Native->Name) : nullptr;
		Semantic = Native->Semantic ? gcnew String(Native->Semantic) : nullptr;
		Flags = (EffectVariableFlag)Native->Flags;
		Annotations = Native->Annotations;
		BufferOffset = Native->BufferOffset;
		ExplicitBindPoint = Native->ExplicitBindPoint;
	}

public:
    String^ Name;
    String^ Semantic;
    EffectVariableFlag Flags;
    unsigned int Annotations;
    unsigned int BufferOffset;
    unsigned int ExplicitBindPoint;

	static bool operator == (EffectVariableDescription Left, EffectVariableDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (EffectVariableDescription Left, EffectVariableDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Name == nullptr ? 1 : Name->GetHashCode() ^
			(Semantic == nullptr ? 1 : Semantic->GetHashCode()) ^
			(int)Flags ^
			Annotations ^
			BufferOffset ^
			ExplicitBindPoint;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(EffectVariableDescription Value)
	{
		return
			Name == Value.Name &&
			Semantic == Value.Semantic &&
			Flags == Value.Flags &&
			Annotations == Value.Annotations &&
			BufferOffset == Value.BufferOffset &&
			ExplicitBindPoint == Value.ExplicitBindPoint;
	}

	static bool Equals(EffectVariableDescription% Value1, EffectVariableDescription% Value2)
	{
		return
			Value1.Name == Value2.Name &&
			Value1.Semantic == Value2.Semantic &&
			Value1.Flags == Value2.Flags &&
			Value1.Annotations == Value2.Annotations &&
			Value1.BufferOffset == Value2.BufferOffset &&
			Value1.ExplicitBindPoint == Value2.ExplicitBindPoint;
	}
};