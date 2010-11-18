public ref class UnmanagedMemory
{
private:
	bool OwnMemory;

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
		if (Size < 1) throw gcnew ArgumentException ("Positive number required.", "Size");
		
		FSize = Size;

		pMemory = new unsigned char[FSize];
		GC::AddMemoryPressure(FSize);

		OwnMemory = true;
	}

	UnmanagedMemory(IntPtr Pointer, unsigned int Size)
	{
		if (Size < 1) throw gcnew ArgumentException ("Positive number required.", "Size");
		
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

	generic<typename T> where T : value class
	void Get(unsigned int Index, T% Value)
	{
		pin_ptr<T> PinnedValue = &Value;
		memcpy(PinnedValue, pMemory + Index * sizeof(T), sizeof(T));
	}

	generic<typename T> where T : value class
	void Set(unsigned int Index, T% Value)
	{									 
		pin_ptr<T> PinnedValue = &Value;
		memcpy(pMemory + Index * sizeof(T), PinnedValue, sizeof(T));
	}

	generic<typename T> where T : value class
	void Read(unsigned int StartIndex, unsigned int Count, array<T>^% Values)
	{
		pin_ptr<T> PinnedValues = &Values[0];
		memcpy(PinnedValues, pMemory + StartIndex * sizeof(T), Count * sizeof(T));
	}

	generic<typename T> where T : value class
	void Write(unsigned int StartIndex, unsigned int Count, array<T>^ Values)
	{
		pin_ptr<T> PinnedValues = &Values[0];
		memcpy(pMemory + StartIndex * sizeof(T), PinnedValues, Count * sizeof(T));
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