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

	int SetProjectionTransform(Matrix% ProjectionTransform)
	{
		pin_ptr<Matrix> PinnedProjectionTransform = &ProjectionTransform;
		return pSprite->SetProjectionTransform((D3DXMATRIX*)PinnedProjectionTransform);
	}

	int DrawSpritesBuffered(array<SpriteStruct>^ Sprites, unsigned int SpriteCount)
	{
		D3DX10_SPRITE* pSprites = 0;

		if (Sprites != nullptr && Sprites->Length > 0)
		{
			unsigned int ElementCount = Math::Min(SpriteCount, (unsigned int)Sprites->Length);
			pSprites = new D3DX10_SPRITE[ElementCount];
			for (int ElementNo = 0; ElementNo < ElementCount; ElementNo++) Sprites[ElementNo].ToNative(&pSprites[ElementNo]);
		}

		return pSprite->DrawSpritesBuffered(pSprites, SpriteCount);
	}
};