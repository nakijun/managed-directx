using System;

namespace Xtro.MDX.Utilities
{
    public enum CameraKeys
    {
        StrafeLeft = 0,
        StrafeRight,
        MoveForward,
        MoveBackward,
        MoveUp,
        MoveDown,
        Reset,
        ControlDown,
        MaximumKeys,
        Unknown = 0xFF
    };

    [Flags]
    public enum MouseKeys
    {
        Left = 0x01,
        Middle = 0x02,
        Right = 0x04,
        Wheel = 0x08,
    };

    [Flags]
    public enum KeyMaskFlag
    {
        WasDown = 0x80,
        IsDown = 0x01
    };

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

        NoInterface = -2147467262,
        OutOfMemory = -2147024882,
        InvalidArgument = -2147024809,
        Fail = -2147467259,
        Abort = -2147467260,
        False = 1
    };

    public enum IndexType
    {
        // ReSharper disable InconsistentNaming
        x16 = 0,
        x32,
        // ReSharper restore InconsistentNaming
    };

    public enum PrimitiveType
    {
        TriangleList = 0,
        TriangleStrip,
        LineList,
        LineStrip,
        PointList,
        TriangleListAdjacency,
        TriangleStripAdjacency,
        LineListAdjacency,
        LineStripAdjacency
    };

    enum FrameTransformType
    {
        Relative = 0,
        Absolute,		//This is not currently used but is here to support absolute transformations in the future
    };

    public enum MatchType
    {
        IgnoreInput = 0,  // Use the closest valid value to a default 
        PreserveInput,    // Use input without change, but may cause no valid device to be found
        ClosestToInput    // Use the closest valid value to the input 
    };
}