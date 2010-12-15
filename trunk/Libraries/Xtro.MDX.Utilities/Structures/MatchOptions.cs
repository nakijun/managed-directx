﻿using Xtro.MDX.DXGI;
using Xtro.MDX.Direct3D10;

namespace Xtro.MDX.Utilities
{
    public struct MatchOptions
    {
        public MatchType AdapterOrdinal;
        public MatchType Output;
        public MatchType DeviceType;
        public MatchType Windowed;
        public MatchType AdapterFormat;
        public MatchType VertexProcessing;
        public MatchType Resolution;
        public MatchType BackBufferFormat;
        public MatchType BackBufferCount;
        public MatchType MultiSample;
        public MatchType SwapEffect;
        public MatchType DepthFormat;
        public MatchType StencilFormat;
        public MatchType PresentFlags;
        public MatchType RefreshRate;
        public MatchType PresentInterval;
    };
}