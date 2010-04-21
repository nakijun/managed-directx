namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public value class InputElementDescription : IEquatable<InputElementDescription>
	{
	internal:
		InputElementDescription(D3D10_INPUT_ELEMENT_DESC* Native)
		{
			SemanticName = gcnew String(Native->SemanticName);
			SemanticIndex = Native->SemanticIndex;
			Format = (Xtro::MDX::DXGI::Format)Native->Format;
			InputSlot = Native->InputSlot;
			AlignedByteOffset = Native->AlignedByteOffset;
			InputSlotClass = (InputClassification)Native->InputSlotClass;
			InstanceDataStepRate = Native->InstanceDataStepRate;
		}
		
		D3D10_INPUT_ELEMENT_DESC ToNative()
		{
			D3D10_INPUT_ELEMENT_DESC Result;
			
			Result.SemanticName = (LPCSTR)Marshal::StringToHGlobalAnsi(SemanticName).ToPointer();
			Result.SemanticIndex = SemanticIndex;
			Result.Format = (DXGI_FORMAT)Format;
			Result.InputSlot = InputSlot;
			Result.AlignedByteOffset = AlignedByteOffset;
			Result.InputSlotClass = (D3D10_INPUT_CLASSIFICATION)InputSlotClass;
			Result.InstanceDataStepRate = InstanceDataStepRate;
			
			return Result;
		}

	public:
		String^ SemanticName;
		unsigned int SemanticIndex;
		Format Format;
		unsigned int InputSlot;
		unsigned int AlignedByteOffset;
		InputClassification InputSlotClass;
		unsigned int InstanceDataStepRate;
		
		InputElementDescription(String^ SemanticName, unsigned int SemanticIndex, Xtro::MDX::DXGI::Format Format, unsigned int InputSlot, unsigned int AlignedByteOffset, InputClassification InputSlotClass, unsigned int InstanceDataStepRate)
		{
			this->SemanticName = SemanticName;
			this->SemanticIndex = SemanticIndex;
			this->Format = Format;
			this->InputSlot = InputSlot;
			this->AlignedByteOffset = AlignedByteOffset;
			this->InputSlotClass = InputSlotClass;
			this->InstanceDataStepRate = InstanceDataStepRate;
		}

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
				SemanticName->GetHashCode() ^
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
}
}
}