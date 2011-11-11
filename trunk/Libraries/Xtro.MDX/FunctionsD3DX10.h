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

	static void MatrixPerspectiveFovLH([Out] Matrix% Out, float FovY, float Aspect, float Z_Near, float Z_Far)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixPerspectiveFovLH((D3DXMATRIX*)PinnedOut, FovY, Aspect, Z_Near, Z_Far);
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

	static void MatrixRotationZ([Out] Matrix% Out, float Angle)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixRotationZ((D3DXMATRIX*)PinnedOut, Angle);
	}

	static void MatrixTranslation([Out] Matrix% Out, float X, float Y, float Z)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixTranslation((D3DXMATRIX*)PinnedOut, X, Y, Z);
	}

	static void MatrixScaling([Out] Matrix% Out, float X, float Y, float Z)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixScaling((D3DXMATRIX*)PinnedOut, X, Y, Z);
	}

	static void MatrixMultiply([Out] Matrix% Out, Matrix% Matrix1, Matrix% Matrix2)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Matrix> PinnedMatrix1 = &Matrix1;
		pin_ptr<Matrix> PinnedMatrix2 = &Matrix2;
		D3DXMatrixMultiply((D3DXMATRIX*)PinnedOut, (D3DXMATRIX*)PinnedMatrix1, (D3DXMATRIX*)PinnedMatrix2);
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

	static void MatrixRotationQuaternion([Out] Matrix% Out, Quaternion% Quaternion)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		pin_ptr<Direct3DX10::Quaternion> PinnedQuaternion = &Quaternion;
		D3DXMatrixRotationQuaternion((D3DXMATRIX*)PinnedOut, (D3DXQUATERNION*)PinnedQuaternion);
	}

	static void MatrixOrthoOffCenterLH([Out] Matrix% Out, float L, float R, float B, float T, float ZN, float ZF)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixOrthoOffCenterLH((D3DXMATRIX*)PinnedOut, L, R, B, T, ZN, ZF);
	}

	static void ColorLerp([Out] Color% Out, Color% Color1, Color% Color2, float S)
	{
		pin_ptr<Color> PinnedOut = &Out;
		pin_ptr<Color> PinnedColor1 = &Color1;
		pin_ptr<Color> PinnedColor2 = &Color2;
		D3DXColorLerp((D3DXCOLOR*)PinnedOut, (D3DXCOLOR*)PinnedColor1, (D3DXCOLOR*)PinnedColor2, S);
	}
};