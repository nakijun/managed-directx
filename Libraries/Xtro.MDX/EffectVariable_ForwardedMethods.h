EffectMatrixVariable^ EffectVariable::AsMatrix()
{
	ID3D10EffectMatrixVariable* pResult = pEffectVariable->AsMatrix();

	EffectMatrixVariable^ Result = nullptr;
	if (pResult) 
	{
		try
		{
			try	{ Result = (EffectMatrixVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { Result = gcnew EffectMatrixVariable(IntPtr(pResult)); }
	}

	return Result;
}

EffectScalarVariable^ EffectVariable::AsScalar()
{
	ID3D10EffectScalarVariable* pResult = pEffectVariable->AsScalar();

	EffectScalarVariable^ Result = nullptr;
	if (pResult) 
	{
		try
		{
			try	{ Result = (EffectScalarVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { Result = gcnew EffectScalarVariable(IntPtr(pResult)); }
	}

	return Result;
}

EffectVectorVariable^ EffectVariable::AsVector()
{
	ID3D10EffectVectorVariable* pResult = pEffectVariable->AsVector();

	EffectVectorVariable^ Result = nullptr;
	if (pResult) 
	{
		try
		{
			try	{ Result = (EffectVectorVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { Result = gcnew EffectVectorVariable(IntPtr(pResult)); }
	}

	return Result;
}

EffectShaderResourceVariable^ EffectVariable::AsShaderResource()
{
	ID3D10EffectShaderResourceVariable* pResult = pEffectVariable->AsShaderResource();

	EffectShaderResourceVariable^ Result = nullptr;
	if (pResult) 
	{
		try
		{
			try	{ Result = (EffectShaderResourceVariable^)Interfaces[IntPtr(pResult)]; }
			catch (InvalidCastException^)
			{
				Interfaces.Remove(IntPtr(pResult));
				throw gcnew	KeyNotFoundException();
			}
		}
		catch (KeyNotFoundException^) { Result = gcnew EffectShaderResourceVariable(IntPtr(pResult)); }
	}

	return Result;
}