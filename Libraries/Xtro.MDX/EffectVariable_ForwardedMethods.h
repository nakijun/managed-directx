EffectBlendVariable^ EffectVariable::AsBlend()
{
	ID3D10EffectBlendVariable* pResult = pEffectVariable->AsBlend();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectBlendVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectBlendVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectConstantBuffer^ EffectVariable::AsConstantBuffer()
{
	ID3D10EffectConstantBuffer* pResult = pEffectVariable->AsConstantBuffer();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectConstantBuffer^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectConstantBuffer(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectDepthStencilVariable^ EffectVariable::AsDepthStencil()
{
	ID3D10EffectDepthStencilVariable* pResult = pEffectVariable->AsDepthStencil();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectDepthStencilVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectDepthStencilVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectDepthStencilViewVariable^ EffectVariable::AsDepthStencilView()
{
	ID3D10EffectDepthStencilViewVariable* pResult = pEffectVariable->AsDepthStencilView();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectDepthStencilViewVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectDepthStencilViewVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectMatrixVariable^ EffectVariable::AsMatrix()
{
	ID3D10EffectMatrixVariable* pResult = pEffectVariable->AsMatrix();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectMatrixVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectMatrixVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectRasterizerVariable^ EffectVariable::AsRasterizer()
{
	ID3D10EffectRasterizerVariable* pResult = pEffectVariable->AsRasterizer();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectRasterizerVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectRasterizerVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectRenderTargetViewVariable^ EffectVariable::AsRenderTargetView()
{
	ID3D10EffectRenderTargetViewVariable* pResult = pEffectVariable->AsRenderTargetView();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectRenderTargetViewVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectRenderTargetViewVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectSamplerVariable^ EffectVariable::AsSampler()
{
	ID3D10EffectSamplerVariable* pResult = pEffectVariable->AsSampler();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectSamplerVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectSamplerVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectScalarVariable^ EffectVariable::AsScalar()
{
	ID3D10EffectScalarVariable* pResult = pEffectVariable->AsScalar();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectScalarVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectScalarVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectShaderVariable^ EffectVariable::AsShader()
{
	ID3D10EffectShaderVariable* pResult = pEffectVariable->AsShader();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectShaderVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectShaderVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectShaderResourceVariable^ EffectVariable::AsShaderResource()
{
	ID3D10EffectShaderResourceVariable* pResult = pEffectVariable->AsShaderResource();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectShaderResourceVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectShaderResourceVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectStringVariable^ EffectVariable::AsString()
{
	ID3D10EffectStringVariable* pResult = pEffectVariable->AsString();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectStringVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectStringVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectVectorVariable^ EffectVariable::AsVector()
{
	ID3D10EffectVectorVariable* pResult = pEffectVariable->AsVector();

	if (pResult) 
	{
		try
		{
			try	{ return (EffectVectorVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { return gcnew EffectVectorVariable(IntPtr(pResult)); }
	}

	return nullptr;
}

EffectConstantBuffer^ EffectVariable::GetParentConstantBuffer()
{
	ID3D10EffectConstantBuffer* pResult = pEffectVariable->GetParentConstantBuffer();

	if (pResult)
	{	
		try { return (EffectConstantBuffer^)Interfaces[IntPtr(pResult)]; }
		catch (KeyNotFoundException^) { return gcnew EffectConstantBuffer(IntPtr(pResult)); }
	}
		
	return nullptr;
}
