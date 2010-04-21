namespace Xtro
{
namespace MDX
{
namespace DXGI
{
	[StructLayout(LayoutKind::Sequential)]
	public value class AdapterDescription : IEquatable<AdapterDescription>
	{
	public:
		String^ Description;
		unsigned int VendorId;
		unsigned int DeviceId;
		unsigned int SubSystemId;
		unsigned int Revision;
		SIZE_T DedicatedVideoMemory;
		SIZE_T DedicatedSystemMemory;
		SIZE_T SharedSystemMemory;
		Int64 AdapterLuid;

		AdapterDescription(String^ Description, unsigned int VendorId, unsigned int DeviceId, unsigned int SubSystemId, unsigned int Revision, SIZE_T DedicatedVideoMemory,	SIZE_T DedicatedSystemMemory, SIZE_T SharedSystemMemory, Int64 AdapterLuid)
		{
			this->Description = Description;
			this->VendorId = VendorId;
			this->DeviceId = DeviceId;
			this->SubSystemId = SubSystemId;
			this->Revision = Revision;
			this->DedicatedVideoMemory = DedicatedVideoMemory;
			this->DedicatedSystemMemory = DedicatedSystemMemory;
			this->SharedSystemMemory = SharedSystemMemory;
			this->AdapterLuid = AdapterLuid;
		}

		static bool operator == (AdapterDescription Left, AdapterDescription Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (AdapterDescription Left, AdapterDescription Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				Description->GetHashCode() ^
				VendorId ^
				DeviceId ^
				SubSystemId ^
				Revision ^
				DedicatedVideoMemory.GetHashCode() ^
				DedicatedSystemMemory.GetHashCode() ^
				SharedSystemMemory.GetHashCode() ^
				AdapterLuid.GetHashCode();
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(AdapterDescription Value)
		{
			return
				Description == Value.Description &&
				VendorId == Value.VendorId &&
				DeviceId == Value.DeviceId &&
				SubSystemId == Value.SubSystemId &&
				Revision == Value.Revision &&
				DedicatedVideoMemory == Value.DedicatedVideoMemory &&
				DedicatedSystemMemory == Value.DedicatedSystemMemory &&
				SharedSystemMemory == Value.SharedSystemMemory &&
				AdapterLuid == Value.AdapterLuid;
		}

		static bool Equals(AdapterDescription% Value1, AdapterDescription% Value2)
		{
			return
				Value1.Description == Value2.Description &&
				Value1.VendorId == Value2.VendorId &&
				Value1.DeviceId == Value2.DeviceId &&
				Value1.SubSystemId == Value2.SubSystemId &&
				Value1.Revision == Value2.Revision &&
				Value1.DedicatedVideoMemory == Value2.DedicatedVideoMemory &&
				Value1.DedicatedSystemMemory == Value2.DedicatedSystemMemory &&
				Value1.SharedSystemMemory == Value2.SharedSystemMemory &&
				Value1.AdapterLuid == Value2.AdapterLuid;
		}
	};
}
}
}