using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;

namespace Xtro.MDX.Utilities
{
    public class DialogResourceManager
    {
        public struct FontNode
        {
            public string Face;
            public int Height;
            public uint Weight;
            public Font Font;
        };

        public struct TextureNode
        {
            public string FileName;
            public uint Width;
            public uint Height;
            public Texture2D Texture;
            public ShaderResourceView TextureResourceView;
        };

        Device Device;

        List<TextureNode[]> TextureCache;   // Shared textures
        List<FontNode[]> FontCache;         // Shared fonts

        int CreateFont(uint Font)
        {
            var FontNode = FontCache[(int)Font];

            if (FontNode[0].Font != null) FontNode[0].Font.Release();

            var Result = D3DX10Functions.CreateFontW(Device, FontNode[0].Height, 0, FontNode[0].Weight, 1, false, FontCharacterSet.Default, FontPrecision.Default, FontQuality.Default, FontPitchAndFamily.Default | FontPitchAndFamily.DontCare, FontNode[0].Face, out FontNode[0].Font);

            return Result < 0 ? Result : 0;
        }
    }
}