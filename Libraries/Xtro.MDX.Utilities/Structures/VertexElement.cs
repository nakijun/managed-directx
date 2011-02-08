namespace Xtro.MDX.Utilities
{
    public struct VertexElement
    {
        ushort Stream;     // Stream index
        ushort Offset;     // Offset in the stream in bytes
        byte Type;       // Data type
        byte Method;     // Processing method
        byte Usage;      // Semantics
        byte UsageIndex; // Semantic index
    }
}
