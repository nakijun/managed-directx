public ref class UnmanagedMemory
{
private:
	bool OwnMemory;

	UnmanagedMemoryStream^ Stream;
	
	!UnmanagedMemory()
	{
		if (!OwnMemory) return;

		try
		{
			delete[] pMemory;
			GC::RemoveMemoryPressure(FSize);
		}
		catch (...) { }
	}
	  
	~UnmanagedMemory()
	{
		this->!UnmanagedMemory();
	}
	  	
internal:
	unsigned char* pMemory;

	unsigned int FSize;

public:
	UnmanagedMemory(unsigned int Size)
	{
		if (Size < 1) throw gcnew ArgumentException("Positive number required.", "Size");
		
		FSize = Size;

		pMemory = new unsigned char[FSize];
		GC::AddMemoryPressure(FSize);

		OwnMemory = true;
	}

	UnmanagedMemory(IntPtr Pointer, unsigned int Size)
	{
		if (Size < 1) throw gcnew ArgumentException("Positive number required.", "Size");
		
		FSize = Size;

		pMemory = (unsigned char*)Pointer.ToPointer();

		OwnMemory = false;
	}

	property IntPtr Pointer
	{
		IntPtr get()
		{
			return IntPtr(pMemory);
		}
	}

	UnmanagedMemoryStream^ GetStream()
	{
		return Stream == nullptr ? Stream = gcnew UnmanagedMemoryStream(pMemory, FSize, FSize, FileAccess::ReadWrite) : Stream;
	}

	generic<typename T> where T : value class
	void Get(unsigned int MemoryOffset, unsigned int Index, [Out] T% Value)
	{
		pin_ptr<T> PinnedValue = &Value;
		memcpy(PinnedValue, pMemory + MemoryOffset + Index * sizeof(T), sizeof(T));
	}

	generic<typename T> where T : value class
	void Get(unsigned int MemoryOffset, [Out] T% Value)
	{
		Get(MemoryOffset, 0, Value);
	}

	generic<typename T> where T : value class
	void Set(unsigned int MemoryOffset, unsigned int Index, T% Value)
	{									 
		pin_ptr<T> PinnedValue = &Value;
		memcpy(pMemory + MemoryOffset + Index * sizeof(T), PinnedValue, sizeof(T));
	}

	generic<typename T> where T : value class
	void Set(unsigned int MemoryOffset, T% Value)
	{
		Set(MemoryOffset, 0, Value);
	}

	generic<typename T> where T : value class
	void Read(unsigned int MemoryOffset, unsigned int MemoryIndex, array<T>^ Values, unsigned int ValuesCount, unsigned int ValuesStartIndex)
	{
		if (Values == nullptr) throw gcnew ArgumentException("Parameter can not be null", "Values");

		pin_ptr<T> PinnedValues = &Values[ValuesStartIndex];
		memcpy(PinnedValues, pMemory + MemoryOffset + MemoryIndex * sizeof(T), ValuesCount * sizeof(T));
	}

	generic<typename T> where T : value class
	void Read(unsigned int MemoryOffset, array<T>^ Values)
	{
		if (Values == nullptr) throw gcnew ArgumentException("Parameter can not be null", "Values");

		Read(MemoryOffset, 0, Values, Values->Length, 0);
	}
		
	generic<typename T> where T : value class
	void Write(unsigned int MemoryOffset, unsigned int MemoryIndex, array<T>^ Values, unsigned int ValuesCount, unsigned int ValuesStartIndex)
	{
		if (Values == nullptr) throw gcnew ArgumentException("Parameter can not be null", "Values");

		pin_ptr<T> PinnedValues = &Values[ValuesStartIndex];
		memcpy(pMemory + MemoryOffset + MemoryIndex * sizeof(T), PinnedValues, ValuesCount * sizeof(T));
	}

	generic<typename T> where T : value class
	void Write(unsigned int MemoryOffset, array<T>^ Values)
	{
		if (Values == nullptr) throw gcnew ArgumentException("Parameter can not be null", "Values");

		Write(MemoryOffset, 0, Values, Values->Length, 0);
	}

	property unsigned int Size
	{
		unsigned int get() { return FSize; }
	}

	bool Equals(UnmanagedMemory^ Value)
	{
		if (Value == nullptr) return false;
		if (FSize != Value->FSize) return false;
		if (pMemory != Value->pMemory) return true;

		try	{ return memcmp(pMemory, Value->pMemory, FSize) == 0; }
		catch (...) { return false; }
	}
};