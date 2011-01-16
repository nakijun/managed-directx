namespace Xtro.MDX.Utilities
{
    sealed class Counters
    {
        // ReSharper disable InconsistentNaming
        public float GPU_Idle;
        // ReSharper restore InconsistentNaming
        public float VertexProcessing;
        public float GeometryProcessing;
        public float PixelProcessing;
        // ReSharper disable InconsistentNaming
        public float OtherGPU_Processing;
        // ReSharper restore InconsistentNaming
        public float HostAdapterBandwidthUtilization;
        public float LocalVidmemBandwidthUtilization;
        public float VertexThroughputUtilization;
        public float TriangleSetupThroughputUtilization;
        public float FillrateThroughputUtilization;
        // ReSharper disable InconsistentNaming
        public float VS_MemoryLimited;
        public float VS_ComputationLimited;
        public float GS_MemoryLimited;
        public float GS_ComputationLimited;
        public float PS_MemoryLimited;
        public float PS_ComputationLimited;
        // ReSharper restore InconsistentNaming
        public float PostTransformCacheHitRate;
        public float TextureCacheHitRate;
    };
}