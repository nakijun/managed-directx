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

	int CreateRenderTargetView(Xtro::MDX::Direct3D10::Resource^ Resource, Nullable<RenderTargetViewDescription> Description, [Out] RenderTargetView^% RenderTargetView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<RenderTargetViewDescription> PinnedDescription = Description.HasValue ? &Description.Value : nullptr;

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

	int CreateRenderTargetView(Xtro::MDX::Direct3D10::Resource^ Resource, Nullable<RenderTargetViewDescription> Description)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<RenderTargetViewDescription> PinnedDescription = Description.HasValue ? &Description.Value : nullptr;

		return pDevice->CreateRenderTargetView(pResource, (D3D10_RENDER_TARGET_VIEW_DESC*)PinnedDescription, 0);
	}

	int CreateDepthStencilView(Xtro::MDX::Direct3D10::Resource^ Resource, Nullable<DepthStencilViewDescription> Description, [Out] DepthStencilView^% DepthStencilView)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<DepthStencilViewDescription> PinnedDescription = Description.HasValue ? &Description.Value : nullptr;

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

	int CreateDepthStencilView(Xtro::MDX::Direct3D10::Resource^ Resource, Nullable<DepthStencilViewDescription> Description)
	{
		ID3D10Resource* pResource = Resource == nullptr ? 0 : Resource->pResource;
		pin_ptr<DepthStencilViewDescription> PinnedDescription = Description.HasValue ? &Description.Value : nullptr;

		return pDevice->CreateDepthStencilView(pResource, (D3D10_DEPTH_STENCIL_VIEW_DESC*)PinnedDescription, 0);
	}

	void OMSetRenderTargets(array<RenderTargetView^>^ RenderTargetViews, DepthStencilView^ DepthStencilView)
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

			pDevice->OMSetRenderTargets(RenderTargetViews->Length, ppRenderTargetViews, pDepthStencilView);
		}
		finally
		{
			if (ppRenderTargetViews) delete[] ppRenderTargetViews;
		}
	}

	void RSSetViewports(array<Viewport>^ Viewports)
	{
		pin_ptr<Viewport> PinnedViewports = Viewports != nullptr && Viewports->Length > 0 ? &Viewports[0] : nullptr;

		pDevice->RSSetViewports(Viewports->Length, (D3D10_VIEWPORT*)PinnedViewports);
	}

	void IASetInputLayout(InputLayout^ InputLayout)
	{
		ID3D10InputLayout* pInputLayout = InputLayout == nullptr ? 0 : InputLayout->pInputLayout;

		pDevice->IASetInputLayout(pInputLayout);
	}

	int CreateInputLayout(array<InputElementDescription>^ InputElementDescriptions, array<Byte>^ ShaderBytecodeWithInputSignature, [Out] InputLayout^% InputLayout)
	{
		pin_ptr<Byte> PinnedShaderBytecodeWithInputSignature = nullptr;
		SIZE_T BytecodeLength = 0;
		if (ShaderBytecodeWithInputSignature != nullptr && ShaderBytecodeWithInputSignature->Length > 0)
		{
			BytecodeLength = ShaderBytecodeWithInputSignature->Length;
			PinnedShaderBytecodeWithInputSignature = &ShaderBytecodeWithInputSignature[0];
		}

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

			Result = pDevice->CreateInputLayout(pInputElementDescriptions, ElementCount, PinnedShaderBytecodeWithInputSignature, BytecodeLength, &pInputLayout);
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

	int CreateBuffer(BufferDescription% Description, Nullable<SubResourceData> InitialData, [Out] Buffer^% Buffer)
	{
		pin_ptr<BufferDescription> PinnedDescription = &Description;

		D3D10_SUBRESOURCE_DATA NativeInitialData;
		D3D10_SUBRESOURCE_DATA* pInitialData = 0;
		if (InitialData.HasValue)
		{
			InitialData.Value.ToNative(&NativeInitialData);
			pInitialData = &NativeInitialData;
		}

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

	int CreateTexture2D(Texture2D_Description% Description, Nullable<SubResourceData> InitialData, [Out] Texture2D^% Texture2D)
	{
		pin_ptr<Texture2D_Description> PinnedDescription = &Description;

		D3D10_SUBRESOURCE_DATA NativeInitialData;
		D3D10_SUBRESOURCE_DATA* pInitialData = 0;
		if (InitialData.HasValue)
		{
			InitialData.Value.ToNative(&NativeInitialData);
			pInitialData = &NativeInitialData;
		}

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

	int CreateTexture2D(Texture2D_Description% Description, Nullable<SubResourceData> InitialData)
	{
		pin_ptr<Texture2D_Description> PinnedDescription = &Description;

		D3D10_SUBRESOURCE_DATA NativeInitialData;
		D3D10_SUBRESOURCE_DATA* pInitialData = 0;
		if (InitialData.HasValue)
		{
			InitialData.Value.ToNative(&NativeInitialData);
			pInitialData = &NativeInitialData;
		}

		return pDevice->CreateTexture2D((D3D10_TEXTURE2D_DESC*)PinnedDescription, pInitialData, 0);
	}

	void IASetVertexBuffers(unsigned int StartSlot, array<Buffer^>^ VertexBuffers, array<unsigned int>^ Strides, array<unsigned int>^ Offsets)
	{
		pin_ptr<unsigned int> PinnedStrides = Strides != nullptr && Strides->Length > 0 ? &Strides[0] : nullptr;
		pin_ptr<unsigned int> PinnedOffsets = Offsets != nullptr && Offsets->Length > 0 ? &Offsets[0] : nullptr;

		ID3D10Buffer** pVertexBuffers = 0;
		try
		{
			unsigned int VertexBufferCount = 0;
			if (VertexBuffers != nullptr && VertexBuffers->Length > 0)
			{
				VertexBufferCount = VertexBuffers->Length;
				pVertexBuffers = new ID3D10Buffer*[VertexBufferCount];
				for (unsigned int VertexBufferNo = 0; VertexBufferNo < VertexBufferCount; VertexBufferNo++) pVertexBuffers[VertexBufferNo] = VertexBuffers[VertexBufferNo]->pBuffer;
			}

			pDevice->IASetVertexBuffers(StartSlot, VertexBufferCount, pVertexBuffers, PinnedStrides, PinnedOffsets);
		}
		finally
		{
			if (pVertexBuffers) delete[] pVertexBuffers;
		}
	}

	void IASetIndexBuffer(Buffer^ IndexBuffer, Format Format, unsigned int Offset)
	{
		ID3D10Buffer* pIndexBuffer = IndexBuffer == nullptr ? 0 : IndexBuffer->pBuffer;

		pDevice->IASetIndexBuffer(pIndexBuffer, (DXGI_FORMAT)Format, Offset);
	}

	void IASetPrimitiveTopology(PrimitiveTopology Topology)
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
};