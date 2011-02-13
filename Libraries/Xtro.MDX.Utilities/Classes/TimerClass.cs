using System.Runtime.InteropServices;

namespace Xtro.MDX.Utilities
{
    public sealed class TimerClass
    {
        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity]
        static extern bool QueryPerformanceCounter(out long PerformanceCount);
        [DllImport("kernel32.dll")]
        [System.Security.SuppressUnmanagedCodeSecurity]
        static extern void QueryPerformanceFrequency(out long Frequency);

        bool TimerStopped = true;
        readonly long QPFTicksPerSecond;

        long StopTime;
        long LastElapsedTime;
        long BaseTime;

        long GetAdjustedCurrentTime()
        {
            long Time;

            if (StopTime == 0) QueryPerformanceCounter(out Time);
            else Time = StopTime;

            return Time;
        }

        public TimerClass()
        {
            // Use QueryPerformanceFrequency to get the frequency of the counter
            long TicksPerSecond;
            QueryPerformanceFrequency(out TicksPerSecond);
            QPFTicksPerSecond = TicksPerSecond;
        }

        public void Reset()
        {
            var Time = GetAdjustedCurrentTime();

            BaseTime = Time;
            LastElapsedTime = Time;
            StopTime = 0;
            TimerStopped = false;
        }

        public void Start()
        {
            // Get the current time
            long Time;
            QueryPerformanceCounter(out Time);

            if (TimerStopped) BaseTime += Time - StopTime;
            StopTime = 0;
            LastElapsedTime = Time;
            TimerStopped = false;
        }

        public void Stop()
        {
            if (!TimerStopped)
            {
                long Time;
                QueryPerformanceCounter(out Time);
                StopTime = Time;
                LastElapsedTime = Time;
                TimerStopped = true;
            }
        }

        public void Advance()
        {
            StopTime += QPFTicksPerSecond / 10;
        }

        public double GetAbsoluteTime()
        {
            long Time;
            QueryPerformanceCounter(out Time);

            var Result = Time / (double)QPFTicksPerSecond;

            return Result;
        }

        public double GetTime()
        {
            var Time = GetAdjustedCurrentTime();

            var ApplicationTime = (Time - BaseTime) / (double)QPFTicksPerSecond;

            return ApplicationTime;
        }

        public float GetElapsedTime()
        {
            var Time = GetAdjustedCurrentTime();

            var ElapsedTime = (float)((Time - LastElapsedTime) / (double)QPFTicksPerSecond);
            LastElapsedTime = Time;

            // See the explanation about clamping in CDXUTTimer::GetTimeValues()
            if (ElapsedTime < 0.0f) ElapsedTime = 0.0f;

            return ElapsedTime;
        }

        public void GetTimeValues(out double Time, out double AbsoluteTime, out float ElapsedTime)
        {
            var CurrentTime = GetAdjustedCurrentTime();

            var CurrentElapsedTime = (float)((CurrentTime - LastElapsedTime) / (double)QPFTicksPerSecond);
            LastElapsedTime = CurrentTime;

            // Clamp the timer to non-negative values to ensure the timer is accurate.
            // fElapsedTime can be outside this range if processor goes into a 
            // power save mode or we somehow get shuffled to another processor.  
            // However, the main thread should call SetThreadAffinityMask to ensure that 
            // we don't get shuffled to another processor.  Other worker threads should NOT call 
            // SetThreadAffinityMask, but use a shared copy of the timer data gathered from 
            // the main thread.
            if (CurrentElapsedTime < 0.0f) CurrentElapsedTime = 0.0f;

            AbsoluteTime = CurrentTime / (double)QPFTicksPerSecond;
            Time = (CurrentTime - BaseTime) / (double)QPFTicksPerSecond;
            ElapsedTime = CurrentElapsedTime;
        }

        public bool IsStopped()
        {
            return TimerStopped;
        }
    }
}