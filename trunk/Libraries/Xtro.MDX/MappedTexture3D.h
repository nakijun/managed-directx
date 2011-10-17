public value class MappedTexture3D : IEquatable<MappedTexture3D>
{
internal:
	inline void FromNative(D3D10_MAPPED_TEXTURE3D* Native)
	{
		Data = Native->pData ? gcnew UnmanagedMemory(IntPtr(Native->pData), 0) : nullptr;
		RowPitch = Native->RowPitch;
		DepthPitch = Native->DepthPitch;
	}	
		
public:
	UnmanagedMemory^ Data;
	unsigned int RowPitch;
	unsigned int DepthPitch;

	static bool operator == (MappedTexture3D Left, MappedTexture3D Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (MappedTexture3D Left, MappedTexture3D Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Data == nullptr ? 1 : Data->GetHashCode() ^
			RowPitch ^
			DepthPitch;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(MappedTexture3D Value)
	{
		pin_ptr<UnmanagedMemory^> PinnedThis = &Data;
		pin_ptr<MappedTexture3D> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(MappedTexture3D::typeid)) == 0;
	}

	static bool Equals(MappedTexture3D% Value1, MappedTexture3D% Value2)
	{
		pin_ptr<MappedTexture3D> PinnedValue1 = &Value1;
		pin_ptr<MappedTexture3D> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(MappedTexture3D::typeid)) == 0;
	}
};