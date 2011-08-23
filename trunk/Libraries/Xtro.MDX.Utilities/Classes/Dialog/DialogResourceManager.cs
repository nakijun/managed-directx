using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using Xtro.MDX.DXGI;
using Buffer = Xtro.MDX.Direct3D10.Buffer;
using Constants = Xtro.MDX.Direct3DX10.Constants;
using D3D10Functions = Xtro.MDX.Direct3D10.Functions;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Device = Xtro.MDX.Direct3D10.Device;
using Resource = Xtro.MDX.Direct3D10.Resource;

namespace Xtro.MDX.Utilities
{
    public sealed class DialogResourceManager
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

        // ReSharper disable InconsistentNaming
        const string UI_EffectFile =
            // ReSharper restore InconsistentNaming
            "Texture2D g_Texture;" +
            "" +
            "SamplerState Sampler" +
            "{" +
            "    Filter = MIN_MAG_MIP_LINEAR;" +
            "    AddressU = Wrap;" +
            "    AddressV = Wrap;" +
            "};" +
            "" +
            "BlendState UIBlend" +
            "{" +
            "    AlphaToCoverageEnable = FALSE;" +
            "    BlendEnable[0] = TRUE;" +
            "    SrcBlend = SRC_ALPHA;" +
            "    DestBlend = INV_SRC_ALPHA;" +
            "    BlendOp = ADD;" +
            "    SrcBlendAlpha = ONE;" +
            "    DestBlendAlpha = ZERO;" +
            "    BlendOpAlpha = ADD;" +
            "    RenderTargetWriteMask[0] = 0x0F;" +
            "};" +
            "" +
            "BlendState NoBlending" +
            "{" +
            "    BlendEnable[0] = FALSE;" +
            "    RenderTargetWriteMask[0] = 0x0F;" +
            "};" +
            "" +
            "DepthStencilState DisableDepth" +
            "{" +
            "    DepthEnable = false;" +
            "};" +
            "DepthStencilState EnableDepth" +
            "{" +
            "    DepthEnable = true;" +
            "};" +
            "struct VS_OUTPUT" +
            "{" +
            "    float4 Pos : SV_POSITION;" +
            "    float4 Dif : COLOR;" +
            "    float2 Tex : TEXCOORD;" +
            "};" +
            "" +
            "VS_OUTPUT VS( float3 vPos : POSITION," +
            "              float4 Dif : COLOR," +
            "              float2 vTexCoord0 : TEXCOORD )" +
            "{" +
            "    VS_OUTPUT Output;" +
            "" +
            "    Output.Pos = float4( vPos, 1.0f );" +
            "    Output.Dif = Dif;" +
            "    Output.Tex = vTexCoord0;" +
            "" +
            "    return Output;" +
            "}" +
            "" +
            "float4 PS( VS_OUTPUT In ) : SV_Target" +
            "{" +
            "    return g_Texture.Sample( Sampler, In.Tex ) * In.Dif;" +
            "}" +
            "" +
            "float4 PSUntex( VS_OUTPUT In ) : SV_Target" +
            "{" +
            "    return In.Dif;" +
            "}" +
            "" +
            "technique10 RenderUI" +
            "{" +
            "    pass P0" +
            "    {" +
            "        SetVertexShader( CompileShader( vs_4_0, VS() ) );" +
            "        SetGeometryShader( NULL );" +
            "        SetPixelShader( CompileShader( ps_4_0, PS() ) );" +
            "        SetDepthStencilState( DisableDepth, 0 );" +
            "        SetBlendState( UIBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );" +
            "    }" +
            "}" +
            "technique10 RenderUIUntex" +
            "{" +
            "    pass P0" +
            "    {" +
            "        SetVertexShader( CompileShader( vs_4_0, VS() ) );" +
            "        SetGeometryShader( NULL );" +
            "        SetPixelShader( CompileShader( ps_4_0, PSUntex() ) );" +
            "        SetDepthStencilState( DisableDepth, 0 );" +
            "        SetBlendState( UIBlend, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );" +
            "    }" +
            "}" +
            "technique10 RestoreState" +
            "{" +
            "    pass P0" +
            "    {" +
            "        SetDepthStencilState( EnableDepth, 0 );" +
            "        SetBlendState( NoBlending, float4( 0.0f, 0.0f, 0.0f, 0.0f ), 0xFFFFFFFF );" +
            "    }" +
            "}";

