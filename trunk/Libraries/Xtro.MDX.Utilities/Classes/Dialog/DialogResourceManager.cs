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
            public FontWeight Weight;
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

            var Result = D3DX10Functions.CreateFontW(Device, FontNode[0].Height, 0, (uint)FontNode[0].Weight, 1, false, FontCharacterSet.Default, FontPrecision.Default, FontQuality.Default, FontPitchAndFamily.Default | FontPitchAndFamily.DontCare, FontNode[0].Face, out FontNode[0].Font);

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
        // ReSharper disable InconsistentNaming
        public EffectTechnique TechRenderUI_Untex;  // Technique: RenderUI without texture
        // ReSharper restore InconsistentNaming
        public EffectShaderResourceVariable FxTexture;
        public InputLayout InputLayout;
        public Buffer VertexBufferScreenQuad;
        public StateBlock StateBlock;
        public Sprite Sprite;
        public uint BackBufferWidth;
        public uint BackBufferHeight;

        public List<Dialog> Dialogs;            // Dialogs registered

        public Device GetDevice()
        {
            return Device;
        }

        public TextureNode[] GetTextureNode(int Index)
        {
            return TextureCache[Index];
        }

        public FontNode[] GetFontNode(int Index)
        {
            return FontCache[Index];
        }

        public int AddFont(string FaceName, int Height, FontWeight Weight)
        {
            // See if this font already exists
            for (var I = 0; I < FontCache.Count; I++)
            {
                var FontNode = FontCache[I];
                if (FontNode[0].Face == FaceName && FontNode[0].Height == Height && FontNode[0].Weight == Weight) return I;
            }

            // Add a new font and try to create it
            var NewFontNode = new FontNode
            {
                Face = FaceName,
                Height = Height,
                Weight = Weight
            };
            FontCache.Add(new[] { NewFontNode });

            var Result = FontCache.Count - 1;

            // If a device is available, try to create immediately
            if (Device != null) CreateFont((uint)Result);

            return Result;
        }

        public bool RegisterDialog(Dialog Dialog)
        {
            // Check that the dialog isn't already registered.
            foreach (var D in Dialogs)
                if (D == Dialog) return true;

            // Add to the list.
            Dialogs.Add(Dialog);

            // Set up next and prev pointers.
            if (Dialogs.Count > 1)
                Dialogs[Dialogs.Count - 2].SetNextDialog(Dialog);
            Dialogs[Dialogs.Count - 1].SetNextDialog(Dialogs[0]);

            return true;
        }

        public int AddTexture(string Filename)
        {
            // See if this texture already exists
            for( var I = 0; I < TextureCache.Count; I++ )
            {
                var TextureNode = TextureCache[I];
                if( TextureNode[0].FileName== Filename )return I;
            }

            // Add a new texture and try to create it
            var NewTextureNode = new TextureNode{FileName = Filename};

            TextureCache.Add(new[]{ NewTextureNode });

            var Texture = TextureCache.Count - 1;

            return Texture;
        }

        public void EnableKeyboardInputForAllDialogs()
        {
            // Enable keyboard input for all registered dialogs
            foreach (var Dialog in Dialogs)
            {
                Dialog.EnableKeyboardInput(true);
            }
        }
    }
}