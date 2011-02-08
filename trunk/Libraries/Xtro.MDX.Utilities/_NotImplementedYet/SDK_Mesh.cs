using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Xtro.MDX.DXGI;
using Xtro.MDX.Generic;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using Buffer = Xtro.MDX.Direct3D10.Buffer;
using D3D10X_Mesh = Xtro.MDX.Direct3DX10.Mesh;
using Device = Xtro.MDX.Direct3D10.Device;
using Usage = Xtro.MDX.Direct3D10.Usage;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
#if WIN64
using SizeT = System.Int32;
#else
using SizeT = System.Int64;
#endif

namespace Xtro.MDX.Utilities
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AnimationFrameData
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = SDK_Mesh.MaxFrameName)]
        public byte[] FrameName;
        public ulong DataOffset;
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct AnimationData
    {
        Vector3 Translation;
        Vector4 Orientation;
        Vector3 Scaling;
    };

    public struct AnimationDataPair
    {
        AnimationData[] AnimationData;
    }

    public class SDK_Mesh
    {
        public enum IndexType
        {
            // ReSharper disable InconsistentNaming
            x16 = 0,
            x32,
            // ReSharper restore InconsistentNaming
        };

        public enum PrimitiveType
        {
            TriangleList = 0,
            TriangleStrip,
            LineList,
            LineStrip,
            PointList,
            TriangleListAdjacency,
            TriangleStripAdjacency,
            LineListAdjacency,
            LineStripAdjacency
        };

        public const int FileVersion = 101;
        public const int MaxVertexElements = 32;
        public const int MaxVertexStreams = 16;
        public const int MaxFrameName = 100;
        public const int MaxMeshName = 100;
        public const int MaxSubsetName = 100;
        public const int MaxMaterialName = 100;
        public const int MaxTextureName = 20;
        public const int MaxMaterialPath = 20;
        public const uint InvalidFrame = uint.MaxValue;
        public const uint InvalidMesh = uint.MaxValue;
        public const uint InvalidMaterial = uint.MaxValue;
        public const uint InvalidSubset = uint.MaxValue;
        public const uint InvalidAnimationData = uint.MaxValue;
        public const int ErrorResourceValue = 1;
        public const uint InvalidSamplerSlot = uint.MaxValue;

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Header
        {
            //Basic Info and sizes
            public uint Version;
            public byte IsBigEndian;
            public ulong HeaderSize;
            public ulong NonBufferDataSize;
            public ulong BufferDataSize;

            //Stats
            public uint NumberOfVertexBuffers;
            public uint NumberOfIndexBuffers;
            public uint NumberOfMeshes;
            public uint NumberOfTotalSubsets;
            public uint NumberOfFrames;
            public uint NumberOfMaterials;

            //Offsets to Data
            public ulong VertexStreamHeadersOffset;
            public ulong IndexStreamHeadersOffset;
            public ulong MeshDataOffset;
            public ulong SubsetDataOffset;
            public ulong FrameDataOffset;
            public ulong MaterialDataOffset;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct VertexBufferHeader
        {
            public ulong NumberOfVertices;
            public ulong SizeBytes;
            public ulong StrideBytes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxVertexElements)]
            public VertexElement[] Decl;
            public ulong DataOffset;
        };

        public struct VertexBufferHeaderPair
        {
            public Buffer VertexBuffer;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct IndexBufferHeader
        {
            public ulong NumberOfIndices;
            public ulong SizeBytes;
            public uint IndexType;
            public ulong DataOffset;
        };
        public struct IndexBufferHeaderPair
        {
            public Buffer IndexBuffer;
        }
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Mesh
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxMeshName)]
            public byte[] Name;
            public byte NumberOfVertexBuffers;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxVertexStreams)]
            public uint[] VertexBuffers;
            public uint IndexBuffer;
            public uint NumberOfSubsets;
            public uint NumberOfFrameInfluences; //aka bones

            public Vector3 BoundingBoxCenter;
            public Vector3 BoundingBoxExtents;
            public ulong SubsetOffset;
            public ulong FrameInfluenceOffset;
        };

        public struct MeshPair
        {
            public UnmanagedMemory<uint> Subsets;
            public UnmanagedMemory<uint> FrameInfluences;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Subset
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxSubsetName)]
            public byte[] Name;
            public uint MaterialID;
            public uint PrimitiveType;
            public ulong IndexStart;
            public ulong IndexCount;
            public ulong VertexStart;
            public ulong VertexCount;
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Frame
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxFrameName)]
            public byte[] Name;
            public uint Mesh;
            public uint ParentFrame;
            public uint ChildFrame;
            public uint SiblingFrame;
            public Matrix Matrix;
            public uint AnimationDataIndex;		//Used to index which set of keyframes transforms this frame
        };

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct Material
        {
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxMaterialName)]
            public byte[] Name;

            // Use MaterialInstancePath
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxMaterialPath)]
            public byte[] MaterialInstancePath;

            // Or fall back to d3d8-type materials
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxTextureName)]
            public byte[] DiffuseTexture;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxTextureName)]
            public byte[] NormalTexture;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxTextureName)]
            public byte[] SpecularTexture;

            public Vector4 Diffuse;
            public Vector4 Ambient;
            public Vector4 Specular;
            public Vector4 Emissive;
            public float Power;

            public ulong Force64_1;			//Force the union to 64bits
            public ulong Force64_2;			//Force the union to 64bits
            public ulong Force64_3;			//Force the union to 64bits
            public ulong Force64_4;			//Force the union to 64bits
            public ulong Force64_5;		    //Force the union to 64bits
            public ulong Force64_6;			//Force the union to 64bits
        };
        public struct MaterialPair
        {
            public Texture2D DiffuseTexture;
            public Texture2D NormalTexture;
            public Texture2D SpecularTexture;
            public ShaderResourceView DiffuseResourceView;
            public ShaderResourceView NormalResourceView;
            public ShaderResourceView SpecularResourceView;
        };
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        public struct AnimationFileHeader
        {
            public uint Version;
            public byte IsBigEndian;
            public uint FrameTransformType;
            public uint NumFrames;
            public uint NumAnimationKeys;
            public uint AnimationFPS;
            public ulong AnimationDataSize;
            public ulong AnimationDataOffset;
        };

        public struct CallbacksStruct
        {
            public Callbacks.CreateTextureFromFile CreateTextureFromFile;
            public Callbacks.CreateVertexBuffer CreateVertexBuffer;
            public Callbacks.CreateIndexBuffer CreateIndexBuffer;
            public object Context;
        };

        uint NumberOfOutstandingResources;
        bool Loading;
        FileStream File;
        IntPtr FileMappingObject;
        List<UnmanagedMemory> MappedPointers;
        Device Device;

        //These are the pointers to the two chunks of data loaded in from the mesh file
        UnmanagedMemory StaticMeshData;
        UnmanagedMemory HeapData;
        UnmanagedMemory AnimationData;
        List<UnmanagedMemory> Vertices;
        VertexBufferHeaderPair[] VerticesPair;
        List<UnmanagedMemory> Indices;
        IndexBufferHeaderPair[] IndicesPair;

        //Keep track of the path
        string Path;

        //General mesh info
        UnmanagedMemory<Header> MeshHeader;
        UnmanagedMemory<VertexBufferHeader> VertexBufferArray;
        UnmanagedMemory<IndexBufferHeader> IndexBufferArray;
        IndexBufferHeaderPair[] IndexBufferPairArray;
        UnmanagedMemory<Mesh> MeshArray;
        MeshPair[] MeshPairArray;
        UnmanagedMemory<Subset> SubsetArray;
        UnmanagedMemory<Frame> FrameArray;
        UnmanagedMemory<Material> MaterialArray;
        MaterialPair[] MaterialPairArray;

        // Adjacency information (not part of the m_pStaticMeshData, so it must be created and destroyed separately )
        UnmanagedMemory<IndexBufferHeader> AdjacencyIndexBufferArray;
        IndexBufferHeaderPair[] AdjacencyIndexBufferPairArray;

        //Animation (TODO: Add ability to load/track multiple animation sets)
        AnimationFileHeader[] AnimationHeader;
        AnimationFrameData[] AnimationFrameData;
        Matrix[] BindPoseFrameMatrices;
        Matrix[] TransformedFrameMatrices;

        void LoadMaterials(Device Device, UnmanagedMemory<Material> Materials, MaterialPair[] MaterialPairs, uint NumberOfMaterials, CallbacksStruct[] LoaderCallbacks = null)
        {
            string TexturePath;

            var Material = new Material();

            if (LoaderCallbacks != null && LoaderCallbacks.Length > 0 && LoaderCallbacks[0].CreateTextureFromFile != null)
            {
                for (uint M = 0; M < NumberOfMaterials; M++)
                {
                    MaterialPairs[M].DiffuseTexture = null;
                    MaterialPairs[M].NormalTexture = null;
                    MaterialPairs[M].SpecularTexture = null;
                    MaterialPairs[M].DiffuseResourceView = null;
                    MaterialPairs[M].NormalResourceView = null;
                    MaterialPairs[M].SpecularResourceView = null;

                    // load textures
                    Materials.Get(M, out Material);

                    if (Material.DiffuseTexture[0] != 0) LoaderCallbacks[0].CreateTextureFromFile(Device, Encoding.Default.GetString(Material.DiffuseTexture), out MaterialPairs[M].DiffuseResourceView, LoaderCallbacks[0].Context);
                    if (Material.NormalTexture[0] != 0) LoaderCallbacks[0].CreateTextureFromFile(Device, Encoding.Default.GetString(Material.NormalTexture), out MaterialPairs[M].NormalResourceView, LoaderCallbacks[0].Context);
                    if (Material.SpecularTexture[0] != 0) LoaderCallbacks[0].CreateTextureFromFile(Device, Encoding.Default.GetString(Material.SpecularTexture), out MaterialPairs[M].SpecularResourceView, LoaderCallbacks[0].Context);
                }
            }
            else
            {
                for (uint M = 0; M < NumberOfMaterials; M++)
                {
                    MaterialPairs[M].DiffuseTexture = null;
                    MaterialPairs[M].NormalTexture = null;
                    MaterialPairs[M].SpecularTexture = null;
                    MaterialPairs[M].DiffuseResourceView = null;
                    MaterialPairs[M].NormalResourceView = null;
                    MaterialPairs[M].SpecularResourceView = null;

                    // load textures
                    Materials.Get(M, out Material);

                    if (Material.DiffuseTexture[0] != 0)
                    {
                        TexturePath = Path + Material.DiffuseTexture;
                        if (Functions.GetGlobalResourceCache().CreateTextureFromFile(Device, TexturePath, null, out MaterialPairs[M].DiffuseResourceView, true) < 0) MaterialPairs[M].DiffuseResourceView = null;// ERROR_RESOURCE_VALUE;
                    }
                    if (Material.NormalTexture[0] != 0)
                    {
                        TexturePath = Path + Material.NormalTexture;
                        if (Functions.GetGlobalResourceCache().CreateTextureFromFile(Device, TexturePath, null, out MaterialPairs[M].NormalResourceView, true) < 0) MaterialPairs[M].NormalResourceView = null;// ERROR_RESOURCE_VALUE;
                    }
                    if (Material.SpecularTexture[0] != 0)
                    {
                        TexturePath = Path + Material.SpecularTexture;
                        if (Functions.GetGlobalResourceCache().CreateTextureFromFile(Device, TexturePath, null, out MaterialPairs[M].SpecularResourceView, true) < 0) MaterialPairs[M].SpecularResourceView = null;// ERROR_RESOURCE_VALUE;
                    }
                }
            }
        }

        int CreateVertexBuffer(Device Device, ref VertexBufferHeader Header, ref VertexBufferHeaderPair HeaderPair, UnmanagedMemory Vertices, CallbacksStruct[] LoaderCallbacks = null)
        {
            var Result = 0;
            Header.DataOffset = 0;
            //Vertex Buffer
            BufferDescription BufferDescription;
            BufferDescription.ByteWidth = (uint)Header.SizeBytes;
            BufferDescription.Usage = Usage.Default;
            BufferDescription.BindFlags = BindFlag.VertexBuffer;
            BufferDescription.CPU_AccessFlags = 0;
            BufferDescription.MiscFlags = 0;

            if (LoaderCallbacks != null && LoaderCallbacks.Length > 0 && LoaderCallbacks[0].CreateVertexBuffer != null) LoaderCallbacks[0].CreateVertexBuffer(Device, out HeaderPair.VertexBuffer, BufferDescription, Vertices, LoaderCallbacks[0].Context);
            else
            {
                var InitData = new SubResourceData
                {
                    SystemMemory = Vertices
                };
                Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out HeaderPair.VertexBuffer);
            }

            return Result;
        }

        int CreateIndexBuffer(Device Device, ref IndexBufferHeader Header, ref IndexBufferHeaderPair HeaderPair, UnmanagedMemory Indices, CallbacksStruct[] LoaderCallbacks = null)
        {
            var Result = 0;
            Header.DataOffset = 0;
            //Index Buffer
            BufferDescription BufferDescription;
            BufferDescription.ByteWidth = (uint)Header.SizeBytes;
            BufferDescription.Usage = Usage.Default;
            BufferDescription.BindFlags = BindFlag.IndexBuffer;
            BufferDescription.CPU_AccessFlags = 0;
            BufferDescription.MiscFlags = 0;

            if (LoaderCallbacks != null && LoaderCallbacks.Length > 0 && LoaderCallbacks[0].CreateVertexBuffer != null) LoaderCallbacks[0].CreateIndexBuffer(Device, out HeaderPair.IndexBuffer, BufferDescription, Indices, LoaderCallbacks[0].Context);
            else
            {
                var InitData = new SubResourceData
                {
                    SystemMemory = Indices
                };
                Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out HeaderPair.IndexBuffer);
            }

            return Result;
        }

        int CreateFromFile(Device Device, string FileName, bool CreateAdjacencyIndices, CallbacksStruct[] LoaderCallbacks = null)
        {
            // Find the path for the file
            var Result = Functions.FindSDK_MediaFileCch(out Path, FileName);
            if (Result < 0) return Result;

            // Open the file
            try { File = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch { return (int)Error.MediaNotFound; }

            // Change the path to just the directory
            var LastSlashIndex = Path.LastIndexOf('\\');
            Path = LastSlashIndex > -1 ? Path.Substring(0, LastSlashIndex + 1) : "";

            // Get the file size
            var FileSize = (uint)File.Length;

            // Allocate memory
            try { StaticMeshData = new UnmanagedMemory(FileSize); }
            catch
            {
                File.Close();
                return (int)Error.OutOfMemory;
            }

            // Read in the file
            try { File.CopyTo(StaticMeshData.GetStream()); }
            catch { Result = (int)Error.Fail; }

            File.Close();

            if (Result >= 0)
            {
                Result = CreateFromMemory(Device, StaticMeshData, StaticMeshData.Size, CreateAdjacencyIndices, false, LoaderCallbacks);
                if (Result < 0) StaticMeshData.Dispose();
            }

            return Result;
        }

        int CreateFromMemory(Device Device, UnmanagedMemory Data, uint DataBytes, bool CreateAdjacencyIndices, bool CopyStatic, CallbacksStruct[] LoaderCallbacks)
        {
            var Result = (int)Error.Fail;
            Vector3 Lower;
            Vector3 Upper;
            this.Device = Device;

            // Set outstanding resources to zero
            NumberOfOutstandingResources = 0;

            if (CopyStatic)
            {
                var Header = new Header();
                Data.Get(0, out Header);

                var StaticSize = Header.HeaderSize + Header.NonBufferDataSize;
                try { HeapData = new UnmanagedMemory((uint)StaticSize); }
                catch { return Result; }

                StaticMeshData = HeapData;

                var DataStream = Data.GetStream();
                DataStream.Seek(0, SeekOrigin.Begin);
                DataStream.CopyTo(StaticMeshData.GetStream());
            }
            else
            {
                HeapData = Data;
                StaticMeshData = Data;
            }

            // Pointer fixup
            MeshHeader = new UnmanagedMemory<Header>(StaticMeshData.Pointer, (uint)Marshal.SizeOf(typeof(Header)));
            var MeshHeaderData = new Header();
            MeshHeader.Get(out MeshHeaderData);
            VertexBufferArray = new UnmanagedMemory<VertexBufferHeader>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.VertexStreamHeadersOffset), MeshHeaderData.NumberOfVertexBuffers * (uint)Marshal.SizeOf(typeof(VertexBufferHeader)));
            IndexBufferArray = new UnmanagedMemory<IndexBufferHeader>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.IndexStreamHeadersOffset), MeshHeaderData.NumberOfIndexBuffers * (uint)Marshal.SizeOf(typeof(IndexBufferHeader)));
            MeshArray = new UnmanagedMemory<Mesh>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.MeshDataOffset), MeshHeaderData.NumberOfMeshes * (uint)Marshal.SizeOf(typeof(Mesh)));
            MeshPairArray = new MeshPair[MeshHeaderData.NumberOfMeshes];
            SubsetArray = new UnmanagedMemory<Subset>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.SubsetDataOffset), MeshHeaderData.NumberOfTotalSubsets * (uint)Marshal.SizeOf(typeof(Subset)));
            FrameArray = new UnmanagedMemory<Frame>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.FrameDataOffset), MeshHeaderData.NumberOfFrames * (uint)Marshal.SizeOf(typeof(Frame)));
            MaterialArray = new UnmanagedMemory<Material>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.MaterialDataOffset), MeshHeaderData.NumberOfMaterials * (uint)Marshal.SizeOf(typeof(Material)));
            MaterialPairArray = new MaterialPair[MeshHeaderData.NumberOfMaterials];

            // Setup subsets
            var Mesh = new Mesh();
            for (uint I = 0; I < MeshHeaderData.NumberOfMeshes; I++)
            {
                MeshArray.Get(I, out Mesh);
                MeshPairArray[I].Subsets = new UnmanagedMemory<uint>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)Mesh.SubsetOffset), Mesh.NumberOfSubsets * sizeof(int));
                MeshPairArray[I].FrameInfluences = new UnmanagedMemory<uint>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)Mesh.FrameInfluenceOffset), Mesh.NumberOfFrameInfluences * sizeof(int));
            }

            // error condition
            if (MeshHeaderData.Version != FileVersion)
            {
                Result = (int)Error.NoInterface;
                goto Error;
            }

            // Setup buffer data pointer
            var BufferData = new UnmanagedMemory(new IntPtr(Data.Pointer.ToInt64() + (long)(MeshHeaderData.HeaderSize + MeshHeaderData.NonBufferDataSize)), (uint)MeshHeaderData.BufferDataSize);

            // Get the start of the buffer data
            var BufferDataStart = MeshHeaderData.HeaderSize + MeshHeaderData.NonBufferDataSize;

            // Create Adjacency Indices
            if (Device != null && CreateAdjacencyIndices) this.CreateAdjacencyIndices(Device, 0.001f, Data);

            // Create VBs
            Vertices = new List<UnmanagedMemory>((int)MeshHeaderData.NumberOfVertexBuffers);
            VerticesPair = new VertexBufferHeaderPair[MeshHeaderData.NumberOfVertexBuffers];
            var VertexBufferHeaderPair = new VertexBufferHeaderPair();
            for (var I = 0; I < MeshHeaderData.NumberOfVertexBuffers; I++)
            {
                VertexBufferHeader VertexBufferHeader;
                VertexBufferArray.Get((uint)I, out VertexBufferHeader);
                Vertices[I] = new UnmanagedMemory(new IntPtr(BufferData.Pointer.ToInt64() + (long)(VertexBufferHeader.DataOffset - BufferDataStart)), (uint)VertexBufferHeader.SizeBytes);

                if (Device != null) CreateVertexBuffer(Device, ref VertexBufferHeader, ref VertexBufferHeaderPair, Vertices[I], LoaderCallbacks);

                VerticesPair[I] = VertexBufferHeaderPair;
            }

            // Create IBs
            Indices = new List<UnmanagedMemory>((int)MeshHeaderData.NumberOfIndexBuffers);
            IndicesPair = new IndexBufferHeaderPair[(int)MeshHeaderData.NumberOfIndexBuffers];
            var IndexBufferHeaderPair = new IndexBufferHeaderPair();
            for (var I = 0; I < MeshHeaderData.NumberOfIndexBuffers; I++)
            {
                IndexBufferHeader IndexBufferHeader;
                IndexBufferArray.Get((uint)I, out IndexBufferHeader);
                Indices[I] = new UnmanagedMemory(new IntPtr(BufferData.Pointer.ToInt64() + (long)(IndexBufferHeader.DataOffset - BufferDataStart)), (uint)IndexBufferHeader.SizeBytes);

                if (Device != null) CreateIndexBuffer(Device, ref IndexBufferHeader, ref IndexBufferHeaderPair, Indices[I], LoaderCallbacks);

                IndicesPair[I] = IndexBufferHeaderPair;
            }

            // Load Materials
            if (Device != null) LoadMaterials(Device, MaterialArray, MaterialPairArray, MeshHeaderData.NumberOfMaterials, LoaderCallbacks);

            // Create a place to store our bind pose frame matrices
            try { BindPoseFrameMatrices = new Matrix[MeshHeaderData.NumberOfFrames]; }
            catch { goto Error; }

            // Create a place to store our transformed frame matrices
            try { TransformedFrameMatrices = new Matrix[MeshHeaderData.NumberOfFrames]; }
            catch { goto Error; }

            Result = 0;

            // update bounding volume 
            Mesh CurrentMesh;
            MeshArray.Get(out CurrentMesh);
            for (uint I = 0; I < MeshHeaderData.NumberOfMeshes; I++)
            {
                Lower.X = float.MaxValue; Lower.Y = float.MaxValue; Lower.Z = float.MaxValue;
                Upper.X = -float.MaxValue; Upper.Y = -float.MaxValue; Upper.Z = -float.MaxValue;
                MeshArray.Get(I, out CurrentMesh);
                IndexBufferHeader CurrentMeshHeader;
                IndexBufferArray.Get(CurrentMesh.IndexBuffer, out CurrentMeshHeader);
                var Indsize = CurrentMeshHeader.IndexType == (uint)IndexType.x16 ? 2 : 4;

                for (uint A = 0; A < CurrentMesh.NumberOfSubsets; A++)
                {
                    Subset Subset;
                    MeshPairArray[I].Subsets.Get(A, out Subset);

                    var PrimitiveType = GetPrimitiveType((PrimitiveType)Subset.PrimitiveType);

                    var IndexCount = (uint)Subset.IndexCount;
                    var IndexStart = (uint)Subset.IndexStart;

                    /*if( bAdjacent )
                    {
                        IndexCount *= 2;
                        IndexStart *= 2;
                    }*/

                    //BYTE* pIndices = NULL;
                    //m_ppIndices[i]
                    var IndicesMemory = Indices[(int)CurrentMesh.IndexBuffer];
                    var VerticesMemory = Vertices[(int)CurrentMesh.VertexBuffers[0]];
                    VertexBufferHeader VertexBufferHeader;
                    VertexBufferArray.Get(CurrentMesh.VertexBuffers[0], out VertexBufferHeader);
                    var Stride = (uint)VertexBufferHeader.StrideBytes;
                    Stride /= 4;
                    for (var VertexIndex = IndexStart; VertexIndex < IndexStart + IndexCount; VertexIndex++)
                    { //TODO: test 16 bit and 32 bit
                        uint CurrentIndex;
                        if (Indsize == 2)
                        {
                            var IndexDiv2 = VertexIndex / 2;
                            IndicesMemory.Get(IndexDiv2, out CurrentIndex);
                            if (VertexIndex % 2 == 0)
                            {
                                CurrentIndex = CurrentIndex << 16;
                                CurrentIndex = CurrentIndex >> 16;
                            }
                            else CurrentIndex = CurrentIndex >> 16;
                        }
                        else IndicesMemory.Get(VertexIndex, out CurrentIndex);
                        Vector3 Pt;
                        VerticesMemory.Get(Stride * CurrentIndex, out Pt);
                        if (Pt.X < Lower.X) Lower.X = Pt.X;
                        if (Pt.Y < Lower.Y) Lower.Y = Pt.Y;
                        if (Pt.Z < Lower.Z) Lower.Z = Pt.Z;
                        if (Pt.X > Upper.X) Upper.X = Pt.X;
                        if (Pt.Y > Upper.Y) Upper.Y = Pt.Y;
                        if (Pt.Z > Upper.Z) Upper.Z = Pt.Z;
                        //BYTE** m_ppVertices;
                        //BYTE** m_ppIndices;
                    }
                    //pd3dDeviceContext->DrawIndexed( IndexCount, IndexStart, VertexStart );
                }

                var Half = Upper - Lower;
                Half *= 0.5f;

                CurrentMesh.BoundingBoxCenter = Lower + Half;
                CurrentMesh.BoundingBoxExtents = Half;

            }
        // Update 

        Error:

            if (LoaderCallbacks == null) CheckLoadDone();

            return Result;
        }

        public ulong GetNumberOfVertices(uint Mesh, uint VertexBuffer)
        {
            var MeshData = new Mesh();
            MeshArray.Get(Mesh, out MeshData);

            var VertexBufferHeader = new VertexBufferHeader();
            VertexBufferArray.Get(MeshData.VertexBuffers[VertexBuffer], out VertexBufferHeader);

            return VertexBufferHeader.NumberOfVertices;
        }

        public ulong GetNumberOfIndices(uint Mesh)
        {
            var MeshData = new Mesh();
            MeshArray.Get(Mesh, out MeshData);

            var IndexBufferHeader = new IndexBufferHeader();
            IndexBufferArray.Get(MeshData.IndexBuffer, out IndexBufferHeader);

            return IndexBufferHeader.NumberOfIndices;
        }

        public Format GetIndexBufferFormat(uint Mesh)
        {
            var MeshData = new Mesh();
            MeshArray.Get(Mesh, out MeshData);

            var IndexBufferHeader = new IndexBufferHeader();
            IndexBufferArray.Get(MeshData.IndexBuffer, out IndexBufferHeader);

            switch ((IndexType)IndexBufferHeader.IndexType)
            {
            case IndexType.x16:
                return Format.R16_UInt;
            case IndexType.x32:
                return Format.R32_UInt;
            };
            return Format.R16_UInt;
        }

        //--------------------------------------------------------------------------------------
        // Generate an adjacency index buffer for each mesh
        //--------------------------------------------------------------------------------------
        public int CreateAdjacencyIndices(Device Device, float Epsilon, UnmanagedMemory BufferData)
        {
            var Result = 0;

            var MeshHeaderData = new Header();
            MeshHeader.Get(out MeshHeaderData);
            try
            {
                AdjacencyIndexBufferArray = new UnmanagedMemory<IndexBufferHeader>(MeshHeaderData.NumberOfIndexBuffers);
                AdjacencyIndexBufferPairArray = new IndexBufferHeaderPair[MeshHeaderData.NumberOfIndexBuffers];
            }
            catch { return (int)Error.OutOfMemory; }

            var Mesh = new Mesh();
            var VertexBufferHeader = new VertexBufferHeader();
            var IndexBufferHeader = new IndexBufferHeader();
            for (uint I = 0; I < MeshHeaderData.NumberOfMeshes; I++)
            {
                MeshArray.Get(I, out Mesh);
                // ReSharper disable InconsistentNaming
                var VB_Index = Mesh.VertexBuffers[0];
                var IB_Index = Mesh.IndexBuffer;
                // ReSharper restore InconsistentNaming

                VertexBufferArray.Get(VB_Index, out VertexBufferHeader);
                IndexBufferArray.Get(IB_Index, out IndexBufferHeader);
                var VertexData = new UnmanagedMemory(new IntPtr(BufferData.Pointer.ToInt64() + (long)VertexBufferHeader.DataOffset), (uint)VertexBufferHeader.SizeBytes);
                var IndexData = new UnmanagedMemory(new IntPtr(BufferData.Pointer.ToInt64() + (long)IndexBufferHeader.DataOffset), (uint)IndexBufferHeader.SizeBytes);

                var Stride = VertexBufferHeader.StrideBytes;

                var Layout = new[]{
                new InputElementDescription{SemanticName= "POSITION", Format=Format.R32G32B32_Float,InputSlotClass=InputClassification.InputPerVertexData },
                new InputElementDescription{SemanticName= "END",Format=Format.R8_UInt, InputSlotClass=InputClassification.InputPerVertexData },
            };

                Layout[1].AlignedByteOffset = (uint)(Stride - 1);

                // create the mesh 
                var NumberOfVertices = (uint)GetNumberOfVertices(I, 0);
                var NumberOfIndices = (uint)GetNumberOfIndices(I);
                MeshFlag Options = 0;
                // ReSharper disable InconsistentNaming
                D3D10X_Mesh D3D10X_Mesh;
                // ReSharper restore InconsistentNaming

                if (GetIndexBufferFormat(I) == Format.R32_UInt) Options |= MeshFlag.x32;
                Result = D3DX10Functions.CreateMesh(Device, Layout, 2, Layout[0].SemanticName, NumberOfVertices, NumberOfIndices / 3, Options, out D3D10X_Mesh);
                if (Result < 0) return Result;

                //set the VB
                D3D10X_Mesh.SetVertexData(0, VertexData);

                //set the IB
                D3D10X_Mesh.SetIndexData(IndexData, NumberOfIndices);

                //generate adjacency
                D3D10X_Mesh.GenerateAdjacencyAndPointReps(Epsilon);

                //generate adjacency indices
                D3D10X_Mesh.GenerateGeometyShaderAdjacency();

                //get the adjacency data out of the mesh
                MeshBuffer IndexBuffer;
                UnmanagedMemory AdjacencyIndices;
                Result = D3D10X_Mesh.GetIndexBuffer(out IndexBuffer);
                if (Result < 0) return Result;
                Result = IndexBuffer.Map(out AdjacencyIndices);
                if (Result < 0) return Result;

                //Copy info about the original IB with a few modifications
                IndexBufferHeader.SizeBytes *= 2;
                AdjacencyIndexBufferArray.Set(ref IndexBufferHeader);

                //create a new adjacency IB
                BufferDescription BufferDescription;
                BufferDescription.ByteWidth = AdjacencyIndices.Size;
                BufferDescription.Usage = Usage.Immutable;
                BufferDescription.BindFlags = BindFlag.IndexBuffer;
                BufferDescription.CPU_AccessFlags = 0;
                BufferDescription.MiscFlags = 0;

                var InitData = new SubResourceData { SystemMemory = AdjacencyIndices };
                Result = Device.CreateBuffer(ref BufferDescription, ref InitData, out AdjacencyIndexBufferPairArray[IB_Index].IndexBuffer);
                if (Result < 0) return Result;

                //cleanup
                IndexBuffer.Unmap();
                IndexBuffer.Release();

                //release the ID3DX10Mesh
                D3D10X_Mesh.Release();
            }

            return Result;
        }

        public UnmanagedMemory<Mesh> GetMesh(uint Mesh)
        {
            var Size = Marshal.SizeOf(typeof(Mesh));
            return new UnmanagedMemory<Mesh>(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), (uint)Size);
        }

        public uint GetNumberOfSubsets(uint Mesh)
        {
            Mesh MeshHeader;
            MeshArray.Get(Mesh, out MeshHeader);
            return MeshHeader.NumberOfSubsets;
        }

        public UnmanagedMemory<Subset> GetSubset(uint Mesh, uint Subset)
        {
            var Size = Marshal.SizeOf(typeof(Subset));
            return new UnmanagedMemory<Subset>(new IntPtr(MeshPairArray[Mesh].Subsets.Pointer.ToInt64() + Subset * Size), (uint)Size);
        }

        public PrimitiveTopology GetPrimitiveType(PrimitiveType PrimitiveType)
        {
            var Result = PrimitiveTopology.TriangleList;

            switch (PrimitiveType)
            {
            case PrimitiveType.TriangleList:
                Result = PrimitiveTopology.TriangleList;
                break;
            case PrimitiveType.TriangleStrip:
                Result = PrimitiveTopology.TriangleStrip;
                break;
            case PrimitiveType.LineList:
                Result = PrimitiveTopology.LineList;
                break;
            case PrimitiveType.LineStrip:
                Result = PrimitiveTopology.LineStrip;
                break;
            case PrimitiveType.PointList:
                Result = PrimitiveTopology.PointList;
                break;
            case PrimitiveType.TriangleListAdjacency:
                Result = PrimitiveTopology.TriangleListWithAdjacency;
                break;
            case PrimitiveType.TriangleStripAdjacency:
                Result = PrimitiveTopology.TriangleStripWithAdjacency;
                break;
            case PrimitiveType.LineListAdjacency:
                Result = PrimitiveTopology.LineListWithAdjacency;
                break;
            case PrimitiveType.LineStripAdjacency:
                Result = PrimitiveTopology.LineStripWithAdjacency;
                break;
            };

            return Result;
        }

        public bool CheckLoadDone()
        {
            if (0 == GetOutstandingResources())
            {
                Loading = false;
                return true;
            }

            return false;
        }

        public uint GetOutstandingResources()
        {
            uint OutstandingResources = 0;
            if (MeshHeader == null) return 1;

            Header MeshHeaderData;
            MeshHeader.Get(out MeshHeaderData);
            Material Material;
            if (Device != null)
            {
                for (uint I = 0; I < MeshHeaderData.NumberOfMaterials; I++)
                {
                    MaterialArray.Get(I, out Material);
                    if (Material.DiffuseTexture[0] != 0)
                    {
                        if (MaterialPairArray[I].DiffuseResourceView == null) OutstandingResources++;
                    }

                    if (Material.NormalTexture[0] != 0)
                    {
                        if (MaterialPairArray[I].NormalResourceView == null) OutstandingResources++;
                    }

                    if (Material.SpecularTexture[0] != 0)
                    {
                        if (MaterialPairArray[I].SpecularResourceView == null) OutstandingResources++;
                    }
                }
            }

            return OutstandingResources;
        }
    }
}