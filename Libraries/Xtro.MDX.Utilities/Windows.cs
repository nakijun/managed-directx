using System;
using System.Runtime.InteropServices;

namespace Xtro.MDX.Utilities
{
    internal class Windows
    {
        internal enum GetWindowLongConst
        {
            WindowProcedure = -4,
            InstanceHandle = -6,
            ParentWindowHandle = -8,
            Style = -16,
            ExtendedStyle = -20,
            UserData = -21,
            ID = -12
        }
        [Flags]
        internal enum WindowPlacementFlag
        {
            SetMinPosition = 1,
            RestoreToMaximized = 2,
            AsyncWindowPlacement = 4
        }

        internal enum ShowWindowCommand
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3, // is this the right value?
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct Point32
        {
            public int X;
            public int Y;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct Rect32
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct WindowPlacement
        {
            public int Length;
            public WindowPlacementFlag Flags;
            public ShowWindowCommand ShowCommand;
            public Point32 MinPosition;
            public Point32 MaxPosition;
            public Rect32 NormalPosition;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct PointL
        {
            [MarshalAs(UnmanagedType.I4)]
            public int x;
            [MarshalAs(UnmanagedType.I4)]
            public int y;
        }
        
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DevMode
        {
            // You can define the following constant
            // but OUTSIDE the structure because you know
            // that size and layout of the structure
            // is very important
            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmDeviceName;
            // In addition you can define the last character array
            // as following:
            //[MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
            //public Char[] dmDeviceName;

            // After the 32-bytes array
            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSpecVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverVersion;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmSize;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmDriverExtra;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmFields;

            public PointL dmPosition;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayOrientation;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFixedOutput;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmColor;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmDuplex;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmYResolution;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmTTOption;

            [MarshalAs(UnmanagedType.I2)]
            public Int16 dmCollate;

            // CCHDEVICENAME = 32 = 0x50
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string dmFormName;
            // Also can be defined as
            //[MarshalAs(UnmanagedType.ByValArray,
            //    SizeConst = 32, ArraySubType = UnmanagedType.U1)]
            //public Byte[] dmFormName;

            [MarshalAs(UnmanagedType.U2)]
            public UInt16 dmLogPixels;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmBitsPerPel;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPelsHeight;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFlags;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDisplayFrequency;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMMethod;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmICMIntent;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmMediaType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmDitherType;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved1;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmReserved2;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningWidth;

            [MarshalAs(UnmanagedType.U4)]
            public UInt32 dmPanningHeight;
        }
        
        [Flags]
        internal enum ExecutionState : uint
        {
            SystemRequired = 0x00000001,
            DisplayRequired = 0x00000002,
            // Legacy flag, should not be used.
            // ES_USER_PRESENT   = 0x00000004,
            Continuous = 0x80000000
        }

        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity]
        internal static extern bool QueryPerformanceCounter(out long PerformanceCount);
        [DllImport("kernel32.dll")]

        [System.Security.SuppressUnmanagedCodeSecurity]
        internal static extern void QueryPerformanceFrequency(out long Frequency);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetWindowPlacement(IntPtr WindowHandle, ref WindowPlacement WindowPlacement);

        [DllImport("user32.dll")]
        internal static extern bool SetWindowPlacement(IntPtr WindowHandle, ref WindowPlacement WindowPlacement);
    
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr WindowHandle, ShowWindowCommand ShowCommand);
    
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool AdjustWindowRect(ref Rect32 Rect, uint Style, bool Menu);

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint GetWindowLong(IntPtr WindowHandle, GetWindowLongConst Index);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto,SetLastError = true)]
        internal static extern ExecutionState SetThreadExecutionState(ExecutionState Flags);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern Boolean EnumDisplaySettings([param: MarshalAs(UnmanagedType.LPTStr)] string DeviceName, [param: MarshalAs(UnmanagedType.U4)] int ModeNumber, [In, Out] ref DevMode DevMode);
    }
}
