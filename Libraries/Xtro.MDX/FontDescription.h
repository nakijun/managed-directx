public value class FontDescription : IEquatable<FontDescription>
{
internal:
	inline void FromNative(D3DX10_FONT_DESC* Native)
	{
		Height = Native->Height;
		Width = Native->Width;
		Weight = (FontWeight)Native->Weight;
		MipLevels = Native->MipLevels;
		Italic = Native->Italic;
		CharacterSet = (FontCharacterSet)Native->CharSet;
		OutputPrecision = (FontPrecision)Native->OutputPrecision;
		Quality = (FontQuality)Native->Quality;
		PitchAndFamily = (FontPitchAndFamilyFlag)Native->PitchAndFamily;

		FaceName = Native->FaceName ? gcnew String(Native->FaceName) : nullptr;
	}

	// No need to marshal/unmarshal pair since FaceName has fixed size
	inline void ToNative(D3DX10_FONT_DESC* Native)
	{
		Native->Height = Height;
		Native->Width = Width;
		Native->Weight = (unsigned int)Weight;
		Native->MipLevels = MipLevels;
		Native->Italic = Italic;
		Native->CharSet = (unsigned char)CharacterSet;
		Native->OutputPrecision = (unsigned char)OutputPrecision;
		Native->Quality = (unsigned char)Quality;
		Native->PitchAndFamily = (unsigned char)PitchAndFamily;

		int Length = FaceName == nullptr ? 0 : FaceName->Length;
		for (int No = 0; No < LF_FACESIZE; No++)
		{
			Native->FaceName[No] = No < Length ? FaceName[No] : 0;
		}
	}

public:
	int Height;
    unsigned int Width;
    FontWeight Weight;
    unsigned int MipLevels;
    bool Italic;
    FontCharacterSet CharacterSet;
    FontPrecision OutputPrecision;
    FontQuality Quality;
    FontPitchAndFamilyFlag PitchAndFamily;
    String^ FaceName;

	static bool operator == (FontDescription Left, FontDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (FontDescription Left, FontDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Height ^
			Width ^
			(int)Weight ^
			MipLevels ^
			(int)Italic ^
			(int)CharacterSet ^
			(int)OutputPrecision ^
			(int)Quality ^
			(int)PitchAndFamily ^
			(FaceName == nullptr ? 1 : FaceName->GetHashCode());
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(FontDescription Value)
	{
		return
			Height == Value.Height &&
			Width == Value.Width &&
			Weight == Value.Weight &&
			MipLevels == Value.MipLevels &&
			Italic == Value.Italic &&
			CharacterSet == Value.CharacterSet &&
			OutputPrecision == Value.OutputPrecision &&
			Quality == Value.Quality &&
			PitchAndFamily == Value.PitchAndFamily &&
			FaceName == Value.FaceName;
	}

	static bool Equals(FontDescription% Value1, FontDescription% Value2)
	{
		return
			Value1.Height == Value2.Height &&
			Value1.Width == Value2.Width &&
			Value1.Weight == Value2.Weight &&
			Value1.MipLevels == Value2.MipLevels &&
			Value1.Italic == Value2.Italic &&
			Value1.CharacterSet == Value2.CharacterSet &&
			Value1.OutputPrecision == Value2.OutputPrecision &&
			Value1.Quality == Value2.Quality &&
			Value1.PitchAndFamily == Value2.PitchAndFamily &&
			Value1.FaceName == Value2.FaceName;
	}
};