#include "StdAfx.h"
#include "Windows.h"
#include "Microsoft DirectX SDK (August 2009)/Include/D3DX10.h"

#undef DrawText
#undef GetTextMetrics
#undef CreateFont
#undef CreateFontIndirect

using namespace System;
using namespace System::IO;
using namespace System::Drawing;
using namespace System::Windows::Forms::VisualStyles;
using namespace System::Collections::Generic;
using namespace System::Globalization;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace System::Runtime::CompilerServices;

namespace Xtro
{
namespace MDX
{
	#include "Enums.h"

	#include "Boolean8.h"
	#include "Byte2.h"
	#include "Byte8.h"
	#include "Byte16.h"
	#include "Float4.h"
	#include "Float8.h"
	#include "Float1025.h"

	#include "IID_Converter.h"
	#include "Interface.h"
	#include "Unknown.h"
	#include "UnmanagedMemory.h"

namespace Generic
{
	#include "UnmanagedMemoryGeneric.h"
}

namespace DXGI
{
	ref class Device;

	#include "EnumsDXGI.h"

	#include "AdapterDescription.h"
	#include "FrameStatistics.h"
	#include "GammaControlCapabilities.h"
	#include "MappedRectangle.h"
	#include "OutputDescription.h"
	#include "Rational.h"
	#include "ModeDescription.h"
	#include "Rgb.h"
	#include "Rgb1025.h"
	#include "GammaControl.h"
	#include "SampleDescription.h"
	#include "SharedResource.h"
	#include "SurfaceDescription.h"
	#include "SwapChainDescription.h"

	#include "Object.h"
	#include "DeviceSubObject.h"
	#include "ResourceDXGI.h"
	#include "Surface.h"
	#include "Output.h"
	#include "SwapChain.h"
	#include "Adapter.h"
	#include "Factory.h"
	#include "DeviceDXGI.h"

	#include "FunctionsDXGI.h"
}

namespace Direct3D10
{
	using namespace Xtro::MDX::DXGI;

	ref class Device;

	#include "EnumsD3D10.h"
	#include "ConstantsD3D10.h"

	#include "BlendDescription.h"
	#include "StateBlock.h"
	#include "Include.h"
	#include "ShaderMacro.h"
	#include "BufferRTV.h"
	#include "Texture1D_RTV.h"
	#include "Texture1D_ArrayRTV.h"
	#include "Texture2D_RTV.h"
	#include "Texture2D_ArrayRTV.h"
	#include "Texture2D_MS_RTV.h"
	#include "Texture2D_MS_ArrayRTV.h"
	#include "Texture3D_RTV.h"
	#include "BufferSRV.h"
	#include "Texture1D_SRV.h"
	#include "Texture1D_ArraySRV.h"
	#include "Texture2D_SRV.h"
	#include "Texture2D_ArraySRV.h"
	#include "Texture2D_MS_SRV.h"
	#include "Texture2D_MS_ArraySRV.h"
	#include "Texture3D_SRV.h"
	#include "TextureCubeSRV.h"
	#include "Texture1D_DSV.h"
	#include "Texture1D_ArrayDSV.h"
	#include "Texture2D_DSV.h"
	#include "Texture2D_ArrayDSV.h"
	#include "Texture2D_MS_DSV.h"
	#include "Texture2D_MS_ArrayDSV.h"
	#include "RenderTargetViewDescription.h"
	#include "DepthStencilViewDescription.h"
	#include "ShaderResourceViewDescription.h"
	#include "DeviceChild.h"
	#include "SwitchToRef.h"
	#include "Viewport.h"
	#include "InfoQueueFilterDescription.h"
	#include "InfoQueueFilter.h"
	#include "Message.h"
	#include "QueryDataPipelineStatistics.h"
	#include "QueryDataSO_Statistics.h"
	#include "QueryDataTimestampDisjoint.h"
	#include "QueryDescription.h"
	#include "SamplerDescription.h"
	#include "SO_DeclarationEntry.h"
	#include "EffectDescription.h"
	#include "EffectShaderDescription.h"
	#include "EffectTypeDescription.h"
	#include "EffectVariableDescription.h"
	#include "BufferDescription.h"
	#include "TechniqueDescription.h"
	#include "SubResourceData.h"
	#include "InputElementDescription.h"
	#include "PassDescription.h"
	#include "ResourceD3D10.h"
	#include "InputLayout.h"
	#include "BlendState.h"
	#include "Buffer.h"
	#include "Texture1D_Description.h"
	#include "Texture2D_Description.h"
	#include "Texture3D_Description.h"
	#include "MappedTexture2D.h"
	#include "MappedTexture3D.h"
	#include "Texture1D.h"
	#include "Texture2D.h"
	#include "Texture3D.h"
	#include "View.h"
	#include "RenderTargetView.h"
	#include "DepthStencilView.h"
	#include "ShaderResourceView.h"
	#include "DepthStencilOperationDescription.h"
	#include "DepthStencilDescription.h"
	#include "ShaderBufferDescription.h"
	#include "ShaderDescription.h"
	#include "ShaderInputBindDescription.h"
	#include "ShaderTypeDescription.h"
	#include "ShaderVariableDescription.h"
	#include "SignatureParameterDescription.h"
	#include "RasterizerDescription.h"			 
	#include "RasterizerState.h"
	#include "DepthStencilState.h"
	#include "Box.h"
	#include "CounterDescription.h"
	#include "CounterInfo.h"
	#include "DeviceD3D10.h"
	#include "DeviceChild_ForwardedMethods.h"
	#include "StateBlockMask.h"
	#include "EffectPass.h"
	#include "EffectTechnique.h"
	#include "EffectVariable.h"
	#include "EffectVectorVariable.h"
	#include "EffectMatrixVariable.h"
	#include "EffectScalarVariable.h"
	#include "EffectShaderResourceVariable.h"
	#include "EffectVariable_ForwardedMethods.h"
	#include "EffectShaderVariable.h"
	#include "PassShaderDescription.h"
	#include "Effect.h"
	#include "EffectPool.h"
	#include "Blob.h"
	#include "Asynchronous.h"
	#include "Counter.h"

	#include "FunctionsD3D10.h"
}

namespace Direct3DX10
{
	using namespace Xtro::MDX::Direct3D10;

	#include "EnumsD3DX10.h"
	#include "ConstantsD3DX10.h"

	#include "AttributeRange.h"
	#include "AttributeWeights.h"
	#include "Float16bit.h"
	#include "Color.h"
	#include "FontDescription.h"
	#include "ImageInfo.h"
	#include "ImageLoadInfo.h"
	#include "IntersectionInfo.h"
	#include "Matrix.h"
	#include "Plane.h"
	#include "Quaternion.h"
	#include "SkinningChannel.h"
	#include "TextureLoadInfo.h"
	#include "Vector.h"
	#include "Vector2.h"
	#include "SpriteStruct.h"
	#include "Vector3.h"
	#include "Vector4.h"
	#include "WeldEpsilons.h"

	#include "MeshBuffer.h"
	#include "Mesh.h"
	#include "Sprite.h"
	#include "Font.h"

	#include "FunctionsD3DX10.h"
	#include "Quaternion_ForwardedMethods.h"
}
}
}