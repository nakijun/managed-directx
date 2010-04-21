namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	[System::Flags]
	public enum class BindFlag
	{
		VertexBuffer = D3D10_BIND_VERTEX_BUFFER,
		IndexBuffer = D3D10_BIND_INDEX_BUFFER,
		ConstantBuffer = D3D10_BIND_CONSTANT_BUFFER,
		ShaderResource = D3D10_BIND_SHADER_RESOURCE, 
		StreamOutput = D3D10_BIND_STREAM_OUTPUT,
		RenderTarget = D3D10_BIND_RENDER_TARGET,
		DepthStencil = D3D10_BIND_DEPTH_STENCIL
	};

	[System::Flags]
	public enum class CPU_AccessFlag
	{
		Write = D3D10_CPU_ACCESS_WRITE,
		Read = D3D10_CPU_ACCESS_READ
	};

	[System::Flags]
	public enum class CreateDeviceFlag
	{
		SingleThreaded = D3D10_CREATE_DEVICE_SINGLETHREADED,
		Debug = D3D10_CREATE_DEVICE_DEBUG,
		SwitchToRef = D3D10_CREATE_DEVICE_SWITCH_TO_REF,
		PreventInternalThreadingOptimizations = D3D10_CREATE_DEVICE_PREVENT_INTERNAL_THREADING_OPTIMIZATIONS,
		AllowNullFromMap = D3D10_CREATE_DEVICE_ALLOW_NULL_FROM_MAP,
		BGRA_Support = D3D10_CREATE_DEVICE_BGRA_SUPPORT,
		StrictValidation = D3D10_CREATE_DEVICE_STRICT_VALIDATION
	};

	[System::Flags]
	public enum class ClearFlag
	{
		Depth = D3D10_CLEAR_DEPTH,
		Stencil = D3D10_CLEAR_STENCIL
	};

	public enum class DSV_Dimension
	{
		Unknown = D3D10_DSV_DIMENSION_UNKNOWN,
		Texture1D = D3D10_DSV_DIMENSION_TEXTURE1D ,
		Texture1DArray = D3D10_DSV_DIMENSION_TEXTURE1DARRAY,
		Texture2D = D3D10_DSV_DIMENSION_TEXTURE2D,
		Texture2DArray = D3D10_DSV_DIMENSION_TEXTURE2DARRAY,
		Texture2DMS = D3D10_DSV_DIMENSION_TEXTURE2DMS,
		Texture2DMSArray = D3D10_DSV_DIMENSION_TEXTURE2DMSARRAY
	};

	public enum class DriverType
	{	
		Hardware = D3D10_DRIVER_TYPE_HARDWARE,
		Reference = D3D10_DRIVER_TYPE_REFERENCE,
		Null = D3D10_DRIVER_TYPE_NULL,
		Software = D3D10_DRIVER_TYPE_SOFTWARE,
		Warp = D3D10_DRIVER_TYPE_WARP
	};

	[System::Flags]
	public enum class EffectFlag
	{
		CompileChildEffect = D3D10_EFFECT_COMPILE_CHILD_EFFECT,
		CompileAllowSlowOperations = D3D10_EFFECT_COMPILE_ALLOW_SLOW_OPS,
		SingleThreaded = D3D10_EFFECT_SINGLE_THREADED,
		VariableAnnotation = D3D10_EFFECT_VARIABLE_ANNOTATION,
		VariablePooled = D3D10_EFFECT_VARIABLE_POOLED,
		VariableExplicitBindPoint = D3D10_EFFECT_VARIABLE_EXPLICIT_BIND_POINT
	};

	public enum class Error
	{
		FileNotFound = D3D10_ERROR_FILE_NOT_FOUND,
		TooManyUniqueStateObjects = D3D10_ERROR_TOO_MANY_UNIQUE_STATE_OBJECTS,
		InvalidCall = D3DERR_INVALIDCALL,
		WasStillDrawing = D3DERR_WASSTILLDRAWING,
	};

	public enum class InputClassification
	{	
		InputPerVertexData = D3D10_INPUT_PER_VERTEX_DATA,
		InputPerInstanceData = D3D10_INPUT_PER_INSTANCE_DATA,
	};

	public enum class PrimitiveTopology
	{
		Undefined = D3D10_PRIMITIVE_TOPOLOGY_UNDEFINED,
		PointList = D3D10_PRIMITIVE_TOPOLOGY_POINTLIST,
		LineList = D3D10_PRIMITIVE_TOPOLOGY_LINELIST,
		LineStrip = D3D10_PRIMITIVE_TOPOLOGY_LINESTRIP,
		TriangleList = D3D10_PRIMITIVE_TOPOLOGY_TRIANGLELIST,
		TriangleStrip = D3D10_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP,
		LineListWithAdjacency = D3D10_PRIMITIVE_TOPOLOGY_LINELIST_ADJ,
		LineStripWithAdjacency = D3D10_PRIMITIVE_TOPOLOGY_LINESTRIP_ADJ,
		TriangleListWithAdjacency = D3D10_PRIMITIVE_TOPOLOGY_TRIANGLELIST_ADJ,
		TriangleStripWithAdjacency = D3D10_PRIMITIVE_TOPOLOGY_TRIANGLESTRIP_ADJ
	};

	public enum class ResourceDimension
	{
		Unknown = D3D10_RESOURCE_DIMENSION_UNKNOWN,
		Buffer = D3D10_RESOURCE_DIMENSION_BUFFER,
		Texture1D = D3D10_RESOURCE_DIMENSION_TEXTURE1D,
		Texture2D = D3D10_RESOURCE_DIMENSION_TEXTURE2D,
		Texture3D = D3D10_RESOURCE_DIMENSION_TEXTURE3D
	};

	[System::Flags]
	public enum class ResourceMiscFlag
	{
		GenerateMips = D3D10_RESOURCE_MISC_GENERATE_MIPS,
		Shared = D3D10_RESOURCE_MISC_SHARED,
		TextureCube = D3D10_RESOURCE_MISC_TEXTURECUBE,
		SharedKeyedMutex = D3D10_RESOURCE_MISC_SHARED_KEYEDMUTEX,
		GDI_Compatible = D3D10_RESOURCE_MISC_GDI_COMPATIBLE
	};

	public enum class RTV_Dimension
	{
		Unknown = D3D10_RTV_DIMENSION_UNKNOWN,
		Buffer = D3D10_RTV_DIMENSION_BUFFER,
		Texture1D = D3D10_RTV_DIMENSION_TEXTURE1D ,
		Texture1DArray = D3D10_RTV_DIMENSION_TEXTURE1DARRAY,
		Texture2D = D3D10_RTV_DIMENSION_TEXTURE2D,
		Texture2DArray = D3D10_RTV_DIMENSION_TEXTURE2DARRAY,
		Texture2DMS = D3D10_RTV_DIMENSION_TEXTURE2DMS,
		Texture2DMSArray = D3D10_RTV_DIMENSION_TEXTURE2DMSARRAY,
		Texture3D = D3D10_RTV_DIMENSION_TEXTURE3D
	};

	[System::Flags]
	public enum class ShaderFlag
	{
		Debug = D3D10_SHADER_DEBUG,
		SkipValidation = D3D10_SHADER_SKIP_VALIDATION,
		SkipOptimization = D3D10_SHADER_SKIP_OPTIMIZATION,
		PackMatrixRowMajor = D3D10_SHADER_PACK_MATRIX_ROW_MAJOR,
		PackMatrixColumnMajor = D3D10_SHADER_PACK_MATRIX_COLUMN_MAJOR,
		PartialPrecision = D3D10_SHADER_PARTIAL_PRECISION,
		ForceSoftwareVertexShader = D3D10_SHADER_FORCE_VS_SOFTWARE_NO_OPT,
		ForceSoftwarePixelShader = D3D10_SHADER_FORCE_PS_SOFTWARE_NO_OPT,
		NoPreshader = D3D10_SHADER_NO_PRESHADER,
		AvoidFlowControl = D3D10_SHADER_AVOID_FLOW_CONTROL,
		PreferFlowControl = D3D10_SHADER_PREFER_FLOW_CONTROL,
		EnableStrictness = D3D10_SHADER_ENABLE_STRICTNESS,
		EnableBackwardsCompatibility = D3D10_SHADER_ENABLE_BACKWARDS_COMPATIBILITY,
		EnableIEEEStrictness = D3D10_SHADER_IEEE_STRICTNESS,
		WarningsAreErrors = D3D10_SHADER_WARNINGS_ARE_ERRORS,
		OptimizationLevel0 = D3D10_SHADER_OPTIMIZATION_LEVEL0,
		OptimizationLevel1 = D3D10_SHADER_OPTIMIZATION_LEVEL1,
		OptimizationLevel2 = D3D10_SHADER_OPTIMIZATION_LEVEL2,
		OptimizationLevel3 = D3D10_SHADER_OPTIMIZATION_LEVEL3
	};

	public enum class Usage
	{
		Default = D3D10_USAGE_DEFAULT,
		Immutable = D3D10_USAGE_IMMUTABLE,
		Dynamic = D3D10_USAGE_DYNAMIC,
		Staging = D3D10_USAGE_STAGING
	};
}
}
}