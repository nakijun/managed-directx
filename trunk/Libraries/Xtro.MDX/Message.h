public value class Message : IEquatable<Message>
{
internal:				 
	inline void FromNative(D3D10_MESSAGE* Native)
	{
		Category = (MessageCategory)Native->Category;
		Severity = (MessageSeverity)Native->Severity;
		ID = (MessageID)Native->ID;
		Description = Native->pDescription ? gcnew String(Native->pDescription) : nullptr;
		DescriptionByteLength = Native->DescriptionByteLength;
	}

public:
	MessageCategory Category;
    MessageSeverity Severity;
    MessageID ID;
    String^ Description;
    SIZE_T DescriptionByteLength;

	static bool operator == (Message Left, Message Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (Message Left, Message Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			(int)Category ^
			(int)Severity ^
			(int)ID ^
			(Description == nullptr ? 1 : Description->GetHashCode()) ^
			DescriptionByteLength.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(Message Value)
	{
		return
			Category == Value.Category &&
			Severity == Value.Severity &&
			ID == Value.ID &&
			Description == Value.Description &&
			DescriptionByteLength == Value.DescriptionByteLength;
	}

	static bool Equals(Message% Value1, Message% Value2)
	{
		return
			Value1.Category == Value2.Category &&
			Value1.Severity == Value2.Severity &&
			Value1.ID == Value2.ID &&
			Value1.Description == Value2.Description &&
			Value1.DescriptionByteLength == Value2.DescriptionByteLength;
	}
};