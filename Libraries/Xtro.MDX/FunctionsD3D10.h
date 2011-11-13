public ref class Functions abstract sealed
{
public:
	static int CreateBlob(SIZE_T NumberOfBytes, [Out] Blob^% Buffer)
	{
		ID3D10Blob* pBuffer = 0;
		int Result = D3D10CreateBlob(NumberOfBytes, &pBuffer);

		if (pBuffer)
		{
			try { Buffer = (Blob^)Interface::Interfaces[IntPtr(pBuffer)]; }
			catch (KeyNotFoundException^) { Buffer = gcnew Blob(IntPtr(pBuffer)); }					
		}
		else Buffer = nullptr;

		return Result;
	}

	static int CreateDevice(Adapter^ Adapter, DriverType DriverType, Module^ Software, CreateDeviceFlag Flags, unsigned int SdkVersion, [Out] Device^% Device)
	{
		IDXGIAdapter* pAdapter = Adapter == nullptr ? 0 : Adapter->pAdapter;
		HMODULE hSoftware = Software == nullptr ? 0 : (HMODULE)Marshal::GetHINSTANCE(Software).ToPointer();

		ID3D10Device* pDevice = 0;
		int Result = D3D10CreateDevice(pAdapter, (D3D10_DRIVER_TYPE)DriverType, hSoftware, (unsigned int)Flags, SdkVersion, &pDevice);

		if (pDevice)
		{
			try { Device = (Xtro::MDX::Direct3D10::Device^)Interface::Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }					
		}
		else Device = nullptr;

		return Result;
	}

	static int CreateDeviceAndSwapChain(Adapter^ Adapter, DriverType DriverType, Module^ Software, CreateDeviceFlag Flags, unsigned int SdkVersion, SwapChainDescription% SwapChainDescription, [Out] SwapChain^% SwapChain, [Out] Device^% Device)
	{
		IDXGIAdapter* pAdapter = Adapter == nullptr ? 0 : Adapter->pAdapter;
		HMODULE hSoftware = Software == nullptr ? 0 : (HMODULE)Marshal::GetHINSTANCE(Software).ToPointer();
		pin_ptr<Xtro::MDX::DXGI::SwapChainDescription> PinnedSwapChainDescription = &SwapChainDescription;

		ID3D10Device* pDevice = 0;
		IDXGISwapChain* pSwapChain = 0;
		int Result = D3D10CreateDeviceAndSwapChain(pAdapter, (D3D10_DRIVER_TYPE)DriverType, hSoftware, (unsigned int)Flags, SdkVersion, (DXGI_SWAP_CHAIN_DESC*)PinnedSwapChainDescription, &pSwapChain, &pDevice);

		if (pDevice)
		{
			try { Device = (Xtro::MDX::Direct3D10::Device^)Interface::Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }					
		}
		else Device = nullptr;

		if (pSwapChain)
		{
			try { SwapChain = (Xtro::MDX::DXGI::SwapChain^)Interface::Interfaces[IntPtr(pSwapChain)]; }
			catch (KeyNotFoundException^) { SwapChain = gcnew Xtro::MDX::DXGI::SwapChain(IntPtr(pSwapChain)); }
		}
		else SwapChain = nullptr;

		return Result;
	}

	static unsigned int CalculateSubresource(unsigned int MipSlice, unsigned int ArraySlice, unsigned int MipLevels)
	{
		return D3D10CalcSubresource(MipSlice, ArraySlice, MipLevels);
	}

	static int CompileShader(String^ SourceData, SIZE_T SourceDataLength, String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, String^ FunctionName, String^ Profile, ShaderFlag Flags, [Out] Blob^% Shader, [Out] Blob^% ErrorMessages)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pShader = 0;
		ID3D10Blob* pErrorMessages = 0;

		IntPtr pSourceData = IntPtr::Zero;
		IntPtr pFileName = IntPtr::Zero;
		IntPtr pFunctionName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pSourceData = Marshal::StringToHGlobalAnsi(SourceData);
			pFileName = Marshal::StringToHGlobalAnsi(FileName);
			pFunctionName = Marshal::StringToHGlobalAnsi(FunctionName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3D10CompileShader((LPCSTR)pSourceData.ToPointer(), SourceDataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pFunctionName.ToPointer(), (LPCSTR)pProfile.ToPointer(), (unsigned int)Flags, &pShader, &pErrorMessages);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceData); 
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pFunctionName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pShader)
		{
			try { Shader = (Blob^)Interface::Interfaces[IntPtr(pShader)]; }
			catch (KeyNotFoundException^) { Shader = gcnew Blob(IntPtr(pShader)); }
		}
		else Shader = nullptr;

		if (pErrorMessages)
		{
			try { ErrorMessages = (Blob^)Interface::Interfaces[IntPtr(pErrorMessages)]; }
			catch (KeyNotFoundException^) { ErrorMessages = gcnew Blob(IntPtr(pErrorMessages)); }
		}
		else ErrorMessages = nullptr;

		return Result;
	}
	
	static int CompileShader(String^ SourceData, SIZE_T SourceDataLength, String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, String^ FunctionName, String^ Profile, ShaderFlag Flags, [Out] Blob^% Shader)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pShader = 0;

		IntPtr pSourceData = IntPtr::Zero;
		IntPtr pFileName = IntPtr::Zero;
		IntPtr pFunctionName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pSourceData = Marshal::StringToHGlobalAnsi(SourceData);
			pFileName = Marshal::StringToHGlobalAnsi(FileName);
			pFunctionName = Marshal::StringToHGlobalAnsi(FunctionName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3D10CompileShader((LPCSTR)pSourceData.ToPointer(), SourceDataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pFunctionName.ToPointer(), (LPCSTR)pProfile.ToPointer(), (unsigned int)Flags, &pShader, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceData); 
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pFunctionName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pShader)
		{
			try { Shader = (Blob^)Interface::Interfaces[IntPtr(pShader)]; }
			catch (KeyNotFoundException^) { Shader = gcnew Blob(IntPtr(pShader)); }
		}
		else Shader = nullptr;

		return Result;
	}
	
	static int DisassembleShader(UnmanagedMemory^ Shader, SIZE_T BytecodeLength, bool EnableColorCode, String^ Comments, [Out] Blob^% Disassembly)
	{
		void* pShader = Shader == nullptr ? 0 : Shader->pMemory;

		int Result = 0;
		ID3D10Blob* pDisassembly = 0;

		IntPtr pComments = Marshal::StringToHGlobalAnsi(Comments);
		try
		{
			Result = D3D10DisassembleShader(pShader, BytecodeLength, EnableColorCode, (LPCSTR)pComments.ToPointer(), &pDisassembly);
		}
		finally	{ Marshal::FreeHGlobal(pComments); }

		if (pDisassembly)
		{
			try { Disassembly = (Blob^)Interface::Interfaces[IntPtr(pDisassembly)]; }
			catch (KeyNotFoundException^) { Disassembly = gcnew Blob(IntPtr(pDisassembly)); }
		}
		else Disassembly = nullptr;

		return Result;
	}

	static String^ GetGeometryShaderProfile(Device^ Device)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		return gcnew String(D3D10GetGeometryShaderProfile(pDevice));
	}

	static int GetInputAndOutputSignatureBlob(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, [Out] Blob^% SignatureBlob)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		ID3D10Blob* pSignatureBlob = 0;
		int Result = D3D10GetInputAndOutputSignatureBlob(pShaderBytecode, BytecodeLength, &pSignatureBlob);

		if (pSignatureBlob)
		{
			try { SignatureBlob = (Blob^)Interface::Interfaces[IntPtr(pSignatureBlob)]; }
			catch (KeyNotFoundException^) { SignatureBlob = gcnew Blob(IntPtr(pSignatureBlob)); }
		}
		else SignatureBlob = nullptr;

		return Result;
	}

	static int GetInputSignatureBlob(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, [Out] Blob^% SignatureBlob)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		ID3D10Blob* pSignatureBlob = 0;
		int Result = D3D10GetInputSignatureBlob(pShaderBytecode, BytecodeLength, &pSignatureBlob);

		if (pSignatureBlob)
		{
			try { SignatureBlob = (Blob^)Interface::Interfaces[IntPtr(pSignatureBlob)]; }
			catch (KeyNotFoundException^) { SignatureBlob = gcnew Blob(IntPtr(pSignatureBlob)); }
		}
		else SignatureBlob = nullptr;

		return Result;
	}

	static int GetOutputSignatureBlob(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, [Out] Blob^% SignatureBlob)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		ID3D10Blob* pSignatureBlob = 0;
		int Result = D3D10GetOutputSignatureBlob(pShaderBytecode, BytecodeLength, &pSignatureBlob);

		if (pSignatureBlob)
		{
			try { SignatureBlob = (Blob^)Interface::Interfaces[IntPtr(pSignatureBlob)]; }
			catch (KeyNotFoundException^) { SignatureBlob = gcnew Blob(IntPtr(pSignatureBlob)); }
		}
		else SignatureBlob = nullptr;

		return Result;
	}

	static String^ GetPixelShaderProfile(Device^ Device)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		return gcnew String(D3D10GetPixelShaderProfile(pDevice));
	}

	static int GetShaderDebugInfo(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, [Out] Blob^% DebugInfo)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		ID3D10Blob* pDebugInfo = 0;
		int Result = D3D10GetShaderDebugInfo(pShaderBytecode, BytecodeLength, &pDebugInfo);

		if (pDebugInfo)
		{
			try { DebugInfo = (Blob^)Interface::Interfaces[IntPtr(pDebugInfo)]; }
			catch (KeyNotFoundException^) { DebugInfo = gcnew Blob(IntPtr(pDebugInfo)); }
		}
		else DebugInfo = nullptr;

		return Result;
	}

	static String^ GetVertexShaderProfile(Device^ Device)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		return gcnew String(D3D10GetVertexShaderProfile(pDevice));
	}

	static int PreprocessShader(String^ SourceData, SIZE_T SourceDataSize, String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, [Out] Blob^% ShaderText, [Out] Blob^% ErrorMessages)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pShaderText = 0;
		ID3D10Blob* pErrorMessages = 0;

		IntPtr pSourceData = IntPtr::Zero;
		IntPtr pFileName = IntPtr::Zero;
		IntPtr pFunctionName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pSourceData = Marshal::StringToHGlobalAnsi(SourceData);
			pFileName = Marshal::StringToHGlobalAnsi(FileName);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3D10PreprocessShader((LPCSTR)pSourceData.ToPointer(), SourceDataSize, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, &pShaderText, &pErrorMessages);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceData); 
			Marshal::FreeHGlobal(pFileName); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pShaderText)
		{
			try { ShaderText = (Blob^)Interface::Interfaces[IntPtr(pShaderText)]; }
			catch (KeyNotFoundException^) { ShaderText = gcnew Blob(IntPtr(pShaderText)); }
		}
		else ShaderText = nullptr;

		if (pErrorMessages)
		{
			try { ErrorMessages = (Blob^)Interface::Interfaces[IntPtr(pErrorMessages)]; }
			catch (KeyNotFoundException^) { ErrorMessages = gcnew Blob(IntPtr(pErrorMessages)); }
		}
		else ErrorMessages = nullptr;

		return Result;
	}

	static int PreprocessShader(String^ SourceData, SIZE_T SourceDataSize, String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, [Out] Blob^% ShaderText)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pShaderText = 0;
	
		IntPtr pSourceData = IntPtr::Zero;
		IntPtr pFileName = IntPtr::Zero;
		IntPtr pFunctionName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pSourceData = Marshal::StringToHGlobalAnsi(SourceData);
			pFileName = Marshal::StringToHGlobalAnsi(FileName);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3D10PreprocessShader((LPCSTR)pSourceData.ToPointer(), SourceDataSize, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, &pShaderText, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceData); 
			Marshal::FreeHGlobal(pFileName); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pShaderText)
		{
			try { ShaderText = (Blob^)Interface::Interfaces[IntPtr(pShaderText)]; }
			catch (KeyNotFoundException^) { ShaderText = gcnew Blob(IntPtr(pShaderText)); }
		}
		else ShaderText = nullptr;

		return Result;
	}

	static int ReflectShader(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, [Out] ShaderReflection^% Reflector)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		ID3D10ShaderReflection* pReflector = 0;
		int Result = D3D10ReflectShader(pShaderBytecode, BytecodeLength, &pReflector);

		if (pReflector)
		{
			try { Reflector = (ShaderReflection^)Interface::Interfaces[IntPtr(pReflector)]; }
			catch (KeyNotFoundException^) { Reflector = gcnew ShaderReflection(IntPtr(pReflector)); }
		}
		else Reflector = nullptr;

		return Result;
	}

	static int CompileEffectFromMemory(UnmanagedMemory^ Data, SIZE_T DataLength, String^ SourceFileName, array<ShaderMacro>^ Defines, Include^ Include, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, [Out] Blob^% CompiledEffect, [Out] Blob^% Errors)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pCompiledEffect = 0;
		ID3D10Blob* pErrors = 0;

		IntPtr pFileName = Marshal::StringToHGlobalAnsi(SourceFileName);
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3D10CompileEffectFromMemory(pData, DataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, &pCompiledEffect, &pErrors);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pCompiledEffect)
		{
			try { CompiledEffect = (Blob^)Interface::Interfaces[IntPtr(pCompiledEffect)]; }
			catch (KeyNotFoundException^) { CompiledEffect = gcnew Blob(IntPtr(pCompiledEffect)); }
		}
		else CompiledEffect = nullptr;

		if (pErrors)
		{
			try { Errors = (Xtro::MDX::Direct3D10::Blob^)Interface::Interfaces[IntPtr(pErrors)]; }
			catch (KeyNotFoundException^) { Errors = gcnew Xtro::MDX::Direct3D10::Blob(IntPtr(pErrors)); }
		}
		else Errors = nullptr;

		return Result;
	}
		
	static int CompileEffectFromMemory(UnmanagedMemory^ Data, SIZE_T DataLength, String^ SourceFileName, array<ShaderMacro>^ Defines, Include^ Include, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, [Out] Blob^% CompiledEffect)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pCompiledEffect = 0;

		IntPtr pFileName = Marshal::StringToHGlobalAnsi(SourceFileName);
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3D10CompileEffectFromMemory(pData, DataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, &pCompiledEffect, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pCompiledEffect)
		{
			try { CompiledEffect = (Blob^)Interface::Interfaces[IntPtr(pCompiledEffect)]; }
			catch (KeyNotFoundException^) { CompiledEffect = gcnew Blob(IntPtr(pCompiledEffect)); }
		}
		else CompiledEffect = nullptr;

		return Result;
	}
		
	static int CreateEffectFromMemory(UnmanagedMemory^ Data, SIZE_T DataLength, EffectFlag FX_Flags, Device^ Device, EffectPool^ EffectPool, [Out] Effect^% Effect)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		ID3D10EffectPool* pEffectPool = EffectPool == nullptr ? 0 : EffectPool->pEffectPool;

		ID3D10Effect* pEffect = 0;
		int Result = D3D10CreateEffectFromMemory(pData, DataLength, (unsigned int)FX_Flags, pDevice, pEffectPool, &pEffect);

		if (pEffect)
		{
			try { Effect = (Xtro::MDX::Direct3D10::Effect^)Interface::Interfaces[IntPtr(pEffect)]; }
			catch (KeyNotFoundException^) { Effect = gcnew Xtro::MDX::Direct3D10::Effect(IntPtr(pEffect)); }
		}
		else Effect = nullptr;

		return Result;
	}
	
	static int CreateEffectPoolFromMemory(UnmanagedMemory^ Data, SIZE_T DataLength, EffectFlag FX_Flags, Device^ Device, [Out] EffectPool^% EffectPool)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		ID3D10EffectPool* pEffectPool = 0;
		int Result = D3D10CreateEffectPoolFromMemory(pData, DataLength, (unsigned int)FX_Flags, pDevice, &pEffectPool);

		if (pEffectPool)
		{
			try { EffectPool = (Xtro::MDX::Direct3D10::EffectPool^)Interface::Interfaces[IntPtr(pEffectPool)]; }
			catch (KeyNotFoundException^) { EffectPool = gcnew Xtro::MDX::Direct3D10::EffectPool(IntPtr(pEffectPool)); }
		}
		else EffectPool = nullptr;

		return Result;
	}
	
	static int CreateStateBlock(Device^ Device, StateBlockMask% StateBlockMask, [Out] StateBlock^% StateBlock)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		pin_ptr<Xtro::MDX::Direct3D10::StateBlockMask> PinnedStateBlockMask = &StateBlockMask;

		ID3D10StateBlock* pStateBlock = 0;
		int Result = D3D10CreateStateBlock(pDevice, (D3D10_STATE_BLOCK_MASK*)PinnedStateBlockMask, &pStateBlock);

		if (pStateBlock)
		{
			try { StateBlock = (Xtro::MDX::Direct3D10::StateBlock^)Interface::Interfaces[IntPtr(pStateBlock)]; }
			catch (KeyNotFoundException^) { StateBlock = gcnew Xtro::MDX::Direct3D10::StateBlock(IntPtr(pStateBlock)); }					
		}
		else StateBlock = nullptr;

		return Result;
	}

	static int DisassembleEffect(Effect^ Effect, bool EnableColorCode, [Out] Blob^% Disassembly)
	{
		ID3D10Effect* pEffect = Effect == nullptr ? 0 : Effect->pEffect;

		ID3D10Blob* pDisassembly = 0;
		int Result = D3D10DisassembleEffect(pEffect, EnableColorCode, &pDisassembly);

		if (pDisassembly)
		{
			try { Disassembly = (Blob^)Interface::Interfaces[IntPtr(pDisassembly)]; }
			catch (KeyNotFoundException^) { Disassembly = gcnew Blob(IntPtr(pDisassembly)); }
		}
		else Disassembly = nullptr;

		return Result;
	}

	static int StateBlockMaskDifference(StateBlockMask% A, StateBlockMask% B, [Out] StateBlockMask% Result)
	{
		pin_ptr<StateBlockMask> PinnedA = &A;
		pin_ptr<StateBlockMask> PinnedB = &B;
		pin_ptr<StateBlockMask> PinnedResult = &Result;

		return D3D10StateBlockMaskDifference((D3D10_STATE_BLOCK_MASK*)PinnedA, (D3D10_STATE_BLOCK_MASK*)PinnedB, (D3D10_STATE_BLOCK_MASK*)PinnedResult);
	}

	static int StateBlockMaskDisableAll([Out] StateBlockMask% Mask)
	{
		pin_ptr<StateBlockMask> PinnedMask = &Mask;
		return D3D10StateBlockMaskDisableAll((D3D10_STATE_BLOCK_MASK*)PinnedMask);
	}

	static int StateBlockMaskDisableCapture(StateBlockMask% Mask, DeviceStateType StateType, unsigned int RangeStart, unsigned int RangeLength)
	{
		pin_ptr<StateBlockMask> PinnedMask = &Mask;
		return D3D10StateBlockMaskDisableCapture((D3D10_STATE_BLOCK_MASK*)PinnedMask, (D3D10_DEVICE_STATE_TYPES)StateType, RangeStart, RangeLength);
	}

	static int StateBlockMaskEnableAll([Out] StateBlockMask% Mask)
	{
		pin_ptr<StateBlockMask> PinnedMask = &Mask;
		return D3D10StateBlockMaskEnableAll((D3D10_STATE_BLOCK_MASK*)PinnedMask);
	}

	static int StateBlockMaskEnableCapture(StateBlockMask% Mask, DeviceStateType StateType, unsigned int RangeStart, unsigned int RangeLength)
	{
		pin_ptr<StateBlockMask> PinnedMask = &Mask;
		return D3D10StateBlockMaskEnableCapture((D3D10_STATE_BLOCK_MASK*)PinnedMask, (D3D10_DEVICE_STATE_TYPES)StateType, RangeStart, RangeLength);
	}

	static int StateBlockMaskGetSetting(StateBlockMask% Mask, DeviceStateType StateType, unsigned int Entry)
	{
		pin_ptr<StateBlockMask> PinnedMask = &Mask;
		return D3D10StateBlockMaskGetSetting((D3D10_STATE_BLOCK_MASK*)PinnedMask, (D3D10_DEVICE_STATE_TYPES)StateType, Entry);
	}

	static int StateBlockMaskIntersect(StateBlockMask% A, StateBlockMask% B, [Out] StateBlockMask% Result)
	{
		pin_ptr<StateBlockMask> PinnedA = &A;
		pin_ptr<StateBlockMask> PinnedB = &B;
		pin_ptr<StateBlockMask> PinnedResult = &Result;

		return D3D10StateBlockMaskIntersect((D3D10_STATE_BLOCK_MASK*)PinnedA, (D3D10_STATE_BLOCK_MASK*)PinnedB, (D3D10_STATE_BLOCK_MASK*)PinnedResult);
	}

	static int StateBlockMaskUnion(StateBlockMask% A, StateBlockMask% B, [Out] StateBlockMask% Result)
	{
		pin_ptr<StateBlockMask> PinnedA = &A;
		pin_ptr<StateBlockMask> PinnedB = &B;
		pin_ptr<StateBlockMask> PinnedResult = &Result;

		return D3D10StateBlockMaskUnion((D3D10_STATE_BLOCK_MASK*)PinnedA, (D3D10_STATE_BLOCK_MASK*)PinnedB, (D3D10_STATE_BLOCK_MASK*)PinnedResult);
	}
};