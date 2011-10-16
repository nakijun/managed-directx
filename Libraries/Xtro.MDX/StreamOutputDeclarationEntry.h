public value class StreamOutputDeclarationEntry : IEquatable<StreamOutputDeclarationEntry>
{
private:
	IntPtr pSemanticName;

internal: 
	inline void Marshal(D3D10_SO_DECLARATION_ENTRY* Native)
	{
		pSemanticName = Marshal::StringToHGlobalAnsi(SemanticName);
		Native->SemanticName = (LPCSTR)pSemanticName.ToPointer();
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
    unsigned char StartComponent;
    unsigned char ComponentCount;
    unsigned char OutputSlot;

	static bool operator == (StreamOutputDeclarationEntry Left, StreamOutputDeclarationEntry Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (StreamOutputDeclarationEntry Left, StreamOutputDeclarationEntry Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(SemanticName == nullptr ? 1 : SemanticName->GetHashCode()) ^
			SemanticIndex ^
			StartComponent ^
			ComponentCount ^
			OutputSlot;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(StreamOutputDeclarationEntry Value)
	{
		return
			SemanticName == Value.SemanticName &&
			SemanticIndex == Value.SemanticIndex &&
			StartComponent == Value.StartComponent &&
			ComponentCount == Value.ComponentCount &&
			OutputSlot == Value.OutputSlot;
	}

	static bool Equals(StreamOutputDeclarationEntry% Value1, StreamOutputDeclarationEntry% Value2)
	{
		return
			Value1.SemanticName == Value2.SemanticName &&
			Value1.SemanticIndex == Value2.SemanticIndex &&
			Value1.StartComponent == Value2.StartComponent &&
			Value1.ComponentCount == Value2.ComponentCount &&
			Value1.OutputSlot == Value2.OutputSlot;
	}
};