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
	int CheckCounter(CounterDescription% Description, [Out] CounterType% Type, [Out] unsigned int% ActiveCounters, Generic::RefObject<String^>^ Name, Generic::ValueObject<unsigned int>^ NameLength, Generic::RefObject<String^>^ Units, Generic::ValueObject<unsigned int>^ UnitsLength, Generic::RefObject<String^>^ DescriptionString, Generic::ValueObject<unsigned int>^ DescriptionLength)
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
		pin_ptr<Direct3D10::CounterInfo> PinnedCounterInfo = &CounterInfo;
		pDevice->CheckCounterInfo((D3D10_COUNTER_INFO*)PinnedCounterInfo);
	}
  	
	int CheckFormatSupport(Format Format, [Out] FormatSupport% FormatSupport)
	{
		pin_ptr<Direct3D10::FormatSupport> PinnedFormatSupport = &FormatSupport;
		return pDevice->CheckFormatSupport((DXGI_FORMAT)Format, (unsigned int*)PinnedFormatSupport);
	}

	int CheckMultisampleQualityLevels(Format Format, unsigned int SampleCount, [Out] unsigned int% NumberOfQualityLevels)
	{
		pin_ptr<unsigned int> PinnedNumberOfQualityLevels = &NumberOfQualityLevels;
		return pDevice->CheckMultisampleQualityLevels((DXGI_FORMAT)Format, SampleCount, PinnedNumberOfQualityLevels);
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
			try { BlendState = (Direct3D10::BlendState^)Interfaces[IntPtr(pBlendState)]; }
			catch (KeyNotFoundException^) { BlendState = gcnew Direct3D10::BlendState(IntPtr(pBlendState)); }					
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
			try { Buffer = (Direct3D10::Buffer^)Interfaces[IntPtr(pBuffer)]; }
			catch (KeyNotFoundException^) { Buffer = gcnew Direct3D10::Buffer(IntPtr(pBuffer)); }					
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
			try { Buffer = (Direct3D10::Buffer^)Interfaces[IntPtr(pBuffer)]; }
			catch (KeyNotFoundException^) { Buffer = gcnew Direct3D10::Buffer(IntPtr(pBuffer)); }					
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
			try { Counter = (Direct3D10::Counter^)Interfaces[IntPtr(pCounter)]; }
			catch (KeyNotFoundException^) { Counter = gcnew Direct3D10::Counter(IntPtr(pCounter)); }					
		}
		else Counter = nullptr;

		return Result;
	}

	int CreateDepthStencilState(DepthStencilDescription% DepthStencilDescription, [Out] DepthStencilState^% DepthStencilState)
	{
		pin_ptr<Direct3D10::DepthStencilDescription> PinnedDepthStencilDescription = &DepthStencilDescription;

		ID3D10DepthStencilState* pDepthStencilState = 0;
		int Result = pDevice->CreateDepthStencilState((D3D10_DEPTH_STENCIL_DESC*)PinnedDepthStencilDescription, &pDepthStencilState);

		if (pDepthStencilState)
		{
			try { DepthStencilState = (Direct3D10::DepthStencilState^)Interfaces[IntPtr(pDepthStencilState)]; }
			catch (KeyNotFoundException^) { DepthStencilState = gcnew Direct3D10::DepthStencilState(IntPtr(pDepthStencilState)); }					
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
			try { DepthStencilView = (Direct3D10::DepthStencilView^)Interfaces[IntPtr(pDepthStencilView)]; }
			catch (KeyNotFoundException^) { DepthStencilView = gcnew Direct3D10::DepthStencilView(IntPtr(pDepthStencilView)); }
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
			try { DepthStencilView = (Direct3D10::DepthStencilView^)Interfaces[IntPtr(pDepthStencilView)]; }
			catch (KeyNotFoundException^) { DepthStencilView = gcnew Direct3D10::DepthStencilView(IntPtr(pDepthStencilView)); }
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
			try { GeometryShader = (Direct3D10::GeometryShader^)Interfaces[IntPtr(pGeometryShader)]; }
			catch (KeyNotFoundException^) { GeometryShader = gcnew Direct3D10::GeometryShader(IntPtr(pGeometryShader)); }					
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

		unsigned int Length;
		D3D10_SO_DECLARATION_ENTRY* pStreamOutputDeclaration = 0;
		try
		{
			if (StreamOutputDeclaration != nullptr && StreamOutputDeclaration->Length > 0)
			{
				Length = Math::Min(NumberOfEntries, (unsigned int)StreamOutputDeclaration->Length);
				pStreamOutputDeclaration = new D3D10_SO_DECLARATION_ENTRY[Length];
				for (unsigned int No = 0; No < Length; No++)
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
				for (unsigned int No = 0; No < Length; No++)
				{
					StreamOutputDeclaration[No].Unmarshal();
				}

				delete[] pStreamOutputDeclaration;
			}
		}

		if (pGeometryShader)
		{
			try { GeometryShader = (Direct3D10::GeometryShader^)Interfaces[IntPtr(pGeometryShader)]; }
			catch (KeyNotFoundException^) { GeometryShader = gcnew Direct3D10::GeometryShader(IntPtr(pGeometryShader)); }					
		}
		else GeometryShader = nullptr;

		return Result;
	}

	int CreateGeometryShaderWithStreamOutput(UnmanagedMemory^ ShaderBytecode, SIZE_T BytecodeLength, array<StreamOutputDeclarationEntry>^ StreamOutputDeclaration, unsigned int NumberOfEntries, unsigned int OutputStreamStride)
	{
		void* pShaderBytecode = ShaderBytecode == nullptr ? 0 : ShaderBytecode->pMemory;

		unsigned int Length;
		D3D10_SO_DECLARATION_ENTRY* pStreamOutputDeclaration = 0;
		try
		{
			if (StreamOutputDeclaration != nullptr && StreamOutputDeclaration->Length > 0)
			{
				Length = Math::Min(NumberOfEntries, (unsigned int)StreamOutputDeclaration->Length);
				pStreamOutputDeclaration = new D3D10_SO_DECLARATION_ENTRY[Length];
				for (unsigned int No = 0; No < Length; No++)
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
				for (unsigned int No = 0; No < Length; No++)
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

		unsigned int Length;
		D3D10_INPUT_ELEMENT_DESC* pInputElementDescriptions = 0;
		try
		{
			if (InputElementDescriptions != nullptr && InputElementDescriptions->Length > 0)
			{
				Length = Math::Min(NumberOfElements, (unsigned int)InputElementDescriptions->Length);
				pInputElementDescriptions = new D3D10_INPUT_ELEMENT_DESC[Length];
				for (unsigned int No = 0; No < Length; No++)
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
				for (unsigned int No = 0; No < Length; No++)
				{
					InputElementDescriptions[No].Unmarshal();
				}

				delete[] pInputElementDescriptions;
			}
		}

		if (pInputLayout)
		{
			try { InputLayout = (Direct3D10::InputLayout^)Interfaces[IntPtr(pInputLayout)]; }
			catch (KeyNotFoundException^) { InputLayout = gcnew Direct3D10::InputLayout(IntPtr(pInputLayout)); }					
		}
		else InputLayout = nullptr;

		return Result;
	}

	int CreateInputLayout(array<InputElementDescription>^ InputElementDescriptions, unsigned int NumberOfElements, UnmanagedMemory^ ShaderBytecodeWithInputSignature, SIZE_T BytecodeLength)
	{
		void* pShaderBytecodeWithInputSignature = ShaderBytecodeWithInputSignature == nullptr ? 0 : ShaderBytecodeWithInputSignature->pMemory;

		unsigned int Length;
		D3D10_INPUT_ELEMENT_DESC* pInputElementDescriptions = 0;
		try
		{
			if (InputElementDescriptions != nullptr && InputElementDescriptions->Length > 0)
			{
				Length = Math::Min(NumberOfElements, (unsigned int)InputElementDescriptions->Length);
				pInputElementDescriptions = new D3D10_INPUT_ELEMENT_DESC[Length];
				for (unsigned int No = 0; No < Length; No++)
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
				for (unsigned int No = 0; No < Length; No++)
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
			try { PixelShader = (Direct3D10::PixelShader^)Interfaces[IntPtr(pPixelShader)]; }
			catch (KeyNotFoundException^) { PixelShader = gcnew Direct3D10::PixelShader(IntPtr(pPixelShader)); }					
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
			try { Predicate = (Direct3D10::Predicate^)Interfaces[IntPtr(pPredicate)]; }
			catch (KeyNotFoundException^) { Predicate = gcnew Direct3D10::Predicate(IntPtr(pPredicate)); }					
		}
		else Predicate = nullptr;

		return Result;
	}

	int CreateQuery(QueryDescription% QueryDescription, [Out] Query^% Query)
	{
		pin_ptr<Direct3D10::QueryDescription> PinnedQueryDescription = &QueryDescription;

		ID3D10Query* pQuery = 0;
		int Result = pDevice->CreateQuery((D3D10_QUERY_DESC*)PinnedQueryDescription, &pQuery);

		if (pQuery)
		{
			try { Query = (Direct3D10::Query^)Interfaces[IntPtr(pQuery)]; }
			catch (KeyNotFoundException^) { Query = gcnew Direct3D10::Query(IntPtr(pQuery)); }					
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
			try { RasterizerState = (Direct3D10::RasterizerState^)Interfaces[IntPtr(pRasterizerState)]; }
			catch (KeyNotFoundException^) { RasterizerState = gcnew Direct3D10::RasterizerState(IntPtr(pRasterizerState)); }					
		}
		else RasterizerState = nullptr;

		return Result;
	}

	int CreateRenderTargetView(Direct3D10::Resource^ Resource, RenderTargetViewDescription% Description, [Out] RenderTargetView^% RenderTargetView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<RenderTargetViewDescription> PinnedDescription = &Description;

		ID3D10RenderTargetView* pRenderTargetView = 0;
		int Result = pDevice->CreateRenderTargetView(pResource, (D3D10_RENDER_TARGET_VIEW_DESC*)PinnedDescription, &pRenderTargetView);

		if (pRenderTargetView)
		{	
			try { RenderTargetView = (Direct3D10::RenderTargetView^)Interfaces[IntPtr(pRenderTargetView)]; }
			catch (KeyNotFoundException^) { RenderTargetView = gcnew Direct3D10::RenderTargetView(IntPtr(pRenderTargetView)); }
		}
		else RenderTargetView = nullptr;

		return Result;
	}

	int CreateRenderTargetView(Direct3D10::Resource^ Resource, [Out] RenderTargetView^% RenderTargetView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;

		ID3D10RenderTargetView* pRenderTargetView = 0;
		int Result = pDevice->CreateRenderTargetView(pResource, 0, &pRenderTargetView);

		if (pRenderTargetView)
		{	
			try { RenderTargetView = (Direct3D10::RenderTargetView^)Interfaces[IntPtr(pRenderTargetView)]; }
			catch (KeyNotFoundException^) { RenderTargetView = gcnew Direct3D10::RenderTargetView(IntPtr(pRenderTargetView)); }
		}
		else RenderTargetView = nullptr;

		return Result;
	}

	int CreateRenderTargetView(Direct3D10::Resource^ Resource, RenderTargetViewDescription% Description)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<RenderTargetViewDescription> PinnedDescription = &Description;

		return pDevice->CreateRenderTargetView(pResource, (D3D10_RENDER_TARGET_VIEW_DESC*)PinnedDescription, 0);
	}

	int CreateRenderTargetView(Direct3D10::Resource^ Resource)
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
			try { SamplerState = (Direct3D10::SamplerState^)Interfaces[IntPtr(pSamplerState)]; }
			catch (KeyNotFoundException^) { SamplerState = gcnew Direct3D10::SamplerState(IntPtr(pSamplerState)); }					
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
			try { ShaderResourceView = (Direct3D10::ShaderResourceView^)Interfaces[IntPtr(pShaderResourceView)]; }
			catch (KeyNotFoundException^) { ShaderResourceView = gcnew Direct3D10::ShaderResourceView(IntPtr(pShaderResourceView)); }					
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
			try { ShaderResourceView = (Direct3D10::ShaderResourceView^)Interfaces[IntPtr(pShaderResourceView)]; }
			catch (KeyNotFoundException^) { ShaderResourceView = gcnew Direct3D10::ShaderResourceView(IntPtr(pShaderResourceView)); }					
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
			try { Texture1D = (Direct3D10::Texture1D^)Interfaces[IntPtr(pTexture1D)]; }
			catch (KeyNotFoundException^) { Texture1D = gcnew Direct3D10::Texture1D(IntPtr(pTexture1D)); }					
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
			try { Texture2D = (Direct3D10::Texture2D^)Interfaces[IntPtr(pTexture2D)]; }
			catch (KeyNotFoundException^) { Texture2D = gcnew Direct3D10::Texture2D(IntPtr(pTexture2D)); }					
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
			try { Texture3D = (Direct3D10::Texture3D^)Interfaces[IntPtr(pTexture3D)]; }
			catch (KeyNotFoundException^) { Texture3D = gcnew Direct3D10::Texture3D(IntPtr(pTexture3D)); }					
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
			try { VertexShader = (Direct3D10::VertexShader^)Interfaces[IntPtr(pVertexShader)]; }
			catch (KeyNotFoundException^) { VertexShader = gcnew Direct3D10::VertexShader(IntPtr(pVertexShader)); }					
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

	void GenerateMips(ShaderResourceView^ ShaderResourceView)
	{
		ID3D10ShaderResourceView* pShaderResourceView = ShaderResourceView == nullptr ? 0 : ShaderResourceView->pShaderResourceView;
		pDevice->GenerateMips(pShaderResourceView);
	}

	CreateDeviceFlag GetCreationFlags()
	{
		return (CreateDeviceFlag)pDevice->GetCreationFlags();
	}

	int GetDeviceRemovedReason()
	{
		return pDevice->GetDeviceRemovedReason();
	}

	RaiseFlag GetExceptionMode()
	{
		return (RaiseFlag)pDevice->GetExceptionMode();
	}

	void GetPredication([Out] Predicate^% Predicate, [Out] bool% PredicateValue)
	{
		pin_ptr<bool> PinnedPredicateValue = &PredicateValue;

		ID3D10Predicate* pPredicate = 0;
		pDevice->GetPredication(&pPredicate, (int*)PinnedPredicateValue);

		if (pPredicate)
		{
			try { Predicate = (Direct3D10::Predicate^)Interfaces[IntPtr(pPredicate)]; }
			catch (KeyNotFoundException^) { Predicate = gcnew Direct3D10::Predicate(IntPtr(pPredicate)); }					
		}
		else Predicate = nullptr;
	}

	int GetPrivateData(Guid Guid, [Out] unsigned int% DataSize)
	{
		pin_ptr<unsigned int> PinnedDataSize = &DataSize;

		return pDevice->GetPrivateData(IID_Converter::ToNative(Guid), PinnedDataSize, 0);
	}

	int GetPrivateData(Guid Guid, unsigned int DataSize, UnmanagedMemory^ Data)
	{
		pin_ptr<unsigned int> PinnedDataSize = &DataSize;
		void* pData = Data == nullptr ? 0 : Data->pMemory;
		
		return pDevice->GetPrivateData(IID_Converter::ToNative(Guid), PinnedDataSize, pData);
	}

	int GetPrivateData(Guid Guid, [Out] Unknown^% Unknown)
	{
		void* pData = 0;
		
		unsigned int DataSize = Marshal::SizeOf(IntPtr::typeid);
		int Result = pDevice->GetPrivateData(IID_Converter::ToNative(Guid), &DataSize, &pData);

		if (pData)
		{
			try	{ Unknown = (MDX::Unknown^)Interfaces[IntPtr(pData)];	}
			catch (KeyNotFoundException^) { Unknown = nullptr; }
		}

		return Result;
	}

	void GS_GetConstantBuffers(unsigned int StartSlot, unsigned int NumberOfBuffers, array<Buffer^>^ ConstantBuffers)
	{
		unsigned int Length = ConstantBuffers == nullptr ? 0 : Math::Min(NumberOfBuffers, (unsigned int)ConstantBuffers->Length);
		ID3D10Buffer** pConstantBuffers = ConstantBuffers != nullptr && ConstantBuffers->Length > 0 ? new ID3D10Buffer*[Length] : 0;

		try
		{
			pDevice->GSGetConstantBuffers(StartSlot, NumberOfBuffers, pConstantBuffers);

			if (pConstantBuffers)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pConstantBuffers[No])
					{
						try { ConstantBuffers[No] = (Buffer^)Interfaces[IntPtr(pConstantBuffers[No])]; }
						catch (KeyNotFoundException^) { ConstantBuffers[No] = gcnew Buffer(IntPtr(pConstantBuffers[No])); }
					}
					else ConstantBuffers[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pConstantBuffers) delete[] pConstantBuffers;
		}
	}

	void GS_GetSamplers(unsigned int StartSlot, unsigned int NumberOfSamplers, array<SamplerState^>^ Samplers)
	{
		unsigned int Length = Samplers == nullptr ? 0 : Math::Min(NumberOfSamplers, (unsigned int)Samplers->Length);
		ID3D10SamplerState** pSamplers = Samplers != nullptr && Samplers->Length > 0 ? new ID3D10SamplerState*[Length] : 0;
		try
		{
			pDevice->GSGetSamplers(StartSlot, NumberOfSamplers, pSamplers);

			if (pSamplers)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pSamplers[No])
					{
						try { Samplers[No] = (SamplerState^)Interfaces[IntPtr(pSamplers[No])]; }
						catch (KeyNotFoundException^) { Samplers[No] = gcnew SamplerState(IntPtr(pSamplers[No])); }
					}
					else Samplers[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pSamplers) delete[] pSamplers;
		}
	}

	void GS_GetShader([Out] GeometryShader^% GeometryShader)
	{
		ID3D10GeometryShader* pGeometryShader = 0;
		pDevice->GSGetShader(&pGeometryShader);

		if (pGeometryShader)
		{
			try { GeometryShader = (Direct3D10::GeometryShader^)Interfaces[IntPtr(pGeometryShader)]; }
			catch (KeyNotFoundException^) { GeometryShader = gcnew Direct3D10::GeometryShader(IntPtr(pGeometryShader)); }					
		}
		else GeometryShader = nullptr;
	}

	void GS_GetShaderResources(unsigned int StartSlot, unsigned int NumberOfViews, array<ShaderResourceView^>^ ShaderResourceViews)
	{
		unsigned int Length = ShaderResourceViews == nullptr ? 0 : Math::Min(NumberOfViews, (unsigned int)ShaderResourceViews->Length);
		ID3D10ShaderResourceView** pShaderResourceViews = ShaderResourceViews != nullptr && ShaderResourceViews->Length > 0 ? new ID3D10ShaderResourceView*[Length] : 0;
		try
		{
			pDevice->GSGetShaderResources(StartSlot, NumberOfViews, pShaderResourceViews);

			if (pShaderResourceViews)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pShaderResourceViews[No])
					{
						try { ShaderResourceViews[No] = (ShaderResourceView^)Interfaces[IntPtr(pShaderResourceViews[No])]; }
						catch (KeyNotFoundException^) { ShaderResourceViews[No] = gcnew ShaderResourceView(IntPtr(pShaderResourceViews[No])); }
					}
					else ShaderResourceViews[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pShaderResourceViews) delete[] pShaderResourceViews;
		}
	}

	void GS_SetConstantBuffers(unsigned int StartSlot, unsigned int NumberOfBuffers, array<Buffer^>^ ConstantBuffers)
	{
		ID3D10Buffer** pConstantBuffers = 0;
		try
		{
			if (ConstantBuffers != nullptr && ConstantBuffers->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfBuffers, (unsigned int)ConstantBuffers->Length);
				pConstantBuffers = new ID3D10Buffer*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					pConstantBuffers[No] = ConstantBuffers[No] == nullptr ? 0 : ConstantBuffers[No]->pBuffer;
				}
			}

			pDevice->GSSetConstantBuffers(StartSlot, NumberOfBuffers, pConstantBuffers);
		}
		finally
		{
			if (pConstantBuffers) delete[] pConstantBuffers;
		}
	}

	void GS_SetSamplers(unsigned int StartSlot, unsigned int NumberOfSamplers, array<SamplerState^>^ Samplers)
	{
		ID3D10SamplerState** pSamplers = 0;
		try
		{
			if (Samplers != nullptr && Samplers->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfSamplers, (unsigned int)Samplers->Length);
				pSamplers = new ID3D10SamplerState*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					pSamplers[No] = Samplers[No] == nullptr ? 0 : Samplers[No]->pSamplerState;
				}
			}

			pDevice->GSSetSamplers(StartSlot, NumberOfSamplers, pSamplers);
		}
		finally
		{
			if (pSamplers) delete[] pSamplers;
		}
	}

	void GS_SetShader(GeometryShader^ Shader)
	{
		ID3D10GeometryShader* pShader = Shader == nullptr ? 0 : Shader->pGeometryShader;

		pDevice->GSSetShader(pShader);
	}

	void GS_SetShaderResources(unsigned int StartSlot, unsigned int NumberOfViews, array<ShaderResourceView^>^ ShaderResourceViews)
	{
		ID3D10ShaderResourceView** pShaderResourceViews = 0;
		try
		{
			if (ShaderResourceViews != nullptr && ShaderResourceViews->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfViews, (unsigned int)ShaderResourceViews->Length);
				pShaderResourceViews = new ID3D10ShaderResourceView*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					pShaderResourceViews[No] = ShaderResourceViews[No] == nullptr ? 0 : ShaderResourceViews[No]->pShaderResourceView;
				}
			}

			pDevice->GSSetShaderResources(StartSlot, NumberOfViews, pShaderResourceViews);
		}
		finally
		{
			if (pShaderResourceViews) delete[] pShaderResourceViews;
		}
	}

	void IA_GetIndexBuffer([Out] Buffer^% IndexBuffer, [Out] Format% Format, [Out] unsigned int% Offset)
	{
		pin_ptr<Direct3D10::Format> PinnedFormat = &Format;
		pin_ptr<unsigned int> PinnedOffset = &Offset;

		ID3D10Buffer* pIndexBuffer = 0;
		pDevice->IAGetIndexBuffer(&pIndexBuffer, (DXGI_FORMAT*)PinnedFormat, PinnedOffset);

		if (pIndexBuffer)
		{
			try { IndexBuffer = (Buffer^)Interfaces[IntPtr(pIndexBuffer)]; }
			catch (KeyNotFoundException^) { IndexBuffer = gcnew Buffer(IntPtr(pIndexBuffer)); }					
		}
		else IndexBuffer = nullptr;
	}

	void IA_GetInputLayout([Out] InputLayout^% InputLayout)
	{
		ID3D10InputLayout* pInputLayout = 0;
		pDevice->IAGetInputLayout(&pInputLayout);

		if (pInputLayout)
		{
			try { InputLayout = (Direct3D10::InputLayout^)Interfaces[IntPtr(pInputLayout)]; }
			catch (KeyNotFoundException^) { InputLayout = gcnew Direct3D10::InputLayout(IntPtr(pInputLayout)); }					
		}
		else InputLayout = nullptr;
	}

	void IA_GetPrimitiveTopology([Out] PrimitiveTopology% Topology)
	{
		pin_ptr<PrimitiveTopology> PinnedTopology = &Topology;

		pDevice->IAGetPrimitiveTopology((D3D10_PRIMITIVE_TOPOLOGY*)PinnedTopology);
	}

	void IA_GetVertexBuffers(unsigned int StartSlot, unsigned int NumberOfBuffers, array<Buffer^>^ VertexBuffers, array<unsigned int>^ Strides, array<unsigned int>^ Offsets)
	{
		pin_ptr<unsigned int> PinnedStrides = Strides != nullptr && Strides->Length > 0 ? &Strides[0] : nullptr;
		pin_ptr<unsigned int> PinnedOffsets = Offsets != nullptr && Offsets->Length > 0 ? &Offsets[0] : nullptr;

		unsigned int Length = VertexBuffers == nullptr ? 0 : Math::Min(NumberOfBuffers, (unsigned int)VertexBuffers->Length);
		ID3D10Buffer** pVertexBuffers = VertexBuffers != nullptr && VertexBuffers->Length > 0 ? new ID3D10Buffer*[Length] : 0;
		try
		{
			pDevice->IAGetVertexBuffers(StartSlot, NumberOfBuffers, pVertexBuffers, PinnedStrides, PinnedOffsets);

			if (pVertexBuffers)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pVertexBuffers[No])
					{
						try { VertexBuffers[No] = (Buffer^)Interfaces[IntPtr(pVertexBuffers[No])]; }
						catch (KeyNotFoundException^) { VertexBuffers[No] = gcnew Buffer(IntPtr(pVertexBuffers[No])); }
					}
					else VertexBuffers[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pVertexBuffers) delete[] pVertexBuffers;
		}
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
				unsigned int Length = Math::Min(NumberOfBuffers, (unsigned int)VertexBuffers->Length);
				pVertexBuffers = new ID3D10Buffer*[Length];
				for (unsigned int No = 0; No < Length; No++)
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
			try { BlendState = (Direct3D10::BlendState^)Interfaces[IntPtr(pBlendState)]; }
			catch (KeyNotFoundException^) { BlendState = gcnew Direct3D10::BlendState(IntPtr(pBlendState)); }					
		}
		else BlendState = nullptr;
	}

	void OM_GetDepthStencilState([Out] DepthStencilState^% DepthStencilState, [Out] unsigned int% StencilReference)
	{
		pin_ptr<unsigned int> PinnedStencilReference = &StencilReference;

		ID3D10DepthStencilState* pDepthStencilState = 0;
		pDevice->OMGetDepthStencilState(&pDepthStencilState, PinnedStencilReference);

		if (pDepthStencilState)
		{
			try { DepthStencilState = (Direct3D10::DepthStencilState^)Interfaces[IntPtr(pDepthStencilState)]; }
			catch (KeyNotFoundException^) { DepthStencilState = gcnew Direct3D10::DepthStencilState(IntPtr(pDepthStencilState)); }					
		}
		else DepthStencilState = nullptr;
	}

	void OM_GetRenderTargets(unsigned int NumberOfViews, array<RenderTargetView^>^ RenderTargetViews, [Out] DepthStencilView^% DepthStencilView)
	{
		unsigned int Length = RenderTargetViews == nullptr ? 0 : Math::Min(NumberOfViews, (unsigned int)RenderTargetViews->Length);
		ID3D10RenderTargetView** pRenderTargetViews = RenderTargetViews != nullptr && RenderTargetViews->Length > 0 ? new ID3D10RenderTargetView*[Length] : 0;
		try
		{
			ID3D10DepthStencilView* pDepthStencilView = 0;
			pDevice->OMGetRenderTargets(NumberOfViews, pRenderTargetViews, &pDepthStencilView);

			if (pRenderTargetViews)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pRenderTargetViews[No])
					{
						try { RenderTargetViews[No] = (RenderTargetView^)Interfaces[IntPtr(pRenderTargetViews[No])]; }
						catch (KeyNotFoundException^) { RenderTargetViews[No] = gcnew RenderTargetView(IntPtr(pRenderTargetViews[No])); }
					}
					else RenderTargetViews[No] = nullptr;
				}

				if (pDepthStencilView)
				{
					try { DepthStencilView = (Direct3D10::DepthStencilView^)Interfaces[IntPtr(pDepthStencilView)]; }
					catch (KeyNotFoundException^) { DepthStencilView = gcnew Direct3D10::DepthStencilView(IntPtr(pDepthStencilView)); }					
				}
				else DepthStencilView = nullptr;
			}
		}
		finally
		{
			if (pRenderTargetViews) delete[] pRenderTargetViews;
		}
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
				unsigned int Length = Math::Min(NumberOfViews, (unsigned int)RenderTargetViews->Length);
				pRenderTargetViews = new ID3D10RenderTargetView*[Length];
				for (unsigned int No = 0; No < Length; No++)
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

	int OpenSharedResource(IntPtr ResourceHandle, Type^ ReturnedType, [Out] System::Object^% Resource)
	{
		Guid ReturnedInterface;
		try { ReturnedInterface = (Guid)ReturnedType->GetField("IID", BindingFlags::DeclaredOnly | BindingFlags::NonPublic | BindingFlags::Static)->GetValue(nullptr); }
		catch (...) { ReturnedInterface = Guid::Empty; }

		void* pResource = 0;
		int Result = pDevice->OpenSharedResource(ResourceHandle.ToPointer(), IID_Converter::ToNative(ReturnedInterface), &pResource);

		if (pResource) 
		{				
			try { Resource = Interfaces[IntPtr(pResource)]; }
			catch (KeyNotFoundException^) { Resource = Activator::CreateInstance(ReturnedType, BindingFlags::NonPublic | BindingFlags::Instance, nullptr, gcnew array<System::Object^>(1) { IntPtr(pResource) }, CultureInfo::CurrentCulture); }
		}
		else Resource = nullptr;

		return Result;
	}

	void PS_GetConstantBuffers(unsigned int StartSlot, unsigned int NumberOfBuffers, array<Buffer^>^ ConstantBuffers)
	{
		unsigned int Length = ConstantBuffers == nullptr ? 0 : Math::Min(NumberOfBuffers, (unsigned int)ConstantBuffers->Length);
		ID3D10Buffer** pConstantBuffers = ConstantBuffers != nullptr && ConstantBuffers->Length > 0 ? new ID3D10Buffer*[Length] : 0;

		try
		{
			pDevice->PSGetConstantBuffers(StartSlot, NumberOfBuffers, pConstantBuffers);

			if (pConstantBuffers)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pConstantBuffers[No])
					{
						try { ConstantBuffers[No] = (Buffer^)Interfaces[IntPtr(pConstantBuffers[No])]; }
						catch (KeyNotFoundException^) { ConstantBuffers[No] = gcnew Buffer(IntPtr(pConstantBuffers[No])); }
					}
					else ConstantBuffers[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pConstantBuffers) delete[] pConstantBuffers;
		}
	}

	void PS_GetSamplers(unsigned int StartSlot, unsigned int NumberOfSamplers, array<SamplerState^>^ Samplers)
	{
		unsigned int Length = Samplers == nullptr ? 0 : Math::Min(NumberOfSamplers, (unsigned int)Samplers->Length);
		ID3D10SamplerState** pSamplers = Samplers != nullptr && Samplers->Length > 0 ? new ID3D10SamplerState*[Length] : 0;
		try
		{
			pDevice->PSGetSamplers(StartSlot, NumberOfSamplers, pSamplers);

			if (pSamplers)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pSamplers[No])
					{
						try { Samplers[No] = (SamplerState^)Interfaces[IntPtr(pSamplers[No])]; }
						catch (KeyNotFoundException^) { Samplers[No] = gcnew SamplerState(IntPtr(pSamplers[No])); }
					}
					else Samplers[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pSamplers) delete[] pSamplers;
		}
	}

	void PS_GetShader([Out] PixelShader^% PixelShader)
	{
		ID3D10PixelShader* pPixelShader = 0;
		pDevice->PSGetShader(&pPixelShader);

		if (pPixelShader)
		{
			try { PixelShader = (Direct3D10::PixelShader^)Interfaces[IntPtr(pPixelShader)]; }
			catch (KeyNotFoundException^) { PixelShader = gcnew Direct3D10::PixelShader(IntPtr(pPixelShader)); }					
		}
		else PixelShader = nullptr;
	}

	void PS_GetShaderResources(unsigned int StartSlot, unsigned int NumberOfViews, array<ShaderResourceView^>^ ShaderResourceViews)
	{
		unsigned int Length = ShaderResourceViews == nullptr ? 0 : Math::Min(NumberOfViews, (unsigned int)ShaderResourceViews->Length);
		ID3D10ShaderResourceView** pShaderResourceViews = ShaderResourceViews != nullptr && ShaderResourceViews->Length > 0 ? new ID3D10ShaderResourceView*[Length] : 0;
		try
		{
			pDevice->PSGetShaderResources(StartSlot, NumberOfViews, pShaderResourceViews);

			if (pShaderResourceViews)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pShaderResourceViews[No])
					{
						try { ShaderResourceViews[No] = (ShaderResourceView^)Interfaces[IntPtr(pShaderResourceViews[No])]; }
						catch (KeyNotFoundException^) { ShaderResourceViews[No] = gcnew ShaderResourceView(IntPtr(pShaderResourceViews[No])); }
					}
					else ShaderResourceViews[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pShaderResourceViews) delete[] pShaderResourceViews;
		}
	}

	void PS_SetConstantBuffers(unsigned int StartSlot, unsigned int NumberOfBuffers, array<Buffer^>^ ConstantBuffers)
	{
		ID3D10Buffer** pConstantBuffers = 0;
		try
		{
			if (ConstantBuffers != nullptr && ConstantBuffers->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfBuffers, (unsigned int)ConstantBuffers->Length);
				pConstantBuffers = new ID3D10Buffer*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					pConstantBuffers[No] = ConstantBuffers[No] == nullptr ? 0 : ConstantBuffers[No]->pBuffer;
				}
			}

			pDevice->PSSetConstantBuffers(StartSlot, NumberOfBuffers, pConstantBuffers);
		}
		finally
		{
			if (pConstantBuffers) delete[] pConstantBuffers;
		}
	}

	void PS_SetSamplers(unsigned int StartSlot, unsigned int NumberOfSamplers, array<SamplerState^>^ Samplers)
	{
		ID3D10SamplerState** pSamplers = 0;
		try
		{
			if (Samplers != nullptr && Samplers->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfSamplers, (unsigned int)Samplers->Length);
				pSamplers = new ID3D10SamplerState*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					pSamplers[No] = Samplers[No] == nullptr ? 0 : Samplers[No]->pSamplerState;
				}
			}

			pDevice->PSSetSamplers(StartSlot, NumberOfSamplers, pSamplers);
		}
		finally
		{
			if (pSamplers) delete[] pSamplers;
		}
	}

	void PS_SetShader(PixelShader^ Shader)
	{
		ID3D10PixelShader* pShader = Shader == nullptr ? 0 : Shader->pPixelShader;

		pDevice->PSSetShader(pShader);
	}

	void PS_SetShaderResources(unsigned int StartSlot, unsigned int NumberOfViews, array<ShaderResourceView^>^ ShaderResourceViews)
	{
		ID3D10ShaderResourceView** pShaderResourceViews = 0;
		try
		{
			if (ShaderResourceViews != nullptr && ShaderResourceViews->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfViews, (unsigned int)ShaderResourceViews->Length);
				pShaderResourceViews = new ID3D10ShaderResourceView*[Length];
				for (unsigned int No = 0; No < Length; No++)
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

	void ResolveSubresource(Resource^ DestinationResource, unsigned int DestinationSubresource, Resource^ SourceResource, unsigned int SourceSubresource, Format Format)
	{	   
		ID3D10Resource* pDestinationResource = DestinationResource == nullptr ? 0 : DestinationResource->pResource;
		ID3D10Resource* pSourceResource = SourceResource == nullptr ? 0 : SourceResource->pResource;

		pDevice->ResolveSubresource(pDestinationResource, DestinationSubresource, pSourceResource, SourceSubresource, (DXGI_FORMAT)Format);
	}

	void RS_GetScissorRectangles(unsigned int% NumberOfRectangles, array<System::Drawing::Rectangle>^ Rectangles)
	{
		pin_ptr<unsigned int> PinnedNumberOfRectangles = &NumberOfRectangles;

		unsigned int Length = Rectangles == nullptr ? 0 : Math::Min(NumberOfRectangles, (unsigned int)Rectangles->Length);
		RECT* pRectangles = Rectangles != nullptr && Rectangles->Length > 0 ? new RECT[Length] : 0;

		try
		{
			pDevice->RSGetScissorRects(PinnedNumberOfRectangles, pRectangles);

			if (pRectangles)
			{
				for (unsigned int No = 0; No < Length; No++)
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
			try { RasterizerState = (Direct3D10::RasterizerState^)Interfaces[IntPtr(pRasterizerState)]; }
			catch (KeyNotFoundException^) { RasterizerState = gcnew Direct3D10::RasterizerState(IntPtr(pRasterizerState)); }					
		}
		else RasterizerState = nullptr;
	}

    void RS_GetViewports(unsigned int% NumberOfViewports, array<Viewport>^ Viewports)
    {
        pin_ptr<unsigned int> PinnedNumberOfViewports = &NumberOfViewports;
        pin_ptr<Viewport> PinnedViewports = Viewports != nullptr && Viewports->Length > 0 ? &Viewports[0] : nullptr;

        pDevice->RSGetViewports(PinnedNumberOfViewports, (D3D10_VIEWPORT*)PinnedViewports);
    }

	void RS_SetScissorRectangles(unsigned int NumberOfRectangles, array<System::Drawing::Rectangle>^ Rectangles)
	{
		RECT* pRectangles = 0;
		if (Rectangles != nullptr && Rectangles->Length > 0)
		{
			unsigned int Length = Rectangles == nullptr ? 0 : Math::Min(NumberOfRectangles, (unsigned int)Rectangles->Length);
			pRectangles = new RECT[Length];
			for (unsigned int No = 0; No < Length; No++)
			{
				RECT* pRectangle = &pRectangles[No];
				pRectangle->left = Rectangles[No].X;
				pRectangle->top = Rectangles[No].Y;
				pRectangle->right = Rectangles[No].Right;
				pRectangle->bottom = Rectangles[No].Bottom;
			}
		}

		pDevice->RSSetScissorRects(NumberOfRectangles, pRectangles);
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

	void SO_GetTargets(unsigned int NumberOfBuffers, array<Buffer^>^ StreamOutputTargets, array<unsigned int>^ Offsets)
	{
		unsigned int Length = StreamOutputTargets == nullptr ? 0 : Math::Min(NumberOfBuffers, (unsigned int)StreamOutputTargets->Length);
		ID3D10Buffer** pStreamOutputTargets = StreamOutputTargets != nullptr && StreamOutputTargets->Length > 0 ? new ID3D10Buffer*[Length] : 0;

		pin_ptr <unsigned int> PinnedOffsets = Offsets != nullptr && Offsets->Length > 0 ? &Offsets[0] : nullptr;

		try
		{
			pDevice->SOGetTargets(NumberOfBuffers, pStreamOutputTargets, PinnedOffsets);

			if (pStreamOutputTargets)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pStreamOutputTargets[No])
					{
						try { StreamOutputTargets[No] = (Buffer^)Interfaces[IntPtr(pStreamOutputTargets[No])]; }
						catch (KeyNotFoundException^) { StreamOutputTargets[No] = gcnew Buffer(IntPtr(pStreamOutputTargets[No])); }
					}
					else StreamOutputTargets[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pStreamOutputTargets) delete[] pStreamOutputTargets;
		}
	}

	void SO_SetTargets(unsigned int NumberOfBuffers, array<Buffer^>^ StreamOutputTargets, array<unsigned int>^ Offsets)
	{
		pin_ptr <unsigned int> PinnedOffsets = Offsets != nullptr && Offsets->Length > 0 ? &Offsets[0] : nullptr;

		ID3D10Buffer** pStreamOutputTargets = 0;
		try
		{
			if (StreamOutputTargets != nullptr && StreamOutputTargets->Length > 0)
			{
				unsigned int Length = StreamOutputTargets == nullptr ? 0 : Math::Min(NumberOfBuffers, (unsigned int)StreamOutputTargets->Length);
				pStreamOutputTargets = new ID3D10Buffer*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pStreamOutputTargets[No])
					{
						try { StreamOutputTargets[No] = (Buffer^)Interfaces[IntPtr(pStreamOutputTargets[No])]; }
						catch (KeyNotFoundException^) { StreamOutputTargets[No] = gcnew Buffer(IntPtr(pStreamOutputTargets[No])); }
					}
					else StreamOutputTargets[No] = nullptr;
				}
			}

			pDevice->SOSetTargets(NumberOfBuffers, pStreamOutputTargets, PinnedOffsets);
		}
		finally
		{
			if (pStreamOutputTargets) delete[] pStreamOutputTargets;
		}
	}

	int SetExceptionMode(RaiseFlag RaiseFlags)
	{
		return pDevice->SetExceptionMode((unsigned int)RaiseFlags);
	}

	void SetPredication(Predicate^ Predicate, bool PredicateValue)
	{
		ID3D10Predicate* pPredicate = Predicate == nullptr ? 0 : Predicate->pPredicate;

		pDevice->SetPredication(pPredicate, PredicateValue);
	}

	int SetPrivateData(Guid Guid, unsigned int DataSize, UnmanagedMemory^ Data)
	{
		void* pData = Data == nullptr ? 0 : Data->pMemory;

		int Result = pDevice->SetPrivateData(IID_Converter::ToNative(Guid), DataSize, pData);

		return Result;
	}

	int SetPrivateDataInterface(Guid Guid, Unknown^ Data)
	{
		IUnknown* pData = Data == nullptr ? 0 : Data->pUnknown;

		int Result = pDevice->SetPrivateDataInterface(IID_Converter::ToNative(Guid), pData);

		return Result;
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

	void VS_GetConstantBuffers(unsigned int StartSlot, unsigned int NumberOfBuffers, array<Buffer^>^ ConstantBuffers)
	{
		unsigned int Length = ConstantBuffers == nullptr ? 0 : Math::Min(NumberOfBuffers, (unsigned int)ConstantBuffers->Length);
		ID3D10Buffer** pConstantBuffers = ConstantBuffers != nullptr && ConstantBuffers->Length > 0 ? new ID3D10Buffer*[Length] : 0;

		try
		{
			pDevice->VSGetConstantBuffers(StartSlot, NumberOfBuffers, pConstantBuffers);

			if (pConstantBuffers)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pConstantBuffers[No])
					{
						try { ConstantBuffers[No] = (Buffer^)Interfaces[IntPtr(pConstantBuffers[No])]; }
						catch (KeyNotFoundException^) { ConstantBuffers[No] = gcnew Buffer(IntPtr(pConstantBuffers[No])); }
					}
					else ConstantBuffers[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pConstantBuffers) delete[] pConstantBuffers;
		}
	}

	void VS_GetSamplers(unsigned int StartSlot, unsigned int NumberOfSamplers, array<SamplerState^>^ Samplers)
	{
		unsigned int Length = Samplers == nullptr ? 0 : Math::Min(NumberOfSamplers, (unsigned int)Samplers->Length);
		ID3D10SamplerState** pSamplers = Samplers != nullptr && Samplers->Length > 0 ? new ID3D10SamplerState*[Length] : 0;
		try
		{
			pDevice->VSGetSamplers(StartSlot, NumberOfSamplers, pSamplers);

			if (pSamplers)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pSamplers[No])
					{
						try { Samplers[No] = (SamplerState^)Interfaces[IntPtr(pSamplers[No])]; }
						catch (KeyNotFoundException^) { Samplers[No] = gcnew SamplerState(IntPtr(pSamplers[No])); }
					}
					else Samplers[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pSamplers) delete[] pSamplers;
		}
	}

	void VS_GetShader([Out] VertexShader^% VertexShader)
	{
		ID3D10VertexShader* pVertexShader = 0;
		pDevice->VSGetShader(&pVertexShader);

		if (pVertexShader)
		{
			try { VertexShader = (Direct3D10::VertexShader^)Interfaces[IntPtr(pVertexShader)]; }
			catch (KeyNotFoundException^) { VertexShader = gcnew Direct3D10::VertexShader(IntPtr(pVertexShader)); }					
		}
		else VertexShader = nullptr;
	}

	void VS_GetShaderResources(unsigned int StartSlot, unsigned int NumberOfViews, array<ShaderResourceView^>^ ShaderResourceViews)
	{
		unsigned int Length = ShaderResourceViews == nullptr ? 0 : Math::Min(NumberOfViews, (unsigned int)ShaderResourceViews->Length);
		ID3D10ShaderResourceView** pShaderResourceViews = ShaderResourceViews != nullptr && ShaderResourceViews->Length > 0 ? new ID3D10ShaderResourceView*[Length] : 0;
		try
		{
			pDevice->VSGetShaderResources(StartSlot, NumberOfViews, pShaderResourceViews);

			if (pShaderResourceViews)
			{
				for (unsigned int No = 0; No < Length; No++)
				{
					if (pShaderResourceViews[No])
					{
						try { ShaderResourceViews[No] = (ShaderResourceView^)Interfaces[IntPtr(pShaderResourceViews[No])]; }
						catch (KeyNotFoundException^) { ShaderResourceViews[No] = gcnew ShaderResourceView(IntPtr(pShaderResourceViews[No])); }
					}
					else ShaderResourceViews[No] = nullptr;
				}
			}
		}
		finally
		{
			if (pShaderResourceViews) delete[] pShaderResourceViews;
		}
	}

	void VS_SetConstantBuffers(unsigned int StartSlot, unsigned int NumberOfBuffers, array<Buffer^>^ ConstantBuffers)
	{
		ID3D10Buffer** pConstantBuffers = 0;
		try
		{
			if (ConstantBuffers != nullptr && ConstantBuffers->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfBuffers, (unsigned int)ConstantBuffers->Length);
				pConstantBuffers = new ID3D10Buffer*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					pConstantBuffers[No] = ConstantBuffers[No] == nullptr ? 0 : ConstantBuffers[No]->pBuffer;
				}
			}

			pDevice->VSSetConstantBuffers(StartSlot, NumberOfBuffers, pConstantBuffers);
		}
		finally
		{
			if (pConstantBuffers) delete[] pConstantBuffers;
		}
	}

	void VS_SetSamplers(unsigned int StartSlot, unsigned int NumberOfSamplers, array<SamplerState^>^ Samplers)
	{
		ID3D10SamplerState** pSamplers = 0;
		try
		{
			if (Samplers != nullptr && Samplers->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfSamplers, (unsigned int)Samplers->Length);
				pSamplers = new ID3D10SamplerState*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					pSamplers[No] = Samplers[No] == nullptr ? 0 : Samplers[No]->pSamplerState;
				}
			}

			pDevice->VSSetSamplers(StartSlot, NumberOfSamplers, pSamplers);
		}
		finally
		{
			if (pSamplers) delete[] pSamplers;
		}
	}

	void VS_SetShader(VertexShader^ Shader)
	{
		ID3D10VertexShader* pShader = Shader == nullptr ? 0 : Shader->pVertexShader;

		pDevice->VSSetShader(pShader);
	}

	void VS_SetShaderResources(unsigned int StartSlot, unsigned int NumberOfViews, array<ShaderResourceView^>^ ShaderResourceViews)
	{
		ID3D10ShaderResourceView** pShaderResourceViews = 0;
		try
		{
			if (ShaderResourceViews != nullptr && ShaderResourceViews->Length > 0)
			{
				unsigned int Length = Math::Min(NumberOfViews, (unsigned int)ShaderResourceViews->Length);
				pShaderResourceViews = new ID3D10ShaderResourceView*[Length];
				for (unsigned int No = 0; No < Length; No++)
				{
					pShaderResourceViews[No] = ShaderResourceViews[No] == nullptr ? 0 : ShaderResourceViews[No]->pShaderResourceView;
				}
			}

			pDevice->VSSetShaderResources(StartSlot, NumberOfViews, pShaderResourceViews);
		}
		finally
		{
			if (pShaderResourceViews) delete[] pShaderResourceViews;
		}
	}
};