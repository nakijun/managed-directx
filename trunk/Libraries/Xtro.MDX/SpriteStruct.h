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
		return
			MatrixWorld == Value.MatrixWorld &&
			TextureCoordinates == Value.TextureCoordinates &&
			TextureSize == Value.TextureSize &&
			ColorModulate == Value.ColorModulate &&
			Texture == Value.Texture &&
			TextureIndex == Value.TextureIndex;
	}

	static bool Equals(SpriteStruct% Value1, SpriteStruct% Value2)
	{
		return
			Value1.MatrixWorld == Value2.MatrixWorld &&
			Value1.TextureCoordinates == Value2.TextureCoordinates &&
			Value1.TextureSize == Value2.TextureSize &&
			Value1.ColorModulate == Value2.ColorModulate &&
			Value1.Texture == Value2.Texture &&
			Value1.TextureIndex == Value2.TextureIndex;
	}
};