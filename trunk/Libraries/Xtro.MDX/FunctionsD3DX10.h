public ref class Functions abstract sealed
{
private:
	inline static Xtro::MDX::Direct3D10::Resource^ CreateTextureByType(ID3D10Resource*	pResource)
	{
		D3D10_RESOURCE_DIMENSION Type;
		pResource->GetType(&Type);

		switch (Type)
		{
		case D3D10_RESOURCE_DIMENSION_TEXTURE1D : 
			try { return (Xtro::MDX::Direct3D10::Resource^)Interface::Interfaces[IntPtr(pResource)]; }
			catch (KeyNotFoundException^) { return gcnew Texture1D(IntPtr(pResource)); }
			break;
		case D3D10_RESOURCE_DIMENSION_TEXTURE2D : 
			try { return (Xtro::MDX::Direct3D10::Resource^)Interface::Interfaces[IntPtr(pResource)]; }
			catch (KeyNotFoundException^) { return gcnew Texture2D(IntPtr(pResource)); }
			break;
		case D3D10_RESOURCE_DIMENSION_TEXTURE3D : 
			try { return (Xtro::MDX::Direct3D10::Resource^)Interface::Interfaces[IntPtr(pResource)]; }
			catch (KeyNotFoundException^) { return gcnew Texture3D(IntPtr(pResource)); }
			break;
		default: return nullptr;
		}
	}

public:	
	static double ToRadian(double Degree)
	{
		return Degree * (D3DX_PI / 180.0);
	}

	static double ToDegree(double Radian)
	{
		return Radian * (180.0 / D3DX_PI);
	}

	static int CheckVersion(unsigned int D3D_SdkVersion, unsigned int D3DX10_SdkVersion)
	{
		return D3DX10CheckVersion(D3D_SdkVersion, D3DX10_SdkVersion);
	}

	static int CompileFromFile(String^ SourceFile, array<ShaderMacro>^ Defines, Include^ Include, String^ FunctionName, String^ Profile, ShaderFlag Flags1, EffectFlag Flags2, [Out] Blob^% Shader, [Out] Blob^% ErrorMessages)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pShader = 0;
		ID3D10Blob* pErrorMessages = 0;

		IntPtr pSourceFile = IntPtr::Zero;
		IntPtr pFunctionName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pSourceFile = Marshal::StringToHGlobalUni(SourceFile);
			pFunctionName = Marshal::StringToHGlobalUni(FunctionName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CompileFromFile((LPCWSTR)pSourceFile.ToPointer(), pDefines, pInclude, (LPCSTR)pFunctionName.ToPointer(), (LPCSTR)pProfile.ToPointer(), (unsigned int)Flags1, (unsigned int)Flags2, 0, &pShader, &pErrorMessages, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceFile); 
			Marshal::FreeHGlobal(pFunctionName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines)
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}

				 delete[] pDefines;
			}
		}

		if (pShader)
		{
			try { Shader = (Blob^)Interface::Interfaces[IntPtr(pShader)]; }
			catch (KeyNotFoundException^) { Shader = gcnew Blob(IntPtr(pShader)); }
		}
		else Shader = nullptr;

		if (pErrorMessages)
		{
			try { ErrorMessages = (Xtro::MDX::Direct3D10::Blob^)Interface::Interfaces[IntPtr(pErrorMessages)]; }
			catch (KeyNotFoundException^) { ErrorMessages = gcnew Xtro::MDX::Direct3D10::Blob(IntPtr(pErrorMessages)); }
		}
		else ErrorMessages = nullptr;

		return Result;
	}
	
	static int CompileFromFile(String^ SourceFile, array<ShaderMacro>^ Defines, Include^ Include, String^ FunctionName, String^ Profile, ShaderFlag Flags1, EffectFlag Flags2, [Out] Blob^% Shader)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pShader = 0;

		IntPtr pSourceFile = IntPtr::Zero;
		IntPtr pFunctionName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pSourceFile = Marshal::StringToHGlobalUni(SourceFile);
			pFunctionName = Marshal::StringToHGlobalUni(FunctionName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CompileFromFile((LPCWSTR)pSourceFile.ToPointer(), pDefines, pInclude, (LPCSTR)pFunctionName.ToPointer(), (LPCSTR)pProfile.ToPointer(), (unsigned int)Flags1, (unsigned int)Flags2, 0, &pShader, 0, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceFile); 
			Marshal::FreeHGlobal(pFunctionName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines)
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}

				 delete[] pDefines;
			}
		}

		if (pShader)
		{
			try { Shader = (Blob^)Interface::Interfaces[IntPtr(pShader)]; }
			catch (KeyNotFoundException^) { Shader = gcnew Blob(IntPtr(pShader)); }
		}
		else Shader = nullptr;

		return Result;
	}
	
	static int CompileFromMemory(String^ SourceData, SIZE_T SourceDataLength, String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, String^ FunctionName, String^ Profile, ShaderFlag Flags1, EffectFlag Flags2, [Out] Blob^% Shader, [Out] Blob^% ErrorMessages)
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
			pSourceData = Marshal::StringToHGlobalUni(SourceData);
			pFileName = Marshal::StringToHGlobalUni(FileName);
			pFunctionName = Marshal::StringToHGlobalUni(FunctionName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CompileFromMemory((LPCSTR)pSourceData.ToPointer(), SourceDataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pFunctionName.ToPointer(), (LPCSTR)pProfile.ToPointer(), (unsigned int)Flags1, (unsigned int)Flags2, 0, &pShader, &pErrorMessages, 0);
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

				 delete[] pDefines;
			}
		}

		if (pShader)
		{
			try { Shader = (Blob^)Interface::Interfaces[IntPtr(pShader)]; }
			catch (KeyNotFoundException^) { Shader = gcnew Blob(IntPtr(pShader)); }
		}
		else Shader = nullptr;

		if (pErrorMessages)
		{
			try { ErrorMessages = (Xtro::MDX::Direct3D10::Blob^)Interface::Interfaces[IntPtr(pErrorMessages)]; }
			catch (KeyNotFoundException^) { ErrorMessages = gcnew Xtro::MDX::Direct3D10::Blob(IntPtr(pErrorMessages)); }
		}
		else ErrorMessages = nullptr;

		return Result;
	}
	
	static int CompileFromMemory(String^ SourceData, SIZE_T SourceDataLength, String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, String^ FunctionName, String^ Profile, ShaderFlag Flags1, EffectFlag Flags2, [Out] Blob^% Shader)
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
			pSourceData = Marshal::StringToHGlobalUni(SourceData);
			pFileName = Marshal::StringToHGlobalUni(FileName);
			pFunctionName = Marshal::StringToHGlobalUni(FunctionName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CompileFromMemory((LPCSTR)pSourceData.ToPointer(), SourceDataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pFunctionName.ToPointer(), (LPCSTR)pProfile.ToPointer(), (unsigned int)Flags1, (unsigned int)Flags2, 0, &pShader, 0, 0);
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

				 delete[] pDefines;
			}
		}

		if (pShader)
		{
			try { Shader = (Blob^)Interface::Interfaces[IntPtr(pShader)]; }
			catch (KeyNotFoundException^) { Shader = gcnew Blob(IntPtr(pShader)); }
		}
		else Shader = nullptr;

		return Result;
	}
	
	static int CreateDevice(Adapter^ Adapter, DriverType DriverType, Module^ Software, CreateDeviceFlag Flags, [Out] Xtro::MDX::Direct3D10::Device^% Device_)
	{
		IDXGIAdapter* pAdapter = Adapter == nullptr ? 0 : Adapter->pAdapter;
		HMODULE hSoftware = Software == nullptr ? 0 : (HMODULE)Marshal::GetHINSTANCE(Software).ToPointer();

		ID3D10Device* pDevice = 0;
		int Result = D3DX10CreateDevice(pAdapter, (D3D10_DRIVER_TYPE)DriverType, hSoftware, (unsigned int)Flags, &pDevice);

		if (pDevice)
		{
			try { Device_ = (Xtro::MDX::Direct3D10::Device^)Interface::Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device_ = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }					
		}
		else Device_ = nullptr;

		return Result;
	}

	static int CreateDeviceAndSwapChain(Adapter^ Adapter, DriverType DriverType, Module^ Software, CreateDeviceFlag Flags, SwapChainDescription% SwapChainDescription, [Out] SwapChain^% SwapChain, [Out] Xtro::MDX::Direct3D10::Device^% Device_)
	{
		IDXGIAdapter* pAdapter = Adapter == nullptr ? 0 : Adapter->pAdapter;
		HMODULE hSoftware = Software == nullptr ? 0 : (HMODULE)Marshal::GetHINSTANCE(Software).ToPointer();
		pin_ptr<Xtro::MDX::DXGI::SwapChainDescription> PinnedSwapChainDescription = &SwapChainDescription;

		ID3D10Device* pDevice = 0;
		IDXGISwapChain* pSwapChain = 0;
		int Result = D3DX10CreateDeviceAndSwapChain(pAdapter, (D3D10_DRIVER_TYPE)DriverType, hSoftware, (unsigned int)Flags, (DXGI_SWAP_CHAIN_DESC*)PinnedSwapChainDescription, &pSwapChain, &pDevice);

		if (pDevice)
		{
			try { Device_ = (Xtro::MDX::Direct3D10::Device^)Interface::Interfaces[IntPtr(pDevice)]; }
			catch (KeyNotFoundException^) { Device_ = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }					
		}
		else Device_ = nullptr;

		if (pSwapChain)
		{
			try { SwapChain = (Xtro::MDX::DXGI::SwapChain^)Interface::Interfaces[IntPtr(pSwapChain)]; }
			catch (KeyNotFoundException^) { SwapChain = gcnew Xtro::MDX::DXGI::SwapChain(IntPtr(pSwapChain)); }
		}
		else SwapChain = nullptr;

		return Result;
	}

	static int CreateEffectFromFile(String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, String^ Profile, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, Xtro::MDX::Direct3D10::Device^ Device, EffectPool^ EffectPool, [Out] Effect^% Effect, [Out] Blob^% Errors)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		ID3D10EffectPool* pEffectPool = EffectPool == nullptr ? 0 : EffectPool->pEffectPool;

		int Result = 0;
		ID3D10Effect* pEffect = 0;
		ID3D10Blob* pErrors = 0;

		IntPtr pFileName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pFileName = Marshal::StringToHGlobalUni(FileName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CreateEffectFromFile((LPCWSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, pEffectPool, 0, &pEffect, &pErrors, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines)
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}

				 delete[] pDefines;
			}
		}

		if (pEffect)
		{
			try { Effect = (Xtro::MDX::Direct3D10::Effect^)Interface::Interfaces[IntPtr(pEffect)]; }
			catch (KeyNotFoundException^) { Effect = gcnew Xtro::MDX::Direct3D10::Effect(IntPtr(pEffect)); }
		}
		else Effect = nullptr;

		if (pErrors)
		{
			try { Errors = (Xtro::MDX::Direct3D10::Blob^)Interface::Interfaces[IntPtr(pErrors)]; }
			catch (KeyNotFoundException^) { Errors = gcnew Xtro::MDX::Direct3D10::Blob(IntPtr(pErrors)); }
		}
		else Errors = nullptr;

		return Result;
	}
	
	static int CreateEffectFromFile(String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, String^ Profile, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, Xtro::MDX::Direct3D10::Device^ Device, EffectPool^ EffectPool, [Out] Effect^% Effect)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		ID3D10EffectPool* pEffectPool = EffectPool == nullptr ? 0 : EffectPool->pEffectPool;

		int Result = 0;
		ID3D10Effect* pEffect = 0;

		IntPtr pFileName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pFileName = Marshal::StringToHGlobalUni(FileName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CreateEffectFromFile((LPCWSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, pEffectPool, 0, &pEffect, 0, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}

				delete[] pDefines;
			}
		}

		if (pEffect)
		{
			try { Effect = (Xtro::MDX::Direct3D10::Effect^)Interface::Interfaces[IntPtr(pEffect)]; }
			catch (KeyNotFoundException^) { Effect = gcnew Xtro::MDX::Direct3D10::Effect(IntPtr(pEffect)); }
		}
		else Effect = nullptr;

		return Result;
	}

	static int CreateEffectFromMemory(UnmanagedMemory^ Data, SIZE_T DataLength, String^ SourceFileName, array<ShaderMacro>^ Defines, Include^ Include, String^ Profile, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, Xtro::MDX::Direct3D10::Device^ Device, EffectPool^ EffectPool, [Out] Effect^% Effect, [Out] Blob^% Errors)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		ID3D10EffectPool* pEffectPool = EffectPool == nullptr ? 0 : EffectPool->pEffectPool;

		int Result = 0;
		ID3D10Effect* pEffect = 0;
		ID3D10Blob* pErrors = 0;

		IntPtr pFileName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pFileName = Marshal::StringToHGlobalAnsi(SourceFileName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CreateEffectFromMemory(pData, DataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, pEffectPool, 0, &pEffect, &pErrors, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
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

		if (pEffect)
		{
			try { Effect = (Xtro::MDX::Direct3D10::Effect^)Interface::Interfaces[IntPtr(pEffect)]; }
			catch (KeyNotFoundException^) { Effect = gcnew Xtro::MDX::Direct3D10::Effect(IntPtr(pEffect)); }
		}
		else Effect = nullptr;

		if (pErrors)
		{
			try { Errors = (Xtro::MDX::Direct3D10::Blob^)Interface::Interfaces[IntPtr(pErrors)]; }
			catch (KeyNotFoundException^) { Errors = gcnew Xtro::MDX::Direct3D10::Blob(IntPtr(pErrors)); }
		}
		else Errors = nullptr;

		return Result;
	}
	
	static int CreateEffectFromMemory(UnmanagedMemory^ Data, SIZE_T DataLength, String^ SourceFileName, array<ShaderMacro>^ Defines, Include^ Include, String^ Profile, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, Xtro::MDX::Direct3D10::Device^ Device, EffectPool^ EffectPool, [Out] Effect^% Effect)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		ID3D10EffectPool* pEffectPool = EffectPool == nullptr ? 0 : EffectPool->pEffectPool;

		int Result = 0;
		ID3D10Effect* pEffect = 0;

		IntPtr pFileName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pFileName = Marshal::StringToHGlobalAnsi(SourceFileName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CreateEffectFromMemory(pData, DataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, pEffectPool, 0, &pEffect, 0, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}

				delete[] pDefines;
			}
		}

		if (pEffect)
		{
			try { Effect = (Xtro::MDX::Direct3D10::Effect^)Interface::Interfaces[IntPtr(pEffect)]; }
			catch (KeyNotFoundException^) { Effect = gcnew Xtro::MDX::Direct3D10::Effect(IntPtr(pEffect)); }
		}
		else Effect = nullptr;

		return Result;
	}

	static int CreateEffectPoolFromFile(String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, String^ Profile, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, Xtro::MDX::Direct3D10::Device^ Device, [Out] EffectPool^% EffectPool, [Out] Blob^% Errors)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3D10EffectPool* pEffectPool = 0;
		ID3D10Blob* pErrors = 0;

		IntPtr pFileName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pFileName = Marshal::StringToHGlobalUni(FileName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CreateEffectPoolFromFile((LPCWSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, 0, &pEffectPool, &pErrors, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines)
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}

				 delete[] pDefines;
			}
		}

		if (pEffectPool)
		{
			try { EffectPool = (Xtro::MDX::Direct3D10::EffectPool^)Interface::Interfaces[IntPtr(pEffectPool)]; }
			catch (KeyNotFoundException^) { EffectPool = gcnew Xtro::MDX::Direct3D10::EffectPool(IntPtr(pEffectPool)); }
		}
		else EffectPool = nullptr;

		if (pErrors)
		{
			try { Errors = (Xtro::MDX::Direct3D10::Blob^)Interface::Interfaces[IntPtr(pErrors)]; }
			catch (KeyNotFoundException^) { Errors = gcnew Xtro::MDX::Direct3D10::Blob(IntPtr(pErrors)); }
		}
		else Errors = nullptr;

		return Result;
	}
	
	static int CreateEffectPollFromFile(String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, String^ Profile, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, Xtro::MDX::Direct3D10::Device^ Device, [Out] EffectPool^% EffectPool)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3D10EffectPool* pEffectPool = 0;

		IntPtr pFileName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pFileName = Marshal::StringToHGlobalUni(FileName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CreateEffectPoolFromFile((LPCWSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, 0, &pEffectPool, 0, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}

				delete[] pDefines;
			}
		}

		if (pEffectPool)
		{
			try { EffectPool = (Xtro::MDX::Direct3D10::EffectPool^)Interface::Interfaces[IntPtr(pEffectPool)]; }
			catch (KeyNotFoundException^) { EffectPool = gcnew Xtro::MDX::Direct3D10::EffectPool(IntPtr(pEffectPool)); }
		}
		else EffectPool = nullptr;

		return Result;
	}

	static int CreateEffectPoolFromMemory(UnmanagedMemory^ Data, SIZE_T DataLength, String^ SourceFileName, array<ShaderMacro>^ Defines, Include^ Include, String^ Profile, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, Xtro::MDX::Direct3D10::Device^ Device, [Out] EffectPool^% EffectPool, [Out] Blob^% Errors)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3D10EffectPool* pEffectPool = 0;
		ID3D10Blob* pErrors = 0;

		IntPtr pFileName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pFileName = Marshal::StringToHGlobalAnsi(SourceFileName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CreateEffectPoolFromMemory(pData, DataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, 0, &pEffectPool, &pErrors, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
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

		if (pEffectPool)
		{
			try { EffectPool = (Xtro::MDX::Direct3D10::EffectPool^)Interface::Interfaces[IntPtr(pEffectPool)]; }
			catch (KeyNotFoundException^) { EffectPool = gcnew Xtro::MDX::Direct3D10::EffectPool(IntPtr(pEffectPool)); }
		}
		else EffectPool = nullptr;

		if (pErrors)
		{
			try { Errors = (Xtro::MDX::Direct3D10::Blob^)Interface::Interfaces[IntPtr(pErrors)]; }
			catch (KeyNotFoundException^) { Errors = gcnew Xtro::MDX::Direct3D10::Blob(IntPtr(pErrors)); }
		}
		else Errors = nullptr;

		return Result;
	}
	
	static int CreateEffectPoolFromMemory(UnmanagedMemory^ Data, SIZE_T DataLength, String^ SourceFileName, array<ShaderMacro>^ Defines, Include^ Include, String^ Profile, ShaderFlag HLSL_Flags, EffectFlag FX_Flags, Xtro::MDX::Direct3D10::Device^ Device, [Out] EffectPool^% EffectPool)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3D10EffectPool* pEffectPool = 0;

		IntPtr pFileName = IntPtr::Zero;
		IntPtr pProfile = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pFileName = Marshal::StringToHGlobalAnsi(SourceFileName);
			pProfile = Marshal::StringToHGlobalAnsi(Profile);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DX10CreateEffectPoolFromMemory(pData, DataLength, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, 0, &pEffectPool, 0, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pProfile); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}

				delete[] pDefines;
			}
		}

		if (pEffectPool)
		{
			try { EffectPool = (Xtro::MDX::Direct3D10::EffectPool^)Interface::Interfaces[IntPtr(pEffectPool)]; }
			catch (KeyNotFoundException^) { EffectPool = gcnew Xtro::MDX::Direct3D10::EffectPool(IntPtr(pEffectPool)); }
		}
		else EffectPool = nullptr;

		return Result;
	}

	static int CreateFont(Xtro::MDX::Direct3D10::Device^ Device, int Height, unsigned int Width, unsigned int Weight, unsigned int MipLevels, bool Italic, FontCharacterSet CharSet, FontPrecision OutputPrecision, FontQuality Quality, FontPitchAndFamilyFlag PitchAndFamily, String^ FaceName, [Out] Font^% Font)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3DX10Font* pFont = 0;

		IntPtr pFaceName = Marshal::StringToHGlobalUni(FaceName);
		try { Result = D3DX10CreateFontW(pDevice, Height, Width, Weight, MipLevels, Italic, (unsigned int)CharSet, (unsigned int)OutputPrecision, (unsigned int)Quality, (unsigned int)PitchAndFamily, (LPCWSTR)pFaceName.ToPointer(), &pFont); }		
		finally { Marshal::FreeHGlobal(pFaceName); }

		if (pFont)
		{
			try { Font = (Xtro::MDX::Direct3DX10::Font^)Interface::Interfaces[IntPtr(pFont)]; }
			catch (KeyNotFoundException^) { Font = gcnew Xtro::MDX::Direct3DX10::Font(IntPtr(pFont)); }
		}
		else Font = nullptr;

		return Result;
	}

	static int CreateFontIndirect(Xtro::MDX::Direct3D10::Device^ Device, FontDescription% Description, [Out] Font^% Font)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		D3DX10_FONT_DESC NativeDescription;
		Description.ToNative(&NativeDescription);

		ID3DX10Font* pFont = 0;
		int Result = D3DX10CreateFontIndirectW(pDevice, &NativeDescription, &pFont);

		if (pFont)
		{
			try { Font = (Xtro::MDX::Direct3DX10::Font^)Interface::Interfaces[IntPtr(pFont)]; }
			catch (KeyNotFoundException^) { Font = gcnew Xtro::MDX::Direct3DX10::Font(IntPtr(pFont)); }
		}
		else Font = nullptr;

		return Result;
	}

	static int CreateSprite(Xtro::MDX::Direct3D10::Device^ Device, unsigned int DeviceBufferSize, [Out] Sprite^% Sprite)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		ID3DX10Sprite* pSprite = 0;
		int Result = D3DX10CreateSprite(pDevice, DeviceBufferSize, &pSprite);

		if (pSprite)
		{
			try { Sprite = (Xtro::MDX::Direct3DX10::Sprite^)Interface::Interfaces[IntPtr(pSprite)]; }
			catch (KeyNotFoundException^) { Sprite = gcnew Xtro::MDX::Direct3DX10::Sprite(IntPtr(pSprite)); }
		}
		else Sprite = nullptr;

		return Result;
	}

	static int PreprocessShaderFromFile(String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, [Out] Blob^% ShaderText, [Out] Blob^% ErrorMessages)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pShaderText = 0;
		ID3D10Blob* pErrorMessages = 0;

		IntPtr pFileName = Marshal::StringToHGlobalUni(FileName);
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

			Result = D3DX10PreprocessShaderFromFile((LPCWSTR)pFileName.ToPointer(), pDefines, pInclude, 0, &pShaderText, &pErrorMessages, 0);
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

				 delete[] pDefines;
			}
		}

		if (pShaderText)
		{
			try { ShaderText = (Blob^)Interface::Interfaces[IntPtr(pShaderText)]; }
			catch (KeyNotFoundException^) { ShaderText = gcnew Blob(IntPtr(pShaderText)); }
		}
		else ShaderText = nullptr;

		if (pErrorMessages)
		{
			try { ErrorMessages = (Xtro::MDX::Direct3D10::Blob^)Interface::Interfaces[IntPtr(pErrorMessages)]; }
			catch (KeyNotFoundException^) { ErrorMessages = gcnew Xtro::MDX::Direct3D10::Blob(IntPtr(pErrorMessages)); }
		}
		else ErrorMessages = nullptr;

		return Result;
	}
	
	static int PreprocessShaderFromFile(String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, [Out] Blob^% ShaderText)
	{
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pShaderText = 0;

		IntPtr pFileName = Marshal::StringToHGlobalUni(FileName);
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

			Result = D3DX10PreprocessShaderFromFile((LPCWSTR)pFileName.ToPointer(), pDefines, pInclude, 0, &pShaderText, 0, 0);
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

				 delete[] pDefines;
			}
		}

		if (pShaderText)
		{
			try { ShaderText = (Blob^)Interface::Interfaces[IntPtr(pShaderText)]; }
			catch (KeyNotFoundException^) { ShaderText = gcnew Blob(IntPtr(pShaderText)); }
		}
		else ShaderText = nullptr;

		return Result;
	}
	
	static int PreprocessShaderFromMemory(String^ SourceData, SIZE_T SourceDataSize, String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, [Out] Blob^% ShaderText, [Out] Blob^% ErrorMessages)
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

			Result = D3DX10PreprocessShaderFromMemory((LPCSTR)pSourceData.ToPointer(), SourceDataSize, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, 0, &pShaderText, &pErrorMessages, 0);
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

	static int PreprocessShaderFromMemory(String^ SourceData, SIZE_T SourceDataSize, String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, [Out] Blob^% ShaderText)
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

			Result = D3DX10PreprocessShaderFromMemory((LPCSTR)pSourceData.ToPointer(), SourceDataSize, (LPCSTR)pFileName.ToPointer(), pDefines, pInclude, 0, &pShaderText, 0, 0);
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

	static int UnsetAllDeviceObjects(Xtro::MDX::Direct3D10::Device^ Device)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		return D3DX10UnsetAllDeviceObjects(pDevice);
	}

	static void ColorAdjustContrast([Out] Color% Out, Color% Color, float C)
	{
		pin_ptr<Xtro::MDX::Direct3DX10::Color> PinnedOut = &Out;
		pin_ptr<Xtro::MDX::Direct3DX10::Color> PinnedColor = &Color;
		D3DXColorAdjustContrast((D3DXCOLOR*)PinnedOut, (D3DXCOLOR*)PinnedColor, C);
	}

	static void ColorAdjustSaturation([Out] Color% Out, Color% Color, float S)
	{
		pin_ptr<Xtro::MDX::Direct3DX10::Color> PinnedOut = &Out;
		pin_ptr<Xtro::MDX::Direct3DX10::Color> PinnedColor = &Color;
		D3DXColorAdjustSaturation((D3DXCOLOR*)PinnedOut, (D3DXCOLOR*)PinnedColor, S);
	}

	static void ColorLerp([Out] Color% Out, Color% Color1, Color% Color2, float S)
	{
		pin_ptr<Color> PinnedOut = &Out;
		pin_ptr<Color> PinnedColor1 = &Color1;
		pin_ptr<Color> PinnedColor2 = &Color2;
		D3DXColorLerp((D3DXCOLOR*)PinnedOut, (D3DXCOLOR*)PinnedColor1, (D3DXCOLOR*)PinnedColor2, S);
	}

	static int CreateMatrixStack(unsigned int Flags, [Out] MatrixStack^% Stack_)
	{
		ID3DXMatrixStack* pMatrixStack = 0;

		int Result = D3DXCreateMatrixStack(Flags, &pMatrixStack);

		if (pMatrixStack)
		{
			try { Stack_ = (MatrixStack^)Interface::Interfaces[IntPtr(pMatrixStack)]; }
			catch (KeyNotFoundException^) { Stack_ = gcnew MatrixStack(IntPtr(pMatrixStack)); }
		}
		else Stack_ = nullptr;

		return Result;
	}

	static CpuOptimization CpuOptimizations(bool Enable)
	{
		return (CpuOptimization)D3DXCpuOptimizations(Enable);
	}

	static void Float16To32Array(array<float>^ Out, array<Float16bit>^ In, unsigned int N)
	{
		pin_ptr<float> PinnedOut = Out != nullptr && Out->Length > 0 ? &Out[0] : nullptr;
		pin_ptr<Float16bit> PinnedIn = In != nullptr && In->Length > 0 ? &In[0] : nullptr;

		D3DXFloat16To32Array(PinnedOut, (D3DXFLOAT16*)PinnedIn, N);
	}

	static void Float32To16Array(array<Float16bit>^ Out, array<float>^ In, unsigned int N)
	{
		pin_ptr<Float16bit> PinnedOut = Out != nullptr && Out->Length > 0 ? &Out[0] : nullptr;
		pin_ptr<float> PinnedIn = In != nullptr && In->Length > 0 ? &In[0] : nullptr;

		D3DXFloat32To16Array((D3DXFLOAT16*)PinnedOut, PinnedIn, N);
	}

	static float FresnelTerm(float CosTheta, float RefractionIndex)
	{
		return D3DXFresnelTerm(CosTheta, RefractionIndex);
	}

	static void MatrixAffineTransformation([Out] Matrix% Out, float Scaling, Xtro::MDX::Generic::ValueObject<Vector3>^ RotationCenter, Xtro::MDX::Generic::ValueObject<Quaternion>^ Rotation, Xtro::MDX::Generic::ValueObject<Vector3>^ Translation)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedRotationCenter = RotationCenter == nullptr ? nullptr : &RotationCenter->Value;
		pin_ptr<Quaternion> PinnedRotation = Rotation == nullptr ? nullptr : &Rotation->Value;
		pin_ptr<Vector3> PinnedTranslation = Translation == nullptr ? nullptr : &Translation->Value;

		D3DXMatrixAffineTransformation((D3DXMATRIX*)PinnedOut, Scaling, (D3DXVECTOR3*)PinnedRotationCenter, (D3DXQUATERNION*)PinnedRotation, (D3DXVECTOR3*)PinnedTranslation);
	}

	static void MatrixAffineTransformation2D([Out] Matrix% Out, float Scaling, Vector2% RotationCenter, float Rotation, Vector2% Translation)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector2> PinnedRotationCenter = &RotationCenter;
		pin_ptr<Vector2> PinnedTranslation = &Translation;

		D3DXMatrixAffineTransformation2D((D3DXMATRIX*)PinnedOut, Scaling, (D3DXVECTOR2*)PinnedRotationCenter, Rotation, (D3DXVECTOR2*)PinnedTranslation);
	}

	static void MatrixAffineTransformation2D([Out] Matrix% Out, float Scaling, float Rotation, Vector2% Translation)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector2> PinnedTranslation = &Translation;

		D3DXMatrixAffineTransformation2D((D3DXMATRIX*)PinnedOut, Scaling, 0, Rotation, (D3DXVECTOR2*)PinnedTranslation);
	}

	static void MatrixAffineTransformation2D([Out] Matrix% Out, float Scaling, Vector2% RotationCenter, float Rotation)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector2> PinnedRotationCenter = &RotationCenter;

		D3DXMatrixAffineTransformation2D((D3DXMATRIX*)PinnedOut, Scaling, (D3DXVECTOR2*)PinnedRotationCenter, Rotation, 0);
	}

	static void MatrixAffineTransformation2D([Out] Matrix% Out, float Scaling, float Rotation)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixAffineTransformation2D((D3DXMATRIX*)PinnedOut, Scaling, 0, Rotation, 0);
	}

	static int MatrixDecompose([Out] Vector3% OutScale, [Out] Quaternion% OutRotation, [Out] Vector3% OutTranslation, Matrix% Matrix)
	{
		pin_ptr<Vector3> PinnedOutScale = &OutScale;
		pin_ptr<Quaternion> PinnedOutRotation = &OutRotation;
		pin_ptr<Vector3> PinnedOutTranslation = &OutTranslation;
		pin_ptr<Xtro::MDX::Direct3DX10::Matrix> PinnedMatrix = &Matrix;

		return D3DXMatrixDecompose((D3DXVECTOR3*)PinnedOutScale, (D3DXQUATERNION*)PinnedOutRotation, (D3DXVECTOR3*)PinnedOutTranslation, (D3DXMATRIX*)PinnedMatrix);
	}

	static float MatrixDeterminant(Matrix% Matrix)
	{
		pin_ptr<Xtro::MDX::Direct3DX10::Matrix> PinnedMatrix = &Matrix;
		return D3DXMatrixDeterminant((D3DXMATRIX*)PinnedMatrix);
	}

	static void MatrixInverse([Out] Matrix% Out, [Out] float% Determinant, Matrix% Matrix)
	{
		pin_ptr<Direct3DX10::Matrix> PinnedOut = &Out;
		pin_ptr<float> PinnedDeterminant = &Determinant;
		pin_ptr<Direct3DX10::Matrix> PinnedMatrix = &Matrix;

		D3DXMatrixInverse((D3DXMATRIX*)PinnedOut, PinnedDeterminant, (D3DXMATRIX*)PinnedMatrix);
	}

	static void MatrixInverse([Out] Matrix% Out, Matrix% Matrix)
	{
		pin_ptr<Direct3DX10::Matrix> PinnedOut = &Out;
		pin_ptr<Direct3DX10::Matrix> PinnedMatrix = &Matrix;

		D3DXMatrixInverse((D3DXMATRIX*)PinnedOut, 0, (D3DXMATRIX*)PinnedMatrix);
	}

	static void MatrixLookAtLH([Out] Matrix% Out, Vector3% Eye, Vector3% At, Vector3% Up)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedEye = &Eye;
		pin_ptr<Vector3> PinnedAt = &At;
		pin_ptr<Vector3> PinnedUp = &Up;

		D3DXMatrixLookAtLH((D3DXMATRIX*)PinnedOut, (D3DXVECTOR3*)PinnedEye, (D3DXVECTOR3*)PinnedAt, (D3DXVECTOR3*)PinnedUp);
	}

	static void MatrixLookAtRH([Out] Matrix% Out, Vector3% Eye, Vector3% At, Vector3% Up)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedEye = &Eye;
		pin_ptr<Vector3> PinnedAt = &At;
		pin_ptr<Vector3> PinnedUp = &Up;

		D3DXMatrixLookAtRH((D3DXMATRIX*)PinnedOut, (D3DXVECTOR3*)PinnedEye, (D3DXVECTOR3*)PinnedAt, (D3DXVECTOR3*)PinnedUp);
	}

	static void MatrixMultiply([Out] Matrix% Out, Matrix% Matrix1, Matrix% Matrix2)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Matrix> PinnedMatrix1 = &Matrix1;
		pin_ptr<Matrix> PinnedMatrix2 = &Matrix2;

		D3DXMatrixMultiply((D3DXMATRIX*)PinnedOut, (D3DXMATRIX*)PinnedMatrix1, (D3DXMATRIX*)PinnedMatrix2);
	}

	static void MatrixMultiplyTranspose([Out] Matrix% Out, Matrix% Matrix1, Matrix% Matrix2)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Matrix> PinnedMatrix1 = &Matrix1;
		pin_ptr<Matrix> PinnedMatrix2 = &Matrix2;

		D3DXMatrixMultiplyTranspose((D3DXMATRIX*)PinnedOut, (D3DXMATRIX*)PinnedMatrix1, (D3DXMATRIX*)PinnedMatrix2);
	}

	static void MatrixOrthoLH([Out] Matrix% Out, float W, float H, float ZN, float ZF)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixOrthoLH((D3DXMATRIX*)PinnedOut, W, H, ZN, ZF);
	}

	static void MatrixOrthoOffCenterLH([Out] Matrix% Out, float L, float R, float B, float T, float ZN, float ZF)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixOrthoOffCenterLH((D3DXMATRIX*)PinnedOut, L, R, B, T, ZN, ZF);
	}

	static void MatrixOrthoOffCenterRH([Out] Matrix% Out, float L, float R, float B, float T, float ZN, float ZF)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixOrthoOffCenterRH((D3DXMATRIX*)PinnedOut, L, R, B, T, ZN, ZF);
	}

	static void MatrixOrthoRH([Out] Matrix% Out, float W, float H, float ZN, float ZF)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixOrthoRH((D3DXMATRIX*)PinnedOut, W, H, ZN, ZF);
	}

	static void MatrixPerspectiveFovLH([Out] Matrix% Out, float FovY, float Aspect, float Z_Near, float Z_Far)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixPerspectiveFovLH((D3DXMATRIX*)PinnedOut, FovY, Aspect, Z_Near, Z_Far);
	}

	static void MatrixPerspectiveFovRH([Out] Matrix% Out, float FovY, float Aspect, float Z_Near, float Z_Far)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixPerspectiveFovRH((D3DXMATRIX*)PinnedOut, FovY, Aspect, Z_Near, Z_Far);
	}

	static void MatrixPerspectiveLH([Out] Matrix% Out, float W, float H, float Z_Near, float Z_Far)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixPerspectiveLH((D3DXMATRIX*)PinnedOut, W, H, Z_Near, Z_Far);
	}

	static void MatrixPerspectiveOffCenterLH([Out] Matrix% Out, float L, float R, float T, float B, float Z_Near, float Z_Far)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixPerspectiveOffCenterLH((D3DXMATRIX*)PinnedOut, L, R, T, B, Z_Near, Z_Far);
	}

	static void MatrixPerspectiveOffCenterRH([Out] Matrix% Out, float L, float R, float T, float B, float Z_Near, float Z_Far)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixPerspectiveOffCenterRH((D3DXMATRIX*)PinnedOut, L, R, T, B, Z_Near, Z_Far);
	}

	static void MatrixPerspectiveRH([Out] Matrix% Out, float W, float H, float Z_Near, float Z_Far)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixPerspectiveRH((D3DXMATRIX*)PinnedOut, W, H, Z_Near, Z_Far);
	}

	static void MatrixReflect([Out] Matrix% Out, Plane% Plane)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Xtro::MDX::Direct3DX10::Plane> PinnedPlane = &Plane;

		D3DXMatrixReflect((D3DXMATRIX*)PinnedOut, (D3DXPLANE*)PinnedPlane);
	}

	static void MatrixRotationAxis([Out] Matrix% Out, Vector3% Vector, float Angle)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedVector = &Vector;

		D3DXMatrixRotationAxis((D3DXMATRIX*)PinnedOut, (D3DXVECTOR3*)PinnedVector, Angle);
	}

	static void MatrixRotationQuaternion([Out] Matrix% Out, Quaternion% Quaternion)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Direct3DX10::Quaternion> PinnedQuaternion = &Quaternion;

		D3DXMatrixRotationQuaternion((D3DXMATRIX*)PinnedOut, (D3DXQUATERNION*)PinnedQuaternion);
	}

	static void MatrixRotationX([Out] Matrix% Out, float Angle)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixRotationX((D3DXMATRIX*)PinnedOut, Angle);
	}

	static void MatrixRotationY([Out] Matrix% Out, float Angle)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixRotationY((D3DXMATRIX*)PinnedOut, Angle);
	}

	static void MatrixRotationYawPitchRoll([Out] Matrix% Out, float Yaw, float Pitch, float Roll)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixRotationYawPitchRoll((D3DXMATRIX*)PinnedOut, Yaw,  Pitch,  Roll);
	}

	static void MatrixRotationZ([Out] Matrix% Out, float Angle)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixRotationZ((D3DXMATRIX*)PinnedOut, Angle);
	}

	static void MatrixScaling([Out] Matrix% Out, float X, float Y, float Z)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixScaling((D3DXMATRIX*)PinnedOut, X, Y, Z);
	}

	static void MatrixShadow([Out] Matrix% Out, Vector4% Light, Plane% Plane)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector4> PinnedLight = &Light;
		pin_ptr<Xtro::MDX::Direct3DX10::Plane> PinnedPlane = &Plane;

		D3DXMatrixShadow((D3DXMATRIX*)PinnedOut, (D3DXVECTOR4*)PinnedLight, (D3DXPLANE*)PinnedPlane);
	}

	static void MatrixTransformation([Out] Matrix% Out, Xtro::MDX::Generic::ValueObject<Vector3>^ ScalingCenter, Xtro::MDX::Generic::ValueObject<Quaternion>^ ScalingRotation, Xtro::MDX::Generic::ValueObject<Vector3>^ Scaling, Xtro::MDX::Generic::ValueObject<Vector3>^ RotationCenter, Xtro::MDX::Generic::ValueObject<Quaternion>^ Rotation, Xtro::MDX::Generic::ValueObject<Vector3>^ Translation)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedScalingCenter = ScalingCenter == nullptr ? nullptr : &ScalingCenter->Value;
		pin_ptr<Quaternion> PinnedScalingRotation = ScalingRotation == nullptr ? nullptr : &ScalingRotation->Value;
		pin_ptr<Vector3> PinnedScaling = Scaling == nullptr ? nullptr : &Scaling->Value;
		pin_ptr<Vector3> PinnedRotationCenter = RotationCenter == nullptr ? nullptr : &RotationCenter->Value;
		pin_ptr<Quaternion> PinnedRotation = Rotation == nullptr ? nullptr : &Rotation->Value;
		pin_ptr<Vector3> PinnedTranslation = Translation == nullptr ? nullptr : &Translation->Value;

		D3DXMatrixTransformation((D3DXMATRIX*)PinnedOut, (D3DXVECTOR3*)PinnedScalingCenter, (D3DXQUATERNION*)PinnedScalingRotation, (D3DXVECTOR3*)PinnedScaling, (D3DXVECTOR3*)PinnedRotationCenter, (D3DXQUATERNION*)PinnedRotation, (D3DXVECTOR3*)PinnedTranslation);
	}

	static void MatrixTransformation2D([Out] Matrix% Out, Xtro::MDX::Generic::ValueObject<Vector2>^ ScalingCenter, float ScalingRotation, Xtro::MDX::Generic::ValueObject<Vector2>^ Scaling, Xtro::MDX::Generic::ValueObject<Vector2>^ RotationCenter, float Rotation, Xtro::MDX::Generic::ValueObject<Vector2>^ Translation)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Vector2> PinnedScalingCenter = ScalingCenter == nullptr ? nullptr : &ScalingCenter->Value;
		pin_ptr<Vector2> PinnedScaling = Scaling == nullptr ? nullptr : &Scaling->Value;
		pin_ptr<Vector2> PinnedRotationCenter = RotationCenter == nullptr ? nullptr : &RotationCenter->Value;
		pin_ptr<Vector2> PinnedTranslation = Translation == nullptr ? nullptr : &Translation->Value;

		D3DXMatrixTransformation2D((D3DXMATRIX*)PinnedOut, (D3DXVECTOR2*)PinnedScalingCenter, ScalingRotation, (D3DXVECTOR2*)PinnedScaling, (D3DXVECTOR2*)PinnedRotationCenter, Rotation, (D3DXVECTOR2*)PinnedTranslation);
	}

	static void MatrixTranslation([Out] Matrix% Out, float X, float Y, float Z)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixTranslation((D3DXMATRIX*)PinnedOut, X, Y, Z);
	}

	static void MatrixTranspose([Out] Matrix% Out, Matrix% Matrix)
	{
		pin_ptr<Xtro::MDX::Direct3DX10::Matrix> PinnedOut = &Out;
		pin_ptr<Xtro::MDX::Direct3DX10::Matrix> PinnedMatrix = &Matrix;

		D3DXMatrixTranspose((D3DXMATRIX*)PinnedOut, (D3DXMATRIX*)PinnedMatrix);
	}

	static void PlaneFromPointNormal([Out] Plane% Out, Vector3% Point, Vector3% Normal)
	{
		pin_ptr<Plane> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedPoint = &Point;
		pin_ptr<Vector3> PinnedNormal = &Normal;

		D3DXPlaneFromPointNormal((D3DXPLANE*)PinnedOut, (D3DXVECTOR3*)PinnedPoint, (D3DXVECTOR3*)PinnedNormal);
	}

	static void PlaneFromPoints([Out] Plane% Out, Vector3% Vector1, Vector3% Vector2, Vector3% Vector3)
	{
		pin_ptr<Plane> PinnedOut = &Out;
		pin_ptr<Xtro::MDX::Direct3DX10::Vector3> PinnedVector1 = &Vector1;
		pin_ptr<Xtro::MDX::Direct3DX10::Vector3> PinnedVector2 = &Vector2;
		pin_ptr<Xtro::MDX::Direct3DX10::Vector3> PinnedVector3 = &Vector3;

		D3DXPlaneFromPoints((D3DXPLANE*)PinnedOut, (D3DXVECTOR3*)PinnedVector1, (D3DXVECTOR3*)PinnedVector2, (D3DXVECTOR3*)PinnedVector3);
	}

	static void PlaneIntersectLine([Out] Vector3% Out, Plane% Plane, Vector3% Vector1, Vector3% Vector2)
	{
		pin_ptr<Vector3> PinnedOut = &Out;
		pin_ptr<Xtro::MDX::Direct3DX10::Plane> PinnedPlane = &Plane;
		pin_ptr<Vector3> PinnedVector1 = &Vector1;
		pin_ptr<Vector3> PinnedVector2 = &Vector2;

		D3DXPlaneIntersectLine((D3DXVECTOR3*)PinnedOut, (D3DXPLANE*)PinnedPlane, (D3DXVECTOR3*)PinnedVector1, (D3DXVECTOR3*)PinnedVector2);
	}

	static void PlaneNormalize([Out] Plane% Out, Plane% Plane)
	{
		pin_ptr<Xtro::MDX::Direct3DX10::Plane> PinnedOut = &Out;
		pin_ptr<Xtro::MDX::Direct3DX10::Plane> PinnedPlane = &Plane;

		D3DXPlaneNormalize((D3DXPLANE*)PinnedOut, (D3DXPLANE*)PinnedPlane);
	}

	static void PlaneTransform([Out] Plane% Out, Plane% Plane, Matrix% Matrix)
	{
		pin_ptr<Xtro::MDX::Direct3DX10::Plane> PinnedOut = &Out;
		pin_ptr<Xtro::MDX::Direct3DX10::Plane> PinnedPlane = &Plane;
		pin_ptr<Xtro::MDX::Direct3DX10::Matrix> PinnedMatrix = &Matrix;

		D3DXPlaneTransform((D3DXPLANE*)PinnedOut, (D3DXPLANE*)PinnedPlane, (D3DXMATRIX*)PinnedMatrix);
	}

	static void PlaneTransformArray(array<Plane>^ Out, unsigned int OutStride, array<Plane>^ Plane, unsigned int PlaneStride, Matrix% Matrix, unsigned int N)
	{
		pin_ptr<Xtro::MDX::Direct3DX10::Plane> PinnedOut = Out != nullptr && Out->Length > 0 ? &Out[0] : nullptr;
		pin_ptr<Xtro::MDX::Direct3DX10::Plane> PinnedPlane = Plane != nullptr && Plane->Length > 0 ? &Plane[0] : nullptr;
		pin_ptr<Xtro::MDX::Direct3DX10::Matrix> PinnedMatrix = &Matrix;

		D3DXPlaneTransformArray((D3DXPLANE*)PinnedOut, OutStride, (D3DXPLANE*)PinnedPlane, PlaneStride, (D3DXMATRIX*)PinnedMatrix, N);
	}

	static int SaveTextureToFile(Xtro::MDX::Direct3D10::Resource^ SourceTexture, ImageFileFormat DestinationFormat, String^ DestinationFile)
	{
		ID3D10Resource* pSourceTexture = SourceTexture == nullptr ? 0 : SourceTexture->pResource;

		IntPtr pDestinationFile = Marshal::StringToHGlobalUni(DestinationFile);
		try { return D3DX10SaveTextureToFile(pSourceTexture, (D3DX10_IMAGE_FILE_FORMAT)DestinationFormat, (LPCWSTR)pDestinationFile.ToPointer()); }
		finally { Marshal::FreeHGlobal(pDestinationFile); }
	}
	
	static int SaveTextureToMemory(Xtro::MDX::Direct3D10::Resource^ SourceTexture, ImageFileFormat DestinationFormat, [Out] Blob^% DestinationBuffer, unsigned int Flags)
	{
		ID3D10Resource* pSourceTexture = SourceTexture == nullptr ? 0 : SourceTexture->pResource;

		ID3D10Blob* pDestinationBuffer = 0;
		int Result = D3DX10SaveTextureToMemory(pSourceTexture, (D3DX10_IMAGE_FILE_FORMAT)DestinationFormat, &pDestinationBuffer, Flags);

		if (pDestinationBuffer)
		{
			try { DestinationBuffer = (Xtro::MDX::Direct3D10::Blob^)Interface::Interfaces[IntPtr(pDestinationBuffer)]; }
			catch (KeyNotFoundException^) { DestinationBuffer = gcnew Xtro::MDX::Direct3D10::Blob(IntPtr(pDestinationBuffer)); }
		}
		else DestinationBuffer = nullptr;

		return Result;
	}
	
	static int GetImageInfoFromFile(String^ SourceFile, [Out] ImageInfo% SourceInfo)
	{
		pin_ptr<ImageInfo> PinnedSourceInfo = &SourceInfo;

		IntPtr pSourceFile = Marshal::StringToHGlobalUni(SourceFile);
		try { return D3DX10GetImageInfoFromFile((LPCWSTR)pSourceFile.ToPointer(), 0, (D3DX10_IMAGE_INFO*)PinnedSourceInfo, 0); }
		finally { Marshal::FreeHGlobal(pSourceFile); }
	}

	static int GetImageInfoFromMemory(UnmanagedMemory^ SourceData, [Out] ImageInfo% SourceInfo)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;
		SIZE_T Size = SourceData == nullptr ? 0 : SourceData->FSize;
		pin_ptr<ImageInfo> PinnedSourceInfo = &SourceInfo;

		return D3DX10GetImageInfoFromMemory(pSourceData, Size, 0, (D3DX10_IMAGE_INFO*)PinnedSourceInfo, 0);
	}

	static int CreateTextureFromFile(Xtro::MDX::Direct3D10::Device^ Device, String^ SourceFile, [Out] Xtro::MDX::Direct3D10::Resource^% Texture)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3D10Resource* pResource = 0;
	
		IntPtr pSourceFile = Marshal::StringToHGlobalUni(SourceFile);
		try { Result = D3DX10CreateTextureFromFile(pDevice, (LPCWSTR)pSourceFile.ToPointer(), 0, 0, &pResource, 0); }
		finally { Marshal::FreeHGlobal(pSourceFile); }

		Texture = pResource ? CreateTextureByType(pResource) : nullptr;

		return Result;
	}
		
	static int CreateTextureFromFile(Xtro::MDX::Direct3D10::Device^ Device, String^ SourceFile, ImageLoadInfo% LoadInfo, [Out] Xtro::MDX::Direct3D10::Resource^% Texture)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3D10Resource* pResource = 0;
	
		IntPtr pSourceFile = IntPtr::Zero;
		try 
		{
			D3DX10_IMAGE_LOAD_INFO NativeLoadInfo;
			LoadInfo.Marshal(&NativeLoadInfo);
			pSourceFile = Marshal::StringToHGlobalUni(SourceFile);

			Result = D3DX10CreateTextureFromFile(pDevice, (LPCWSTR)pSourceFile.ToPointer(), &NativeLoadInfo, 0, &pResource, 0); 
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceFile); 
			LoadInfo.Unmarshal();
		}

		Texture = pResource ? CreateTextureByType(pResource) : nullptr;

		return Result;
	}
		
	static int CreateTextureFromMemory(Xtro::MDX::Direct3D10::Device^ Device, UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, [Out]Xtro::MDX::Direct3D10::Resource^% Texture)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		int Result = 0;
		ID3D10Resource* pResource = 0;
		Result = D3DX10CreateTextureFromMemory(pDevice, pSourceData, SourceDataSize, 0, 0, &pResource, 0);

		Texture = pResource ? CreateTextureByType(pResource) : nullptr;

		return Result;
	}
		
	static int CreateTextureFromMemory(Xtro::MDX::Direct3D10::Device^ Device, UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, ImageLoadInfo% LoadInfo, [Out]Xtro::MDX::Direct3D10::Resource^% Texture)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		int Result = 0;
		ID3D10Resource* pResource = 0;

		D3DX10_IMAGE_LOAD_INFO NativeLoadInfo;
		LoadInfo.Marshal(&NativeLoadInfo);
		try { Result = D3DX10CreateTextureFromMemory(pDevice, pSourceData, SourceDataSize, &NativeLoadInfo, 0, &pResource, 0); }
		finally { LoadInfo.Unmarshal(); }
			  
		Texture = pResource ? CreateTextureByType(pResource) : nullptr;

		return Result;
	}
		
	static void FilterTexture(Xtro::MDX::Direct3D10::Resource^ Texture, unsigned int SourceLevel, FilterFlag MipFilter)
	{
		ID3D10Resource *pTexture = Texture == nullptr ? 0 : Texture->pResource;

		D3DX10FilterTexture(pTexture, SourceLevel, (unsigned int)MipFilter);
	}

	static int LoadTextureFromTexture(Xtro::MDX::Direct3D10::Resource^ SourceTexture, TextureLoadInfo% LoadInfo, Xtro::MDX::Direct3D10::Resource^ DestinationTexture)
	{
		ID3D10Resource* pSourceTexture = SourceTexture == nullptr ? 0 : SourceTexture->pResource;
		ID3D10Resource* pDestinationTexture = DestinationTexture == nullptr ? 0 : DestinationTexture->pResource;

		D3DX10_TEXTURE_LOAD_INFO NativeLoadInfo;
		LoadInfo.Marshal(&NativeLoadInfo);
		try { return D3DX10LoadTextureFromTexture(pSourceTexture, &NativeLoadInfo, pDestinationTexture); }
		finally { LoadInfo.Unmarshal(); }
	}

	static int CreateShaderResourceViewFromFile(Xtro::MDX::Direct3D10::Device^ Device, String^ SourceFile, [Out] ShaderResourceView^% ShaderResourceView)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3D10ShaderResourceView* pShaderResourceView = 0;

		IntPtr pSourceFile = Marshal::StringToHGlobalUni(SourceFile);
		try { Result = D3DX10CreateShaderResourceViewFromFile(pDevice, (LPCWSTR)pSourceFile.ToPointer(), 0, 0, &pShaderResourceView, 0); }
		finally { Marshal::FreeHGlobal(pSourceFile); }

		if (pShaderResourceView)
		{
			try { ShaderResourceView = (Xtro::MDX::Direct3D10::ShaderResourceView^)Interface::Interfaces[IntPtr(pShaderResourceView)]; }
			catch (KeyNotFoundException^) { ShaderResourceView = gcnew Xtro::MDX::Direct3D10::ShaderResourceView(IntPtr(pShaderResourceView)); }
		}
		else ShaderResourceView = nullptr;

		return Result;
	}
		 
	static int CreateShaderResourceViewFromFile(Xtro::MDX::Direct3D10::Device^ Device, String^ SourceFile, ImageLoadInfo% LoadInfo, [Out] ShaderResourceView^% ShaderResourceView)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3D10ShaderResourceView* pShaderResourceView = 0;

		IntPtr pSourceFile = IntPtr::Zero;
		try
		{
			D3DX10_IMAGE_LOAD_INFO NativeLoadInfo;
			LoadInfo.Marshal(&NativeLoadInfo);
			pSourceFile = Marshal::StringToHGlobalUni(SourceFile);

			Result = D3DX10CreateShaderResourceViewFromFile(pDevice, (LPCWSTR)pSourceFile.ToPointer(), &NativeLoadInfo, 0, &pShaderResourceView, 0); 
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceFile); 
			LoadInfo.Unmarshal();
		}

		if (pShaderResourceView)
		{
			try { ShaderResourceView = (Xtro::MDX::Direct3D10::ShaderResourceView^)Interface::Interfaces[IntPtr(pShaderResourceView)]; }
			catch (KeyNotFoundException^) { ShaderResourceView = gcnew Xtro::MDX::Direct3D10::ShaderResourceView(IntPtr(pShaderResourceView)); }
		}
		else ShaderResourceView = nullptr;

		return Result;
	}
		 
	static int CreateMesh(Xtro::MDX::Direct3D10::Device^ Device, array<InputElementDescription>^ Declaration, unsigned int DeclarationCount, String^ PositionSemantic, unsigned int VertexCount, unsigned int FaceCount, MeshFlag Options, [Out] Mesh^% Mesh)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		int Result = 0;
		ID3DX10Mesh* pMesh = 0;

		IntPtr pPositionSemantic = Marshal::StringToHGlobalAnsi(PositionSemantic);
		unsigned int Length;
		D3D10_INPUT_ELEMENT_DESC* pDeclaration = 0;
		try 
		{
			if (Declaration != nullptr && Declaration->Length > 0)
			{
				Length = Math::Min(DeclarationCount, (unsigned int)Declaration->Length);
				pDeclaration = new D3D10_INPUT_ELEMENT_DESC[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					Declaration[No].Marshal(&pDeclaration[No]);
				}
			}

			Result = D3DX10CreateMesh(pDevice, pDeclaration, DeclarationCount, (LPCSTR)pPositionSemantic.ToPointer(), VertexCount, FaceCount, (unsigned int)Options, &pMesh);
		}
		finally
		{
			Marshal::FreeHGlobal(pPositionSemantic); 

			if (pDeclaration) 
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					Declaration[No].Unmarshal();
				}

				delete[] pDeclaration;
			}
		}

		if (pMesh)
		{
			try { Mesh = (Xtro::MDX::Direct3DX10::Mesh^)Interface::Interfaces[IntPtr(pMesh)]; }
			catch (KeyNotFoundException^) { Mesh = gcnew Xtro::MDX::Direct3DX10::Mesh(IntPtr(pMesh)); }
		}
		else Mesh = nullptr;

		return Result;
	}

	static void MatrixIdentity([Out] Matrix% Out)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixIdentity((D3DXMATRIX*)PinnedOut);
	}

	static void Vector3Transform([Out] Vector4% Out, Vector3% Vector, Matrix% Matrix)
	{
		pin_ptr<Vector4> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedVector = &Vector;
		pin_ptr<Xtro::MDX::Direct3DX10::Matrix> PinnedMatrix = &Matrix;
		D3DXVec3Transform((D3DXVECTOR4*)PinnedOut, (D3DXVECTOR3*)PinnedVector, (D3DXMATRIX*)PinnedMatrix);
	}
	
	static void Vector3TransformNormal([Out] Vector3% Out, Vector3% Vector, Matrix% Matrix)
	{
		pin_ptr<Vector3> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedVector = &Vector;
		pin_ptr<Xtro::MDX::Direct3DX10::Matrix> PinnedMatrix = &Matrix;
		D3DXVec3TransformNormal((D3DXVECTOR3*)PinnedOut, (D3DXVECTOR3*)PinnedVector, (D3DXMATRIX*)PinnedMatrix);
	}
	
	static void Vector3Normalize([Out] Vector3% Out, Vector3% Vector)
	{
		pin_ptr<Vector3> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedVector = &Vector;
		D3DXVec3Normalize((D3DXVECTOR3*)PinnedOut, (D3DXVECTOR3*)PinnedVector);
	}

	static void Vector3Cross([Out] Vector3% Out, Vector3% Vector1, Vector3% Vector2)
	{
		pin_ptr<Vector3> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedVector1 = &Vector1;
		pin_ptr<Vector3> PinnedVector2 = &Vector2;
		D3DXVec3Cross((D3DXVECTOR3*)PinnedOut, (D3DXVECTOR3*)PinnedVector1, (D3DXVECTOR3*)PinnedVector2);
	}

	static void Vector3TransformCoordinates([Out] Vector3% Out, Vector3% Vector, Matrix% Matrix)
	{
		pin_ptr<Vector3> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedVector = &Vector;
		pin_ptr<Direct3DX10::Matrix> PinnedMatrix = &Matrix;
		D3DXVec3TransformCoord((D3DXVECTOR3*)PinnedOut, (D3DXVECTOR3*)PinnedVector, (D3DXMATRIX*)PinnedMatrix);
	}

	static float Vector3Dot(Vector3% Vector1, Vector3% Vector2)
	{
		pin_ptr<Vector3> PinnedVector1 = &Vector1;
		pin_ptr<Vector3> PinnedVector2 = &Vector2;
		return D3DXVec3Dot((D3DXVECTOR3*)PinnedVector1, (D3DXVECTOR3*)PinnedVector2);
	}

	static float Vector3LengthSquare(Vector3% Vector)
	{
		pin_ptr<Vector3> PinnedVector = &Vector;
		return D3DXVec3LengthSq((D3DXVECTOR3*)PinnedVector);
	}

	static float Vector3Length(Vector3% Vector)
	{
		pin_ptr<Vector3> PinnedVector = &Vector;
		return D3DXVec3Length((D3DXVECTOR3*)PinnedVector);
	}

	static void Vector3Subtract([Out] Vector3% Out, Vector3% Vector1, Vector3% Vector2)
	{
		pin_ptr<Vector3> PinnedOut = &Out;
		pin_ptr<Vector3> PinnedVector1 = &Vector1;
		pin_ptr<Vector3> PinnedVector2 = &Vector2;
		D3DXVec3Subtract((D3DXVECTOR3*)PinnedOut, (D3DXVECTOR3*)PinnedVector1, (D3DXVECTOR3*)PinnedVector2);
	}

	static void QuaternionMultiply([Out] Quaternion% Out, Quaternion% Quaternion1, Quaternion% Quaternion2)
	{
		pin_ptr<Quaternion> PinnedOut = &Out;
		pin_ptr<Quaternion> PinnedQuaternion1 = &Quaternion1;
		pin_ptr<Quaternion> PinnedQuaternion2 = &Quaternion2;
		D3DXQuaternionMultiply((D3DXQUATERNION*)PinnedOut, (D3DXQUATERNION*)PinnedQuaternion1, (D3DXQUATERNION*)PinnedQuaternion2);
	}

	static void QuaternionIdentity([Out] Quaternion% Out)
	{
		pin_ptr<Quaternion> PinnedOut = &Out;
		D3DXQuaternionIdentity((D3DXQUATERNION*)PinnedOut);
	}

	static void QuaternionNormalize([Out] Quaternion% Out, Quaternion% Quaternion)
	{
		pin_ptr<Direct3DX10::Quaternion> PinnedOut = &Out;
		pin_ptr<Direct3DX10::Quaternion> PinnedQuaternion = &Quaternion;
		D3DXQuaternionNormalize((D3DXQUATERNION*)PinnedOut, (D3DXQUATERNION*)PinnedQuaternion);
	}

	static void QuaternionInverse([Out] Quaternion% Out, Quaternion% Quaternion)
	{
		pin_ptr<Direct3DX10::Quaternion> PinnedOut = &Out;
		pin_ptr<Direct3DX10::Quaternion> PinnedQuaternion = &Quaternion;
		D3DXQuaternionInverse((D3DXQUATERNION*)PinnedOut, (D3DXQUATERNION*)PinnedQuaternion);
	}

	static void QuaternionRotationMatrix([Out] Quaternion% Out, Matrix% Matrix)
	{
		pin_ptr<Quaternion> PinnedOut = &Out;
		pin_ptr<Direct3DX10::Matrix> PinnedMatrix = &Matrix;
		D3DXQuaternionRotationMatrix((D3DXQUATERNION*)PinnedOut, (D3DXMATRIX*)PinnedMatrix);
	}
};