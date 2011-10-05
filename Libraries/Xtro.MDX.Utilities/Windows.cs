using System;
using System.Runtime.InteropServices;

namespace Xtro.MDX.Utilities
{
    internal static class Windows
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
            int X;
            int Y;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct Rect32
        {
            internal int Left;
            internal int Top;
            internal int Right;
            internal int Bottom;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct WindowPlacement
        {
            internal int Length;
            internal WindowPlacementFlag Flags;
            internal ShowWindowCommand ShowCommand;
            internal Point32 MinPosition;
            internal Point32 MaxPosition;
            internal Rect32 NormalPosition;
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

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern ExecutionState SetThreadExecutionState(ExecutionState Flags);
    }
}