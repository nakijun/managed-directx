[Flags]
public enum class ChannelFlag
{
	Red = D3DX10_CHANNEL_RED,
    Blue = D3DX10_CHANNEL_BLUE,
    Green = D3DX10_CHANNEL_GREEN,
    Alpha = D3DX10_CHANNEL_ALPHA,
    Luminance = D3DX10_CHANNEL_LUMINANCE 
};

public enum class CpuOptimization
{
	None = D3DX_NOT_OPTIMIZED,
    _3D_Now = D3DX_3DNOW_OPTIMIZED,
    SSE2 = D3DX_SSE2_OPTIMIZED,
    SSE = D3DX_SSE_OPTIMIZED 
};
		 
public enum class Error
{
    CannotModifyIndexBuffer = D3DX10_ERR_CANNOT_MODIFY_INDEX_BUFFER,
	InvalidMesh = D3DX10_ERR_INVALID_MESH, 
	CannotAttributeSort = D3DX10_ERR_CANNOT_ATTR_SORT, 
	SkinningNotSupported = D3DX10_ERR_SKINNING_NOT_SUPPORTED, 
	TooManyInfluences = D3DX10_ERR_TOO_MANY_INFLUENCES, 
	InvalidData = D3DX10_ERR_INVALID_DATA, 
	MeshHasNoData = D3DX10_ERR_LOADED_MESH_HAS_NO_DATA, 
	DuplicateNamedFragment = D3DX10_ERR_DUPLICATE_NAMED_FRAGMENT, 
	CannotRemoveLastItem = D3DX10_ERR_CANNOT_REMOVE_LAST_ITEM, 
};

[Flags]
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

public enum class ImageFileFormat
{
	BMP = D3DX10_IFF_BMP,
	JPG = D3DX10_IFF_JPG,
	PNG = D3DX10_IFF_PNG,
	DDS = D3DX10_IFF_DDS,
	TIFF = D3DX10_IFF_TIFF,
	GIF = D3DX10_IFF_GIF,
	WMP = D3DX10_IFF_WMP,
	ForceDWORD = D3DX10_IFF_FORCE_DWORD
}; 

[Flags]
public enum class MeshFlag
{
    x32 = D3DX10_MESH_32_BIT,
    GS_Adjacency = D3DX10_MESH_GS_ADJACENCY
};

[Flags]
public enum class MeshDiscardFlag
{
    AttiributeBuffer = D3DX10_MESH_DISCARD_ATTRIBUTE_BUFFER,
    AttiributeTable = D3DX10_MESH_DISCARD_ATTRIBUTE_TABLE,
    PointerReps = D3DX10_MESH_DISCARD_POINTREPS,
    Adjacency = D3DX10_MESH_DISCARD_ADJACENCY,
    DeviceBuffers = D3DX10_MESH_DISCARD_DEVICE_BUFFERS
};

public enum class MeshOptimization
{
    Compact = D3DX10_MESHOPT_COMPACT,
    AttributeSort = D3DX10_MESHOPT_ATTR_SORT,
    VertexCache = D3DX10_MESHOPT_VERTEX_CACHE,
    StripReorder = D3DX10_MESHOPT_STRIP_REORDER,
    IgnoreVertices = D3DX10_MESHOPT_IGNORE_VERTS,
    DoNotSplit = D3DX10_MESHOPT_DO_NOT_SPLIT,
    DeviceIndependent = D3DX10_MESHOPT_DEVICE_INDEPENDENT
};

[Flags]
public enum class NormalMapFlag
{
    MirrorU = D3DX10_NORMALMAP_MIRROR_U,
    MirrorV = D3DX10_NORMALMAP_MIRROR_V,
    Mirror = D3DX10_NORMALMAP_MIRROR,
    InvertSign = D3DX10_NORMALMAP_INVERTSIGN,
    ComputeOcclusion = D3DX10_NORMALMAP_COMPUTE_OCCLUSION
};

public enum class SkinInfoScale
{
	None = D3DX10_SKININFO_NO_SCALING,
	To1 = D3DX10_SKININFO_SCALE_TO_1,
	ToTotal = D3DX10_SKININFO_SCALE_TO_TOTAL
};

[Flags]
public enum class SpriteFlag
{
	SortTexture = D3DX10_SPRITE_SORT_TEXTURE,
	SortDepthBackToFront = D3DX10_SPRITE_SORT_DEPTH_BACK_TO_FRONT,
	SortDepthFrontToBack = D3DX10_SPRITE_SORT_DEPTH_FRONT_TO_BACK,
	SaveState = D3DX10_SPRITE_SAVE_STATE,
	AddReferenceTextures = D3DX10_SPRITE_ADDREF_TEXTURES
};

[Flags]
public enum class SaveTextureFlag
{
	UseInputBlob = D3DX10_STF_USEINPUTBLOB
};