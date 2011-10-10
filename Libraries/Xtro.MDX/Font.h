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
	int PreloadCharacters(unsigned int First, unsigned int Last)
	{
		return pFont->PreloadCharacters(First, Last);
	}

	int DrawText(Sprite^ Sprite, String^ String, int Count, System::Drawing::Rectangle% Rectangle, FontDrawFlag Format, Color% Color)
	{
		ID3DX10Sprite* pSprite = Sprite == nullptr ? 0 : Sprite->pSprite;
		RECT NativeRectangle = { Rectangle.Left, Rectangle.Top, Rectangle.Right, Rectangle.Bottom };
		pin_ptr<Xtro::MDX::Direct3DX10::Color> PinnedColor = &Color;

		if (Count == -1 && String != nullptr) Count = String->Length;

		IntPtr pString = Marshal::StringToHGlobalUni(String);
		try	{ return pFont->DrawTextW(pSprite, (LPCTSTR)pString.ToPointer(), Count, &NativeRectangle, (unsigned int)Format, *((D3DXCOLOR*)PinnedColor)); }
		finally { Marshal::FreeHGlobal(pString); }
	}

	IntPtr GetDC()
	{
		return IntPtr(pFont->GetDC());
	}

	int GetDevice([Out] Xtro::MDX::Direct3D10::Device^% Device_)
	{
		ID3D10Device* pDevice = 0;
		int Result = pFont->GetDevice(&pDevice);

		if (pDevice) 
		{
			try { Device_ = (Xtro::MDX::Direct3D10::Device^)Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device_ = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }
		}
		else Device_ = nullptr;

		return Result;
	}
};