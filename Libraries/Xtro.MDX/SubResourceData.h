namespace Xtro
{
namespace MDX
{
namespace Direct3D10
{
	public value class SubResourceData : IEquatable<SubResourceData>
	{
	internal:
		/*
		Native system memory size is unknown. We can't construct from native struct
		SubResourceData(D3D10_SUBRESOURCE_DATA* Native)
		{
			SystemMemory = gcnew array<Byte>(Native->??);
			pin_ptr<Byte> PinnedSystemMemory = &SystemMemory[0];
			memcpy(PinnedSystemMemory, Native->pSysMem, SystemMemory->Length);

			SystemMemoryPitch = Native->SysMemPitch;
			SystemMemorySlicePitch = Native->SysMemSlicePitch;
		}
		*/
		
		D3D10_SUBRESOURCE_DATA ToNative()
		{
			D3D10_SUBRESOURCE_DATA Result;
			
			Result.pSysMem = 0;
			if (SystemMemory != nullptr && SystemMemory->Size > 0) Result.pSysMem = SystemMemory->Memory;

			Result.SysMemPitch = SystemMemoryPitch;
			Result.SysMemSlicePitch = SystemMemorySlicePitch;
			
			return Result;
		}

	public:
		UnmanagedMemory^ SystemMemory;
		unsigned int SystemMemoryPitch;
		unsigned int SystemMemorySlicePitch;

		SubResourceData(UnmanagedMemory^ SystemMemory, unsigned int SystemMemoryPitch, unsigned int SystemMemorySlicePitch)
		{
			this->SystemMemory = SystemMemory;
			this->SystemMemoryPitch = SystemMemoryPitch;
			this->SystemMemorySlicePitch = SystemMemorySlicePitch;
		}

		static bool operator == (SubResourceData Left, SubResourceData Right)
		{
			return Equals(Left, Right);
		}

		static bool operator != (SubResourceData Left, SubResourceData Right)
		{
			return !Equals(Left, Right);
		}

		virtual int GetHashCode() override
		{
			return
				SystemMemory->GetHashCode() ^
				(int)SystemMemoryPitch ^
				(int)SystemMemorySlicePitch;
		}

		virtual bool Equals(System::Object^ Value) override
		{
			if (Value == nullptr) return false;

			if (Value->GetType() != GetType()) return false;

			return Equals(Value);
		}

		virtual bool Equals(SubResourceData Value)
		{
			if (SystemMemory == nullptr && Value.SystemMemory != nullptr) return false;
			if (SystemMemory != nullptr && Value.SystemMemory == nullptr) return false;
			if (SystemMemory != nullptr && Value.SystemMemory != nullptr)
			{
				if (!SystemMemory->Equals(Value.SystemMemory)) return false;
			}
				   
			return
				SystemMemoryPitch == Value.SystemMemoryPitch &&
				SystemMemorySlicePitch == Value.SystemMemorySlicePitch;
		}

		static bool Equals(SubResourceData% Value1, SubResourceData% Value2)
		{
			if (Value1.SystemMemory == nullptr && Value2.SystemMemory != nullptr) return false;
			if (Value1.SystemMemory != nullptr && Value2.SystemMemory == nullptr) return false;
			if (Value1.SystemMemory != nullptr && Value2.SystemMemory != nullptr)
			{
				if (!Value1.SystemMemory->Equals(Value2.SystemMemory)) return false;
			}

			return
				Value1.SystemMemoryPitch == Value2.SystemMemoryPitch &&
				Value1.SystemMemorySlicePitch == Value2.SystemMemorySlicePitch;
		}
	};
}
}
}