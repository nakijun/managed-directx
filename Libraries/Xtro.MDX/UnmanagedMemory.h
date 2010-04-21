namespace Xtro
{
namespace MDX
{
	public ref class UnmanagedMemory
	{
	private:
		!UnmanagedMemory()
		{
			try
			{
				delete[] Memory;
				GC::RemoveMemoryPressure(FSize);
			}
			catch (...) { }
		}

		~UnmanagedMemory()
		{
			this->!UnmanagedMemory();
		}

	internal:
		Byte* Memory;

		unsigned int FSize;

	public:
		UnmanagedMemory(unsigned int Size)
		{
			FSize = Size;

			Memory = new Byte[FSize];
			GC::AddMemoryPressure(FSize);
		}

		generic<typename T> where T : value class
		void Get(int Index, T% Value)
		{
			pin_ptr<T> PinnedValue = &Value;
			memcpy(PinnedValue, Memory + Index * sizeof(T), sizeof(T));
		}

		generic<typename T> where T : value class
		void Set(int Index, T% Value)
		{									 
			pin_ptr<T> PinnedValue = &Value;
			memcpy(Memory + Index * sizeof(T), PinnedValue, sizeof(T));
		}

		generic<typename T> where T : value class
		void Read(int StartIndex, int Count, array<T>^ Values)
		{
			pin_ptr<T> PinnedValues = &Values[0];
			memcpy(PinnedValues, Memory + StartIndex * sizeof(T), Count * sizeof(T));
		}

		generic<typename T> where T : value class
		void Write(int StartIndex, int Count, array<T>^ Values)
		{
			pin_ptr<T> PinnedValues = &Values[0];
			memcpy(Memory + StartIndex * sizeof(T), PinnedValues, Count * sizeof(T));
		}

		property int Size
		{
			int get() { return FSize; }
		}

		bool Equals(UnmanagedMemory^ Value)
		{
			if (Value == nullptr) return false;
			if (FSize != Value->FSize) return false;

			try	{ return memcmp(Memory, Value->Memory, FSize) == 0; }
			catch (...) { return false; }
		}
	};
}
}