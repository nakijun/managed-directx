void DeviceChild::GetDevice([Out] Device^% Device)
{
	ID3D10Device* pDevice = 0;
	pDeviceChild->GetDevice(&pDevice);

	if (pDevice) 
	{
		try { Device = (Xtro::MDX::Direct3D10::Device^)Interfaces[IntPtr(pDevice)]; }
		catch (KeyNotFoundException^) { Device = gcnew Xtro::MDX::Direct3D10::Device(IntPtr(pDevice)); }
	}
	else Device = nullptr;
}
