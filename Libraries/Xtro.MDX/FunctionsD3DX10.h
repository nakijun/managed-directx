public ref class Functions abstract sealed
{
private:
	inline static Xtro::MDX::Direct3D10::Resource^ CreateTextureByType(ID3D10Resource*	pResource)
	{
		Xtro::MDX::Direct3D10::Resource^ Result;

		D3D10_RESOURCE_DIMENSION Type;
		pResource->GetType(&Type);

		switch (Type)
		{
			case D3D10_RESOURCE_DIMENSION_TEXTURE1D : 
				try { Result = (Xtro::MDX::Direct3D10::Resource^)Interface::Interfaces[IntPtr(pResource)]; }
				catch (KeyNotFoundException^) { Result = gcnew Texture1D(IntPtr(pResource)); }
				break;
			case D3D10_RESOURCE_DIMENSION_TEXTURE2D : 
				try { Result = (Xtro::MDX::Direct3D10::Resource^)Interface::Interfaces[IntPtr(pResource)]; }
				catch (KeyNotFoundException^) { Result = gcnew Texture2D(IntPtr(pResource)); }
				break;
			case D3D10_RESOURCE_DIMENSION_TEXTURE3D : 
				try { Result = (Xtro::MDX::Direct3D10::Resource^)Interface::Interfaces[IntPtr(pResource)]; }
				catch (KeyNotFoundException^) { Result = gcnew Texture3D(IntPtr(pResource)); }
				break;
		}

		return Result;
	}

public:	
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
		int Size = SourceData == nullptr ? 0 : SourceData->FSize;
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

		if (pResource) Texture = CreateTextureByType(pResource);
		else Texture = nullptr;

		return Result;
	}
		
	static int CreateTextureFromFile(Xtro::MDX::Direct3D10::Device^ Device, String^ SourceFile, ImageLoadInfo% LoadInfo, [Out] Xtro::MDX::Direct3D10::Resource^% Texture)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		pin_ptr<ImageLoadInfo> PinnedLoadInfo = &LoadInfo;

		int Result = 0;
		ID3D10Resource* pResource = 0;
	
		IntPtr pSourceFile = Marshal::StringToHGlobalUni(SourceFile);
		try { Result = D3DX10CreateTextureFromFile(pDevice, (LPCWSTR)pSourceFile.ToPointer(), (D3DX10_IMAGE_LOAD_INFO*)PinnedLoadInfo, 0, &pResource, 0); }
		finally { Marshal::FreeHGlobal(pSourceFile); }

		if (pResource) Texture = CreateTextureByType(pResource);
		else Texture = nullptr;

		return Result;
	}
		
	static int CreateTextureFromMemory(Xtro::MDX::Direct3D10::Device^ Device, UnmanagedMemory^ SourceData, [Out]Xtro::MDX::Direct3D10::Resource^% Texture)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;
		int Size = SourceData == nullptr ? 0 : SourceData->FSize;

		int Result = 0;
		ID3D10Resource* pResource = 0;
		Result = D3DX10CreateTextureFromMemory(pDevice, pSourceData, Size, 0, 0, &pResource, 0);

		if (pResource) Texture = CreateTextureByType(pResource);
		else Texture = nullptr;

		return Result;
	}
		
	static int CreateTextureFromMemory(Xtro::MDX::Direct3D10::Device^ Device, UnmanagedMemory^ SourceData, ImageLoadInfo% LoadInfo, [Out]Xtro::MDX::Direct3D10::Resource^% Texture)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;
		int Size = SourceData == nullptr ? 0 : SourceData->FSize;
		pin_ptr<ImageLoadInfo> PinnedLoadInfo = &LoadInfo;

		int Result = 0;
		ID3D10Resource* pResource = 0;
		Result = D3DX10CreateTextureFromMemory(pDevice, pSourceData, Size, (D3DX10_IMAGE_LOAD_INFO*)PinnedLoadInfo, 0, &pResource, 0);

		if (pResource) Texture = CreateTextureByType(pResource);
		else Texture = nullptr;

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
		pin_ptr<TextureLoadInfo> PinnedLoadInfo = &LoadInfo;

		return D3DX10LoadTextureFromTexture(pSourceTexture, (D3DX10_TEXTURE_LOAD_INFO*)PinnedLoadInfo, pDestinationTexture);
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
		pin_ptr<ImageLoadInfo> PinnedLoadInfo = &LoadInfo;

		int Result = 0;
		ID3D10ShaderResourceView* pShaderResourceView = 0;

		IntPtr pSourceFile = Marshal::StringToHGlobalUni(SourceFile);
		try { Result = D3DX10CreateShaderResourceViewFromFile(pDevice, (LPCWSTR)pSourceFile.ToPointer(), (D3DX10_IMAGE_LOAD_INFO*)PinnedLoadInfo, 0, &pShaderResourceView, 0); }
		finally { Marshal::FreeHGlobal(pSourceFile); }

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

		IntPtr pFileName = Marshal::StringToHGlobalUni(FileName);
		IntPtr pProfile = Marshal::StringToHGlobalAnsi(Profile);
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int DefineNo = 0; DefineNo < Defines->Length; DefineNo++) Defines[DefineNo].Marshal(&pDefines[DefineNo]);
			}

			Result = D3DX10CreateEffectFromFile((LPCWSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, pEffectPool, 0, &pEffect, &pErrors, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pProfile); 

			if (Defines != nullptr)
			{
				for (int DefineNo = 0; DefineNo < Defines->Length; DefineNo++) Defines[DefineNo].Unmarshal();
			}
			if (pDefines) delete[] pDefines;
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

		IntPtr pFileName = Marshal::StringToHGlobalUni(FileName);
		IntPtr pProfile = Marshal::StringToHGlobalAnsi(Profile);
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int DefineNo = 0; DefineNo < Defines->Length; DefineNo++) Defines[DefineNo].Marshal(&pDefines[DefineNo]);
			}

			Result = D3DX10CreateEffectFromFile((LPCWSTR)pFileName.ToPointer(), pDefines, pInclude, (LPCSTR)pProfile.ToPointer(), (unsigned int)HLSL_Flags, (unsigned int)FX_Flags, pDevice, pEffectPool, 0, &pEffect, 0, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pFileName); 
			Marshal::FreeHGlobal(pProfile); 

			if (Defines != nullptr)
			{
				for (int DefineNo = 0; DefineNo < Defines->Length; DefineNo++) Defines[DefineNo].Unmarshal();
			}
			if (pDefines) delete[] pDefines;
		}

		if (pEffect)
		{
			try { Effect = (Xtro::MDX::Direct3D10::Effect^)Interface::Interfaces[IntPtr(pEffect)]; }
			catch (KeyNotFoundException^) { Effect = gcnew Xtro::MDX::Direct3D10::Effect(IntPtr(pEffect)); }
		}
		else Effect = nullptr;

		return Result;
	}

	static int CreateFontIndirect(Xtro::MDX::Direct3D10::Device^ Device, FontDescription% FontDescription, [Out] Font^% Font)
	{
		ID3D10Device* pDevice = Device == nullptr ? 0 : Device->pDevice;

		D3DX10_FONT_DESC NativeFontDescription;
		FontDescription.ToNative(&NativeFontDescription);

		ID3DX10Font* pFont = 0;
		int Result = D3DX10CreateFontIndirect(pDevice, &NativeFontDescription, &pFont);

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

	static void MatrixIdentity([Out] Matrix% Out)
	{
		pin_ptr<Matrix> PinnedOut = &Out;
		D3DXMatrixIdentity((D3DXMATRIX*)PinnedOut);
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
};