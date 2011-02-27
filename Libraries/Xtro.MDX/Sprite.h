public ref class Sprite : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3DX10Sprite);

internal:
	ID3DX10Sprite* pSprite;

	Sprite(IntPtr Sprite) : Unknown(Sprite)
	{	
		pSprite = (ID3DX10Sprite*)Sprite.ToPointer();
	}

public:
	int Begin(SpriteFlag Flags)
	{
		return pSprite->Begin((D3DX10_SPRITE_FLAG)Flags);
	}

	int End()
	{
		return pSprite->End();
	}
};