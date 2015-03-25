# Xtro.MDX #

Xtro.MDX is a managed DirectX layer competing with SlimDX or similar libraries which enable .NET programmers to develop multimedia and game applications in their favorite .NET language like C#, VB, Delphi.NET, Oxygene, etc. The aspect of this library is to preserve the method call structure and the naming convention of native DirectX. So, everything is used in the same way with the DirectX SDK documentation. No additional logic is implemented and no extra method calls are made in the wrapper methods.

Original DirectX utility library, tutorials and samples from the SDK are also implemented with C# in this project. So, you can practice Microsoft sample codes to learn DirectX programming in .NET framework.

# News #

**April 13, 2012 :** It think it's time to end the beta period. First version of Xtro.MDX is released today.

# Details #

Supported APIs :

  * Direct3D 10
  * Direct3DX 10
  * DXGI
  * D3DCompiler
  * DXUT (Direct3D utilities library from DX SDK)

Differences from SlimDX :

  * Does not implement any additional functionalities. All methods are nearly same with DirectX C++ methods. They return result values, don't throw exceptions.

  * DirectX C++ SDK code examples, tutorials and help files are very easy to translate into managed code since all methods, structures, result values etc. were translated into managed code in the same structure and with the same names.

  * Manages the classes like COM interfaces. They have reference counters and they need to be released like in DirectX C++ SDK code examples.

  * Most structures are mapped exactly to their native counterparts so they are used by managed and native code directly without moving any data block. This is a huge performance boost.

## Minimum Requirements ##

Video Adapter supporting Microsoft DirectX 10

Windows Vista SP1

Microsoft .NET Framework 4.0

Visual C++ 2010 SP1 redistributable runtime (x86 or x64)

DirectX 10 June 2010 redistrubutable runtime
  * Include  `D3D10Ref.DLL ` with your software if you are going to use REF device.
  * Include  `D3D10SDKLayers.DLL ` with your software if you are going to use debug device.