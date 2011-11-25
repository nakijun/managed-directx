public value class SoundProperties : IEquatable<SoundProperties>
{
internal:
	inline void FromNative(XACT_SOUND_PROPERTIES* Native)
	{
		Category = Native->category;
		Priority = Native->priority;
		Pitch = Native->pitch;
		Volume = Native->volume;
		NumberOfTracks = Native->numTracks;

		if (Native->numTracks)
		{
			ArrayTrackProperties = gcnew array<TrackProperties>(NumberOfTracks);
			pin_ptr<TrackProperties> PinnedArrayTrackProperties = NumberOfTracks > 0 ? &ArrayTrackProperties[0] : nullptr;
			memcpy(PinnedArrayTrackProperties, Native->arrTrackProperties, NumberOfTracks * sizeof(XACT_TRACK_PROPERTIES));
		}
		else ArrayTrackProperties = nullptr;
	}

public:
	unsigned short Category;             
	unsigned char Priority;             
	short Pitch;                
	float Volume;               
	unsigned short NumberOfTracks;            
	array<TrackProperties>^ ArrayTrackProperties;

	static bool operator == (SoundProperties Left, SoundProperties Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (SoundProperties Left, SoundProperties Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Category ^
			Priority ^
			Pitch ^
			NumberOfTracks;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(SoundProperties Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(SoundProperties::typeid)) == 0;
	}

	static bool Equals(SoundProperties% Value1, SoundProperties% Value2)
	{
		pin_ptr<SoundProperties> PinnedValue1 = &Value1;
		pin_ptr<SoundProperties> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(SoundProperties::typeid)) == 0;
	}
};