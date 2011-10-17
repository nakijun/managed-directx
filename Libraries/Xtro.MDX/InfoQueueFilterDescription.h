public value class InfoQueueFilterDescription : IEquatable<InfoQueueFilterDescription>
{
private:
	D3D10_MESSAGE_CATEGORY* pCategoryList;
	D3D10_MESSAGE_SEVERITY* pSeverityList;
	D3D10_MESSAGE_ID* pIDList;

internal: 		
	inline void FromNative(D3D10_INFO_QUEUE_FILTER_DESC* Native)
	{
		NumberOfCategories = Native->NumCategories;		
		NumberOfSeverities = Native->NumSeverities;		
		NumberOfIDs = Native->NumIDs;		

		if (Native->pCategoryList)
		{
			CategoryList = gcnew array<MessageCategory>(NumberOfCategories);
			pin_ptr<MessageCategory> PinnedCategoryList = NumberOfCategories > 0 ? &CategoryList[0] : nullptr;
			memcpy(PinnedCategoryList, Native->pCategoryList, NumberOfCategories * 4);
		}
		else CategoryList = nullptr;

		if (Native->pSeverityList)
		{
			SeverityList = gcnew array<MessageSeverity>(NumberOfSeverities);
			pin_ptr<MessageSeverity> PinnedSeverityList = NumberOfSeverities > 0 ? &SeverityList[0] : nullptr;
			memcpy(PinnedSeverityList, Native->pSeverityList, NumberOfSeverities * 4);
		}
		else SeverityList = nullptr;

		if (Native->pIDList)
		{
			ID_List = gcnew array<MessageID>(NumberOfIDs);
			pin_ptr<MessageID> PinnedID_List = NumberOfIDs > 0 ? &ID_List[0] : nullptr;
			memcpy(PinnedID_List, Native->pIDList, NumberOfIDs * 4);
		}
		else ID_List = nullptr;
	}
							   
	inline void Marshal(D3D10_INFO_QUEUE_FILTER_DESC* Native)
	{
		Native->NumCategories = NumberOfCategories;
		Native->NumSeverities = NumberOfSeverities;
		Native->NumIDs = NumberOfIDs;

		pCategoryList = new D3D10_MESSAGE_CATEGORY[NumberOfCategories];
		pin_ptr<MessageCategory> PinnedCategoryList = NumberOfCategories > 0 ? &CategoryList[0] : nullptr;
		memcpy(PinnedCategoryList, Native->pCategoryList, NumberOfCategories * 4);

		Native->pCategoryList = pCategoryList;

		pSeverityList = new D3D10_MESSAGE_SEVERITY[NumberOfSeverities];
		pin_ptr<MessageSeverity> PinnedSeverityList = NumberOfSeverities > 0 ? &SeverityList[0] : nullptr;
		memcpy(PinnedSeverityList, Native->pSeverityList, NumberOfSeverities * 4);

		Native->pSeverityList = pSeverityList;

		pIDList = new D3D10_MESSAGE_ID[NumberOfIDs];
		pin_ptr<MessageID> PinnedIDList = NumberOfIDs > 0 ? &ID_List[0] : nullptr;
		memcpy(PinnedIDList, Native->pIDList, NumberOfIDs * 4);

		Native->pIDList = pIDList;
	}

	inline void Unmarshal()
	{
		if (pCategoryList)
		{
			delete pCategoryList;
			pCategoryList = 0;
		}

		if (pSeverityList)
		{
			delete pSeverityList;
			pSeverityList = 0;
		}

		if (pIDList)
		{
			delete pIDList;
			pIDList = 0;
		}
	}
	 
public:
	unsigned int NumberOfCategories;
    array<MessageCategory>^ CategoryList;
    unsigned int NumberOfSeverities;
    array<MessageSeverity>^ SeverityList;
    unsigned int NumberOfIDs;
    array<MessageID>^ ID_List;
		
	static bool operator == (InfoQueueFilterDescription Left, InfoQueueFilterDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (InfoQueueFilterDescription Left, InfoQueueFilterDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			NumberOfCategories ^
			NumberOfSeverities ^
			NumberOfIDs;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(InfoQueueFilterDescription Value)
	{
		pin_ptr<D3D10_MESSAGE_CATEGORY*> PinnedThis = &pCategoryList;
		pin_ptr<InfoQueueFilterDescription> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(InfoQueueFilterDescription::typeid)) == 0;
	}

	static bool Equals(InfoQueueFilterDescription% Value1, InfoQueueFilterDescription% Value2)
	{
		pin_ptr<InfoQueueFilterDescription> PinnedValue1 = &Value1;
		pin_ptr<InfoQueueFilterDescription> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(InfoQueueFilterDescription::typeid)) == 0;
	}
};