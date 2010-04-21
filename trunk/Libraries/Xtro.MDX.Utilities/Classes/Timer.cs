using System.Runtime.InteropServices;

namespace Xtro.MDX.Utilities
{
    public class Timer
    {
        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        static extern bool QueryPerformanceCounter(out long PerformanceCount);
        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity()]
        static extern void QueryPerformanceFrequency(out long Frequency);

        bool TimerStopped;

        long StopTime;
        long BaseTime;
        long LastElapsedTime;

        long GetAdjustedCurrentTime()
        {
            long Time;

            if (StopTime == 0) QueryPerformanceCounter(out Time);
            else Time = StopTime;

            return Time;
        }

        public void Reset()
        {
            long Time = GetAdjustedCurrentTime();

            BaseTime = Time;
            LastElapsedTime = Time;
            StopTime = 0;
            TimerStopped = false;
        }
    }
}