        Device Device;

        readonly List<TextureNode[]> TextureCache = new List<TextureNode[]>();   // Shared textures
        readonly List<FontNode[]> FontCache = new List<FontNode[]>();         // Shared fonts

        int CreateFont(uint Font)
        {
            var FontNode = FontCache[(int)Font];

            if (FontNode[0].Font != null) FontNode[0].Font.Release();

            var Result = D3DX10Functions.CreateFont(Device, FontNode[0].Height, 0, (uint)FontNode[0].Weight, 1, false, FontCharacterSet.Default, FontPrecision.Default, FontQuality.Default, FontPitchAndFamily.Default | FontPitchAndFamily.DontCare, FontNode[0].Face, out FontNode[0].Font);

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

        public readonly List<Dialog> Dialogs = new List<Dialog>();            // Dialogs registered

        ~DialogResourceManager()
        {
            Delete();
        }

        public void Delete()
        {
            FontCache.Clear();
            TextureCache.Clear();
        }

        public int OnCreateDevice(Device Device)
        {
            this.Device = Device;

            var Encoding = new ASCIIEncoding();
            var Bytes = Encoding.GetBytes(UI_EffectFile);
            var Data = new UnmanagedMemory((uint)Bytes.Length);
            Data.Write(0, Bytes);

            // Create the UI effect object
            var Result = D3DX10Functions.CreateEffectFromMemory(Data, Data.Size, null, null, null, "fx_4_0", ShaderFlag.EnableStrictness, 0, Device, null, out Effect);
            if (Result < 0) return Result;

            TechRenderUI = Effect.GetTechniqueByName("RenderUI");
            TechRenderUI_Untex = Effect.GetTechniqueByName("RenderUIUntex");
            FxTexture = Effect.GetVariableByName("g_Texture").AsShaderResource();

            // Create the font and texture objects in the cache arrays.
            uint I;
            for (I = 0; I < FontCache.Count; I++)
            {
                Result = CreateFont(I);
                if (Result < 0) return Result;
            }

            for (I = 0; I < TextureCache.Count; I++)
            {
                Result = CreateTexture(I);
                if (Result < 0) return Result;
            }

            // Create input layout
            var Layout = new[]
            {
                new InputElementDescription{SemanticName= "POSITION",SemanticIndex=  0,Format=Format.R32G32B32_Float,InputSlot=  0,AlignedByteOffset= 0,InputSlotClass=InputClassification.InputPerVertexData,InstanceDataStepRate= 0 },
                new InputElementDescription{SemanticName= "COLOR",   SemanticIndex=  0,Format= Format.R32G32B32_Float,InputSlot= 0,AlignedByteOffset= 12, InputSlotClass=InputClassification.InputPerVertexData,InstanceDataStepRate= 0 },
                new InputElementDescription{SemanticName= "TEXCOORD",SemanticIndex=  0,Format= Format.R32G32_Float,   InputSlot= 0,AlignedByteOffset= 28, InputSlotClass=InputClassification.InputPerVertexData,InstanceDataStepRate= 0 }
            };

            PassDescription PassDescription;
            Result = TechRenderUI.GetPassByIndex(0).GetDescription(out PassDescription);
            if (Result < 0) return Result;
            Result = Device.CreateInputLayout(Layout, 3, PassDescription.IA_InputSignature, PassDescription.IA_InputSignature.Size, out InputLayout);
            if (Result < 0) return Result;

            // Create a vertex buffer quad for rendering later
            BufferDescription BufferDescription;
            BufferDescription.ByteWidth = (uint)Marshal.SizeOf(typeof(Dialog.ScreenVertex)) * 4;
            BufferDescription.Usage = Usage.Dynamic;
            BufferDescription.BindFlags = BindFlag.VertexBuffer;
            BufferDescription.CPU_AccessFlags = CPU_AccessFlag.Write;
            BufferDescription.MiscellaneousFlags = 0;
            Result = Device.CreateBuffer(ref BufferDescription, out VertexBufferScreenQuad);

            return Result < 0 ? Result : 0;
        }

        public int OnResizedSwapChain(Device Device, ref SurfaceDescription BackBufferSurfaceDescription)
        {
            BackBufferWidth = BackBufferSurfaceDescription.Width;
            BackBufferHeight = BackBufferSurfaceDescription.Height;

            var Result = D3DX10Functions.CreateSprite(Device, 500, out Sprite);
            if (Result < 0) return Result;

            StateBlockMask StateBlockMask;
            D3D10Functions.StateBlockMaskEnableAll(out StateBlockMask);
            D3D10Functions.StateBlockMaskDisableCapture(ref StateBlockMask, DeviceStateType.OutputMergerRenderTargets, 0, 1);
            Result = D3D10Functions.CreateStateBlock(Device, ref StateBlockMask, out StateBlock);

            return Result;
        }

        public void OnReleasingSwapChain()
        {
            if (Sprite != null) Sprite.Release();
            Sprite = null;

            if (StateBlock != null) StateBlock.Release();
            StateBlock = null;
        }

        public void OnDestroyDevice()
        {
            int I;

            Device = null;

            // Release the resources but don't clear the cache, as these will need to be
            // recreated if the device is recreated
            for (I = 0; I < FontCache.Count; I++)
            {
                var FontNode = FontCache[I];
                if (FontNode[0].Font != null) FontNode[0].Font.Release();
                FontNode[0].Font = null;
            }

            for (I = 0; I < TextureCache.Count; I++)
            {
                var TextureNode = TextureCache[I];
                if (TextureNode[0].TextureResourceView != null) TextureNode[0].TextureResourceView.Release();
                TextureNode[0].TextureResourceView = null;
                if (TextureNode[0].Texture != null) TextureNode[0].Texture.Release();
                TextureNode[0].Texture = null;
            }

            if (VertexBufferScreenQuad != null) VertexBufferScreenQuad.Release();
            VertexBufferScreenQuad = null;

            if (StateBlock != null) StateBlock.Release();
            StateBlock = null;

            if (Sprite != null) Sprite.Release();
            Sprite = null;

            if (InputLayout != null) InputLayout.Release();
            InputLayout = null;

            if (Effect != null) Effect.Release();
            Effect = null;
        }

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
            {
                if (D == Dialog) return true;
            }

            // Add to the list.
            Dialogs.Add(Dialog);

            // Set up next and prev pointers.
            if (Dialogs.Count > 1)
                Dialogs[Dialogs.Count - 2].SetNextDialog(Dialog);
            Dialogs[Dialogs.Count - 1].SetNextDialog(Dialogs[0]);

            return true;
        }

