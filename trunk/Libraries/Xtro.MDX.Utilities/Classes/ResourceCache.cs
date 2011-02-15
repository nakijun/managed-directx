using System.Collections.Generic;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using Xtro.MDX.DXGI;
using Device = Xtro.MDX.Direct3D10.Device;
using Usage = Xtro.MDX.Direct3D10.Usage;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Resource = Xtro.MDX.Direct3D10.Resource;

namespace Xtro.MDX.Utilities
{
    public class ResourceCache
    {
        public struct TextureStruct
        {
            public string Source;
            public uint Width;
            public uint Height;
            public uint Depth;
            public uint MipLevels;
            public ResourceMiscFlag MiscFlags;
            public Usage Usage;
            public Format Format;
            public CPU_AccessFlag CpuAccessFlags;
            public BindFlag BindFlags;
            public ShaderResourceView ShaderResourceView;
        };

        List<TextureStruct> TextureCache = new List<TextureStruct>();

        internal static ResourceCache Singular;

        internal ResourceCache()
        {
            Singular = this;
        }

        ~ResourceCache()
        {
            OnDestroyDevice();

            TextureCache.Clear();
        }

        public int CreateTextureFromFile(Device Device, string SourceFile, ImageLoadInfo[] LoadInfo, out ShaderResourceView OutputResourceView, bool SRGB = false)
        {
            int Result;
            OutputResourceView = null;

            var ZeroInfo = new[] { ImageLoadInfo.Default };
            var SourceInfo = new[] { new ImageInfo() };

            if (LoadInfo == null || LoadInfo.Length < 1) LoadInfo = ZeroInfo;

            if (!LoadInfo[0].SourceInfo.HasValue)
            {
                D3DX10Functions.GetImageInfoFromFile(SourceFile, out SourceInfo[0]);
                LoadInfo[0].SourceInfo = SourceInfo[0];

                LoadInfo[0].Format = LoadInfo[0].SourceInfo.Value.Format;
            }

            // Search the cache for a matching entry.
            foreach (var Entry in TextureCache)
            {
                if (Entry.Source == SourceFile &&
                    Entry.Width == LoadInfo[0].Width &&
                    Entry.Height == LoadInfo[0].Height &&
                    Entry.MipLevels == LoadInfo[0].MipLevels &&
                    Entry.Usage == LoadInfo[0].Usage &&
                    Entry.Format == LoadInfo[0].Format &&
                    Entry.CpuAccessFlags == LoadInfo[0].CPU_AccessFlags &&
                    Entry.BindFlags == LoadInfo[0].BindFlags &&
                    Entry.MiscFlags == LoadInfo[0].MiscFlags)
                {
                    // A match is found. Obtain the IDirect3DTexture9 interface and return that.
                    object Object;
                    Result = Entry.ShaderResourceView.QueryInterface(typeof(ShaderResourceView), out Object);
                    OutputResourceView = (ShaderResourceView)Object;
                    return Result;
                }
            }

            //Ready a new entry to the texture cache
            //Do this before creating the texture since pLoadInfo may be volatile
            var NewEntry = new TextureStruct
            {
                Source = SourceFile,
                Width = LoadInfo[0].Width,
                Height = LoadInfo[0].Height,
                MipLevels = LoadInfo[0].MipLevels,
                Usage = LoadInfo[0].Usage,
                Format = LoadInfo[0].Format,
                CpuAccessFlags = LoadInfo[0].CPU_AccessFlags,
                BindFlags = LoadInfo[0].BindFlags,
                MiscFlags = LoadInfo[0].MiscFlags
            };

            //Create the rexture
            Resource Resource;
            Result = D3DX10Functions.CreateTextureFromFile(Device, SourceFile, ref LoadInfo[0], out Resource);
            if (Result < 0) return Result;

            var Texture = (Texture2D)Resource;

            if (SRGB)
            {
                // This is a workaround so that we can load linearly, but sample in SRGB.  Right now, we can't load
                // as linear since D3DX will try to do conversion on load.  Loading as TYPELESS doesn't work either, and
                // loading as typed _UNORM doesn't allow us to create an SRGB view.
                if (LoadInfo[0].SourceInfo.Value.ResourceDimension == ResourceDimension.Texture2D)
                {
                    Texture2D CopyTexture;
                    Texture2D_Description CopyDescription;
                    Texture.GetDescription(out CopyDescription);
                    CopyDescription.Format = Functions.MakeTypeless(CopyDescription.Format);
                    Result = Device.CreateTexture2D(ref CopyDescription, out CopyTexture);
                    if (Result < 0) return Result;

                    Device.CopyResource(CopyTexture, Texture);
                    Texture.Release();

                    Texture = CopyTexture;
                }
                else
                {
                    SRGB = false;
                }
            }

            var ShaderResourceViewDescription = new ShaderResourceViewDescription
            {
                Format = SRGB ? Functions.MakeSRGB(ZeroInfo[0].Format) : ZeroInfo[0].Format
            };
            switch (LoadInfo[0].SourceInfo.Value.ResourceDimension)
            {
            case ResourceDimension.Texture1D:
                ShaderResourceViewDescription.ViewDimension = SRV_Dimension.Texture1D;
                ShaderResourceViewDescription.Texture1D.MostDetailedMip = 0;
                ShaderResourceViewDescription.Texture1D.MipLevels = LoadInfo[0].SourceInfo.Value.MipLevels;
                break;
            case ResourceDimension.Texture2D:
                ShaderResourceViewDescription.ViewDimension = SRV_Dimension.Texture2D;
                ShaderResourceViewDescription.Texture2D.MostDetailedMip = 0;
                ShaderResourceViewDescription.Texture2D.MipLevels = LoadInfo[0].SourceInfo.Value.MipLevels;
                if ((LoadInfo[0].SourceInfo.Value.MiscFlags & ResourceMiscFlag.TextureCube) != 0)
                {
                    ShaderResourceViewDescription.ViewDimension = SRV_Dimension.TextureCube;
                    ShaderResourceViewDescription.TextureCube.MostDetailedMip = 0;
                    ShaderResourceViewDescription.TextureCube.MipLevels = LoadInfo[0].SourceInfo.Value.MipLevels;
                }
                break;
            case ResourceDimension.Texture3D:
                ShaderResourceViewDescription.ViewDimension = SRV_Dimension.Texture3D;
                ShaderResourceViewDescription.Texture3D.MostDetailedMip = 0;
                ShaderResourceViewDescription.Texture3D.MipLevels = LoadInfo[0].SourceInfo.Value.MipLevels;
                break;
            }

            Result = Device.CreateShaderResourceView(Texture, ref ShaderResourceViewDescription, out OutputResourceView);
            Texture.Release();
            if (Result < 0) return Result;

            object Object2;
            OutputResourceView.QueryInterface(typeof(ShaderResourceView), out Object2);
            NewEntry.ShaderResourceView = (ShaderResourceView)Object2;

            TextureCache.Add(NewEntry);

            return 0;
        }

        public int OnDestroyDevice()
        {
            foreach (var Texture in TextureCache)
            {
                if (Texture.ShaderResourceView != null) Texture.ShaderResourceView.Release();
            }
            TextureCache.Clear();

            return 0;
        }
    }
}