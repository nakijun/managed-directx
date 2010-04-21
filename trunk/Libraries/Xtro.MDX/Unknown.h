namespace Xtro
{
namespace MDX
{
	public ref class Unknown : Interface
	{
	internal:
		IUnknown* pUnknown;

		static Guid IID = IID_Converter::ToManaged(IID_IUnknown);

	protected:
		Unknown(IntPtr Unknown) : Interface(Unknown)
		{
			pUnknown = (IUnknown*)Unknown.ToPointer();
		}

	public:
		unsigned int AddRef()
		{
			return pUnknown->AddRef();
		}

		unsigned int Release()
		{
			unsigned int Result = pUnknown->Release();

			if(Result == 0) Interfaces.Remove(InterfacePointer);

			return Result;
		}
	};
}
}