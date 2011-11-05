public ref class InfoQueue sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10InfoQueue);

internal:
	ID3D10InfoQueue* pInfoQueue;

	InfoQueue(IntPtr InfoQueue) : Unknown(InfoQueue)
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

	int AddRetrievalFilterEntries(array<InfoQueueFilter>^ Filter)
	{
		D3D10_INFO_QUEUE_FILTER* pFilter = 0;
		try
		{
			if (Filter != nullptr && Filter->Length > 0)
			{
				pFilter = new D3D10_INFO_QUEUE_FILTER[Filter->Length];
				for (int No = 0; No < Filter->Length; No++)
				{
					Filter[No].Marshal(&pFilter[No]);
				}
			}

			return pInfoQueue->AddRetrievalFilterEntries(pFilter);
		}
		finally
		{
			if (Filter != nullptr && Filter->Length > 0)
			{
				for (int No = 0; No < Filter->Length; No++)
				{
					Filter[No].Unmarshal();
				}
			}
			if (pFilter) delete[] pFilter;
		}
	}

	int AddStorageFilterEntries(array<InfoQueueFilter>^ Filter)
	{
		D3D10_INFO_QUEUE_FILTER* pFilter = 0;
		try
		{
			if (Filter != nullptr && Filter->Length > 0)
			{
				pFilter = new D3D10_INFO_QUEUE_FILTER[Filter->Length];
				for (int No = 0; No < Filter->Length; No++)
				{
					Filter[No].Marshal(&pFilter[No]);
				}
			}

			return pInfoQueue->AddStorageFilterEntries(pFilter);
		}
		finally
		{
			if (Filter != nullptr && Filter->Length > 0)
			{
				for (int No = 0; No < Filter->Length; No++)
				{
					Filter[No].Unmarshal();
				}
			}
			if (pFilter) delete[] pFilter;
		}
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

	int GetMessage(long long MessageIndex, [Out] SIZE_T% MessageByteLength)
	{
		pin_ptr<SIZE_T> PinnedMessageByteLength = &MessageByteLength;

		return pInfoQueue->GetMessage(MessageIndex, 0, PinnedMessageByteLength);
	}

	int GetMessage(long long MessageIndex, [Out] Message% Message, SIZE_T MessageByteLength)
	{
		pin_ptr<SIZE_T> PinnedMessageByteLength = &MessageByteLength;

		D3D10_MESSAGE* pMessage = (D3D10_MESSAGE*)malloc(MessageByteLength);
		try
		{
			int Result = pInfoQueue->GetMessage(MessageIndex, pMessage, PinnedMessageByteLength);
			Message.FromNative(pMessage);

			return Result;
		}
		finally	{ free(pMessage); }
	}

	long long GetMessageCountLimit()
	{
		return pInfoQueue->GetMessageCountLimit();
	}

	bool GetMuteDebugOutput()
	{
		return pInfoQueue->GetMuteDebugOutput() != 0;
	}

	long long GetNumberOfMessagesAllowedByStorageFilter()
	{
		return pInfoQueue->GetNumMessagesAllowedByStorageFilter();
	}

	long long GetNumberOfMessagesDeniedByStorageFilter()
	{
		return pInfoQueue->GetNumMessagesDeniedByStorageFilter();
	}

	long long GetNumberOfMessagesDiscardedByMessageCountLimit()
	{
		return pInfoQueue->GetNumMessagesDiscardedByMessageCountLimit();
	}

	long long GetNumberOfStoredMessages()
	{
		return pInfoQueue->GetNumStoredMessages();
	}

	long long GetNumberOfStoredMessagesAllowedByRetrievalFilter()
	{
		return pInfoQueue->GetNumStoredMessagesAllowedByRetrievalFilter();
	}

	int GetRetrievalFilter([Out] SIZE_T% FilterByteLength)
	{
		pin_ptr<SIZE_T> PinnedFilterByteLength = &FilterByteLength;

		return pInfoQueue->GetRetrievalFilter(0, PinnedFilterByteLength);
	}

	int GetRetrievalFilter([Out] InfoQueueFilter% Filter, SIZE_T FilterByteLength)
	{
		D3D10_INFO_QUEUE_FILTER NativeFilter;

		int Result = pInfoQueue->GetRetrievalFilter(&NativeFilter, &FilterByteLength);

		Filter.FromNative(&NativeFilter);

		return Result;
	}

	unsigned int GetRetrievalFilterStackSize()
	{
		return pInfoQueue->GetRetrievalFilterStackSize();
	}

	int GetStorageFilter([Out] SIZE_T% FilterByteLength)
	{
		pin_ptr<SIZE_T> PinnedFilterByteLength = &FilterByteLength;

		return pInfoQueue->GetStorageFilter(0, PinnedFilterByteLength);
	}

	int GetStorageFilter([Out] InfoQueueFilter% Filter, SIZE_T FilterByteLength)
	{
		D3D10_INFO_QUEUE_FILTER NativeFilter;

		int Result = pInfoQueue->GetStorageFilter(&NativeFilter, &FilterByteLength);

		Filter.FromNative(&NativeFilter);

		return Result;
	}

	unsigned int GetStorageFilterStackSize()
	{
		return pInfoQueue->GetStorageFilterStackSize();
	}

	void PopRetrievalFilter()
	{
		pInfoQueue->PopRetrievalFilter();
	}

	void PopStorageFilter()
	{
		pInfoQueue->PopStorageFilter();
	}

	int PushCopyOfRetrievalFilter()
	{
		return pInfoQueue->PushCopyOfRetrievalFilter();
	}

	int PushCopyOfStorageFilter()
	{
		return pInfoQueue->PushCopyOfStorageFilter();
	}

	int PushEmptyRetrievalFilter()
	{
		return pInfoQueue->PushEmptyRetrievalFilter();
	}

	int PushEmptyStorageFilter()
	{
		return pInfoQueue->PushEmptyStorageFilter();
	}

	int PushRetrievalFilter(InfoQueueFilter% Filter)
	{
		D3D10_INFO_QUEUE_FILTER NativeFilter;
		Filter.Marshal(&NativeFilter);

		try	{ return pInfoQueue->PushRetrievalFilter(&NativeFilter); }
		finally	{ Filter.Unmarshal(); }
	}

	int PushStorageFilter(InfoQueueFilter% Filter)
	{
		D3D10_INFO_QUEUE_FILTER NativeFilter;
		Filter.Marshal(&NativeFilter);

		try	{ return pInfoQueue->PushStorageFilter(&NativeFilter); }
		finally	{ Filter.Unmarshal(); }
	}

	int SetBreakOnCategory(MessageCategory Category, bool Enable)
	{
		return pInfoQueue->SetBreakOnCategory((D3D10_MESSAGE_CATEGORY)Category, Enable);
	}

	int SetBreakOnID(MessageID ID, bool Enable)
	{
		return pInfoQueue->SetBreakOnID((D3D10_MESSAGE_ID)ID, Enable);
	}

	int SetBreakOnSeverity(MessageSeverity Severity, bool Enable)
	{
		return pInfoQueue->SetBreakOnSeverity((D3D10_MESSAGE_SEVERITY)Severity, Enable);
	}

	int SetMessageCountLimit(long long MessageCountLimit)
	{
		return pInfoQueue->SetMessageCountLimit(MessageCountLimit);
	}

	void SetMuteDebugOutput(bool Mute)
	{
		pInfoQueue->SetMuteDebugOutput(Mute);
	}
};