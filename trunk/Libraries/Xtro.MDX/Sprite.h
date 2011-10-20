public ref class Sprite sealed : Unknown
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

	int DrawSpritesBuffered(array<SpriteStruct>^ Sprites, unsigned int SpriteCount)
	{
		D3DX10_SPRITE* pSprites = 0;

		try
		{
			if (Sprites != nullptr && Sprites->Length > 0)
			{
				unsigned int Count = Math::Min(SpriteCount, (unsigned int)Sprites->Length);
				pSprites = new D3DX10_SPRITE[Count];
				for (unsigned int No = 0; No < Count; No++)
				{
					Sprites[No].ToNative(&pSprites[No]);
				}
			}

			return pSprite->DrawSpritesBuffered(pSprites, SpriteCount);
		}
		finally
		{
			if (pSprites) delete[] pSprites;
		}
	}

	int DrawSpritesImmediate(array<SpriteStruct>^ Sprites, unsigned int SpriteCount, unsigned int SpriteSize, unsigned int Flags)
	{
		D3DX10_SPRITE* pSprites = 0;
		try
		{
			if (Sprites != nullptr && Sprites->Length > 0)
			{
				unsigned int Count = Math::Min(SpriteCount, (unsigned int)Sprites->Length);
				pSprites = new D3DX10_SPRITE[Count];
				for (unsigned int No = 0; No < Count; No++)
				{
					Sprites[No].ToNative(&pSprites[No]);
				}
			}

			return pSprite->DrawSpritesImmediate(pSprites, SpriteCount, SpriteSize, Flags);
		}
		finally
		{
			if (pSprites) delete[] pSprites;
		}
	}

	int End()
	{
		return pSprite->End();
	}

	int Flush()
	{
		return pSprite->Flush();
	}

	int GetDevice([Out] Xtro::MDX::Direct3D10::Device^% Device_)
	{
		ID3D10Device* pDevice = 0;
		int Result = pSprite->GetDevice(&pDevice);

		if (pDevice) 
		{
			try { Device_ = (Xtro::MDX::Direct3D10::Device^)Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device_ = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }
		}
		else Device_ = nullptr;

		return Result;
	}

	int GetProjectionTransform([Out] Matrix% ProjectionTransform)
	{
		pin_ptr<Matrix> PinnedProjectionTransform = &ProjectionTransform;
		return pSprite->GetProjectionTransform((D3DXMATRIX*)PinnedProjectionTransform);
	}

	int GetViewTransform([Out] Matrix% ViewTransform)
	{
		pin_ptr<Matrix> PinnedViewTransform = &ViewTransform;
		return pSprite->GetViewTransform((D3DXMATRIX*)PinnedViewTransform);
	}

	int SetProjectionTransform(Matrix% ProjectionTransform)
	{
		pin_ptr<Matrix> PinnedProjectionTransform = &ProjectionTransform;
		return pSprite->SetProjectionTransform((D3DXMATRIX*)PinnedProjectionTransform);
	}

	int SetViewTransform(Matrix% ViewTransform)
	{
		pin_ptr<Matrix> PinnedViewTransform = &ViewTransform;
		return pSprite->SetViewTransform((D3DXMATRIX*)PinnedViewTransform);
	}
};