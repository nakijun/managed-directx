public ref class Functions abstract sealed
{
public:
	static String^ GetErrorDescription(int Result)
	{
		return gcnew String(DXGetErrorDescriptionW(Result));
	}

	static String^ DXGetErrorString(int Result)
	{
		return gcnew String(DXGetErrorStringW(Result));
	}

	static int Trace(String^ File, unsigned int Line, int Result, String^ Message, bool PopMessageBox)
	{
		IntPtr pFile = IntPtr::Zero;
		IntPtr pMessage = IntPtr::Zero;
		try
		{
			pFile = Marshal::StringToHGlobalAnsi(File);
			pMessage = Marshal::StringToHGlobalUni(Message);

			Result = DXTraceW((char*)pFile.ToPointer(), Line, Result, (WCHAR*)pMessage.ToPointer(), PopMessageBox);
		}
		finally
		{
			Marshal::FreeHGlobal(pFile); 
			Marshal::FreeHGlobal(pMessage); 
		}

		return Result;
	}

#if defined(DEBUG) | defined(_DEBUG)
	static int TraceMessage(String^ File, unsigned int Line, String^ String)
	{
		return Trace(File, Line, 0, String, false);
	}

	static int TraceError(String^ File, unsigned int Line, String^ String, int Result)
	{
		return Trace(File, Line, Result, String, false);
	}

	static int TraceErrorMessageBox(String^ File, unsigned int Line, String^ String, int Result)
	{
		return Trace(File, Line, Result, String, true);
	}
#else
	static int TraceMessage(String^ File, unsigned int Line, String^ String)
	{
		return 0;
	}

	static int TraceError(String^ File, unsigned int Line, String^ String, int Result)
	{
		return Result;
	}

	static int TraceErrorMessageBox(String^ File, unsigned int Line, String^ String, int Result)
	{
		return Result;
	}
#endif
};