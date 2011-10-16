public ref class InfoQueue sealed : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10InfoQueue);

internal:
	ID3D10InfoQueue* pInfoQueue;

	InfoQueue(IntPtr InfoQueue) : DeviceChild(InfoQueue)
	{	
		pInfoQueue = (ID3D10InfoQueue*)InfoQueue.ToPointer();
	}

public:
	int AddApplicationMessage(MessageSeverity Severity, String^ Description)
	{
		IntPtr pDescription = Marshal::StringToHGlobalAnsi(Description);
		try { return pInfoQueue->AddApplicationMessage((D3D10_MESSAGE_SEVERITY)Severity, (LPCSTR)pDescription.ToPointer()); }
		finally { Marshal::FreeHGlobal(pDescription); }
	}

	int AddMessage(MessageCategory Category, MessageSeverity Severity, MessageID ID, String^ Description)
	{
		IntPtr pDescription = Marshal::StringToHGlobalAnsi(Description);
		try { return pInfoQueue->AddMessage((D3D10_MESSAGE_CATEGORY)Category, (D3D10_MESSAGE_SEVERITY)Severity, (D3D10_MESSAGE_ID)ID, (LPCSTR)pDescription.ToPointer()); }
		finally { Marshal::FreeHGlobal(pDescription); }
	}

	int AddRetrievalFilterEntries(Xtro::MDX::Generic::UnmanagedMemory<InfoQueueFilter>^ Filter)
	{
		D3D10_INFO_QUEUE_FILTER* pFilter = Filter == nullptr ? 0 : (D3D10_INFO_QUEUE_FILTER*)Filter->pMemory;

		return pInfoQueue->AddRetrievalFilterEntries(pFilter);
	}

	int AddStorageFilterEntries(Xtro::MDX::Generic::UnmanagedMemory<InfoQueueFilter>^ Filter)
	{
		D3D10_INFO_QUEUE_FILTER* pFilter = Filter == nullptr ? 0 : (D3D10_INFO_QUEUE_FILTER*)Filter->pMemory;

		return pInfoQueue->AddStorageFilterEntries(pFilter);
	}

	void ClearRetrievalFilter()
	{
		pInfoQueue->ClearRetrievalFilter();
	}

	void ClearStorageFilter()
	{
		pInfoQueue->ClearStorageFilter();
	}

	void ClearStoredMessages()
	{
		pInfoQueue->ClearStoredMessages();
	}

	bool GetBreakOnCategory(MessageCategory Category)
	{
		return pInfoQueue->GetBreakOnCategory((D3D10_MESSAGE_CATEGORY)Category) != 0;
	}

	bool GetBreakOnID(MessageID ID)
	{
		return pInfoQueue->GetBreakOnID((D3D10_MESSAGE_ID)ID) != 0;
	}

	bool GetBreakOnSeverity(MessageSeverity Severity)
	{
		return pInfoQueue->GetBreakOnSeverity((D3D10_MESSAGE_SEVERITY)Severity) != 0;
	}
};