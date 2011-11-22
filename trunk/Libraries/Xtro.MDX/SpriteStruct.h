public value class SpriteStruct : IEquatable<SpriteStruct>
{
internal:
	inline void ToNative(D3DX10_SPRITE* Native)
	{
		pin_ptr<Matrix> PinnedMatrixWorld = &MatrixWorld;
		Native->matWorld = *(D3DXMATRIX*)PinnedMatrixWorld;

		pin_ptr<Vector2> PinnedTextureCoordinates = &TextureCoordinates;
		Native->TexCoord = *(D3DXVECTOR2*)PinnedTextureCoordinates;

		pin_ptr<Vector2> PinnedTextureSize = &TextureSize;
		Native->TexSize = *(D3DXVECTOR2*)PinnedTextureSize;

		pin_ptr<Color> PinnedColorModulate = &ColorModulate;
		Native->ColorModulate = *(D3DXCOLOR*)PinnedColorModulate;

		Native->pTexture = Texture == nullptr ? 0 : Texture->pShaderResourceView;
		Native->TextureIndex = TextureIndex;
	}

public:
	Matrix MatrixWorld;
    Vector2 TextureCoordinates;
    Vector2 TextureSize;
    Color ColorModulate;
    ShaderResourceView^ Texture;
    unsigned int TextureIndex;

	static bool operator == (SpriteStruct Left, SpriteStruct Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SpriteStruct Left, SpriteStruct Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			MatrixWorld.GetHashCode() ^
			TextureCoordinates.GetHashCode() ^
			TextureSize.GetHashCode() ^
			ColorModulate.GetHashCode() ^
			(Texture == nullptr ? 1 : Texture->GetHashCode()) ^
			TextureIndex;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SpriteStruct Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(SpriteStruct::typeid)) == 0;
	}

	static bool Equals(SpriteStruct% Value1, SpriteStruct% Value2)
	{
		pin_ptr<SpriteStruct> PinnedValue1 = &Value1;
		pin_ptr<SpriteStruct> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SpriteStruct::typeid)) == 0;
	}
};