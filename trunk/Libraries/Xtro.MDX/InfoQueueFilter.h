public value class InfoQueueFilter : IEquatable<InfoQueueFilter>
{
internal: 		
	inline void FromNative(D3D10_INFO_QUEUE_FILTER* Native)
	{
		AllowList.FromNative(&Native->AllowList);
		DenyList.FromNative(&Native->DenyList);
	}
							   
	inline void Marshal(D3D10_INFO_QUEUE_FILTER* Native)
	{
		AllowList.Marshal(&Native->AllowList);
		DenyList.Marshal(&Native->DenyList);
	}

	inline void Unmarshal()
	{
		AllowList.Unmarshal();
		DenyList.Unmarshal();
	}

public:
	InfoQueueFilterDescription AllowList;
	InfoQueueFilterDescription DenyList;

	static bool operator == (InfoQueueFilter Left, InfoQueueFilter Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (InfoQueueFilter Left, InfoQueueFilter Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			AllowList.GetHashCode() ^
			DenyList.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(InfoQueueFilter Value)
	{
		pin_ptr<InfoQueueFilterDescription> PinnedThis = &AllowList;
		pin_ptr<InfoQueueFilter> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(InfoQueueFilter::typeid)) == 0;
	}

	static bool Equals(InfoQueueFilter% Value1, InfoQueueFilter% Value2)
	{
		pin_ptr<InfoQueueFilter> PinnedValue1 = &Value1;
		pin_ptr<InfoQueueFilter> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(InfoQueueFilter::typeid)) == 0;
	}
};