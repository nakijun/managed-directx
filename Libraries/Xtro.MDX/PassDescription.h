public value class PassDescription : IEquatable<PassDescription>
{
internal:				 
	inline void FromNative(D3D10_PASS_DESC* Native)
	{
		Name = Native->Name ? gcnew String(Native->Name) : nullptr;
		Annotations = Native->Annotations;
		IA_InputSignature = Native->pIAInputSignature ? gcnew UnmanagedMemory(IntPtr(Native->pIAInputSignature), (unsigned int)Native->IAInputSignatureSize) : nullptr;
		StencilRef = Native->StencilRef;
		SampleMask = Native->SampleMask;
			
		pin_ptr<Float4> PinnedBlendFactor = &BlendFactor;
		memcpy(PinnedBlendFactor, Native->BlendFactor, Marshal::SizeOf(Float4::typeid));
	}

public:
	String^ Name;
	unsigned int Annotations;
	UnmanagedMemory^ IA_InputSignature;
	unsigned int StencilRef;
	unsigned int SampleMask;
	Float4 BlendFactor;
		
	static bool operator == (PassDescription Left, PassDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (PassDescription Left, PassDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Name == nullptr ? 1 : Name->GetHashCode() ^
			Annotations ^
			(IA_InputSignature == nullptr ? 1 : IA_InputSignature->GetHashCode()) ^
			StencilRef ^
			SampleMask ^
			BlendFactor.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(PassDescription Value)
	{
		return
			Name == Value.Name &&
			Annotations == Value.Annotations &&
			IA_InputSignature == Value.IA_InputSignature &&
			StencilRef == Value.StencilRef &&
			SampleMask == Value.SampleMask &&
			BlendFactor == Value.BlendFactor;
	}

	static bool Equals(PassDescription% Value1, PassDescription% Value2)
	{
		return
			Value1.Name == Value2.Name && 
			Value1.Annotations == Value2.Annotations && 
			Value1.IA_InputSignature == Value2.IA_InputSignature &&
			Value1.StencilRef == Value2.StencilRef && 
			Value1.SampleMask == Value2.SampleMask && 
			Value1.BlendFactor == Value2.BlendFactor;
	}
};