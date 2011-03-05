using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using Buffer = Xtro.MDX.Direct3D10.Buffer;
using Constants = Xtro.MDX.Direct3DX10.Constants;
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

        int CreateTexture(uint Texture)
        {
            var TextureNode = TextureCache[(int)Texture];

            // Make sure there's a texture to create
            if (string.IsNullOrEmpty(TextureNode[0].FileName)) return 0;

            ImageInfo SourceInfo;
            D3DX10Functions.GetImageInfoFromFile(TextureNode[0].FileName, out SourceInfo);

            // Create texture from file
            Resource Resource;
            ImageLoadInfo LoadInfo;
            LoadInfo.Width = Constants.Default;
            LoadInfo.Height = Constants.Default;
            LoadInfo.Depth = Constants.Default;
            LoadInfo.FirstMipLevel = 0;
            LoadInfo.MipLevels = 1;
            LoadInfo.Usage = Usage.Default;
            LoadInfo.BindFlags = BindFlag.ShaderResource;
            LoadInfo.CPU_AccessFlags = 0;
            LoadInfo.MiscellaneousFlags = 0;
            LoadInfo.Format = Functions.MakeTypeless(SourceInfo.Format);
            LoadInfo.Filter = FilterFlag.None;
            LoadInfo.MipFilter = FilterFlag.None;
            LoadInfo.SourceInfo = SourceInfo;
            var Result = D3DX10Functions.CreateTextureFromFile(Device, TextureNode[0].FileName, ref LoadInfo, out Resource);
            if (Result < 0) return Result;

            object Object;
            Result = Resource.QueryInterface(typeof(Texture2D), out Object);
            TextureNode[0].Texture = (Texture2D)Object;
            Resource.Release();
            if (Result < 0) return Result;

            // Store dimensions
            Texture2D_Description Description;
            TextureNode[0].Texture.GetDescription(out Description);
            TextureNode[0].Width = Description.Width;
            TextureNode[0].Height = Description.Height;

            // Create resource view
            var ShaderResourceViewDescription = new ShaderResourceViewDescription
            {
                ViewDimension = ShaderResourceViewDimension.Texture2D,
                Format = Functions.MakeSRGB(Description.Format),
                Texture2D =
                {
                    MipLevels = 1,
                    MostDetailedMip = 0
                }
            };
            Result = Device.CreateShaderResourceView(TextureNode[0].Texture, ref ShaderResourceViewDescription, out TextureNode[0].TextureResourceView);

            return Result;
        }

        public Effect Effect;        // Effect used to render UI with D3D10
        public EffectTechnique TechRenderUI;  // Technique: RenderUI
        public EffectTechnique TechRenderUIUntex;  // Technique: RenderUI without texture
        public EffectShaderResourceVariable FxTexture;
        public InputLayout InputLayout;
        public Buffer VertexBufferScreenQuad;
        public StateBlock StateBlock;
        public Sprite Sprite;
        public uint BackBufferWidth;
        public uint BackBufferHeight;
        
        public List<Dialog> Dialogs;            // Dialogs registered
    }
}