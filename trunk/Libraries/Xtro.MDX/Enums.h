public enum class FontCharacterSet
{
	Ansi = ANSI_CHARSET,
	Default = DEFAULT_CHARSET,
	Symbol = SYMBOL_CHARSET,
	ShiftJIS = SHIFTJIS_CHARSET,
	Hangul = HANGUL_CHARSET,
	GB2312 = GB2312_CHARSET,
	ChineseBig5 = CHINESEBIG5_CHARSET,
	Oem = OEM_CHARSET,
	Johab = JOHAB_CHARSET,
	Hebrew = HEBREW_CHARSET,
	Arabic = ARABIC_CHARSET,
	Greek = GREEK_CHARSET,
	Turkish = TURKISH_CHARSET,
	Vietnamese = VIETNAMESE_CHARSET,
	Thai = THAI_CHARSET,
	EastEurope = EASTEUROPE_CHARSET,
	Russian = RUSSIAN_CHARSET,
	Baltic = BALTIC_CHARSET,
	Mac = MAC_CHARSET
};

[Flags]
public enum class FontDrawFlag
{
	Top = DT_TOP,
	CalculateRectange = DT_CALCRECT,
	Left = DT_LEFT,
	Center = DT_CENTER,
	Right = DT_RIGHT,
	VerticalCenter = DT_VCENTER,
	Bottom = DT_BOTTOM,
	WordBreak = DT_WORDBREAK,
	SingleLine = DT_SINGLELINE,
	ExpandTabs = DT_EXPANDTABS,
	NoClip = DT_NOCLIP,
	RtlReading = DT_RTLREADING,
};

[Flags]
public enum class FontPitchAndFamily
{
	Default = DEFAULT_PITCH,
	Fixed = FIXED_PITCH,
	Variable = VARIABLE_PITCH,
	Mono = MONO_FONT,
	DontCare = FF_DONTCARE,
	Roman = FF_ROMAN,
	Swiss = FF_SWISS,
	Modern = FF_MODERN,
	Script = FF_SCRIPT,
	Decorative = FF_DECORATIVE,
};

public enum class FontPrecision
{
	Default = OUT_DEFAULT_PRECIS,
	String = OUT_STRING_PRECIS,
	Character = OUT_CHARACTER_PRECIS,
	Stroke = OUT_STROKE_PRECIS,
	TrueType = OUT_TT_PRECIS,
	Device = OUT_DEVICE_PRECIS,
	Raster = OUT_RASTER_PRECIS,
	TrueTypeOnly = OUT_TT_ONLY_PRECIS,
	Outline = OUT_OUTLINE_PRECIS,
	ScreenOutline = OUT_SCREEN_OUTLINE_PRECIS,
	PostScriptOnly = OUT_PS_ONLY_PRECIS,
};

public enum class FontQuality
{
	Default = DEFAULT_QUALITY,
	Draft = DRAFT_QUALITY,
	Proof = PROOF_QUALITY,
	NonAntialiased = NONANTIALIASED_QUALITY,
	Antialiased = ANTIALIASED_QUALITY,
	ClearType = CLEARTYPE_QUALITY,
	ClearTypeNatural = CLEARTYPE_NATURAL_QUALITY,
};

public enum class FontWeight
{
	DoNotCare = FW_DONTCARE,
	Thin = FW_THIN,
	ExtraLight = FW_EXTRALIGHT,
	UltraLight = FW_ULTRALIGHT,
	Light = FW_LIGHT,
	Normal = FW_NORMAL,
	Regular = FW_REGULAR,
	Medium = FW_MEDIUM,
	SemiBold = FW_SEMIBOLD,
	DemiBold = FW_DEMIBOLD,
	Bold = FW_BOLD,
	ExtraBold = FW_EXTRABOLD,
	UltraBold = FW_ULTRABOLD,
	Heavy = FW_HEAVY,
	Black = FW_BLACK,
};