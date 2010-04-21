namespace Xtro
{
namespace MDX
{
namespace Direct3DX10
{
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
}
}
}