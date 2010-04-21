namespace Xtro
{
namespace MDX
{
namespace Direct3DX10
{
	public ref class Functions
	{
	public:
		static int CreateEffectFromFile(String^ FileName, array<ShaderMacro>^ Defines, Include^ Include, String^ Profile, ShaderFlag HLSLFlags, EffectFlag FXFlags, Device^ Device, EffectPool^ EffectPool, [Out] Effect^% Effect, [Out] Blob^% Errors)
		{
			LPCWSTR pFileName = 0;
			if (FileName != nullptr) pFileName = (LPCWSTR)Marshal::StringToHGlobalUni(FileName).ToPointer();

			ID3D10Include* pInclude = 0;
			if (Include != nullptr) pInclude = Include->pInclude;

			LPCSTR pProfile = 0;
			if (Profile != nullptr) pProfile = (LPCSTR)Marshal::StringToHGlobalAnsi(Profile).ToPointer();

			ID3D10Device* pDevice = 0;
			if (Device != nullptr) pDevice = Device->pDevice;

			ID3D10EffectPool* pEffectPool = 0;
			if (EffectPool != nullptr) pEffectPool = EffectPool->pEffectPool;

			ID3D10Effect* pEffect = 0;
			ID3D10Blob* pErrors = 0;
			int Result = 0;

			D3D10_SHADER_MACRO* pDefines = 0;
			try
			{
				if (Defines != nullptr && Defines->Length > 0)
				{
					pDefines = new D3D10_SHADER_MACRO[Defines->Length];
					for (int DefineNo = 0; DefineNo < Defines->Length; DefineNo++) pDefines[DefineNo] = Defines[DefineNo].ToNative();
				}

				Result = D3DX10CreateEffectFromFile(pFileName, pDefines, pInclude, pProfile, (unsigned int)HLSLFlags, (unsigned int)FXFlags, pDevice, pEffectPool, 0, &pEffect, &pErrors, 0);
			}
			finally
			{
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
		
		static int CreateShaderResourceViewFromFile(Device^ Device, String^ SourceFile, Nullable<ImageLoadInfo> LoadInfo, [Out] ShaderResourceView^% ShaderResourceView)
		{
			ID3D10Device* pDevice = 0;
			if (Device != nullptr) pDevice = Device->pDevice;

			LPCWSTR pSourceFile = 0;
			if (SourceFile != nullptr) pSourceFile = (LPCWSTR)Marshal::StringToHGlobalUni(SourceFile).ToPointer();

			D3DX10_IMAGE_LOAD_INFO* pLoadInfo = 0;
			if (LoadInfo.HasValue) pLoadInfo = &LoadInfo.Value.ToNative();

			ID3D10ShaderResourceView* pShaderResourceView = 0;
			int Result = D3DX10CreateShaderResourceViewFromFile(pDevice, pSourceFile, pLoadInfo, 0, &pShaderResourceView, 0);

			if (pShaderResourceView)
			{
				try { ShaderResourceView = (Xtro::MDX::Direct3D10::ShaderResourceView^)Interface::Interfaces[IntPtr(pShaderResourceView)]; }
				catch (KeyNotFoundException^) { ShaderResourceView = gcnew Xtro::MDX::Direct3D10::ShaderResourceView(IntPtr(pShaderResourceView)); }
			}
			else ShaderResourceView = nullptr;

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

		static void Vector3Transform([Out] Vector4% Out, Vector3% Vector, Xtro::MDX::Direct3DX10::Matrix% Matrix)
		{
			pin_ptr<Vector4> PinnedOut = &Out;
			pin_ptr<Vector3> PinnedVector = &Vector;
			pin_ptr<Xtro::MDX::Direct3DX10::Matrix> PinnedMatrix = &Matrix;
			D3DXVec3Transform((D3DXVECTOR4*)PinnedOut, (D3DXVECTOR3*)PinnedVector, (D3DXMATRIX*)PinnedMatrix);
		}
	};
}
}
}