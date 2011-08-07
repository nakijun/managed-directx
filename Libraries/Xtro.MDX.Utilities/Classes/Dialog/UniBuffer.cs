using System;
using System.Runtime.InteropServices;

namespace Xtro.MDX.Utilities
{
    public class UniBuffer
    {
        [Flags]
        internal enum StringAnalysisFlags : uint
        {
            None = 0,
            Break = 0x0040, //SSA_BREAK       Return break flags, that is, character and word stops.
            Clip = 0x0004, //SSA_CLIP        Clip the string at iReqWidth.
            Dzwg = 0x0010, //SSA_DZWG        Provide representation glyphs for control characters.
            Fallback = 0x0020, //SSA_FALLBACK    Use fallback fonts.
            Fit = 0x0008, //SSA_FIT         Justify the string to iReqWidth.
            Glyphs = 0x0080, //SSA_GLYPHS      Generate glyphs, positions, and attributes.
            Gcp = 0x0200, //SSA_GCP         Return missing glyphs and pwLogClust with GetCharacterPlacement conventions.
            HideHotKey = 0x2000, //SSA_HIDEHOTKEY  Remove the first '&' from displayed string.
            HotKey = 0x0400, //SSA_HOTKEY      Replace '&' with underline on subsequent code point.
            HotKeyOnly = 0x2400, //SSA_HOTKEYONLY  Display underline only. [EDIT: *snip*]
            Link = 0x1000, //SSA_LINK        Apply East Asian font linking and association to noncomplex text.
            Metafile = 0x0800, //SSA_METAFILE    Write items with ExtTextOutW calls, not with glyphs.
            Password = 0x0001, //SSA_PASSWORD    Duplicate input string containing a single character tcString times.
            Rtl = 0x0100, //SSA_RTL         Use base embedding level 1.
            Tab = 0x0002, //SSA_TAB         Expand tabs.
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class ScriptControl
        {
            public uint Data;
            public int DefaultLanguage { get { return (int)((Data & 0x0000FFFF) >> 0); } }
            public bool ContextDigits { get { return (Data & 0x00010000) != 0; } }
            public bool InvertPreBoundDir { get { return (Data & 0x00020000) != 0; } }
            public bool InvertPostBoundDir { get { return (Data & 0x00040000) != 0; } }
            public bool LinkStringBefore { get { return (Data & 0x00080000) != 0; } }
            public bool LinkStringAfter { get { return (Data & 0x00100000) != 0; } }
            public bool NeutralOverride { get { return (Data & 0x00200000) != 0; } }
            public bool NumericOverride { get { return (Data & 0x00400000) != 0; } }
            public bool LegacyBidiClass { get { return (Data & 0x00800000) != 0; } }
            public bool MergeNeutralItems { get { return (Data & 0x01000000) != 0; } }
            // DWORD fReserved : 7
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class ScriptState
        {
            public ushort Data;
            public int BidiLevel { get { return (Data & 0x001F) >> 0; } }
            public bool OverrideDirection { get { return (Data & 0x0020) != 0; } }
            public bool InhibitSymSwap { get { return (Data & 0x0040) != 0; } }
            public bool CharShape { get { return (Data & 0x0080) != 0; } }
            public bool DigitSubstitute { get { return (Data & 0x0100) != 0; } }
            public bool InhibitLigate { get { return (Data & 0x0200) != 0; } }
            public bool DisplayZwg { get { return (Data & 0x0400) != 0; } }
            public bool ArabicNumContext { get { return (Data & 0x0800) != 0; } }
            public bool GcpClusters { get { return (Data & 0x1000) != 0; } }
            // WORD fReserved : 1
            // WORD fEngineReserved : 2
        }

        [StructLayout(LayoutKind.Sequential)]
        internal class ScriptDigitSubstitute
        {
            public uint Data;
            public int NationalDigitLanguage { get { return (int)((Data & 0x0000FFFF) >> 0); } }
            public int TraditionalDigitLanguage { get { return (int)((Data & 0xFFFF0000) >> 0); } }
            public int DigitSubstitute { get { return (int)((Data & 0xFF00000000) >> 0); } }
            // DWORD fReserved
        }

        string Buffer;    // Buffer to hold text

        // Uniscribe-specific
        bool AnalyseRequired;            // True if the string has changed since last analysis.
        IntPtr Analysis;  // Analysis for the current string

        [DllImport("usp10.dll")]
        static extern int ScriptStringCPtoX(IntPtr Analysis, int CharacterPosition, bool Trailing, out int X);

        [DllImport("usp10.dll")]
        static extern int ScriptStringFree(ref IntPtr Analysis);

        [DllImport("usp10.dll")]
        static extern int ScriptApplyDigitSubstitution(IntPtr ScriptDigitSubstitute, out  ScriptControl ScriptControl, out  ScriptState ScriptState);

        [DllImport("usp10.dll")]
        static extern int ScriptStringAnalyse(IntPtr Dc, string String, int StringLength, int Glyphs, int Charset, StringAnalysisFlags Flags, int ReqWidth, ref ScriptControl Control, ref ScriptState State, IntPtr Dx, IntPtr Tabdef, IntPtr InClass, IntPtr Analysis);

        [DllImport("usp10.dll")]
        static extern int ScriptStringXtoCP(IntPtr Analysis, int X, out int Ch, out int Trailing);

        public DialogResourceManager.FontNode[] FontNode;          // Font node for the font that this buffer uses

        public int GetTextSize()
        {
            return Buffer.Length;
        }

        public int CharacterPositionToX(int CharacterPosition, bool Trail, out int X)
        {
            X = 0;  // Default

            var Result = 0;
            if (AnalyseRequired) Result = Analyse();

            if (Result >= 0) Result = ScriptStringCPtoX(Analysis, CharacterPosition, Trail, out X);

            return Result;
        }

        public int Analyse()
        {
            if (Analysis != IntPtr.Zero) ScriptStringFree(ref Analysis);

            ScriptControl ScriptControl; // For uniscribe
            ScriptState ScriptState;   // For uniscribe
            ScriptApplyDigitSubstitution(IntPtr.Zero, out ScriptControl, out ScriptState);

            if (FontNode == null) return (int)Error.Fail;

            var Dc = FontNode[0].Font != null ? FontNode[0].Font.GetDC() : IntPtr.Zero;
            var Result = ScriptStringAnalyse(Dc, Buffer, Buffer.Length, Buffer.Length * 3 / 2 + 16, -1, StringAnalysisFlags.Break | StringAnalysisFlags.Glyphs | StringAnalysisFlags.Fallback | StringAnalysisFlags.Link, 0, ref ScriptControl, ref ScriptState, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, Analysis);
            if (Result >= 0) AnalyseRequired = false;  // Analysis is up-to-date

            return Result;
        }

        public int XtoCharacterPosition(int X, out int CharacterPosition, out int Trail)
        {
            CharacterPosition = 0;
            Trail = 0;  // Default

            var Result = 0;
            if (AnalyseRequired) Result = Analyse();

            if (Result >= 0) Result = ScriptStringXtoCP(Analysis, X, out CharacterPosition, out Trail);

            // If the coordinate falls outside the text region, we
            // can get character positions that don't exist.  We must
            // filter them here and convert them to those that do exist.
            if (CharacterPosition == -1 && Trail != 0)
            {
                CharacterPosition = 0;
                Trail = 0;
            }
            else if (CharacterPosition > Buffer.Length && Trail == 0)
            {
                CharacterPosition = Buffer.Length;
                Trail = 1;
            }

            return Result;
        }

        public bool RemoveChar(int Index)
        {
            if (Buffer.Length == 0 || Index < 0 || Index >= Buffer.Length) return false;  // Invalid index

            Buffer = Buffer.Remove(Index, 1);
            AnalyseRequired = true;
            return true;
        }
    }
}