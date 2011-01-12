namespace Xtro.MDX.Utilities
{
    public struct Timer
    {
        public Callbacks.Timer CallbackTimer;
        public object CallbackUserContext;
        public float TimeoutInSecs;
        public float Countdown;
        public bool Enabled;
        public uint ID;
    }
}
