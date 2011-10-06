public value class InputElementDescription : IEquatable<InputElementDescription>
{
private:
	IntPtr pSemanticName;

internal: 		
	inline void Marshal(D3D10_INPUT_ELEMENT_DESC* Native)
	{
		pSemanticName = Marshal::StringToHGlobalAnsi(SemanticName);

		Native->SemanticName = (LPCSTR)pSemanticName.ToPointer();
		Native->SemanticIndex = SemanticIndex;
		Native->Format = (DXGI_FORMAT)Format;
		Native->InputSlot = InputSlot;
		Native->AlignedByteOffset = AlignedByteOffset;
		Native->InputSlotClass = (D3D10_INPUT_CLASSIFICATION)InputSlotClass;
		Native->InstanceDataStepRate = InstanceDataStepRate;
	}

	inline void Unmarshal()
	{
		if (pSemanticName != IntPtr::Zero)
		{
			Marshal::FreeHGlobal(pSemanticName); 
			pSemanticName = IntPtr::Zero;
		}
	}

public:
	String^ SemanticName;
	unsigned int SemanticIndex;
	Format Format;
	unsigned int InputSlot;
	unsigned int AlignedByteOffset;
	InputClassification InputSlotClass;
	unsigned int InstanceDataStepRate;
		
	static bool operator == (InputElementDescription Left, InputElementDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (InputElementDescription Left, InputElementDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			SemanticName == nullptr ? 1 : SemanticName->GetHashCode() ^
			SemanticIndex ^
			(int)Format ^
			InputSlot ^
			AlignedByteOffset ^
			(int)InputSlotClass ^
			InstanceDataStepRate;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(InputElementDescription Value)
	{
		return
			SemanticName == Value.SemanticName &&
			SemanticIndex == Value.SemanticIndex &&
			Format == Value.Format &&
			InputSlot == Value.InputSlot &&
			AlignedByteOffset == Value.AlignedByteOffset &&
			InputSlotClass == Value.InputSlotClass &&
			InstanceDataStepRate == Value.InstanceDataStepRate;
	}

	static bool Equals(InputElementDescription% Value1, InputElementDescription% Value2)
	{
		return
			Value1.SemanticName == Value2.SemanticName && 
			Value1.SemanticIndex == Value2.SemanticIndex && 
			Value1.Format == Value2.Format && 
			Value1.InputSlot == Value2.InputSlot && 
			Value1.AlignedByteOffset == Value2.AlignedByteOffset && 
			Value1.InputSlotClass == Value2.InputSlotClass && 
			Value1.InstanceDataStepRate == Value2.InstanceDataStepRate;
	}
};