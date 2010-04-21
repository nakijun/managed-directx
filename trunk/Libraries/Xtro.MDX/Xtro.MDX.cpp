#include "StdAfx.h"

// Standard headers
#include "Windows.h"

// Directx headers
#include "Microsoft DirectX SDK (August 2009)/Include/D3DX10.h"

// Base
using namespace System;
using namespace System::Collections::Generic;
using namespace System::Globalization;
using namespace System::Reflection;
using namespace System::Runtime::InteropServices;
using namespace System::Runtime::CompilerServices;

#include "Interface.h"
#include "IID_Converter.h"
#include "Unknown.h"
#include "Float4.h"
#include "UnmanagedMemory.h"

// DXGI
#include "Object.h"
#include "DeviceSubObject.h"
#include "Surface.h"
#include "ResourceDXGI.h"
#include "Adapter.h"
#include "Output.h"
#include "Rational.h"
#include "EnumsDXGI.h"
#include "ModeDescription.h"
#include "OutputDescription.h"
#include "AdapterDescription.h"
#include "SampleDescription.h"
#include "SurfaceDescription.h"
#include "SwapChainDescription.h"
#include "SwapChain.h"
#include "Factory.h"
#include "FunctionsDXGI.h"

// Direct3D10
using namespace Xtro::MDX::DXGI;

#include "EnumsD3D10.h"
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
#include "Texture1D_DSV.h"
#include "Texture1D_ArrayDSV.h"
#include "Texture2D_DSV.h"
#include "Texture2D_ArrayDSV.h"
#include "Texture2D_MS_DSV.h"
#include "Texture2D_MS_ArrayDSV.h"
#include "RenderTargetViewDescription.h"
#include "DepthStencilViewDescription.h"
#include "DeviceChild.h"
#include "View.h"
#include "Viewport.h"
#include "BufferDescription.h"
#include "TechniqueDescription.h"
#include "SubResourceData.h"
#include "InputElementDescription.h"
#include "PassDescription.h"
#include "RenderTargetView.h"
#include "DepthStencilView.h"
#include "ShaderResourceView.h"
#include "ResourceD3D10.h"
#include "InputLayout.h"
#include "Buffer.h"
#include "Texture2D_Description.h"
#include "Texture2D.h"
#include "DeviceD3D10.h"
#include "FunctionsD3D10.h"
#include "EffectPass.h"
#include "EffectTechnique.h"
#include "EffectVariable.h"
#include "EffectVectorVariable.h"
#include "EffectMatrixVariable.h"
#include "EffectShaderResourceVariable.h"
#include "EffectVariable_ForwardedMethods.h"
#include "Effect.h"
#include "EffectPool.h"
#include "Blob.h"

// Direct3DX10
using namespace Xtro::MDX::Direct3D10;

#include "EnumsD3DX10.h"
#include "Float16.h"
#include "Vector.h"
#include "Vector2.h"
#include "Vector3.h"
#include "Vector4.h"
#include "Matrix.h"
#include "ImageInfo.h"
#include "ImageLoadInfo.h"
#include "FunctionsD3DX10.h"
