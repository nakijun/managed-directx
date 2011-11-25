public value class TrackProperties : IEquatable<TrackProperties>
{
public:
	long Duration;      
	unsigned short NumberOfVariations; 
	unsigned char NumberOfChannels;   
	unsigned short WaveVariation; 
	unsigned char LoopCount;     

	static bool operator == (TrackProperties Left, TrackProperties Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (TrackProperties Left, TrackProperties Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			Duration ^
			NumberOfVariations ^
			NumberOfChannels ^
			WaveVariation ^
			LoopCount;
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(TrackProperties Value)
	{
		return memcmp(&*this, &Value, Marshal::SizeOf(TrackProperties::typeid)) == 0;
	}

	static bool Equals(TrackProperties% Value1, TrackProperties% Value2)
	{
		pin_ptr<TrackProperties> PinnedValue1 = &Value1;
		pin_ptr<TrackProperties> PinnedValue2 = &Value2;

		return memcmp(PinnedValue1, PinnedValue2, Marshal::SizeOf(TrackProperties::typeid)) == 0;
	}
};