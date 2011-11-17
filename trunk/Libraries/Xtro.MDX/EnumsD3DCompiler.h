[Flags]
public enum class DisassembleFlag
{
	EnableColorCode = D3D_DISASM_ENABLE_COLOR_CODE,
	EnableDefaultValuePrints = D3D_DISASM_ENABLE_DEFAULT_VALUE_PRINTS,
	EnableInstructionNumbering = D3D_DISASM_ENABLE_INSTRUCTION_NUMBERING,
	EnableInstructionCycle = D3D_DISASM_ENABLE_INSTRUCTION_CYCLE
};

[Flags]
public enum class StripFlag
{
    ReflectionData = D3DCOMPILER_STRIP_REFLECTION_DATA,
    DebugInfo = D3DCOMPILER_STRIP_DEBUG_INFO,
    TestBlobs = D3DCOMPILER_STRIP_TEST_BLOBS,
    ForceDWORD = D3DCOMPILER_STRIP_FORCE_DWORD
};