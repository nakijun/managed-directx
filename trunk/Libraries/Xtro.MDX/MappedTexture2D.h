public value class MappedTexture2D : IEquatable<MappedTexture2D>
{
internal:
	inline void FromNative(D3D10_MAPPED_TEXTURE2D* Native)
	{
		Data = gcnew UnmanagedMemory(IntPtr(Native->pData), 0);
		RowPitch = Native->RowPitch;
	}	
		
	inline void ToNative(D3D10_MAPPED_TEXTURE2D* Native)
	{
		Native->pData = (Data != nullptr && Data->Size > 0) ? Data->pMemory : 0;
		Native->RowPitch = RowPitch;
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
			Data->GetHashCode() ^
			(int)RowPitch;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(MappedTexture2D Value)
	{
		return
			Data == Value.Data &&
			RowPitch == Value.RowPitch;
	}

	static bool Equals(MappedTexture2D% Value1, MappedTexture2D% Value2)
	{
		return
			Value1.Data == Value2.Data &&
			Value1.RowPitch == Value2.RowPitch;
	}
};