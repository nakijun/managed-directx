namespace Xtro.MDX.Utilities
{
    class Counters
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
        public float VSMemoryLimited;
        public float VSComputationLimited;
        public float GSMemoryLimited;
        public float GSComputationLimited;
        public float PSMemoryLimited;
        public float PSComputationLimited;
        public float PostTransformCacheHitRate;
        public float TextureCacheHitRate;
    };
}