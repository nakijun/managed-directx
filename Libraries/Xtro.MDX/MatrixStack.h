public ref class MatrixStack sealed : Unknown
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3DXMatrixStack);

internal:
	ID3DXMatrixStack* pMatrixStack;

	MatrixStack(IntPtr MatrixStack) : Unknown(MatrixStack)
	{	
		pMatrixStack = (ID3DXMatrixStack*)MatrixStack.ToPointer();
	}

public:
	Generic::UnmanagedMemory<Matrix>^ GetTop()
	{
		D3DXMATRIX* pResult = pMatrixStack->GetTop();

		return gcnew Generic::UnmanagedMemory<Matrix>(IntPtr(pResult), sizeof(D3DXMATRIX));
	}

	int LoadIdentity()
	{
		return pMatrixStack->LoadIdentity();
	}

	int LoadMatrix(Matrix% M)
	{
		pin_ptr<Matrix> PinnedM = &M;
		
		return pMatrixStack->LoadMatrix((D3DXMATRIX*)PinnedM);
	}

	int MultiplyMatrix(Matrix% M)
	{
		pin_ptr<Matrix> PinnedM = &M;
		
		return pMatrixStack->MultMatrix((D3DXMATRIX*)PinnedM);
	}

	int MultiplyMatrixLocal(Matrix% M)
	{
		pin_ptr<Matrix> PinnedM = &M;
		
		return pMatrixStack->MultMatrixLocal((D3DXMATRIX*)PinnedM);
	}

	int Pop()
	{
		return pMatrixStack->Pop();
	}

	int RotateAxis(Vector% V, float Angle)
	{
		pin_ptr<Vector> PinnedV = &V;
		
		return pMatrixStack->RotateAxis((D3DXVECTOR3*)PinnedV, Angle);
	}

	int RotateAxisLocal(Vector% V, float Angle)
	{
		pin_ptr<Vector> PinnedV = &V;
		
		return pMatrixStack->RotateAxisLocal((D3DXVECTOR3*)PinnedV, Angle);
	}

	int RotateYawPitchRoll(float Yaw, float Pitch, float Roll)
	{
		return pMatrixStack->RotateYawPitchRoll(Yaw, Pitch, Roll);
	}

	int RotateYawPitchRollLocal(float Yaw, float Pitch, float Roll)
	{
		return pMatrixStack->RotateYawPitchRollLocal(Yaw, Pitch, Roll);
	}

	int Scale(float X, float Y, float Z)
	{
		return pMatrixStack->Scale(X, Y, Z);
	}

	int ScaleLocal(float X, float Y, float Z)
	{
		return pMatrixStack->ScaleLocal(X, Y, Z);
	}

	int Translate(float X, float Y, float Z)
	{
		return pMatrixStack->Translate(X, Y, Z);
	}

	int TranslateLocal(float X, float Y, float Z)
	{
		return pMatrixStack->TranslateLocal(X, Y, Z);
	}
};