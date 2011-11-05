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