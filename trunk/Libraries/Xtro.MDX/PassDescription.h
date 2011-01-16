public value class PassDescription : IEquatable<PassDescription>
{
internal:				 
	void FromNative(D3D10_PASS_DESC* Native)
	{
		Name = gcnew String(Native->Name);
		Annotations = Native->Annotations;
			
		IA_InputSignature = gcnew array<Byte>(Native->IAInputSignatureSize);
		pin_ptr<Byte> PinnedIA_InputSignature = &IA_InputSignature[0];
		memcpy(PinnedIA_InputSignature, Native->pIAInputSignature, IA_InputSignature->Length);
			
		StencilRef = Native->StencilRef;
		SampleMask = Native->SampleMask;
			
		pin_ptr<Float4> PinnedBlendFactor = &BlendFactor;
		memcpy(PinnedBlendFactor, Native->BlendFactor, sizeof(Float4));
	}

	/* When needed this method will be converted to Marshal/Unmarshal pair	
	inline D3D10_PASS_DESC ToNative()
	{
		D3D10_PASS_DESC Result;
			
		Result.Name = (LPCSTR)Marshal::StringToHGlobalAnsi(Name).ToPointer();
		Result.Annotations = Annotations;

		pin_ptr<Byte> PinnedIA_InputSignature = nullptr;
		if (IA_InputSignature != nullptr && IA_InputSignature->Length > 0)
		{
			PinnedIA_InputSignature = &IA_InputSignature[0];
			Result.IAInputSignatureSize = IA_InputSignature->Length;
		}
		else Result.IAInputSignatureSize = 0;
		Result.pIAInputSignature = PinnedIA_InputSignature;

		Result.StencilRef = StencilRef;
		Result.SampleMask = SampleMask;

		pin_ptr<Float4> PinnedBlendFactor = &BlendFactor;
		memcpy(Result.BlendFactor, PinnedBlendFactor, sizeof(Float4));
			
		return Result;
	}
	*/
public:
	String^ Name;
	unsigned int Annotations;
	array<Byte>^ IA_InputSignature;
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
			Name->GetHashCode() ^
			Annotations ^
			IA_InputSignature->GetHashCode() ^
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
		if (IA_InputSignature == nullptr && Value.IA_InputSignature != nullptr) return false;
		if (IA_InputSignature != nullptr && Value.IA_InputSignature == nullptr) return false;
		if (IA_InputSignature != nullptr && Value.IA_InputSignature != nullptr)
		{
			if (IA_InputSignature->Length != Value.IA_InputSignature->Length) return false;

			pin_ptr<Byte> PinnedIA_InputSignature1 = &IA_InputSignature[0];
			pin_ptr<Byte> PinnedIA_InputSignature2 = &Value.IA_InputSignature[0];
			if (memcmp(PinnedIA_InputSignature1, PinnedIA_InputSignature2, IA_InputSignature->Length) != 0) return false;
		}
				   
		return
			Name == Value.Name &&
			Annotations == Value.Annotations &&
			StencilRef == Value.StencilRef &&
			SampleMask == Value.SampleMask &&
			BlendFactor == Value.BlendFactor;
	}

	static bool Equals(PassDescription% Value1, PassDescription% Value2)
	{
		if (Value1.IA_InputSignature == nullptr && Value2.IA_InputSignature != nullptr) return false;
		if (Value1.IA_InputSignature != nullptr && Value2.IA_InputSignature == nullptr) return false;
		if (Value1.IA_InputSignature != nullptr && Value2.IA_InputSignature != nullptr)
		{
			if (Value1.IA_InputSignature->Length != Value2.IA_InputSignature->Length) return false;

			pin_ptr<Byte> PinnedIA_InputSignature1 = &Value1.IA_InputSignature[0];
			pin_ptr<Byte> PinnedIA_InputSignature2 = &Value2.IA_InputSignature[0];
			if (memcmp(PinnedIA_InputSignature1, PinnedIA_InputSignature2, Value1.IA_InputSignature->Length) != 0) return false;
		}
				   
		return
			Value1.Name == Value2.Name && 
			Value1.Annotations == Value2.Annotations && 
			Value1.StencilRef == Value2.StencilRef && 
			Value1.SampleMask == Value2.SampleMask && 
			Value1.BlendFactor == Value2.BlendFactor;
	}
};