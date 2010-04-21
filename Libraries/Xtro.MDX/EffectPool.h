namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public ref class EffectPool : Unknown
	{
	internal:
		ID3D10EffectPool* pEffectPool;

		static Guid IID = IID_Converter::ToManaged(IID_ID3D10EffectPool);

		EffectPool(IntPtr EffectPool) : Unknown(EffectPool)
		{	
			pEffectPool = (ID3D10EffectPool*)EffectPool.ToPointer();
		}

	public:
	};
}
}
}