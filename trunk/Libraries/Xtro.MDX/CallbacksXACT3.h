public ref class Callbacks sealed
{
private:
	Callbacks()
	{
	}
public:
	delegate int GetOverlappedResult(IntPtr, /*Overlapped%,*/ unsigned int%, bool);
	// http://msdn.microsoft.com/en-us/library/367eeye0.aspx
	// struct OVERLAPPED
};