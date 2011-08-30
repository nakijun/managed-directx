public value class MappedTexture3D : IEquatable<MappedTexture3D>
{
internal:
	inline void FromNative(D3D10_MAPPED_TEXTURE3D* Native)
	{
		Data = gcnew UnmanagedMemory(IntPtr(Native->pData), 0);
		RowPitch = Native->RowPitch;
		DepthPitch = Native->DepthPitch;
	}	
		
	inline void ToNative(D3D10_MAPPED_TEXTURE3D* Native)
	{
		Native->pData = (Data != nullptr && Data->Size > 0) ? Data->pMemory : 0;
		Native->RowPitch = RowPitch;
		Native->DepthPitch = DepthPitch;
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
			Data->GetHashCode() ^
			(int)RowPitch ^
			(int)DepthPitch;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(MappedTexture3D Value)
	{
		return
			Data == Value.Data &&
			RowPitch == Value.RowPitch &&
			DepthPitch == Value.DepthPitch;
	}

	static bool Equals(MappedTexture3D% Value1, MappedTexture3D% Value2)
	{
		return
			Value1.Data == Value2.Data &&
			Value1.RowPitch == Value2.RowPitch &&
			Value1.DepthPitch == Value2.DepthPitch;
	}
};