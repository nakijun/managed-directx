generic<typename T> where T : value class
public ref class UnmanagedMemory : Xtro::MDX::UnmanagedMemory
{
public:
	UnmanagedMemory(unsigned int Size) : Xtro::MDX::UnmanagedMemory(Size)
	{
	}

	UnmanagedMemory(IntPtr Pointer, unsigned int Size) : Xtro::MDX::UnmanagedMemory(Pointer, Size)
	{
	}

	void Get(unsigned int Index, [Out] T% Value)
	{
		pin_ptr<T> PinnedValue = &Value;
		memcpy(PinnedValue, pMemory + Index * sizeof(T), sizeof(T));
	}

	void Get([Out] T% Value)
	{
		Get(0, Value);
	}

	void Set(unsigned int Index, T% Value)
	{									 
		pin_ptr<T> PinnedValue = &Value;
		memcpy(pMemory + Index * sizeof(T), PinnedValue, sizeof(T));
	}

	void Set(T% Value)
	{
		Set(0, Value);
	}

	void Read(unsigned int MemoryIndex, array<T>^ Values, unsigned int ValuesCount, unsigned int ValuesStartIndex)
	{
		if (Values == nullptr) throw gcnew ArgumentException("Parameter can not be null", "Values");

		pin_ptr<T> PinnedValues = &Values[ValuesStartIndex];
		memcpy(PinnedValues, pMemory + MemoryIndex * sizeof(T), ValuesCount * sizeof(T));
	}

	void Read(array<T>^ Values)
	{
		if (Values == nullptr) throw gcnew ArgumentException("Parameter can not be null", "Values");

		Read(0, Values, Values->Length, 0);
	}

	void Write(unsigned int MemoryIndex, array<T>^ Values, unsigned int ValuesCount, unsigned int ValuesStartIndex)
	{
		if (Values == nullptr) throw gcnew ArgumentException("Parameter can not be null", "Values");

		pin_ptr<T> PinnedValues = &Values[ValuesStartIndex];
		memcpy(pMemory + MemoryIndex * sizeof(T), PinnedValues, ValuesCount * sizeof(T));
	}

	void Write(array<T>^ Values)
	{
		if (Values == nullptr) throw gcnew ArgumentException("Parameter can not be null", "Values");

		Write(0, Values, Values->Length, 0);
	}
};