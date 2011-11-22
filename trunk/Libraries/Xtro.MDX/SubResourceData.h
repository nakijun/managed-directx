public value class SubResourceData : IEquatable<SubResourceData>
{
internal:
	inline void ToNative(D3D10_SUBRESOURCE_DATA* Native)
	{
		Native->pSysMem = SystemMemory == nullptr ? 0 : SystemMemory->pMemory;
		Native->SysMemPitch = SystemMemoryPitch;
		Native->SysMemSlicePitch = SystemMemorySlicePitch;
	}

public:
	UnmanagedMemory^ SystemMemory;
	unsigned int SystemMemoryPitch;
	unsigned int SystemMemorySlicePitch;

	static bool operator == (SubResourceData Left, SubResourceData Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SubResourceData Left, SubResourceData Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			SystemMemory == nullptr ? 1 : SystemMemory->GetHashCode() ^
			SystemMemoryPitch ^
			SystemMemorySlicePitch;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SubResourceData Value)
	{
		pin_ptr<SubResourceData> PinnedValue = &Value;
		return memcmp(&*this, PinnedValue, Marshal::SizeOf(SubResourceData::typeid)) == 0;
	}

	static bool Equals(SubResourceData% Value1, SubResourceData% Value2)
	{
		pin_ptr<SubResourceData> PinnedValue1 = &Value1;
		pin_ptr<SubResourceData> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SubResourceData::typeid)) == 0;
	}
};