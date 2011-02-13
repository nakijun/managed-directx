public ref class Device : Unknown
{
internal:
	ID3D10Device* pDevice;

	static Guid IID = IID_Converter::ToManaged(IID_ID3D10Device);

	Device(IntPtr Device) : Unknown(Device)
	{	
		pDevice = (ID3D10Device*)Device.ToPointer();
	}

public:
	void ClearRenderTargetView(RenderTargetView^ RenderTargetView, Float4% ColorRGBA)
	{
		ID3D10RenderTargetView* pRenderTargetView = RenderTargetView == nullptr ? 0 : RenderTargetView->pRenderTargetView;
		pin_ptr<Float4> PinnedColorRGBA = &ColorRGBA;

		pDevice->ClearRenderTargetView(pRenderTargetView, (float*)PinnedColorRGBA);
	}

	void ClearDepthStencilView(DepthStencilView^ DepthStencilView, ClearFlag ClearFlags, float Depth, Byte Stencil)
	{
		ID3D10DepthStencilView* pDepthStencilView = DepthStencilView == nullptr ? 0 : DepthStencilView->pDepthStencilView;

		pDevice->ClearDepthStencilView(pDepthStencilView, (unsigned int)ClearFlags, Depth, Stencil);
	}

	void ClearState()
	{
		pDevice->ClearState();
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

	int CreateRenderTargetView(Xtro::MDX::Direct3D10::Resource^ Resource)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;

		return pDevice->CreateRenderTargetView(pResource, 0, 0);
	}

	int CreateRenderTargetView(Xtro::MDX::Direct3D10::Resource^ Resource, RenderTargetViewDescription% Description)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<RenderTargetViewDescription> PinnedDescription = &Description;

		return pDevice->CreateRenderTargetView(pResource, (D3D10_RENDER_TARGET_VIEW_DESC*)PinnedDescription, 0);
	}

	int CreateDepthStencilView(Xtro::MDX::Direct3D10::Resource^ Resource, [Out] DepthStencilView^% DepthStencilView)
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

	int CreateDepthStencilView(Xtro::MDX::Direct3D10::Resource^ Resource, DepthStencilViewDescription% Description, [Out] DepthStencilView^% DepthStencilView)
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

	int CreateDepthStencilView(Xtro::MDX::Direct3D10::Resource^ Resource)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;

		return pDevice->CreateDepthStencilView(pResource, 0, 0);
	}

	int CreateDepthStencilView(Xtro::MDX::Direct3D10::Resource^ Resource, DepthStencilViewDescription% Description)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<DepthStencilViewDescription> PinnedDescription = &Description;

		return pDevice->CreateDepthStencilView(pResource, (D3D10_DEPTH_STENCIL_VIEW_DESC*)PinnedDescription, 0);
	}

	void OM_SetRenderTargets(unsigned int NumberOfViews, array<RenderTargetView^>^ RenderTargetViews, DepthStencilView^ DepthStencilView)
	{
		ID3D10DepthStencilView* pDepthStencilView = DepthStencilView == nullptr ? 0 : DepthStencilView->pDepthStencilView;

		ID3D10RenderTargetView** ppRenderTargetViews = 0;
		try
		{
			if (RenderTargetViews != nullptr && RenderTargetViews->Length > 0)
			{
				ppRenderTargetViews = new ID3D10RenderTargetView*[RenderTargetViews->Length];
				for (int RenderTargetViewNo = 0; RenderTargetViewNo < RenderTargetViews->Length; RenderTargetViewNo++) ppRenderTargetViews[RenderTargetViewNo] = RenderTargetViews[RenderTargetViewNo] == nullptr ? 0 : RenderTargetViews[RenderTargetViewNo]->pRenderTargetView;
			}

			pDevice->OMSetRenderTargets(NumberOfViews, ppRenderTargetViews, pDepthStencilView);
		}
		finally
		{
			if (ppRenderTargetViews) delete[] ppRenderTargetViews;
		}
	}

	void RS_GetViewports([Out] unsigned int% NumberOfViewports)
	{
		pin_ptr<unsigned int> PinnedNumberOfViewports = &NumberOfViewports;

		pDevice->RSGetViewports(PinnedNumberOfViewports, 0);
	}

	void RS_GetViewports(unsigned int NumberOfViewports, [Out] array<Viewport>^% Viewports)
	{
		pin_ptr<unsigned int> PinnedNumberOfViewports = &NumberOfViewports;
		pin_ptr<Viewport> PinnedViewports = Viewports != nullptr && Viewports->Length > 0 ? &Viewports[0] : nullptr;

		pDevice->RSGetViewports(PinnedNumberOfViewports, (D3D10_VIEWPORT*)PinnedViewports);
	}

	void RS_SetViewports(unsigned int NumberOfViewports, array<Viewport>^ Viewports)
	{
		pin_ptr<Viewport> PinnedViewports = Viewports != nullptr && Viewports->Length > 0 ? &Viewports[0] : nullptr;

		pDevice->RSSetViewports(NumberOfViewports, (D3D10_VIEWPORT*)PinnedViewports);
	}

	void IA_SetInputLayout(InputLayout^ InputLayout)
	{
		ID3D10InputLayout* pInputLayout = InputLayout == nullptr ? 0 : InputLayout->pInputLayout;

		pDevice->IASetInputLayout(pInputLayout);
	}

	void RS_SetState(RasterizerState^ RasterizerState)
	{
		ID3D10RasterizerState* pRasterizerState = RasterizerState == nullptr ? 0 : RasterizerState->pRasterizerState;

		pDevice->RSSetState(pRasterizerState);
	}

	void PS_SetShaderResources(unsigned int StartSlot, unsigned int NumberOfViews, array<ShaderResourceView^>^ ShaderResourceViews)
	{
		ID3D10ShaderResourceView** ppShaderResourceViews = 0;
		try
		{
			if (ShaderResourceViews != nullptr && ShaderResourceViews->Length > 0)
			{
				ppShaderResourceViews = new ID3D10ShaderResourceView*[ShaderResourceViews->Length];
				for (int ShaderResourceViewNo = 0; ShaderResourceViewNo < ShaderResourceViews->Length; ShaderResourceViewNo++) ppShaderResourceViews[ShaderResourceViewNo] = ShaderResourceViews[ShaderResourceViewNo] == nullptr ? 0 : ShaderResourceViews[ShaderResourceViewNo]->pShaderResourceView;
			}

			pDevice->PSSetShaderResources(StartSlot, NumberOfViews, ppShaderResourceViews);
		}
		finally
		{
			if (ppShaderResourceViews) delete[] ppShaderResourceViews;
		}
	}

	int CreateInputLayout(array<InputElementDescription>^ InputElementDescriptions, unsigned int NumberOfElements, UnmanagedMemory^ ShaderBytecodeWithInputSignature, SIZE_T BytecodeLength, [Out] InputLayout^% InputLayout)
	{
		void* pShaderBytecodeWithInputSignature = ShaderBytecodeWithInputSignature == nullptr ? 0 : ShaderBytecodeWithInputSignature->pMemory;

		int Result = 0;
		ID3D10InputLayout* pInputLayout = 0;

		int ElementCount = 0;
		D3D10_INPUT_ELEMENT_DESC* pInputElementDescriptions = 0;
		try
		{
			if (InputElementDescriptions != nullptr && InputElementDescriptions->Length > 0)
			{
				ElementCount = InputElementDescriptions->Length;
				pInputElementDescriptions = new D3D10_INPUT_ELEMENT_DESC[ElementCount];
				for (int ElementNo = 0; ElementNo < ElementCount; ElementNo++) InputElementDescriptions[ElementNo].Marshal(&pInputElementDescriptions[ElementNo]);
			}

			Result = pDevice->CreateInputLayout(pInputElementDescriptions, NumberOfElements, pShaderBytecodeWithInputSignature, BytecodeLength, &pInputLayout);
		}
		finally
		{
			if (InputElementDescriptions != nullptr)
			{
				for (int ElementNo = 0; ElementNo < ElementCount; ElementNo++) InputElementDescriptions[ElementNo].Unmarshal();
			}
			if (pInputElementDescriptions) delete[] pInputElementDescriptions;
		}

		if (pInputLayout)
		{
			try { InputLayout = (Xtro::MDX::Direct3D10::InputLayout^)Interfaces[IntPtr(pInputLayout)]; }
			catch (KeyNotFoundException^) { InputLayout = gcnew Xtro::MDX::Direct3D10::InputLayout(IntPtr(pInputLayout)); }					
		}
		else InputLayout = nullptr;

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
		D3D10_SUBRESOURCE_DATA* pInitialData = &NativeInitialData;

		ID3D10Buffer* pBuffer = 0;
		int Result = pDevice->CreateBuffer((D3D10_BUFFER_DESC*)PinnedDescription, pInitialData, &pBuffer);

		if (pBuffer)
		{
			try { Buffer = (Xtro::MDX::Direct3D10::Buffer^)Interfaces[IntPtr(pBuffer)]; }
			catch (KeyNotFoundException^) { Buffer = gcnew Xtro::MDX::Direct3D10::Buffer(IntPtr(pBuffer)); }					
		}
		else Buffer = nullptr;

		return Result;
	}

	int CreateTexture2D(Texture2D_Description% Description, [Out] Texture2D^% Texture2D)
	{
		pin_ptr<Texture2D_Description> PinnedDescription = &Description;

		ID3D10Texture2D* pTexture2D = 0;
		int Result = pDevice->CreateTexture2D((D3D10_TEXTURE2D_DESC*)PinnedDescription, 0, &pTexture2D);

		if (pTexture2D)
		{
			try { Texture2D = (Xtro::MDX::Direct3D10::Texture2D^)Interfaces[IntPtr(pTexture2D)]; }
			catch (KeyNotFoundException^) { Texture2D = gcnew Xtro::MDX::Direct3D10::Texture2D(IntPtr(pTexture2D)); }					
		}
		else Texture2D = nullptr;

		return Result;
	}

	int CreateTexture2D(Texture2D_Description% Description, SubResourceData% InitialData, [Out] Texture2D^% Texture2D)
	{
		pin_ptr<Texture2D_Description> PinnedDescription = &Description;

		D3D10_SUBRESOURCE_DATA NativeInitialData;
		InitialData.ToNative(&NativeInitialData);
		D3D10_SUBRESOURCE_DATA* pInitialData = &NativeInitialData;

		ID3D10Texture2D* pTexture2D = 0;
		int Result = pDevice->CreateTexture2D((D3D10_TEXTURE2D_DESC*)PinnedDescription, pInitialData, &pTexture2D);

		if (pTexture2D)
		{
			try { Texture2D = (Xtro::MDX::Direct3D10::Texture2D^)Interfaces[IntPtr(pTexture2D)]; }
			catch (KeyNotFoundException^) { Texture2D = gcnew Xtro::MDX::Direct3D10::Texture2D(IntPtr(pTexture2D)); }					
		}
		else Texture2D = nullptr;

		return Result;
	}

	int CreateTexture2D(Texture2D_Description% Description)
	{
		pin_ptr<Texture2D_Description> PinnedDescription = &Description;

		return pDevice->CreateTexture2D((D3D10_TEXTURE2D_DESC*)PinnedDescription, 0, 0);
	}

	int CreateTexture2D(Texture2D_Description% Description, SubResourceData% InitialData)
	{
		pin_ptr<Texture2D_Description> PinnedDescription = &Description;

		D3D10_SUBRESOURCE_DATA NativeInitialData;
		InitialData.ToNative(&NativeInitialData);
		D3D10_SUBRESOURCE_DATA* pInitialData = &NativeInitialData;

		return pDevice->CreateTexture2D((D3D10_TEXTURE2D_DESC*)PinnedDescription, pInitialData, 0);
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
				unsigned int VertexBufferCount = VertexBuffers->Length;
				pVertexBuffers = new ID3D10Buffer*[VertexBufferCount];
				for (unsigned int VertexBufferNo = 0; VertexBufferNo < VertexBufferCount; VertexBufferNo++) pVertexBuffers[VertexBufferNo] = VertexBuffers[VertexBufferNo]->pBuffer;
			}

			pDevice->IASetVertexBuffers(StartSlot, NumberOfBuffers, pVertexBuffers, PinnedStrides, PinnedOffsets);
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

	void IA_SetPrimitiveTopology(PrimitiveTopology Topology)
	{
		pDevice->IASetPrimitiveTopology((D3D10_PRIMITIVE_TOPOLOGY)Topology);
	}

	void Draw(unsigned int VertexCount, unsigned int StartVertexLocation)
	{
		pDevice->Draw(VertexCount, StartVertexLocation);
	}

	void DrawIndexed(unsigned int IndexCount, unsigned int StartIndexLocation, int BaseVertexLocation)
	{
		pDevice->DrawIndexed(IndexCount, StartIndexLocation, BaseVertexLocation);
	}

	int CheckMultisampleQualityLevels(Format Format, unsigned int SampleCount, [Out] unsigned int% NumberOfQualityLevels)
	{
		pin_ptr<unsigned int> PinnedNumberOfQualityLevels = &NumberOfQualityLevels;

		return pDevice->CheckMultisampleQualityLevels((DXGI_FORMAT)Format, SampleCount, (unsigned int*)PinnedNumberOfQualityLevels);
	}

	void CopyResource(Resource^ DestinationResource, Resource^ SourceResource)
	{
		ID3D10Resource* pDestinationResource = DestinationResource == nullptr ? 0 : DestinationResource->pResource;
		ID3D10Resource* pSourceResource = SourceResource == nullptr ? 0 : SourceResource->pResource;

		pDevice->CopyResource(pDestinationResource, pSourceResource);
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

	int CreateShaderResourceView(Resource^ Resource)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;

		return pDevice->CreateShaderResourceView(pResource, 0, 0);
	}

	int CreateShaderResourceView(Resource^ Resource, ShaderResourceViewDescription% Description)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<ShaderResourceViewDescription> PinnedDescription = &Description;

		return pDevice->CreateShaderResourceView(pResource, (D3D10_SHADER_RESOURCE_VIEW_DESC*)PinnedDescription, 0);
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
};