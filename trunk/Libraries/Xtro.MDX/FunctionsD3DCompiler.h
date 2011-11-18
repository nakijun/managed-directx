public ref class Functions abstract sealed
{
public:
	static int Compile(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, String^ SourceName, array<ShaderMacro>^ Defines, Include^ Include, String^ EntryPoint, String^ Target, ShaderFlag Flags1, EffectFlag Flags2, [Out] Blob^% Code, [Out] Blob^% ErrorMessages)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pCode = 0;
		ID3D10Blob* pErrorMessages = 0;

		IntPtr pSourceName = IntPtr::Zero;
		IntPtr pEntryPoint = IntPtr::Zero;
		IntPtr pTarget = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pSourceName = Marshal::StringToHGlobalAnsi(SourceName);
			pEntryPoint = Marshal::StringToHGlobalAnsi(EntryPoint);
			pTarget = Marshal::StringToHGlobalAnsi(Target);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DCompile(pSourceData, SourceDataSize, (LPCSTR)pSourceName.ToPointer(), pDefines, pInclude, (LPCSTR)pEntryPoint.ToPointer(), (LPCSTR)pTarget.ToPointer(), (unsigned int)Flags1, (unsigned int)Flags2, &pCode, &pErrorMessages);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceName); 
			Marshal::FreeHGlobal(pEntryPoint); 
			Marshal::FreeHGlobal(pTarget); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pCode)
		{
			try { Code = (Blob^)Interface::Interfaces[IntPtr(pCode)]; }
			catch (KeyNotFoundException^) { Code = gcnew Blob(IntPtr(pCode)); }
		}
		else Code = nullptr;

		if (pErrorMessages)
		{
			try { ErrorMessages = (Blob^)Interface::Interfaces[IntPtr(pErrorMessages)]; }
			catch (KeyNotFoundException^) { ErrorMessages = gcnew Blob(IntPtr(pErrorMessages)); }
		}
		else ErrorMessages = nullptr;

		return Result;
	}

	static int Compile(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, String^ SourceName, array<ShaderMacro>^ Defines, Include^ Include, String^ EntryPoint, String^ Target, ShaderFlag Flags1, EffectFlag Flags2, [Out] Blob^% Code)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pCode = 0;

		IntPtr pSourceName = IntPtr::Zero;
		IntPtr pEntryPoint = IntPtr::Zero;
		IntPtr pTarget = IntPtr::Zero;
		D3D10_SHADER_MACRO* pDefines = 0;
		try
		{
			pSourceName = Marshal::StringToHGlobalAnsi(SourceName);
			pEntryPoint = Marshal::StringToHGlobalAnsi(EntryPoint);
			pTarget = Marshal::StringToHGlobalAnsi(Target);

			if (Defines != nullptr && Defines->Length > 0)
			{
				pDefines = new D3D10_SHADER_MACRO[Defines->Length];
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Marshal(&pDefines[No]);
				}
			}

			Result = D3DCompile(pSourceData, SourceDataSize, (LPCSTR)pSourceName.ToPointer(), pDefines, pInclude, (LPCSTR)pEntryPoint.ToPointer(), (LPCSTR)pTarget.ToPointer(), (unsigned int)Flags1, (unsigned int)Flags2, &pCode, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceName); 
			Marshal::FreeHGlobal(pEntryPoint); 
			Marshal::FreeHGlobal(pTarget); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pCode)
		{
			try { Code = (Blob^)Interface::Interfaces[IntPtr(pCode)]; }
			catch (KeyNotFoundException^) { Code = gcnew Blob(IntPtr(pCode)); }
		}
		else Code = nullptr;

		return Result;
	}

	static int Preprocess(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, String^ SourceName, array<ShaderMacro>^ Defines, Include^ Include, [Out] Blob^% CodeText, [Out] Blob^% ErrorMessages)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pCodeText = 0;
		ID3D10Blob* pErrorMessages = 0;

		IntPtr pSourceName = Marshal::StringToHGlobalAnsi(SourceName);
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

			Result = D3DPreprocess(pSourceData, SourceDataSize, (LPCSTR)pSourceName.ToPointer(), pDefines, pInclude, &pCodeText, &pErrorMessages);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceName); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pCodeText)
		{
			try { CodeText = (Blob^)Interface::Interfaces[IntPtr(pCodeText)]; }
			catch (KeyNotFoundException^) { CodeText = gcnew Blob(IntPtr(pCodeText)); }
		}
		else CodeText = nullptr;

		if (pErrorMessages)
		{
			try { ErrorMessages = (Blob^)Interface::Interfaces[IntPtr(pErrorMessages)]; }
			catch (KeyNotFoundException^) { ErrorMessages = gcnew Blob(IntPtr(pErrorMessages)); }
		}
		else ErrorMessages = nullptr;

		return Result;
	}

	static int Preprocess(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, String^ SourceName, array<ShaderMacro>^ Defines, Include^ Include, [Out] Blob^% CodeText)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;
		ID3D10Include* pInclude = Include == nullptr ? 0 : Include->pInclude;

		int Result = 0;
		ID3D10Blob* pCodeText = 0;

		IntPtr pSourceName = Marshal::StringToHGlobalAnsi(SourceName);
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

			Result = D3DPreprocess(pSourceData, SourceDataSize, (LPCSTR)pSourceName.ToPointer(), pDefines, pInclude, &pCodeText, 0);
		}
		finally
		{
			Marshal::FreeHGlobal(pSourceName); 

			if (pDefines) 
			{
				for (int No = 0; No < Defines->Length; No++)
				{
					Defines[No].Unmarshal();
				}
			}

			delete[] pDefines;
		}

		if (pCodeText)
		{
			try { CodeText = (Blob^)Interface::Interfaces[IntPtr(pCodeText)]; }
			catch (KeyNotFoundException^) { CodeText = gcnew Blob(IntPtr(pCodeText)); }
		}
		else CodeText = nullptr;

		return Result;
	}

	static int GetDebugInfo(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, [Out] Blob^% DebugInfo)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		ID3D10Blob *pDebugInfo = 0;
		int Result = D3DGetDebugInfo(pSourceData, SourceDataSize, &pDebugInfo);

		if (pDebugInfo)
		{
			try { DebugInfo = (Blob^)Interface::Interfaces[IntPtr(pDebugInfo)]; }
			catch (KeyNotFoundException^) { DebugInfo = gcnew Blob(IntPtr(pDebugInfo)); }
		}
		else DebugInfo = nullptr;

		return Result;
	}

	static int Reflect(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, Type^ Type, [Out] Object^% Reflector)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		Guid RIID;
		try { RIID = (Guid)Type->GetField("IID", BindingFlags::DeclaredOnly | BindingFlags::NonPublic | BindingFlags::Static)->GetValue(nullptr); }
		catch (...) { RIID = Guid::Empty; }

		void* pReflector = 0;
		int Result = D3DReflect(pSourceData, SourceDataSize, IID_Converter::ToNative(RIID), &pReflector);

		if (pReflector)
		{
			try { Reflector = Interface::Interfaces[IntPtr(pReflector)]; }
			catch (KeyNotFoundException^) { Reflector = Activator::CreateInstance(Type, BindingFlags::NonPublic | BindingFlags::Instance, nullptr, gcnew array<System::Object^>(1) { IntPtr(pReflector) }, CultureInfo::CurrentCulture); }
		}
		else Reflector = nullptr;

		return Result;
	}

	static int Disassemble(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, DisassembleFlag Flags, String^ Comments, [Out] Blob^% Disassembly)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		int Result = 0;
		ID3D10Blob* pDisassembly = 0;

		IntPtr pComments = Marshal::StringToHGlobalAnsi(Comments);
		try	{ Result = D3DDisassemble(pSourceData, SourceDataSize, (unsigned int)Flags, (LPCSTR)pComments.ToPointer(), &pDisassembly); }
		finally	{ Marshal::FreeHGlobal(pComments); }

		if (pDisassembly)
		{
			try { Disassembly = (Blob^)Interface::Interfaces[IntPtr(pDisassembly)]; }
			catch (KeyNotFoundException^) { Disassembly = gcnew Blob(IntPtr(pDisassembly)); }
		}
		else Disassembly = nullptr;

		return Result;
	}

	static int DisassembleEffect(Effect^ Effect, ShaderFlag Flags, [Out] Blob^% Disassembly)
	{
		ID3D10Effect* pEffect = Effect == nullptr ? 0 : Effect->pEffect;

		ID3D10Blob* pDisassembly = 0;
		int Result = D3DDisassemble10Effect(pEffect, (unsigned int)Flags, &pDisassembly);

		if (pDisassembly)
		{
			try { Disassembly = (Blob^)Interface::Interfaces[IntPtr(pDisassembly)]; }
			catch (KeyNotFoundException^) { Disassembly = gcnew Blob(IntPtr(pDisassembly)); }
		}
		else Disassembly = nullptr;

		return Result;
	}

	static int GetInputSignatureBlob(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, [Out] Blob^% SignatureBlob)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		ID3D10Blob* pSignatureBlob = 0;
		int Result = D3DGetInputSignatureBlob(pSourceData, SourceDataSize, &pSignatureBlob);

		if (pSignatureBlob)
		{
			try { SignatureBlob = (Blob^)Interface::Interfaces[IntPtr(pSignatureBlob)]; }
			catch (KeyNotFoundException^) { SignatureBlob = gcnew Blob(IntPtr(pSignatureBlob)); }
		}
		else SignatureBlob = nullptr;

		return Result;
	}

	static int GetOutputSignatureBlob(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, [Out] Blob^% SignatureBlob)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		ID3D10Blob* pSignatureBlob = 0;
		int Result = D3DGetOutputSignatureBlob(pSourceData, SourceDataSize, &pSignatureBlob);

		if (pSignatureBlob)
		{
			try { SignatureBlob = (Blob^)Interface::Interfaces[IntPtr(pSignatureBlob)]; }
			catch (KeyNotFoundException^) { SignatureBlob = gcnew Blob(IntPtr(pSignatureBlob)); }
		}
		else SignatureBlob = nullptr;

		return Result;
	}

	static int GetInputAndOutputSignatureBlob(UnmanagedMemory^ SourceData, SIZE_T SourceDataSize, [Out] Blob^% SignatureBlob)
	{
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		ID3D10Blob* pSignatureBlob = 0;
		int Result = D3DGetInputAndOutputSignatureBlob(pSourceData, SourceDataSize, &pSignatureBlob);

		if (pSignatureBlob)
		{
			try { SignatureBlob = (Blob^)Interface::Interfaces[IntPtr(pSignatureBlob)]; }
			catch (KeyNotFoundException^) { SignatureBlob = gcnew Blob(IntPtr(pSignatureBlob)); }
		}
		else SignatureBlob = nullptr;

		return Result;
	}

	static int StripShader(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, StripFlag StripFlags, [Out] Blob^% StrippedBlob)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		ID3D10Blob* pStrippedBlob = 0;
		int Result = D3DStripShader(pShaderBytecode, BytecodeLength, (unsigned int)StripFlags, &pStrippedBlob);

		if (pStrippedBlob)
		{
			try { StrippedBlob = (Blob^)Interface::Interfaces[IntPtr(pStrippedBlob)]; }
			catch (KeyNotFoundException^) { StrippedBlob = gcnew Blob(IntPtr(pStrippedBlob)); }
		}
		else StrippedBlob = nullptr;

		return Result;
	}
};