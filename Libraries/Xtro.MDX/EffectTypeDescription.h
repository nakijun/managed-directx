public value class EffectTypeDescription : IEquatable<EffectTypeDescription>
{
internal:				 
	inline void FromNative(D3D10_EFFECT_TYPE_DESC* Native)
	{
		TypeName = gcnew String(Native->TypeName);
		Class = (ShaderVariableClass)Native->Class;
		Type = (ShaderVariableType)Native->Type;
		Elements = Native->Elements;
		Members = Native->Members;
		Rows = Native->Rows;
		Columns = Native->Columns;
		PackedSize = Native->PackedSize;
		UnpackedSize = Native->UnpackedSize;
		Stride = Native->Stride;
	}

public:
    String^ TypeName;
    ShaderVariableClass Class;
    ShaderVariableType Type;
    unsigned int Elements;
    unsigned int Members;
    unsigned int Rows;
    unsigned int Columns;
    unsigned int PackedSize;
    unsigned int UnpackedSize;
    unsigned int Stride;

	static bool operator == (EffectTypeDescription Left, EffectTypeDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (EffectTypeDescription Left, EffectTypeDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			TypeName == nullptr ? 1 : TypeName->GetHashCode() ^
			(int)Class ^
			(int)Type ^
			Elements ^
			Members ^
			Rows ^
			Columns ^
			PackedSize ^
			UnpackedSize ^
			Stride;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(EffectTypeDescription Value)
	{
		return
			TypeName == Value.TypeName &&
			Class == Value.Class &&
			Type == Value.Type &&
			Elements == Value.Elements &&
			Members == Value.Members &&
			Rows == Value.Rows &&
			Columns == Value.Columns &&
			PackedSize == Value.PackedSize &&
			UnpackedSize == Value.UnpackedSize &&
			Stride == Value.Stride;
	}

	static bool Equals(EffectTypeDescription% Value1, EffectTypeDescription% Value2)
	{
		return
			Value1.TypeName == Value2.TypeName &&
			Value1.Class == Value2.Class &&
			Value1.Type == Value2.Type &&
			Value1.Elements == Value2.Elements &&
			Value1.Members == Value2.Members &&
			Value1.Rows == Value2.Rows &&
			Value1.Columns == Value2.Columns &&
			Value1.PackedSize == Value2.PackedSize &&
			Value1.UnpackedSize == Value2.UnpackedSize &&
			Value1.Stride == Value2.Stride;
	}
};