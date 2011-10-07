public value class MappedTexture2D : IEquatable<MappedTexture2D>
{
internal:
	inline void FromNative(D3D10_MAPPED_TEXTURE2D* Native)
	{
		Data = gcnew UnmanagedMemory(IntPtr(Native->pData), 0);
		RowPitch = Native->RowPitch;
	}	
		
public:
	UnmanagedMemory^ Data;
	unsigned int RowPitch;

	static bool operator == (MappedTexture2D Left, MappedTexture2D Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (MappedTexture2D Left, MappedTexture2D Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Data == nullptr ? 1 : Data->GetHashCode() ^
			RowPitch;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(MappedTexture2D Value)
	{
		pin_ptr<UnmanagedMemory^> PinnedThis = &Data;
		pin_ptr<MappedTexture2D> PinnedValue = &Value;

		return memcmp(PinnedThis, PinnedValue, Marshal::SizeOf(MappedTexture2D::typeid)) == 0;
	}

	static bool Equals(MappedTexture2D% Value1, MappedTexture2D% Value2)
	{
		pin_ptr<MappedTexture2D> PinnedValue1 = &Value1;
		pin_ptr<MappedTexture2D> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(MappedTexture2D::typeid)) == 0;
	}
};