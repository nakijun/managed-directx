public value class EffectDescription : IEquatable<EffectDescription>
{
public:
    bool IsChildEffect;
    unsigned int ConstantBuffers;
    unsigned int SharedConstantBuffers;
    unsigned int GlobalVariables;
    unsigned int SharedGlobalVariables;
    unsigned int Techniques;

	static bool operator == (EffectDescription Left, EffectDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (EffectDescription Left, EffectDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)IsChildEffect ^
			ConstantBuffers ^
			SharedConstantBuffers ^
			GlobalVariables ^
			SharedGlobalVariables ^
			Techniques;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(EffectDescription Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(EffectDescription::typeid)) == 0;
	}

	static bool Equals(EffectDescription% Value1, EffectDescription% Value2)
	{
		pin_ptr<EffectDescription> PinnedValue1 = &Value1;
		pin_ptr<EffectDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(EffectDescription::typeid)) == 0;
	}
};