        public void UnregisterDialog(Dialog Dialog)
        {
            // Search for the dialog in the list.
            for (var I = 0; I < Dialogs.Count; I++)
            {
                if (Dialogs[I] == Dialog)
                {
                    Dialogs.RemoveAt(I);
                    if (Dialogs.Count > 0)
                    {
                        int L;

                        if (0 == I) L = Dialogs.Count - 1;
                        else L = I - 1;

                        var R = Dialogs.Count == I ? 0 : I;

                        Dialogs[L].SetNextDialog(Dialogs[R]);
                    }
                    return;
                }
            }
        }

        public int AddTexture(string Filename)
        {
            // See if this texture already exists
            for (var I = 0; I < TextureCache.Count; I++)
            {
                var TextureNode = TextureCache[I];
                if (TextureNode[0].FileName == Filename) return I;
            }

            // Add a new texture and try to create it
            var NewTextureNode = new TextureNode { FileName = Filename };

            TextureCache.Add(new[] { NewTextureNode });

            var Texture = TextureCache.Count - 1;

            return Texture;
        }

        public void EnableKeyboardInputForAllDialogs()
        {
            // Enable keyboard input for all registered dialogs
            foreach (var Dialog in Dialogs)
            {
                Dialog.KeyboardInput = true;
            }
        }
    }
}