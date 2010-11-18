public value class OutputDescription : IEquatable<OutputDescription>
{
internal:
	inline void FromNative(DXGI_OUTPUT_DESC* Native)
	{
		DeviceName = gcnew String(Native->DeviceName);

		DesktopCoordinates.X = Native->DesktopCoordinates.left;
		DesktopCoordinates.Width = Native->DesktopCoordinates.right - Native->DesktopCoordinates.left;
		DesktopCoordinates.Y = Native->DesktopCoordinates.top;
		DesktopCoordinates.Height = Native->DesktopCoordinates.bottom - Native->DesktopCoordinates.top;

		AttachedToDesktop = Native->AttachedToDesktop != 0;
		Rotation = (Xtro::MDX::DXGI::ModeRotation)Native->Rotation;
		Monitor = IntPtr(Native->Monitor);
	}

public:
	String^ DeviceName;
	System::Drawing::Rectangle DesktopCoordinates;
	bool AttachedToDesktop;
	ModeRotation Rotation;
	IntPtr Monitor;

	static bool operator == (OutputDescription Left, OutputDescription Right)
	{
		return Equals(Left, Right);
	}

	static bool operator != (OutputDescription Left, OutputDescription Right)
	{
		return !Equals(Left, Right);
	}

	virtual int GetHashCode() override
	{
		return
			DeviceName->GetHashCode() ^
			DesktopCoordinates.GetHashCode() ^
			AttachedToDesktop.GetHashCode() ^
			(int)Rotation ^
			Monitor.GetHashCode();
	}

	virtual bool Equals(System::Object^ Value) override
	{
		if (Value == nullptr) return false;

		if (Value->GetType() != GetType()) return false;

		return Equals(Value);
	}

	virtual bool Equals(OutputDescription Value)
	{
		return
			DeviceName == Value.DeviceName &&
			DesktopCoordinates == Value.DesktopCoordinates &&
			AttachedToDesktop == Value.AttachedToDesktop &&
			Rotation == Value.Rotation &&
			Monitor == Value.Monitor;
	}

	static bool Equals(OutputDescription% Value1, OutputDescription% Value2)
	{
		return
			Value1.DeviceName == Value2.DeviceName &&
			Value1.DesktopCoordinates == Value2.DesktopCoordinates &&
			Value1.AttachedToDesktop == Value2.AttachedToDesktop &&
			Value1.Rotation == Value2.Rotation &&
			Value1.Monitor == Value2.Monitor;
	}
};