namespace Xtro.MDX.Utilities
{
    public enum Error
    {
        NoDirect3D = -2147219199,
        NoCompatibleDevices = -2147219198,
        MediaNotFound = -2147219197,
        NonZeroRefCount = -2147219196,
        CreatingDevice = -2147219195,
        ResettingDevice = -2147219194,
        CreatingDeviceObjects = -2147219193,
        ResettingDeviceObjects = -2147219192,
        DeviceRemoved = -2147219190,

        Fail = -2147467259,
        Abort = -2147467260,
        False = 1
    };

    public enum MatchType
    {
        IgnoreInput = 0,  // Use the closest valid value to a default 
        PreserveInput,    // Use input without change, but may cause no valid device to be found
        ClosestToInput    // Use the closest valid value to the input 
    };
}