public ref class Font sealed : Unknown
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
	int DrawText(Sprite^ Sprite, String^ String, int Count, System::Drawing::Rectangle% Rectangle, FontDrawFlag Format, Color% Color)
	{
		ID3DX10Sprite* pSprite = Sprite == nullptr ? 0 : Sprite->pSprite;
		RECT NativeRectangle = { Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom };
		pin_ptr<Direct3DX10::Color> PinnedColor = &Color;

		if (Count == -1 && String != nullptr) Count = String->Length;

		IntPtr pString = Marshal::StringToHGlobalUni(String);
		try	{ return pFont->DrawTextW(pSprite, (LPCWSTR)pString.ToPointer(), Count, &NativeRectangle, (unsigned int)Format, *(D3DXCOLOR*)PinnedColor); }
		finally { Marshal::FreeHGlobal(pString); }
	}

	IntPtr GetDC()
	{
		return IntPtr(pFont->GetDC());
	}

	int GetDescription([Out] FontDescription% Description)
	{
		D3DX10_FONT_DESC NativeDescription;

		int Result = pFont->GetDesc(&NativeDescription);

		Description.FromNative(&NativeDescription);

		return Result;
	}

	int GetDevice([Out] Direct3D10::Device^% Device_)
	{
		ID3D10Device* pDevice = 0;
		int Result = pFont->GetDevice(&pDevice);

		if (pDevice) 
		{
			try { Device_ = (Direct3D10::Device^)Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device_ = gcnew Direct3D10::Device(IntPtr(pDevice)); }
		}
		else Device_ = nullptr;

		return Result;
	}

	int GetGlyphData(unsigned int Glyph, [Out] ShaderResourceView^ Texture, System::Drawing::Rectangle% BlackBox, Point% CellInc)
	{
		RECT NativeBlackBox = { BlackBox.Left, BlackBox.Top, BlackBox.Right, BlackBox.Bottom };
		POINT NativeCellInc = { CellInc.X, CellInc.Y };

		ID3D10ShaderResourceView* pTexture = 0;
		int Result = pFont->GetGlyphData(Glyph, &pTexture, &NativeBlackBox, &NativeCellInc);

		if (pTexture)
		{	
			try { Texture = (ShaderResourceView^)Interfaces[IntPtr(pTexture)]; }
			catch (KeyNotFoundException^) { Texture = gcnew ShaderResourceView(IntPtr(pTexture)); }
		}
		else Texture = nullptr;

		return Result;
	}

	bool GetTextMetrics([Out] TextMetrics% TextMetrics)
	{
		TEXTMETRIC NativeTextMetrics;

		bool Result = pFont->GetTextMetricsW(&NativeTextMetrics);
														 
		TextMetrics.Ascent = NativeTextMetrics.tmAscent;
		TextMetrics.AverageCharWidth = NativeTextMetrics.tmAveCharWidth;
		TextMetrics.BreakChar = NativeTextMetrics.tmBreakChar;
		TextMetrics.CharSet = (TextMetricsCharacterSet)NativeTextMetrics.tmCharSet;
		TextMetrics.DefaultChar = NativeTextMetrics.tmDefaultChar;
		TextMetrics.Descent = NativeTextMetrics.tmDescent;
		TextMetrics.DigitizedAspectX = NativeTextMetrics.tmDigitizedAspectX;
		TextMetrics.DigitizedAspectY = NativeTextMetrics.tmDigitizedAspectY;
		TextMetrics.ExternalLeading = NativeTextMetrics.tmExternalLeading;
		TextMetrics.FirstChar = NativeTextMetrics.tmFirstChar;
		TextMetrics.Height = NativeTextMetrics.tmHeight;
		TextMetrics.InternalLeading = NativeTextMetrics.tmInternalLeading;
		TextMetrics.Italic = NativeTextMetrics.tmItalic;
		TextMetrics.LastChar = NativeTextMetrics.tmLastChar;
		TextMetrics.MaxCharWidth = NativeTextMetrics.tmMaxCharWidth;
		TextMetrics.Overhang = NativeTextMetrics.tmOverhang;
		TextMetrics.PitchAndFamily = (TextMetricsPitchAndFamilyValues)NativeTextMetrics.tmPitchAndFamily;
		TextMetrics.StruckOut = NativeTextMetrics.tmStruckOut;
		TextMetrics.Underlined = NativeTextMetrics.tmUnderlined;
		TextMetrics.Weight = NativeTextMetrics.tmWeight;

		return Result;
	}

	int PreloadCharacters(unsigned int First, unsigned int Last)
	{
		return pFont->PreloadCharacters(First, Last);
	}

	int PreloadGlyphs(unsigned int First, unsigned int Last)
	{
		return pFont->PreloadGlyphs(First, Last);
	}

	int PreloadText(String^ String, int Count)
	{				 
		IntPtr pString = Marshal::StringToHGlobalUni(String);
		try { return pFont->PreloadText((LPCWSTR)pString.ToPointer(), Count); }
		finally { Marshal::FreeHGlobal(pString); }
	}
};