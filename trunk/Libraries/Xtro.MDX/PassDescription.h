public value class PassDescription : IEquatable<PassDescription>
{
internal:				 
	void FromNative(D3D10_PASS_DESC* Native)
	{
		Name = gcnew String(Native->Name);
		Annotations = Native->Annotations;
			
		IAInputSignature = gcnew array<Byte>(Native->IAInputSignatureSize);
		pin_ptr<Byte> PinnedIAInputSignature = &IAInputSignature[0];
		memcpy(PinnedIAInputSignature, Native->pIAInputSignature, IAInputSignature->Length);
			
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

		pin_ptr<Byte> PinnedIAInputSignature = nullptr;
		if (IAInputSignature != nullptr && IAInputSignature->Length > 0)
		{
			PinnedIAInputSignature = &IAInputSignature[0];
			Result.IAInputSignatureSize = IAInputSignature->Length;
		}
		else Result.IAInputSignatureSize = 0;
		Result.pIAInputSignature = PinnedIAInputSignature;

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
	array<Byte>^ IAInputSignature;
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
			IAInputSignature->GetHashCode() ^
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
		if (IAInputSignature == nullptr && Value.IAInputSignature != nullptr) return false;
		if (IAInputSignature != nullptr && Value.IAInputSignature == nullptr) return false;
		if (IAInputSignature != nullptr && Value.IAInputSignature != nullptr)
		{
			if (IAInputSignature->Length != Value.IAInputSignature->Length) return false;

			pin_ptr<Byte> Pinned_IAInputSignature1 = &IAInputSignature[0];
			pin_ptr<Byte> Pinned_IAInputSignature2 = &Value.IAInputSignature[0];
			if (memcmp(Pinned_IAInputSignature1, Pinned_IAInputSignature2, IAInputSignature->Length) != 0) return false;
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
		if (Value1.IAInputSignature == nullptr && Value2.IAInputSignature != nullptr) return false;
		if (Value1.IAInputSignature != nullptr && Value2.IAInputSignature == nullptr) return false;
		if (Value1.IAInputSignature != nullptr && Value2.IAInputSignature != nullptr)
		{
			if (Value1.IAInputSignature->Length != Value2.IAInputSignature->Length) return false;

			pin_ptr<Byte> Pinned_IAInputSignature1 = &Value1.IAInputSignature[0];
			pin_ptr<Byte> Pinned_IAInputSignature2 = &Value2.IAInputSignature[0];
			if (memcmp(Pinned_IAInputSignature1, Pinned_IAInputSignature2, Value1.IAInputSignature->Length) != 0) return false;
		}
				   
		return
			Value1.Name == Value2.Name && 
			Value1.Annotations == Value2.Annotations && 
			Value1.StencilRef == Value2.StencilRef && 
			Value1.SampleMask == Value2.SampleMask && 
			Value1.BlendFactor == Value2.BlendFactor;
	}
};