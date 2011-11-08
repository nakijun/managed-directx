public ref class Device sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Device);

internal:
	ID3D10Device* pDevice;

	Device(IntPtr Device) : Unknown(Device)
	{	
		pDevice = (ID3D10Device*)Device.ToPointer();
	}

public:
	int CheckCounter(CounterDescription% Description, [Out] CounterType% Type, [Out] unsigned int% ActiveCounters, Xtro::MDX::Generic::RefObject<String^>^ Name, Xtro::MDX::Generic::ValueObject<unsigned int>^ NameLength, Xtro::MDX::Generic::RefObject<String^>^ Units, Xtro::MDX::Generic::ValueObject<unsigned int>^ UnitsLength, Xtro::MDX::Generic::RefObject<String^>^ DescriptionString, Xtro::MDX::Generic::ValueObject<unsigned int>^ DescriptionLength)
	{
		pin_ptr<CounterDescription> PinnedDescription = &Description;
		pin_ptr<CounterType> PinnedType = &Type;
		pin_ptr<unsigned int> PinnedActiveCounters = &ActiveCounters;
		pin_ptr<unsigned int> PinnedNameLength = NameLength == nullptr ? nullptr : &NameLength->Value;
		pin_ptr<unsigned int> PinnedUnitsLength = UnitsLength == nullptr ? nullptr : &UnitsLength->Value;
		pin_ptr<unsigned int> PinnedDescriptionLength = DescriptionLength == nullptr ? nullptr : &DescriptionLength->Value;

		LPSTR pName = 0;
		LPSTR pUnits = 0;
		LPSTR pDescriptionString = 0;

		try
		{
			if (Name != nullptr) pName = new char[NameLength == nullptr ? 0 : NameLength->Value];
			if (Units != nullptr) pUnits = new char[UnitsLength == nullptr ? 0 : UnitsLength->Value];
			if (DescriptionString != nullptr) pDescriptionString = new char[DescriptionLength == nullptr ? 0 : DescriptionLength->Value];

			int Result = pDevice->CheckCounter((D3D10_COUNTER_DESC*)PinnedDescription, (D3D10_COUNTER_TYPE*)PinnedType, PinnedActiveCounters, pName, PinnedNameLength, pUnits, PinnedUnitsLength, pDescriptionString, PinnedDescriptionLength);

			if (Name != nullptr) Name->Value = pName ? gcnew String(pName) : nullptr;
			if (Units != nullptr) Units->Value = pUnits ? gcnew String(pUnits) : nullptr;
			if (DescriptionString != nullptr) DescriptionString->Value = pDescriptionString ? gcnew String(pDescriptionString) : nullptr;

			return Result;
		}
		finally
		{
			if (pName) delete[] pName;
			if (pUnits) delete[] pUnits;
			if (pDescriptionString) delete[] pDescriptionString;
		}
	}

	void CheckCounterInfo(CounterInfo% CounterInfo)
	{
		pin_ptr<Xtro::MDX::Direct3D10::CounterInfo> PinnedCounterInfo = &CounterInfo;
		pDevice->CheckCounterInfo((D3D10_COUNTER_INFO*)PinnedCounterInfo);
	}
  	
	int CheckFormatSupport(Format Format, [Out] FormatSupport% FormatSupport)
	{
		pin_ptr<Xtro::MDX::Direct3D10::FormatSupport> PinnedFormatSupport = &FormatSupport;
		return pDevice->CheckFormatSupport((DXGI_FORMAT)Format, (unsigned int*)PinnedFormatSupport);
	}

	int CheckMultisampleQualityLevels(Format Format, unsigned int SampleCount, [Out] unsigned int% NumberOfQualityLevels)
	{
		pin_ptr<unsigned int> PinnedNumberOfQualityLevels = &NumberOfQualityLevels;
		return pDevice->CheckMultisampleQualityLevels((DXGI_FORMAT)Format, SampleCount, (unsigned int*)PinnedNumberOfQualityLevels);
	}

	void ClearDepthStencilView(DepthStencilView^ DepthStencilView, ClearFlag ClearFlags, float Depth, Byte Stencil)
	{
		ID3D10DepthStencilView* pDepthStencilView = DepthStencilView == nullptr ? 0 : DepthStencilView->pDepthStencilView;

		pDevice->ClearDepthStencilView(pDepthStencilView, (unsigned int)ClearFlags, Depth, Stencil);
	}

	void ClearRenderTargetView(RenderTargetView^ RenderTargetView, Float4% ColorRGBA)
	{
		ID3D10RenderTargetView* pRenderTargetView = RenderTargetView == nullptr ? 0 : RenderTargetView->pRenderTargetView;
		pin_ptr<Float4> PinnedColorRGBA = &ColorRGBA;

		pDevice->ClearRenderTargetView(pRenderTargetView, (float*)PinnedColorRGBA);
	}

	void ClearState()
	{
		pDevice->ClearState();
	}

	void CopyResource(Resource^ DestinationResource, Resource^ SourceResource)
	{
		ID3D10Resource* pDestinationResource = DestinationResource == nullptr ? 0 : DestinationResource->pResource;
		ID3D10Resource* pSourceResource = SourceResource == nullptr ? 0 : SourceResource->pResource;

		pDevice->CopyResource(pDestinationResource, pSourceResource);
	}

	void CopySubresourceRegion(Resource^ DestinationResource, unsigned int DestinationSubresource, unsigned int DestinationX, unsigned int DestinationY, unsigned int DestinationZ, Resource^ SourceResource, unsigned int SourceSubresource, Box% SourceBox)
	{
		ID3D10Resource* pDestinationResource = DestinationResource == nullptr ? 0 : DestinationResource->pResource;
		ID3D10Resource* pSourceResource = SourceResource == nullptr ? 0 : SourceResource->pResource;
		pin_ptr<Box> PinnedSourceBox = &SourceBox;

		pDevice->CopySubresourceRegion(pDestinationResource, DestinationSubresource, DestinationX, DestinationY, DestinationZ, pSourceResource, SourceSubresource, (D3D10_BOX*)PinnedSourceBox);
	}

	void CopySubresourceRegion(Resource^ DestinationResource, unsigned int DestinationSubresource, unsigned int DestinationX, unsigned int DestinationY, unsigned int DestinationZ, Resource^ SourceResource, unsigned int SourceSubresource)
	{
		ID3D10Resource* pDestinationResource = DestinationResource == nullptr ? 0 : DestinationResource->pResource;
		ID3D10Resource* pSourceResource = SourceResource == nullptr ? 0 : SourceResource->pResource;

		pDevice->CopySubresourceRegion(pDestinationResource, DestinationSubresource, DestinationX, DestinationY, DestinationZ, pSourceResource, SourceSubresource, 0);
	}

	int CreateBlendState(BlendDescription% BlendStateDescription, [Out] BlendState^% BlendState)
	{
		pin_ptr<BlendDescription> PinnedBlendDescription = &BlendStateDescription;

		ID3D10BlendState* pBlendState = 0;
		int Result = pDevice->CreateBlendState((D3D10_BLEND_DESC*)PinnedBlendDescription, &pBlendState);

		if (pBlendState)
		{
			try { BlendState = (Xtro::MDX::Direct3D10::BlendState^)Interfaces[IntPtr(pBlendState)]; }
			catch (KeyNotFoundException^) { BlendState = gcnew Xtro::MDX::Direct3D10::BlendState(IntPtr(pBlendState)); }					
		}
		else BlendState = nullptr;

		return Result;
	}

	int CreateBuffer(BufferDescription% Description)
	{
		pin_ptr<BufferDescription> PinnedDescription = &Description;
		return pDevice->CreateBuffer((D3D10_BUFFER_DESC*)PinnedDescription, 0, 0);
	}

	int CreateBuffer(BufferDescription% Description, [Out] Buffer^% Buffer)
	{
		pin_ptr<BufferDescription> PinnedDescription = &Description;

		ID3D10Buffer* pBuffer = 0;
		int Result = pDevice->CreateBuffer((D3D10_BUFFER_DESC*)PinnedDescription, 0, &pBuffer);

		if (pBuffer)
		{
			try { Buffer = (Xtro::MDX::Direct3D10::Buffer^)Interfaces[IntPtr(pBuffer)]; }
			catch (KeyNotFoundException^) { Buffer = gcnew Xtro::MDX::Direct3D10::Buffer(IntPtr(pBuffer)); }					
		}
		else Buffer = nullptr;

		return Result;
	}

	int CreateBuffer(BufferDescription% Description, SubResourceData% InitialData, [Out] Buffer^% Buffer)
	{
		pin_ptr<BufferDescription> PinnedDescription = &Description;

		D3D10_SUBRESOURCE_DATA NativeInitialData;
		InitialData.ToNative(&NativeInitialData);

		ID3D10Buffer* pBuffer = 0;
		int Result = pDevice->CreateBuffer((D3D10_BUFFER_DESC*)PinnedDescription, &NativeInitialData, &pBuffer);

		if (pBuffer)
		{
			try { Buffer = (Xtro::MDX::Direct3D10::Buffer^)Interfaces[IntPtr(pBuffer)]; }
			catch (KeyNotFoundException^) { Buffer = gcnew Xtro::MDX::Direct3D10::Buffer(IntPtr(pBuffer)); }					
		}
		else Buffer = nullptr;

		return Result;
	}

	int CreateCounter(CounterDescription% Description, [Out] Counter^% Counter)
	{
		pin_ptr<CounterDescription> PinnedDescription = &Description;

		ID3D10Counter* pCounter = 0;
		int Result = pDevice->CreateCounter((D3D10_COUNTER_DESC*)PinnedDescription, &pCounter);

		if (pCounter)
		{
			try { Counter = (Xtro::MDX::Direct3D10::Counter^)Interfaces[IntPtr(pCounter)]; }
			catch (KeyNotFoundException^) { Counter = gcnew Xtro::MDX::Direct3D10::Counter(IntPtr(pCounter)); }					
		}
		else Counter = nullptr;

		return Result;
	}

	int CreateDepthStencilState(DepthStencilDescription% DepthStencilDescription, [Out] DepthStencilState^% DepthStencilState)
	{
		pin_ptr<Xtro::MDX::Direct3D10::DepthStencilDescription> PinnedDepthStencilDescription = &DepthStencilDescription;

		ID3D10DepthStencilState* pDepthStencilState = 0;
		int Result = pDevice->CreateDepthStencilState((D3D10_DEPTH_STENCIL_DESC*)PinnedDepthStencilDescription, &pDepthStencilState);

		if (pDepthStencilState)
		{
			try { DepthStencilState = (Xtro::MDX::Direct3D10::DepthStencilState^)Interfaces[IntPtr(pDepthStencilState)]; }
			catch (KeyNotFoundException^) { DepthStencilState = gcnew Xtro::MDX::Direct3D10::DepthStencilState(IntPtr(pDepthStencilState)); }					
		}
		else DepthStencilState = nullptr;

		return Result;
	}

	int CreateDepthStencilView(Resource^ Resource, DepthStencilViewDescription% Description, [Out] DepthStencilView^% DepthStencilView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<DepthStencilViewDescription> PinnedDescription = &Description;

		ID3D10DepthStencilView* pDepthStencilView = 0;
		int Result = pDevice->CreateDepthStencilView(pResource, (D3D10_DEPTH_STENCIL_VIEW_DESC*)PinnedDescription, &pDepthStencilView);

		if (pDepthStencilView)
		{	
			try { DepthStencilView = (Xtro::MDX::Direct3D10::DepthStencilView^)Interfaces[IntPtr(pDepthStencilView)]; }
			catch (KeyNotFoundException^) { DepthStencilView = gcnew Xtro::MDX::Direct3D10::DepthStencilView(IntPtr(pDepthStencilView)); }
		}
		else DepthStencilView = nullptr;

		return Result;
	}

	int CreateDepthStencilView(Resource^ Resource, [Out] DepthStencilView^% DepthStencilView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;

		ID3D10DepthStencilView* pDepthStencilView = 0;
		int Result = pDevice->CreateDepthStencilView(pResource, 0, &pDepthStencilView);

		if (pDepthStencilView)
		{	
			try { DepthStencilView = (Xtro::MDX::Direct3D10::DepthStencilView^)Interfaces[IntPtr(pDepthStencilView)]; }
			catch (KeyNotFoundException^) { DepthStencilView = gcnew Xtro::MDX::Direct3D10::DepthStencilView(IntPtr(pDepthStencilView)); }
		}
		else DepthStencilView = nullptr;

		return Result;
	}

	int CreateDepthStencilView(Resource^ Resource, DepthStencilViewDescription% Description)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<DepthStencilViewDescription> PinnedDescription = &Description;

		return pDevice->CreateDepthStencilView(pResource, (D3D10_DEPTH_STENCIL_VIEW_DESC*)PinnedDescription, 0);
	}

	int CreateDepthStencilView(Resource^ Resource)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		return pDevice->CreateDepthStencilView(pResource, 0, 0);
	}

	int CreateGeometryShader(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, [Out] GeometryShader^% GeometryShader)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		ID3D10GeometryShader* pGeometryShader = 0;
		int Result = pDevice->CreateGeometryShader(pShaderBytecode, BytecodeLength, &pGeometryShader);

		if (pGeometryShader)
		{
			try { GeometryShader = (Xtro::MDX::Direct3D10::GeometryShader^)Interfaces[IntPtr(pGeometryShader)]; }
			catch (KeyNotFoundException^) { GeometryShader = gcnew Xtro::MDX::Direct3D10::GeometryShader(IntPtr(pGeometryShader)); }					
		}
		else GeometryShader = nullptr;

		return Result;
	}

	int CreateGeometryShader(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;
		return pDevice->CreateGeometryShader(pShaderBytecode, BytecodeLength, 0);
	}

	int CreateGeometryShaderWithStreamOutput(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, array<StreamOutputDeclarationEntry>^ StreamOutputDeclaration, unsigned int NumberOfEntries, unsigned int OutputStreamStride, [Out] GeometryShader^% GeometryShader)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		int Result = 0;
		ID3D10GeometryShader* pGeometryShader = 0;

		unsigned int Count;
		D3D10_SO_DECLARATION_ENTRY* pStreamOutputDeclaration = 0;
		try
		{
			if (StreamOutputDeclaration != nullptr && StreamOutputDeclaration->Length > 0)
			{
				Count = Math::Min(NumberOfEntries, (unsigned int)StreamOutputDeclaration->Length);
				pStreamOutputDeclaration = new D3D10_SO_DECLARATION_ENTRY[Count];
				for (unsigned int No = 0; No < Count; No++)
				{
					StreamOutputDeclaration[No].Marshal(&pStreamOutputDeclaration[No]);
				}
			}

			Result = pDevice->CreateGeometryShaderWithStreamOutput(pShaderBytecode, BytecodeLength, pStreamOutputDeclaration, NumberOfEntries, OutputStreamStride, &pGeometryShader);
		}
		finally
		{
			if (pStreamOutputDeclaration)
			{
				for (unsigned int No = 0; No < Count; No++)
				{
					StreamOutputDeclaration[No].Unmarshal();
				}

				delete[] pStreamOutputDeclaration;
			}
		}

		if (pGeometryShader)
		{
			try { GeometryShader = (Xtro::MDX::Direct3D10::GeometryShader^)Interfaces[IntPtr(pGeometryShader)]; }
			catch (KeyNotFoundException^) { GeometryShader = gcnew Xtro::MDX::Direct3D10::GeometryShader(IntPtr(pGeometryShader)); }					
		}
		else GeometryShader = nullptr;

		return Result;
	}

	int CreateGeometryShaderWithStreamOutput(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, array<StreamOutputDeclarationEntry>^ StreamOutputDeclaration, unsigned int NumberOfEntries, unsigned int OutputStreamStride)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		unsigned int Count;
		D3D10_SO_DECLARATION_ENTRY* pStreamOutputDeclaration = 0;
		try
		{
			if (StreamOutputDeclaration != nullptr && StreamOutputDeclaration->Length > 0)
			{
				Count = Math::Min(NumberOfEntries, (unsigned int)StreamOutputDeclaration->Length);
				pStreamOutputDeclaration = new D3D10_SO_DECLARATION_ENTRY[Count];
				for (unsigned int No = 0; No < Count; No++)
				{
					StreamOutputDeclaration[No].Marshal(&pStreamOutputDeclaration[No]);
				}
			}

			return pDevice->CreateGeometryShaderWithStreamOutput(pShaderBytecode, BytecodeLength, pStreamOutputDeclaration, NumberOfEntries, OutputStreamStride, 0);
		}
		finally
		{
			if (pStreamOutputDeclaration)
			{
				for (unsigned int No = 0; No < Count; No++)
				{
					StreamOutputDeclaration[No].Unmarshal();
				}

				delete[] pStreamOutputDeclaration;
			}
		}
	}

	int CreateInputLayout(array<InputElementDescription>^ InputElementDescriptions, unsigned int NumberOfElements, UnmanagedMemory^ ShaderBytecodeWithInputSignature, SIZE_T BytecodeLength, [Out] InputLayout^% InputLayout)
	{
		void* pShaderBytecodeWithInputSignature = ShaderBytecodeWithInputSignature == nullptr ? 0 : ShaderBytecodeWithInputSignature->pMemory;

		int Result = 0;
		ID3D10InputLayout* pInputLayout = 0;

		unsigned int Count;
		D3D10_INPUT_ELEMENT_DESC* pInputElementDescriptions = 0;
		try
		{
			if (InputElementDescriptions != nullptr && InputElementDescriptions->Length > 0)
			{
				Count = Math::Min(NumberOfElements, (unsigned int)InputElementDescriptions->Length);
				pInputElementDescriptions = new D3D10_INPUT_ELEMENT_DESC[Count];
				for (unsigned int No = 0; No < Count; No++)
				{
					InputElementDescriptions[No].Marshal(&pInputElementDescriptions[No]);
				}
			}

			Result = pDevice->CreateInputLayout(pInputElementDescriptions, NumberOfElements, pShaderBytecodeWithInputSignature, BytecodeLength, &pInputLayout);
		}
		finally
		{
			if (pInputElementDescriptions)
			{
				for (unsigned int No = 0; No < Count; No++)
				{
					InputElementDescriptions[No].Unmarshal();
				}

				delete[] pInputElementDescriptions;
			}
		}

		if (pInputLayout)
		{
			try { InputLayout = (Xtro::MDX::Direct3D10::InputLayout^)Interfaces[IntPtr(pInputLayout)]; }
			catch (KeyNotFoundException^) { InputLayout = gcnew Xtro::MDX::Direct3D10::InputLayout(IntPtr(pInputLayout)); }					
		}
		else InputLayout = nullptr;

		return Result;
	}

	int CreateInputLayout(array<InputElementDescription>^ InputElementDescriptions, unsigned int NumberOfElements, UnmanagedMemory^ ShaderBytecodeWithInputSignature, SIZE_T BytecodeLength)
	{
		void* pShaderBytecodeWithInputSignature = ShaderBytecodeWithInputSignature == nullptr ? 0 : ShaderBytecodeWithInputSignature->pMemory;

		unsigned int Count;
		D3D10_INPUT_ELEMENT_DESC* pInputElementDescriptions = 0;
		try
		{
			if (InputElementDescriptions != nullptr && InputElementDescriptions->Length > 0)
			{
				Count = Math::Min(NumberOfElements, (unsigned int)InputElementDescriptions->Length);
				pInputElementDescriptions = new D3D10_INPUT_ELEMENT_DESC[Count];
				for (unsigned int No = 0; No < Count; No++)
				{
					InputElementDescriptions[No].Marshal(&pInputElementDescriptions[No]);
				}
			}

			return pDevice->CreateInputLayout(pInputElementDescriptions, NumberOfElements, pShaderBytecodeWithInputSignature, BytecodeLength, 0);
		}
		finally
		{
			if (pInputElementDescriptions)
			{
				for (unsigned int No = 0; No < Count; No++)
				{
					InputElementDescriptions[No].Unmarshal();
				}

				delete[] pInputElementDescriptions;
			}
		}
	}

	int CreatePixelShader(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, [Out] PixelShader^% PixelShader)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		ID3D10PixelShader* pPixelShader = 0;
		int Result = pDevice->CreatePixelShader(pShaderBytecode, BytecodeLength, &pPixelShader);

		if (pPixelShader)
		{
			try { PixelShader = (Xtro::MDX::Direct3D10::PixelShader^)Interfaces[IntPtr(pPixelShader)]; }
			catch (KeyNotFoundException^) { PixelShader = gcnew Xtro::MDX::Direct3D10::PixelShader(IntPtr(pPixelShader)); }					
		}
		else PixelShader = nullptr;

		return Result;
	}

	int CreatePixelShader(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;
		return pDevice->CreatePixelShader(pShaderBytecode, BytecodeLength, 0);
	}

	int CreatePredicate(QueryDescription% PredicateDescription, [Out] Predicate^% Predicate)
	{
		pin_ptr<QueryDescription> PinnedPredicateDescription = &PredicateDescription;

		ID3D10Predicate* pPredicate = 0;
		int Result = pDevice->CreatePredicate((D3D10_QUERY_DESC*)PinnedPredicateDescription, &pPredicate);

		if (pPredicate)
		{
			try { Predicate = (Xtro::MDX::Direct3D10::Predicate^)Interfaces[IntPtr(pPredicate)]; }
			catch (KeyNotFoundException^) { Predicate = gcnew Xtro::MDX::Direct3D10::Predicate(IntPtr(pPredicate)); }					
		}
		else Predicate = nullptr;

		return Result;
	}

	int CreateQuery(QueryDescription% QueryDescription, [Out] Query^% Query)
	{
		pin_ptr<Xtro::MDX::Direct3D10::QueryDescription> PinnedQueryDescription = &QueryDescription;

		ID3D10Query* pQuery = 0;
		int Result = pDevice->CreateQuery((D3D10_QUERY_DESC*)PinnedQueryDescription, &pQuery);

		if (pQuery)
		{
			try { Query = (Xtro::MDX::Direct3D10::Query^)Interfaces[IntPtr(pQuery)]; }
			catch (KeyNotFoundException^) { Query = gcnew Xtro::MDX::Direct3D10::Query(IntPtr(pQuery)); }					
		}
		else Query = nullptr;

		return Result;
	}

	int CreateRasterizerState(RasterizerDescription% Description, [Out] RasterizerState^% RasterizerState)
	{
		pin_ptr<RasterizerDescription> PinnedDescription = &Description;

		ID3D10RasterizerState* pRasterizerState = 0;
		int Result = pDevice->CreateRasterizerState((D3D10_RASTERIZER_DESC*)PinnedDescription, &pRasterizerState);

		if (pRasterizerState)
		{
			try { RasterizerState = (Xtro::MDX::Direct3D10::RasterizerState^)Interfaces[IntPtr(pRasterizerState)]; }
			catch (KeyNotFoundException^) { RasterizerState = gcnew Xtro::MDX::Direct3D10::RasterizerState(IntPtr(pRasterizerState)); }					
		}
		else RasterizerState = nullptr;

		return Result;
	}

	int CreateRenderTargetView(Xtro::MDX::Direct3D10::Resource^ Resource, RenderTargetViewDescription% Description, [Out] RenderTargetView^% RenderTargetView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<RenderTargetViewDescription> PinnedDescription = &Description;

		ID3D10RenderTargetView* pRenderTargetView = 0;
		int Result = pDevice->CreateRenderTargetView(pResource, (D3D10_RENDER_TARGET_VIEW_DESC*)PinnedDescription, &pRenderTargetView);

		if (pRenderTargetView)
		{	
			try { RenderTargetView = (Xtro::MDX::Direct3D10::RenderTargetView^)Interfaces[IntPtr(pRenderTargetView)]; }
			catch (KeyNotFoundException^) { RenderTargetView = gcnew Xtro::MDX::Direct3D10::RenderTargetView(IntPtr(pRenderTargetView)); }
		}
		else RenderTargetView = nullptr;

		return Result;
	}

	int CreateRenderTargetView(Xtro::MDX::Direct3D10::Resource^ Resource, [Out] RenderTargetView^% RenderTargetView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;

		ID3D10RenderTargetView* pRenderTargetView = 0;
		int Result = pDevice->CreateRenderTargetView(pResource, 0, &pRenderTargetView);

		if (pRenderTargetView)
		{	
			try { RenderTargetView = (Xtro::MDX::Direct3D10::RenderTargetView^)Interfaces[IntPtr(pRenderTargetView)]; }
			catch (KeyNotFoundException^) { RenderTargetView = gcnew Xtro::MDX::Direct3D10::RenderTargetView(IntPtr(pRenderTargetView)); }
		}
		else RenderTargetView = nullptr;

		return Result;
	}

	int CreateRenderTargetView(Xtro::MDX::Direct3D10::Resource^ Resource, RenderTargetViewDescription% Description)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<RenderTargetViewDescription> PinnedDescription = &Description;

		return pDevice->CreateRenderTargetView(pResource, (D3D10_RENDER_TARGET_VIEW_DESC*)PinnedDescription, 0);
	}

	int CreateRenderTargetView(Xtro::MDX::Direct3D10::Resource^ Resource)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;

		return pDevice->CreateRenderTargetView(pResource, 0, 0);
	}

	int CreateSamplerState(SamplerDescription% Description, [Out] SamplerState^% SamplerState)
	{
		pin_ptr<SamplerDescription> PinnedDescription = &Description;

		ID3D10SamplerState* pSamplerState = 0;
		int Result = pDevice->CreateSamplerState((D3D10_SAMPLER_DESC*)PinnedDescription, &pSamplerState);

		if (pSamplerState)
		{
			try { SamplerState = (Xtro::MDX::Direct3D10::SamplerState^)Interfaces[IntPtr(pSamplerState)]; }
			catch (KeyNotFoundException^) { SamplerState = gcnew Xtro::MDX::Direct3D10::SamplerState(IntPtr(pSamplerState)); }					
		}
		else SamplerState = nullptr;

		return Result;
	}

	int CreateShaderResourceView(Resource^ Resource, ShaderResourceViewDescription% Description, [Out] ShaderResourceView^% ShaderResourceView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<ShaderResourceViewDescription> PinnedDescription = &Description;

		ID3D10ShaderResourceView* pShaderResourceView = 0;
		int Result = pDevice->CreateShaderResourceView(pResource, (D3D10_SHADER_RESOURCE_VIEW_DESC*)PinnedDescription, &pShaderResourceView);

		if (pShaderResourceView)
		{
			try { ShaderResourceView = (Xtro::MDX::Direct3D10::ShaderResourceView^)Interfaces[IntPtr(pShaderResourceView)]; }
			catch (KeyNotFoundException^) { ShaderResourceView = gcnew Xtro::MDX::Direct3D10::ShaderResourceView(IntPtr(pShaderResourceView)); }					
		}
		else ShaderResourceView = nullptr;

		return Result;
	}

	int CreateShaderResourceView(Resource^ Resource, [Out] ShaderResourceView^% ShaderResourceView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;

		ID3D10ShaderResourceView* pShaderResourceView = 0;
		int Result = pDevice->CreateShaderResourceView(pResource, 0, &pShaderResourceView);

		if (pShaderResourceView)
		{
			try { ShaderResourceView = (Xtro::MDX::Direct3D10::ShaderResourceView^)Interfaces[IntPtr(pShaderResourceView)]; }
			catch (KeyNotFoundException^) { ShaderResourceView = gcnew Xtro::MDX::Direct3D10::ShaderResourceView(IntPtr(pShaderResourceView)); }					
		}
		else ShaderResourceView = nullptr;

		return Result;
	}

	int CreateShaderResourceView(Resource^ Resource, ShaderResourceViewDescription% Description)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<ShaderResourceViewDescription> PinnedDescription = &Description;

		return pDevice->CreateShaderResourceView(pResource, (D3D10_SHADER_RESOURCE_VIEW_DESC*)PinnedDescription, 0);
	}

	int CreateShaderResourceView(Resource^ Resource)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;

		return pDevice->CreateShaderResourceView(pResource, 0, 0);
	}

	int CreateTexture1D(Texture1D_Description% Description, array<SubResourceData>^ InitialData, [Out] Texture1D^% Texture1D)
	{
		pin_ptr<Texture1D_Description> PinnedDescription = &Description;

		int Result = 0;
		ID3D10Texture1D* pTexture1D = 0;

		D3D10_SUBRESOURCE_DATA* pInitialData = 0;
		try
		{
			if (InitialData != nullptr && InitialData->Length > 0)
			{
				pInitialData = new D3D10_SUBRESOURCE_DATA[InitialData->Length];

				for (int No = 0; No < InitialData->Length; No++)
				{
					InitialData[No].ToNative(&pInitialData[No]);
				}
			}

			Result = pDevice->CreateTexture1D((D3D10_TEXTURE1D_DESC*)PinnedDescription, pInitialData, &pTexture1D);
		}
		finally
		{
			if (pInitialData) delete[] pInitialData;
		}

		if (pTexture1D)
		{
			try { Texture1D = (Xtro::MDX::Direct3D10::Texture1D^)Interfaces[IntPtr(pTexture1D)]; }
			catch (KeyNotFoundException^) { Texture1D = gcnew Xtro::MDX::Direct3D10::Texture1D(IntPtr(pTexture1D)); }					
		}
		else Texture1D = nullptr;

		return Result;
	}

	int CreateTexture1D(Texture1D_Description% Description, array<SubResourceData>^ InitialData)
	{
		pin_ptr<Texture1D_Description> PinnedDescription = &Description;

		D3D10_SUBRESOURCE_DATA* pInitialData = 0;
		try
		{
			if (InitialData != nullptr && InitialData->Length > 0)
			{
				pInitialData = new D3D10_SUBRESOURCE_DATA[InitialData->Length];

				for (int No = 0; No < InitialData->Length; No++)
				{
					InitialData[No].ToNative(&pInitialData[No]);
				}
			}

			return pDevice->CreateTexture1D((D3D10_TEXTURE1D_DESC*)PinnedDescription, pInitialData, 0);
		}
		finally
		{
			if (pInitialData) delete[] pInitialData;
		}
	}

	int CreateTexture2D(Texture2D_Description% Description, array<SubResourceData>^ InitialData, [Out] Texture2D^% Texture2D)
	{
		pin_ptr<Texture2D_Description> PinnedDescription = &Description;

		int Result = 0;
		ID3D10Texture2D* pTexture2D = 0;

		D3D10_SUBRESOURCE_DATA* pInitialData = 0;
		try
		{
			if (InitialData != nullptr && InitialData->Length > 0)
			{
				pInitialData = new D3D10_SUBRESOURCE_DATA[InitialData->Length];

				for (int No = 0; No < InitialData->Length; No++)
				{
					InitialData[No].ToNative(&pInitialData[No]);
				}
			}

			Result = pDevice->CreateTexture2D((D3D10_TEXTURE2D_DESC*)PinnedDescription, pInitialData, &pTexture2D);
		}
		finally
		{
			if (pInitialData) delete[] pInitialData;
		}

		if (pTexture2D)
		{
			try { Texture2D = (Xtro::MDX::Direct3D10::Texture2D^)Interfaces[IntPtr(pTexture2D)]; }
			catch (KeyNotFoundException^) { Texture2D = gcnew Xtro::MDX::Direct3D10::Texture2D(IntPtr(pTexture2D)); }					
		}
		else Texture2D = nullptr;

		return Result;
	}

	int CreateTexture2D(Texture2D_Description% Description, array<SubResourceData>^ InitialData)
	{
		pin_ptr<Texture2D_Description> PinnedDescription = &Description;

		D3D10_SUBRESOURCE_DATA* pInitialData = 0;
		try
		{
			if (InitialData != nullptr && InitialData->Length > 0)
			{
				pInitialData = new D3D10_SUBRESOURCE_DATA[InitialData->Length];

				for (int No = 0; No < InitialData->Length; No++)
				{
					InitialData[No].ToNative(&pInitialData[No]);
				}
			}

			return pDevice->CreateTexture2D((D3D10_TEXTURE2D_DESC*)PinnedDescription, pInitialData, 0);
		}
		finally
		{
			if (pInitialData) delete[] pInitialData;
		}
	}

	int CreateTexture3D(Texture3D_Description% Description, array<SubResourceData>^ InitialData, [Out] Texture3D^% Texture3D)
	{
		pin_ptr<Texture3D_Description> PinnedDescription = &Description;

		int Result = 0;
		ID3D10Texture3D* pTexture3D = 0;

		D3D10_SUBRESOURCE_DATA* pInitialData = 0;
		try
		{
			if (InitialData != nullptr && InitialData->Length > 0)
			{
				pInitialData = new D3D10_SUBRESOURCE_DATA[InitialData->Length];

				for (int No = 0; No < InitialData->Length; No++)
				{
					InitialData[No].ToNative(&pInitialData[No]);
				}
			}

			Result = pDevice->CreateTexture3D((D3D10_TEXTURE3D_DESC*)PinnedDescription, pInitialData, &pTexture3D);
		}
		finally
		{
			if (pInitialData) delete[] pInitialData;
		}

		if (pTexture3D)
		{
			try { Texture3D = (Xtro::MDX::Direct3D10::Texture3D^)Interfaces[IntPtr(pTexture3D)]; }
			catch (KeyNotFoundException^) { Texture3D = gcnew Xtro::MDX::Direct3D10::Texture3D(IntPtr(pTexture3D)); }					
		}
		else Texture3D = nullptr;

		return Result;
	}

	int CreateTexture3D(Texture3D_Description% Description, array<SubResourceData>^ InitialData)
	{
		pin_ptr<Texture3D_Description> PinnedDescription = &Description;

		D3D10_SUBRESOURCE_DATA* pInitialData = 0;
		try
		{
			if (InitialData != nullptr && InitialData->Length > 0)
			{
				pInitialData = new D3D10_SUBRESOURCE_DATA[InitialData->Length];

				for (int No = 0; No < InitialData->Length; No++)
				{
					InitialData[No].ToNative(&pInitialData[No]);
				}
			}

			return pDevice->CreateTexture3D((D3D10_TEXTURE3D_DESC*)PinnedDescription, pInitialData, 0);
		}
		finally
		{
			if (pInitialData) delete[] pInitialData;
		}
	}

	int CreateVertexShader(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, [Out] VertexShader^% VertexShader)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		ID3D10VertexShader* pVertexShader = 0;
		int Result = pDevice->CreateVertexShader(pShaderBytecode, BytecodeLength, &pVertexShader);

		if (pVertexShader)
		{
			try { VertexShader = (Xtro::MDX::Direct3D10::VertexShader^)Interfaces[IntPtr(pVertexShader)]; }
			catch (KeyNotFoundException^) { VertexShader = gcnew Xtro::MDX::Direct3D10::VertexShader(IntPtr(pVertexShader)); }					
		}
		else VertexShader = nullptr;

		return Result;
	}

	int CreateVertexShader(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;
		return pDevice->CreateVertexShader(pShaderBytecode, BytecodeLength, 0);
	}

	void Draw(unsigned int VertexCount, unsigned int StartVertexLocation)
	{
		pDevice->Draw(VertexCount, StartVertexLocation);
	}

	void DrawAuto()
	{
		pDevice->DrawAuto();
	}

	void DrawIndexed(unsigned int IndexCount, unsigned int StartIndexLocation, int BaseVertexLocation)
	{
		pDevice->DrawIndexed(IndexCount, StartIndexLocation, BaseVertexLocation);
	}

	void DrawIndexedInstanced(unsigned int IndexCountPerInstance, unsigned int InstanceCount, unsigned int StartIndexLocation, int BaseVertexLocation, unsigned int StartInstanceLocation)
	{
		pDevice->DrawIndexedInstanced(IndexCountPerInstance, InstanceCount, StartIndexLocation, BaseVertexLocation, StartInstanceLocation);
	}

	void DrawInstanced(unsigned int VertexCountPerInstance, unsigned int InstanceCount, unsigned int StartVertexLocation, unsigned int StartInstanceLocation)
	{
		pDevice->DrawInstanced(VertexCountPerInstance, InstanceCount, StartVertexLocation, StartInstanceLocation);
	}

	void Flush()
	{
		pDevice->Flush();
	}

	void IA_GetInputLayout([Out] InputLayout^% InputLayout)
	{
		ID3D10InputLayout* pInputLayout = 0;
		pDevice->IAGetInputLayout(&pInputLayout);

		if (pInputLayout)
		{
			try { InputLayout = (Xtro::MDX::Direct3D10::InputLayout^)Interfaces[IntPtr(pInputLayout)]; }
			catch (KeyNotFoundException^) { InputLayout = gcnew Xtro::MDX::Direct3D10::InputLayout(IntPtr(pInputLayout)); }					
		}
		else InputLayout = nullptr;
	}

	void IA_GetPrimitiveTopology([Out] PrimitiveTopology% Topology)
	{
		pin_ptr<PrimitiveTopology> PinnedTopology = &Topology;

		pDevice->IAGetPrimitiveTopology((D3D10_PRIMITIVE_TOPOLOGY*)PinnedTopology);
	}

	void IA_SetIndexBuffer(Buffer^ IndexBuffer, Format Format, unsigned int Offset)
	{
		ID3D10Buffer* pIndexBuffer = IndexBuffer == nullptr ? 0 : IndexBuffer->pBuffer;

		pDevice->IASetIndexBuffer(pIndexBuffer, (DXGI_FORMAT)Format, Offset);
	}

	void IA_SetInputLayout(InputLayout^ InputLayout)
	{
		ID3D10InputLayout* pInputLayout = InputLayout == nullptr ? 0 : InputLayout->pInputLayout;

		pDevice->IASetInputLayout(pInputLayout);
	}

	void IA_SetPrimitiveTopology(PrimitiveTopology Topology)
	{
		pDevice->IASetPrimitiveTopology((D3D10_PRIMITIVE_TOPOLOGY)Topology);
	}

	void IA_SetVertexBuffers(unsigned int StartSlot, unsigned int NumberOfBuffers, array<Buffer^>^ VertexBuffers, array<unsigned int>^ Strides, array<unsigned int>^ Offsets)
	{
		pin_ptr<unsigned int> PinnedStrides = Strides != nullptr && Strides->Length > 0 ? &Strides[0] : nullptr;
		pin_ptr<unsigned int> PinnedOffsets = Offsets != nullptr && Offsets->Length > 0 ? &Offsets[0] : nullptr;

		ID3D10Buffer** pVertexBuffers = 0;
		try
		{
			if (VertexBuffers != nullptr && VertexBuffers->Length > 0)
			{
				unsigned int Count = Math::Min(NumberOfBuffers, (unsigned int)VertexBuffers->Length);
				pVertexBuffers = new ID3D10Buffer*[Count];
				for (unsigned int No = 0; No < Count; No++)
				{
					pVertexBuffers[No] = VertexBuffers[No] == nullptr ? 0 : VertexBuffers[No]->pBuffer;
				}
			}

			pDevice->IASetVertexBuffers(StartSlot, NumberOfBuffers, pVertexBuffers, PinnedStrides, PinnedOffsets);
		}
		finally
		{
			if (pVertexBuffers) delete[] pVertexBuffers;
		}
	}

	void OM_GetBlendState([Out] BlendState^% BlendState, [Out] Float4% BlendFactor, [Out] unsigned int% SampleMask)
	{
		pin_ptr<Float4> PinnedBlendFactor = &BlendFactor;
		pin_ptr<unsigned int> PinnedSampleMask = &SampleMask;

		ID3D10BlendState* pBlendState = 0;
		pDevice->OMGetBlendState(&pBlendState, (float*)PinnedBlendFactor, PinnedSampleMask);

		if (pBlendState)
		{
			try { BlendState = (Xtro::MDX::Direct3D10::BlendState^)Interfaces[IntPtr(pBlendState)]; }
			catch (KeyNotFoundException^) { BlendState = gcnew Xtro::MDX::Direct3D10::BlendState(IntPtr(pBlendState)); }					
		}
		else BlendState = nullptr;
	}

	void OM_SetBlendState(BlendState^ BlendState, Float4% BlendFactor, unsigned int SampleMask)
	{
		ID3D10BlendState* pBlendState = BlendState == nullptr ? 0 : BlendState->pBlendState;
		pin_ptr<Float4> PinnedBlendFactor = &BlendFactor;

		pDevice->OMSetBlendState(pBlendState, (float*)PinnedBlendFactor, SampleMask);
	}

	void OM_SetDepthStencilState(DepthStencilState^ DepthStencilState, unsigned int StencilReference)
	{
		ID3D10DepthStencilState* pDepthStencilState = DepthStencilState == nullptr ? 0 : DepthStencilState->pDepthStencilState;

		pDevice->OMSetDepthStencilState(pDepthStencilState, StencilReference);
	}

	void OM_SetRenderTargets(unsigned int NumberOfViews, array<RenderTargetView^>^ RenderTargetViews, DepthStencilView^ DepthStencilView)
	{
		ID3D10DepthStencilView* pDepthStencilView = DepthStencilView == nullptr ? 0 : DepthStencilView->pDepthStencilView;

		ID3D10RenderTargetView** pRenderTargetViews = 0;
		try
		{
			if (RenderTargetViews != nullptr && RenderTargetViews->Length > 0)
			{
				unsigned int Count = Math::Min(NumberOfViews, (unsigned int)RenderTargetViews->Length);
				pRenderTargetViews = new ID3D10RenderTargetView*[Count];
				for (unsigned int No = 0; No < Count; No++)
				{
					pRenderTargetViews[No] = RenderTargetViews[No] == nullptr ? 0 : RenderTargetViews[No]->pRenderTargetView;
				}
			}

			pDevice->OMSetRenderTargets(NumberOfViews, pRenderTargetViews, pDepthStencilView);
		}
		finally
		{
			if (pRenderTargetViews) delete[] pRenderTargetViews;
		}
	}

	void PS_SetShaderResources(unsigned int StartSlot, unsigned int NumberOfViews, array<ShaderResourceView^>^ ShaderResourceViews)
	{
		ID3D10ShaderResourceView** pShaderResourceViews = 0;
		try
		{
			if (ShaderResourceViews != nullptr && ShaderResourceViews->Length > 0)
			{
				unsigned int Count = Math::Min(NumberOfViews, (unsigned int)ShaderResourceViews->Length);
				pShaderResourceViews = new ID3D10ShaderResourceView*[Count];
				for (unsigned int No = 0; No < Count; No++)
				{
					pShaderResourceViews[No] = ShaderResourceViews[No] == nullptr ? 0 : ShaderResourceViews[No]->pShaderResourceView;
				}
			}

			pDevice->PSSetShaderResources(StartSlot, NumberOfViews, pShaderResourceViews);
		}
		finally
		{
			if (pShaderResourceViews) delete[] pShaderResourceViews;
		}
	}

	void RS_GetScissorRectangles(unsigned int% NumberOfRectangles, array<System::Drawing::Rectangle>^ Rectangles)
	{
		pin_ptr<unsigned int> PinnedNumberOfRectangles = &NumberOfRectangles;
		RECT* pRectangles = Rectangles != nullptr && Rectangles->Length > 0 ? new RECT[Rectangles->Length] : 0;

		try
		{
			pDevice->RSGetScissorRects(PinnedNumberOfRectangles, pRectangles);

			if (pRectangles)
			{
				unsigned int Count = Math::Min(NumberOfRectangles, (unsigned int)Rectangles->Length);
				for (unsigned int No = 0; No < Count; No++)
				{
					RECT* pRectangle = &pRectangles[No];
					Rectangles[No].X = pRectangle->left;
					Rectangles[No].Y = pRectangle->top;
					Rectangles[No].Width = pRectangle->right - pRectangle->left;
					Rectangles[No].Height = pRectangle->bottom - pRectangle->top;
				}
			}
		}
		finally
		{
			if (pRectangles) delete[] pRectangles;
		}
	}

	void RS_GetState([Out] RasterizerState^% RasterizerState)
	{
		ID3D10RasterizerState* pRasterizerState = 0;
		pDevice->RSGetState(&pRasterizerState);

		if (pRasterizerState)
		{
			try { RasterizerState = (Xtro::MDX::Direct3D10::RasterizerState^)Interfaces[IntPtr(pRasterizerState)]; }
			catch (KeyNotFoundException^) { RasterizerState = gcnew Xtro::MDX::Direct3D10::RasterizerState(IntPtr(pRasterizerState)); }					
		}
		else RasterizerState = nullptr;
	}

    void RS_GetViewports(unsigned int% NumberOfViewports, array<Viewport>^ Viewports)
    {
        pin_ptr<unsigned int> PinnedNumberOfViewports = &NumberOfViewports;
        pin_ptr<Viewport> PinnedViewports = Viewports != nullptr && Viewports->Length > 0 ? &Viewports[0] : nullptr;

        pDevice->RSGetViewports(PinnedNumberOfViewports, (D3D10_VIEWPORT*)PinnedViewports);
    }

	void RS_SetState(RasterizerState^ RasterizerState)
	{
		ID3D10RasterizerState* pRasterizerState = RasterizerState == nullptr ? 0 : RasterizerState->pRasterizerState;

		pDevice->RSSetState(pRasterizerState);
	}

    void RS_SetViewports(unsigned int NumberOfViewports, array<Viewport>^ Viewports)
    {
        pin_ptr<Viewport> PinnedViewports = Viewports != nullptr && Viewports->Length > 0 ? &Viewports[0] : nullptr;

        pDevice->RSSetViewports(NumberOfViewports, (D3D10_VIEWPORT*)PinnedViewports);
    }

	void UpdateSubresource(Resource^ DestinationResource, unsigned int DestinationSubresource, Box% DestinationBox, UnmanagedMemory^ SourceData, unsigned int SourceRowPitch, unsigned int SourceDepthPitch)
	{
		ID3D10Resource* pDestinationResource = DestinationResource == nullptr ? 0 : DestinationResource->pResource;
		pin_ptr<Box> PinnedDestinationBox = &DestinationBox;
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		pDevice->UpdateSubresource(pDestinationResource, DestinationSubresource, (D3D10_BOX*)PinnedDestinationBox, pSourceData, SourceRowPitch, SourceDepthPitch);
	}

	void UpdateSubresource(Resource^ DestinationResource, unsigned int DestinationSubresource, UnmanagedMemory^ SourceData, unsigned int SourceRowPitch, unsigned int SourceDepthPitch)
	{
		ID3D10Resource* pDestinationResource = DestinationResource == nullptr ? 0 : DestinationResource->pResource;
		void* pSourceData = SourceData == nullptr ? 0 : SourceData->pMemory;

		pDevice->UpdateSubresource(pDestinationResource, DestinationSubresource, 0, pSourceData, SourceRowPitch, SourceDepthPitch);
	}
};