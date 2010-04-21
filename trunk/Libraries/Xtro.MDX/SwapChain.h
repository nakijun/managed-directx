namespace Xtro
{
namespace MDX
{
namespace DXGI
{
	public ref class SwapChain : Object
	{
	internal:
		IDXGISwapChain* pSwapChain;

		static Guid IID = IID_Converter::ToManaged(IID_IDXGISwapChain);

		SwapChain(IntPtr SwapChain) : Object(SwapChain)
		{
			pSwapChain = (IDXGISwapChain*)SwapChain.ToPointer();
		}

	public:
		int GetBuffer(unsigned int Buffer, Type^ Type, [Out] Unknown^% Surface)
		{
			Guid RIID;
			try { RIID = (Guid)Type->GetField("IID", BindingFlags::NonPublic | BindingFlags::Static)->GetValue(nullptr); }
			catch (...) { RIID = Guid::Empty; }

			IUnknown* pUnknown = 0;
			int Result = pSwapChain->GetBuffer(Buffer, IID_Converter::ToNative(RIID), (void**)&pUnknown);

			if (pUnknown) 
			{
				try { Surface = (Unknown^)Interfaces[IntPtr(pUnknown)]; }
				catch (KeyNotFoundException^) { Surface = (Unknown^)Activator::CreateInstance(Type, BindingFlags::NonPublic | BindingFlags::Instance, nullptr, gcnew array<System::Object^>(1) { IntPtr(pUnknown) }, CultureInfo::CurrentCulture); }
			}
			else Surface = nullptr;

			return Result;
		}

		int Present(unsigned int SyncInterval, PresentFlag Flags)
		{
			return pSwapChain->Present(SyncInterval, (unsigned int)Flags);
		}
	};
}
}
}