/// <summary>
/// Error codes that can be returned by Direct3DX
/// </summary>
public enum class ErrorCode
{
    /// <summary>
    /// The index buffer cannot be modified.
    /// <para>(Also see DirectX SDK:  D3DX10_ERR_CANNOT_MODIFY_INDEX_BUFFER )</para>
    /// </summary>
    Direct3DX10ErrorCannotModifyIndexBuffer = D3DX10_ERR_CANNOT_MODIFY_INDEX_BUFFER,

    /// <summary>
    /// The mesh is invalid. 
    /// <para>(Also see DirectX SDK:  D3DX10_ERR_INVALID_MESH )</para>
    /// </summary>
	Direct3DX10ErrorInvalidMesh = D3DX10_ERR_INVALID_MESH, 
	
    /// <summary>
    /// Attribute sort (D3DXMESHOPT_ATTRSORT) is not supported as an optimization technique. 
    /// <para>(Also see DirectX SDK:  D3DX10_ERR_CANNOT_ATTR_SORT )</para>
    /// </summary>
	Direct3DX10ErrorCannotAttributeSort = D3DX10_ERR_CANNOT_ATTR_SORT, 
	
    /// <summary>
    /// Skinning is not supported. 
    /// <para>(Also see DirectX SDK:  D3DX10_ERR_SKINNING_NOT_SUPPORTED )</para>
    /// </summary>
	Direct3DX10ErrorSkinningNotSupported = D3DX10_ERR_SKINNING_NOT_SUPPORTED, 
	
    /// <summary>
    /// Too many influences specified. 
    /// <para>(Also see DirectX SDK:  D3DX10_ERR_TOO_MANY_INFLUENCES )</para>
    /// </summary>
	Direct3DX10ErrorTooManyInfluences = D3DX10_ERR_TOO_MANY_INFLUENCES, 
	
    /// <summary>
    /// The data is invalid. 
    /// <para>(Also see DirectX SDK:  D3DX10_ERR_INVALID_DATA )</para>
    /// </summary>
	Direct3DX10ErrorInvalidData = D3DX10_ERR_INVALID_DATA, 
	
    /// <summary>
    /// The mesh has no data. 
    /// <para>(Also see DirectX SDK:  D3DX10_ERR_LOADED_MESH_HAS_NO_DATA )</para>
    /// </summary>
	Direct3DX10ErrorMeshHasNoData = D3DX10_ERR_LOADED_MESH_HAS_NO_DATA, 
	
    /// <summary>
    /// A fragment with that name already exists. 
    /// <para>(Also see DirectX SDK:  D3DX10_ERR_DUPLICATE_NAMED_FRAGMENT )</para>
    /// </summary>
	Direct3DX10ErrorDuplicateNamedFragment = D3DX10_ERR_DUPLICATE_NAMED_FRAGMENT, 
	
    /// <summary>
    /// The last item cannot be deleted. 
    /// <para>(Also see DirectX SDK:  D3DX10_ERR_CANNOT_REMOVE_LAST_ITEM )</para>
    /// </summary>
	Direct3DX10ErrorCannotRemoveLastItem = D3DX10_ERR_CANNOT_REMOVE_LAST_ITEM, 
};

[System::Flags]
public enum class FilterFlag
{
	None = D3DX10_FILTER_NONE,
	Point = D3DX10_FILTER_POINT,
	Linear = D3DX10_FILTER_LINEAR,
	Triangle = D3DX10_FILTER_TRIANGLE,
	Box = D3DX10_FILTER_BOX,
	MirrorU = D3DX10_FILTER_MIRROR_U,
	MirrorV = D3DX10_FILTER_MIRROR_V,
	MirrorW = D3DX10_FILTER_MIRROR_W,
	Mirror = D3DX10_FILTER_MIRROR,
	Dither = D3DX10_FILTER_DITHER,
	DitherDiffusion = D3DX10_FILTER_DITHER_DIFFUSION,
	SRGB_In = D3DX10_FILTER_SRGB_IN,
	SRGB_Out = D3DX10_FILTER_SRGB_OUT,
	SRGB = D3DX10_FILTER_SRGB
};

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

[System::Flags]
public enum class FontDrawFlag
{
	Top = DT_TOP,
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

public enum class ImageFileFormat
{
	BMP = D3DX10_IFF_BMP,
	JPG = D3DX10_IFF_JPG,
	PNG = D3DX10_IFF_PNG,
	DDS = D3DX10_IFF_DDS,
	TIFF = D3DX10_IFF_TIFF,
	GIF = D3DX10_IFF_GIF,
	WMP = D3DX10_IFF_WMP,
	ForceDword = D3DX10_IFF_FORCE_DWORD
}; 

[System::Flags]
public enum class MeshFlag
{
    x32 = D3DX10_MESH_32_BIT,
    GS_Adjacency = D3DX10_MESH_GS_ADJACENCY
};

[System::Flags]
public enum class SpriteFlag
{
	GroupByTexture = D3DX10_SPRITE_SORT_TEXTURE,
	SortBackToFront = D3DX10_SPRITE_SORT_DEPTH_BACK_TO_FRONT,
	SortFrontToBack = D3DX10_SPRITE_SORT_DEPTH_FRONT_TO_BACK,
	SaveState = D3DX10_SPRITE_SAVE_STATE,
	IncreaseTextureReferenceCount = D3DX10_SPRITE_ADDREF_TEXTURES
};