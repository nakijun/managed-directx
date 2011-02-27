public ref class Font : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3DX10Font);

internal:
	ID3DX10Font* pFont;

	Font(IntPtr Font) : Unknown(Font)
	{	
		pFont = (ID3DX10Font*)Font.ToPointer();
	}

public:
	int PreloadCharacters(unsigned int First, unsigned int Last)
	{
		return pFont->PreloadCharacters(First, Last);
	}

	int DrawText(Sprite^ Sprite, String^ String, int Count, System::Drawing::Rectangle% Rectangle, FontDrawFlag Format, Color% Color)
	{
		ID3DX10Sprite* pSprite = Sprite == nullptr ? 0 : Sprite->pSprite;
		RECT NativeRectangle = { Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom };
		pin_ptr<Xtro::MDX::Direct3DX10::Color> PinnedColor = &Color;

		IntPtr pString = Marshal::StringToHGlobalUni(String);
		try	{ return pFont->DrawText(pSprite, (LPCTSTR)pString.ToPointer(), Count, &NativeRectangle, (unsigned int)Format, ((D3DXCOLOR*)PinnedColor)[0]); }
		finally { Marshal::FreeHGlobal(pString); }
	}
};