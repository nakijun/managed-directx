public ref class GeometryShader sealed : DeviceChild
{
private:
	static Guid IID = IID_Converter::ToManaged(IID_ID3D10GeometryShader);

	int Size;

internal:
	ID3D10GeometryShader* pGeometryShader;

	GeometryShader(IntPtr GeometryShader) : DeviceChild(GeometryShader)
	{	
		Size = -1;

		pGeometryShader = (ID3D10GeometryShader*)GeometryShader.ToPointer();
	}

public:
};