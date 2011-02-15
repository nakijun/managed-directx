using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Xtro.MDX.DXGI;
using Xtro.MDX.Generic;
using Xtro.MDX.Direct3D10;
using Xtro.MDX.Direct3DX10;
using Buffer = Xtro.MDX.Direct3D10.Buffer;
using D3D10X_Mesh = Xtro.MDX.Direct3DX10.Mesh;
using Device = Xtro.MDX.Direct3D10.Device;
using Usage = Xtro.MDX.Direct3D10.Usage;
using D3DX10Functions = Xtro.MDX.Direct3DX10.Functions;
using Resource = Xtro.MDX.Direct3D10.Resource;
#if WIN64
using SizeT = System.Int32;
#else
using SizeT = System.Int64;
#endif

namespace Xtro.MDX.Utilities
{
    enum FrameTransformType
    {
        Relative = 0,
        Absolute,		//This is not currently used but is here to support absolute transformations in the future
    };

    public struct AnimationData
    {
        public Vector3 Translation;
        public Vector4 Orientation;
        public Vector3 Scaling;
    };

    public struct AnimationFrameData
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = SDK_Mesh.MaxFrameName)]
        public string FrameName;
        public ulong DataOffset;
    };

    // ReSharper disable InconsistentNaming
    public class SDK_Mesh
    // ReSharper restore InconsistentNaming
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
        public const int MaxTextureName = 260;
        public const int MaxMaterialPath = 260;
        public const uint InvalidFrame = uint.MaxValue;
        public const uint InvalidMesh = uint.MaxValue;
        public const uint InvalidAnimationData = uint.MaxValue;
        public const uint InvalidSamplerSlot = uint.MaxValue;

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

        public struct VertexBufferHeader
        {
            public ulong NumberOfVertices;
            public ulong SizeBytes;
            public ulong StrideBytes;
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxVertexElements)]
            public VertexElement[] Declaration;
            public ulong DataOffset;
        };

        public struct VertexBufferHeaderPair
        {
            public Buffer VertexBuffer;
        }

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

        public struct Mesh
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxMeshName)]
            public string Name;
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

        public struct Subset
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxSubsetName)]
            public string Name;
            public uint MaterialID;
            public uint PrimitiveType;
            public ulong IndexStart;
            public ulong IndexCount;
            public ulong VertexStart;
            public ulong VertexCount;
        };

        public struct Frame
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxFrameName)]
            public string Name;
            public uint Mesh;
            public uint ParentFrame;
            public uint ChildFrame;
            public uint SiblingFrame;
            public Matrix Matrix;
            public uint AnimationDataIndex;		//Used to index which set of keyframes transforms this frame
        };

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct Material
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxMaterialName)]
            public string Name;

            // Use MaterialInstancePath
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxMaterialPath)]
            public string MaterialInstancePath;

            // Or fall back to d3d8-type materials
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxTextureName)]
            public string DiffuseTexture;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxTextureName)]
            public string NormalTexture;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MaxTextureName)]
            public string SpecularTexture;

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
        public struct AnimationFileHeader
        {
            public uint Version;
            public byte IsBigEndian;
            public uint FrameTransformType;
            public uint NumberOfFrames;
            public uint NumberOfAnimationKeys;
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

        public bool Loading;
        FileStream File;
        Device Device;

        //These are the pointers to the two chunks of data loaded in from the mesh file
        UnmanagedMemory StaticMeshData;
        UnmanagedMemory HeapData;
        UnmanagedMemory AnimationData;
        UnmanagedMemory[] VerticesList;
        UnmanagedMemory[] IndicesList;

        //Keep track of the path
        public string Path { get; private set; }

        //General mesh info
        UnmanagedMemory<Header> MeshHeader;
        UnmanagedMemory<VertexBufferHeader> VertexBufferArray;
        VertexBufferHeaderPair[] VertexBufferPairArray;
        UnmanagedMemory<IndexBufferHeader> IndexBufferArray;
        IndexBufferHeaderPair[] IndexBufferPairArray;
        public UnmanagedMemory<Mesh> MeshArray { get; private set; }
        public MeshPair[] MeshPairArray { get; private set; }
        public UnmanagedMemory<Subset> SubsetArray;
        public UnmanagedMemory<Frame> FrameArray;
        public UnmanagedMemory<Material> MaterialArray { get; private set; }
        public MaterialPair[] MaterialPairArray { get; private set; }

        // Adjacency information (not part of the m_pStaticMeshData, so it must be created and destroyed separately )
        UnmanagedMemory<IndexBufferHeader> AdjacencyIndexBufferArray;
        IndexBufferHeaderPair[] AdjacencyIndexBufferPairArray;

        //Animation (xTODO: Add ability to load/track multiple animation sets)
        UnmanagedMemory<AnimationFileHeader> AnimationHeader;
        UnmanagedMemory<AnimationFrameData> AnimationFrameData;
        Matrix[] BindPoseFrameMatrices;
        Matrix[] TransformedFrameMatrices;

        void LoadMaterials(Device Device, UnmanagedMemory Materials, MaterialPair[] MaterialPairs, uint NumberOfMaterials, CallbacksStruct[] LoaderCallbacks = null)
        {
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

                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    var Size = Marshal.SizeOf(typeof(Material));
                    var Material = (Material)Marshal.PtrToStructure(new IntPtr(Materials.Pointer.ToInt64() + M * Size), typeof(Material));

                    if (!string.IsNullOrEmpty(Material.DiffuseTexture)) LoaderCallbacks[0].CreateTextureFromFile(Device, Material.DiffuseTexture, out MaterialPairs[M].DiffuseResourceView, LoaderCallbacks[0].Context);
                    if (!string.IsNullOrEmpty(Material.NormalTexture)) LoaderCallbacks[0].CreateTextureFromFile(Device, Material.NormalTexture, out MaterialPairs[M].NormalResourceView, LoaderCallbacks[0].Context);
                    if (!string.IsNullOrEmpty(Material.SpecularTexture)) LoaderCallbacks[0].CreateTextureFromFile(Device, Material.SpecularTexture, out MaterialPairs[M].SpecularResourceView, LoaderCallbacks[0].Context);
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

                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    var Size = Marshal.SizeOf(typeof(Material));
                    var Material = (Material)Marshal.PtrToStructure(new IntPtr(Materials.Pointer.ToInt64() + M * Size), typeof(Material));

                    string TexturePath;
                    if (!string.IsNullOrEmpty(Material.DiffuseTexture))
                    {
                        TexturePath = Path + Material.DiffuseTexture;
                        if (Functions.GetGlobalResourceCache().CreateTextureFromFile(Device, TexturePath, null, out MaterialPairs[M].DiffuseResourceView, true) < 0) MaterialPairs[M].DiffuseResourceView = null;// ERROR_RESOURCE_VALUE;
                    }
                    if (!string.IsNullOrEmpty(Material.NormalTexture))
                    {
                        TexturePath = Path + Material.NormalTexture;
                        if (Functions.GetGlobalResourceCache().CreateTextureFromFile(Device, TexturePath, null, out MaterialPairs[M].NormalResourceView, true) < 0) MaterialPairs[M].NormalResourceView = null;// ERROR_RESOURCE_VALUE;
                    }
                    if (!string.IsNullOrEmpty(Material.SpecularTexture))
                    {
                        TexturePath = Path + Material.SpecularTexture;
                        if (Functions.GetGlobalResourceCache().CreateTextureFromFile(Device, TexturePath, null, out MaterialPairs[M].SpecularResourceView, true) < 0) MaterialPairs[M].SpecularResourceView = null;// ERROR_RESOURCE_VALUE;
                    }
                }
            }
        }

        static void CreateVertexBuffer(Device Device, ref VertexBufferHeader Header, ref VertexBufferHeaderPair HeaderPair, UnmanagedMemory Vertices, CallbacksStruct[] LoaderCallbacks = null)
        {
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
                Device.CreateBuffer(ref BufferDescription, ref InitData, out HeaderPair.VertexBuffer);
            }

            return;
        }

        static void CreateIndexBuffer(Device Device, ref IndexBufferHeader Header, ref IndexBufferHeaderPair HeaderPair, UnmanagedMemory Indices, CallbacksStruct[] LoaderCallbacks = null)
        {
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
                Device.CreateBuffer(ref BufferDescription, ref InitData, out HeaderPair.IndexBuffer);
            }

            return;
        }

        int CreateFromFile(Device Device, string FileName, bool CreateAdjacencyIndices, CallbacksStruct[] LoaderCallbacks = null)
        {
            // Find the path for the file
            string DestinationPath;
            var Result = Functions.FindSDK_MediaFileCch(out DestinationPath, FileName);
            Path = DestinationPath;
            if (Result < 0) return Result;

            // Open the file
            try { File = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch { return (int)Error.MediaNotFound; }

            // Change the path to just the directory
            var LastSlashIndex = Path.LastIndexOf('\\');
            Path = LastSlashIndex > -1 ? Path.Substring(0, LastSlashIndex + 1) : "";

            // Allocate memory
            try { StaticMeshData = new UnmanagedMemory((uint)File.Length); }
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
                Result = CreateFromMemory(Device, StaticMeshData, CreateAdjacencyIndices, false, LoaderCallbacks);
                if (Result < 0) StaticMeshData.Dispose();
            }

            return Result;
        }

        int CreateFromMemory(Device Device, UnmanagedMemory Data, bool CreateAdjacencyIndices, bool CopyStatic, CallbacksStruct[] LoaderCallbacks)
        {
            var Result = (int)Error.Fail;
            Vector3 Lower;
            Vector3 Upper;
            this.Device = Device;

            if (CopyStatic)
            {
                Header Header;
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
            Header MeshHeaderData;
            MeshHeader.Get(out MeshHeaderData);
            VertexBufferArray = new UnmanagedMemory<VertexBufferHeader>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.VertexStreamHeadersOffset), MeshHeaderData.NumberOfVertexBuffers * (uint)Marshal.SizeOf(typeof(VertexBufferHeader)));
            VertexBufferPairArray = new VertexBufferHeaderPair[MeshHeaderData.NumberOfVertexBuffers];
            IndexBufferArray = new UnmanagedMemory<IndexBufferHeader>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.IndexStreamHeadersOffset), MeshHeaderData.NumberOfIndexBuffers * (uint)Marshal.SizeOf(typeof(IndexBufferHeader)));
            IndexBufferPairArray = new IndexBufferHeaderPair[MeshHeaderData.NumberOfIndexBuffers];
            MeshArray = new UnmanagedMemory<Mesh>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.MeshDataOffset), MeshHeaderData.NumberOfMeshes * (uint)Marshal.SizeOf(typeof(Mesh)));
            MeshPairArray = new MeshPair[MeshHeaderData.NumberOfMeshes];
            SubsetArray = new UnmanagedMemory<Subset>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.SubsetDataOffset), MeshHeaderData.NumberOfTotalSubsets * (uint)Marshal.SizeOf(typeof(Subset)));
            FrameArray = new UnmanagedMemory<Frame>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.FrameDataOffset), MeshHeaderData.NumberOfFrames * (uint)Marshal.SizeOf(typeof(Frame)));
            MaterialArray = new UnmanagedMemory<Material>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshHeaderData.MaterialDataOffset), MeshHeaderData.NumberOfMaterials * (uint)Marshal.SizeOf(typeof(Material)));
            MaterialPairArray = new MaterialPair[MeshHeaderData.NumberOfMaterials];

            // Setup subsets
            for (uint I = 0; I < MeshHeaderData.NumberOfMeshes; I++)
            {
                // UnmanagedMemory.Get is not working for MarshalAs structs
                var Size = Marshal.SizeOf(typeof(Mesh));
                var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + I * Size), typeof(Mesh));

                MeshPairArray[I].Subsets = new UnmanagedMemory<uint>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshData.SubsetOffset), MeshData.NumberOfSubsets * sizeof(int));
                var NumberOfFrameInfluences = MeshData.NumberOfFrameInfluences == 0 ? 1 : MeshData.NumberOfFrameInfluences;
                MeshPairArray[I].FrameInfluences = new UnmanagedMemory<uint>(new IntPtr(StaticMeshData.Pointer.ToInt64() + (long)MeshData.FrameInfluenceOffset), NumberOfFrameInfluences * sizeof(int));
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
            VerticesList = new UnmanagedMemory[MeshHeaderData.NumberOfVertexBuffers];
            for (var I = 0; I < MeshHeaderData.NumberOfVertexBuffers; I++)
            {
                // UnmanagedMemory.Get is not working for MarshalAs structs
                var Size = Marshal.SizeOf(typeof(VertexBufferHeader));
                var VertexBufferHeader = (VertexBufferHeader)Marshal.PtrToStructure(new IntPtr(VertexBufferArray.Pointer.ToInt64() + I * Size), typeof(VertexBufferHeader));

                VerticesList[I] = new UnmanagedMemory(new IntPtr(BufferData.Pointer.ToInt64() + (long)(VertexBufferHeader.DataOffset - BufferDataStart)), (uint)VertexBufferHeader.SizeBytes);

                if (Device != null) CreateVertexBuffer(Device, ref VertexBufferHeader, ref VertexBufferPairArray[I], VerticesList[I], LoaderCallbacks);
            }

            // Create IBs
            IndicesList = new UnmanagedMemory[MeshHeaderData.NumberOfIndexBuffers];
            for (var I = 0; I < MeshHeaderData.NumberOfIndexBuffers; I++)
            {
                IndexBufferHeader IndexBufferHeader;
                IndexBufferArray.Get((uint)I, out IndexBufferHeader);
                IndicesList[I] = new UnmanagedMemory(new IntPtr(BufferData.Pointer.ToInt64() + (long)(IndexBufferHeader.DataOffset - BufferDataStart)), (uint)IndexBufferHeader.SizeBytes);

                if (Device != null) CreateIndexBuffer(Device, ref IndexBufferHeader, ref IndexBufferPairArray[I], IndicesList[I], LoaderCallbacks);
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
            for (uint I = 0; I < MeshHeaderData.NumberOfMeshes; I++)
            {
                Lower.X = float.MaxValue; Lower.Y = float.MaxValue; Lower.Z = float.MaxValue;
                Upper.X = -float.MaxValue; Upper.Y = -float.MaxValue; Upper.Z = -float.MaxValue;

                // UnmanagedMemory.Get is not working for MarshalAs structs
                var Size = Marshal.SizeOf(typeof(Mesh));
                var CurrentMesh = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + I * Size), typeof(Mesh));

                IndexBufferHeader CurrentMeshHeader;
                IndexBufferArray.Get(CurrentMesh.IndexBuffer, out CurrentMeshHeader);
                var IndexSize = CurrentMeshHeader.IndexType == (uint)IndexType.x16 ? 2 : 4;

                for (uint S = 0; S < CurrentMesh.NumberOfSubsets; S++)
                {
                    uint SubsetIndex;
                    MeshPairArray[I].Subsets.Get(S, out SubsetIndex);

                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    Size = Marshal.SizeOf(typeof(Subset));
                    var Subset = (Subset)Marshal.PtrToStructure(new IntPtr(SubsetArray.Pointer.ToInt64() + SubsetIndex * Size), typeof(Subset));

                    //var PrimitiveType = GetPrimitiveType((PrimitiveType)Subset.PrimitiveType);

                    var IndexCount = (uint)Subset.IndexCount;
                    var IndexStart = (uint)Subset.IndexStart;

                    /*if( Adjacent )
                    {
                        IndexCount *= 2;
                        IndexStart *= 2;
                    }*/

                    //BYTE* pIndices = NULL;
                    //m_ppIndices[i]
                    var Indices = IndicesList[(int)CurrentMesh.IndexBuffer];
                    var Vertices = VerticesList[(int)CurrentMesh.VertexBuffers[0]];

                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    Size = Marshal.SizeOf(typeof(VertexBufferHeader));
                    var VertexBufferHeader = (VertexBufferHeader)Marshal.PtrToStructure(new IntPtr(VertexBufferArray.Pointer.ToInt64() + CurrentMesh.VertexBuffers[0] * Size), typeof(VertexBufferHeader));

                    var Stride = (uint)VertexBufferHeader.StrideBytes;
                    Stride /= 4;
                    for (var VertexIndex = IndexStart; VertexIndex < IndexStart + IndexCount; VertexIndex++)
                    { //xTODO: test 16 bit and 32 bit
                        uint CurrentIndex;
                        if (IndexSize == 2)
                        {
                            var IndexDiv2 = VertexIndex / 2;
                            Indices.Get(0, IndexDiv2, out CurrentIndex);
                            if (VertexIndex % 2 == 0)
                            {
                                CurrentIndex = CurrentIndex << 16;
                                CurrentIndex = CurrentIndex >> 16;
                            }
                            else CurrentIndex = CurrentIndex >> 16;
                        }
                        else Indices.Get(0, VertexIndex, out CurrentIndex);
                        Vector3 Pt;
                        Vertices.Get(sizeof(float) * Stride * CurrentIndex, out Pt);
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

        // transform frame using a recursive traversal
        void TransformFrame(uint Frame, ref Matrix ParentWorld, double Time)
        {
            // Get the tick data
            Matrix LocalTransform;
            var Tick = GetAnimationKeyFromTime(Time);

            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Frame));
            var FrameHeaderData = (Frame)Marshal.PtrToStructure(new IntPtr(FrameArray.Pointer.ToInt64() + Frame * Size), typeof(Frame));

            if (FrameHeaderData.AnimationDataIndex != InvalidAnimationData)
            {
                // UnmanagedMemory.Get is not working for MarshalAs structs
                Size = Marshal.SizeOf(typeof(AnimationFrameData));
                var AnimationFrame = (AnimationFrameData)Marshal.PtrToStructure(new IntPtr(AnimationFrameData.Pointer.ToInt64() + FrameHeaderData.AnimationDataIndex * Size), typeof(AnimationFrameData));

                AnimationData Data;
                AnimationData.Get((uint)AnimationFrame.DataOffset, Tick, out Data);

                // turn it into a matrix (Ignore scaling for now)
                var ParentPosition = Data.Translation;
                Matrix Translate;
                D3DX10Functions.MatrixTranslation(out Translate, ParentPosition.X, ParentPosition.Y, ParentPosition.Z);

                Quaternion Quaternion;
                Matrix QuaternionMatrix;
                Quaternion.W = Data.Orientation.W;
                Quaternion.X = Data.Orientation.X;
                Quaternion.Y = Data.Orientation.Y;
                Quaternion.Z = Data.Orientation.Z;
                if (Quaternion.W == 0 && Quaternion.X == 0 && Quaternion.Y == 0 && Quaternion.Z == 0) D3DX10Functions.QuaternionIdentity(out Quaternion);
                D3DX10Functions.QuaternionNormalize(out Quaternion, ref Quaternion);
                D3DX10Functions.MatrixRotationQuaternion(out QuaternionMatrix, ref Quaternion);
                LocalTransform = (QuaternionMatrix * Translate);
            }
            else LocalTransform = FrameHeaderData.Matrix;

            // Transform ourselves
            Matrix LocalWorld;
            D3DX10Functions.MatrixMultiply(out LocalWorld, ref LocalTransform, ref ParentWorld);
            TransformedFrameMatrices[Frame] = LocalWorld;

            // Transform our siblings
            if (FrameHeaderData.SiblingFrame != InvalidFrame) TransformFrame(FrameHeaderData.SiblingFrame, ref ParentWorld, Time);

            // Transform our children
            if (FrameHeaderData.ChildFrame != InvalidFrame) TransformFrame(FrameHeaderData.ChildFrame, ref LocalWorld, Time);
        }

        // transform frame assuming that it is an absolute transformation
        void TransformFrameAbsolute(uint Frame, double Time)
        {
            Matrix Transform1;
            Matrix Transform2;
            Matrix Rotation1;
            Matrix Rotation2;
            Quaternion Quaternion1;
            Quaternion Quaternion2;
            Matrix MatrixInvTo;
            Matrix MatrixFrom;

            var Tick = GetAnimationKeyFromTime(Time);

            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Frame));
            var FrameHeaderData = (Frame)Marshal.PtrToStructure(new IntPtr(FrameArray.Pointer.ToInt64() + Frame * Size), typeof(Frame));

            if (FrameHeaderData.AnimationDataIndex != InvalidAnimationData)
            {
                // UnmanagedMemory.Get is not working for MarshalAs structs
                Size = Marshal.SizeOf(typeof(AnimationFrameData));
                var AnimationFrame = (AnimationFrameData)Marshal.PtrToStructure(new IntPtr(AnimationFrameData.Pointer.ToInt64() + FrameHeaderData.AnimationDataIndex * Size), typeof(AnimationFrameData));

                AnimationData Data;
                AnimationData.Get((uint)AnimationFrame.DataOffset, Tick, out Data);
                AnimationData DataOriginal;
                AnimationData.Get((uint)AnimationFrame.DataOffset, out DataOriginal);

                D3DX10Functions.MatrixTranslation(out Transform1, -DataOriginal.Translation.X, -DataOriginal.Translation.Y, -DataOriginal.Translation.Z);
                D3DX10Functions.MatrixTranslation(out Transform2, Data.Translation.X, Data.Translation.Y, Data.Translation.Z);

                Quaternion1.X = DataOriginal.Orientation.X;
                Quaternion1.Y = DataOriginal.Orientation.Y;
                Quaternion1.Z = DataOriginal.Orientation.Z;
                Quaternion1.W = DataOriginal.Orientation.W;
                D3DX10Functions.QuaternionInverse(out Quaternion1, ref Quaternion1);
                D3DX10Functions.MatrixRotationQuaternion(out Rotation1, ref Quaternion1);
                MatrixInvTo = Transform1 * Rotation1;

                Quaternion2.X = Data.Orientation.X;
                Quaternion2.Y = Data.Orientation.Y;
                Quaternion2.Z = Data.Orientation.Z;
                Quaternion2.W = Data.Orientation.W;
                D3DX10Functions.MatrixRotationQuaternion(out Rotation2, ref Quaternion2);
                MatrixFrom = Rotation2 * Transform2;

                var Output = MatrixInvTo * MatrixFrom;
                TransformedFrameMatrices[Frame] = Output;
            }
        }

        void RenderMesh(uint Mesh, bool Adjacent, Device Device, EffectTechnique Technique, EffectShaderResourceVariable Diffuse, EffectShaderResourceVariable Normal, EffectShaderResourceVariable Specular, EffectVectorVariable DiffuseVector, EffectVectorVariable SpecularVector)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            var MeshDataPair = MeshPairArray[Mesh];

            var Strides = new uint[(int)IA.VertexInputResourceSlotCount];
            var Offsets = new uint[(int)IA.VertexInputResourceSlotCount];
            var VertexBuffers = new Buffer[(int)IA.VertexInputResourceSlotCount];

            if (MeshData.NumberOfVertexBuffers > (int)IA.VertexInputResourceSlotCount) return;

            for (var I = 0; I < MeshData.NumberOfVertexBuffers; I++)
            {
                // UnmanagedMemory.Get is not working for MarshalAs structs
                Size = Marshal.SizeOf(typeof(VertexBufferHeader));
                var VertexBufferHeader = (VertexBufferHeader)Marshal.PtrToStructure(new IntPtr(VertexBufferArray.Pointer.ToInt64() + MeshData.VertexBuffers[I] * Size), typeof(VertexBufferHeader));

                VertexBuffers[I] = VertexBufferPairArray[MeshData.VertexBuffers[I]].VertexBuffer;
                Strides[I] = (uint)VertexBufferHeader.StrideBytes;
                Offsets[I] = 0;
            }

            var IndexBuffers = Adjacent ? AdjacencyIndexBufferArray : IndexBufferArray;
            var IndexBufferPairs = Adjacent ? AdjacencyIndexBufferPairArray : IndexBufferPairArray;

            var IndexBuffer = IndexBufferPairs[MeshData.IndexBuffer].IndexBuffer;
            IndexBufferHeader IndexBufferHeader;
            IndexBuffers.Get(MeshData.IndexBuffer, out IndexBufferHeader);
            var IndexBufferFormat = (IndexType)IndexBufferHeader.IndexType == IndexType.x32 ? Format.R32_UInt : Format.R16_UInt;

            Device.IA_SetVertexBuffers(0, MeshData.NumberOfVertexBuffers, VertexBuffers, Strides, Offsets);
            Device.IA_SetIndexBuffer(IndexBuffer, IndexBufferFormat, 0);

            TechniqueDescription TechniqueDescription;
            Technique.GetDescription(out TechniqueDescription);

            for (uint P = 0; P < TechniqueDescription.Passes; P++)
            {
                for (uint S = 0; S < MeshData.NumberOfSubsets; S++)
                {
                    uint SubsetIndex;
                    MeshDataPair.Subsets.Get(S, out SubsetIndex);

                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    Size = Marshal.SizeOf(typeof(Subset));
                    var Subset = (Subset)Marshal.PtrToStructure(new IntPtr(SubsetArray.Pointer.ToInt64() + SubsetIndex * Size), typeof(Subset));

                    var PrimitiveType = GetPrimitiveType((PrimitiveType)Subset.PrimitiveType);
                    if (Adjacent)
                    {
                        switch (PrimitiveType)
                        {
                        case PrimitiveTopology.TriangleList:
                            PrimitiveType = PrimitiveTopology.TriangleListAdjacency;
                            break;
                        case PrimitiveTopology.TriangleStrip:
                            PrimitiveType = PrimitiveTopology.TriangleStripAdjacency;
                            break;
                        case PrimitiveTopology.LineList:
                            PrimitiveType = PrimitiveTopology.LineListAdjacency;
                            break;
                        case PrimitiveTopology.LineStrip:
                            PrimitiveType = PrimitiveTopology.LineStripAdjacency;
                            break;
                        }
                    }

                    Device.IA_SetPrimitiveTopology(PrimitiveType);

                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    Size = Marshal.SizeOf(typeof(Material));
                    var Material = (Material)Marshal.PtrToStructure(new IntPtr(MaterialArray.Pointer.ToInt64() + Subset.MaterialID * Size), typeof(Material));

                    var MaterialPair = MaterialPairArray[Subset.MaterialID];
                    if (Diffuse != null && MaterialPair.DiffuseResourceView != null) Diffuse.SetResource(MaterialPair.DiffuseResourceView);
                    if (Normal != null && MaterialPair.NormalResourceView != null) Normal.SetResource(MaterialPair.NormalResourceView);
                    if (Specular != null && MaterialPair.SpecularResourceView != null) Specular.SetResource(MaterialPair.SpecularResourceView);
                    if (DiffuseVector != null) DiffuseVector.SetFloatVector((float[])Material.Diffuse);
                    if (SpecularVector != null) SpecularVector.SetFloatVector((float[])Material.Specular);

                    Technique.GetPassByIndex(P).Apply(0);

                    var IndexCount = (uint)Subset.IndexCount;
                    var IndexStart = (uint)Subset.IndexStart;
                    var VertexStart = (int)Subset.VertexStart;
                    if (Adjacent)
                    {
                        IndexCount *= 2;
                        IndexStart *= 2;
                    }
                    Device.DrawIndexed(IndexCount, IndexStart, VertexStart);
                }
            }
        }

        void RenderMesh(uint Mesh, bool Adjacent, Device Device, uint DiffuseSlot, uint NormalSlot, uint SpecularSlot)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            var MeshDataPair = MeshPairArray[Mesh];

            var Strides = new uint[(int)IA.VertexInputResourceSlotCount];
            var Offsets = new uint[(int)IA.VertexInputResourceSlotCount];
            var VertexBuffers = new Buffer[(int)IA.VertexInputResourceSlotCount];

            if (MeshData.NumberOfVertexBuffers > (int)IA.VertexInputResourceSlotCount) return;

            for (var I = 0; I < MeshData.NumberOfVertexBuffers; I++)
            {
                // UnmanagedMemory.Get is not working for MarshalAs structs
                Size = Marshal.SizeOf(typeof(VertexBufferHeader));
                var VertexBufferHeader = (VertexBufferHeader)Marshal.PtrToStructure(new IntPtr(VertexBufferArray.Pointer.ToInt64() + MeshData.VertexBuffers[I] * Size), typeof(VertexBufferHeader));

                VertexBuffers[I] = VertexBufferPairArray[MeshData.VertexBuffers[I]].VertexBuffer;
                Strides[I] = (uint)VertexBufferHeader.StrideBytes;
                Offsets[I] = 0;
            }

            var IndexBuffers = Adjacent ? AdjacencyIndexBufferArray : IndexBufferArray;
            var IndexBufferPairs = Adjacent ? AdjacencyIndexBufferPairArray : IndexBufferPairArray;

            var IndexBuffer = IndexBufferPairs[MeshData.IndexBuffer].IndexBuffer;
            IndexBufferHeader IndexBufferHeader;
            IndexBuffers.Get(MeshData.IndexBuffer, out IndexBufferHeader);
            var IndexBufferFormat = (IndexType)IndexBufferHeader.IndexType == IndexType.x32 ? Format.R32_UInt : Format.R16_UInt;

            Device.IA_SetVertexBuffers(0, MeshData.NumberOfVertexBuffers, VertexBuffers, Strides, Offsets);
            Device.IA_SetIndexBuffer(IndexBuffer, IndexBufferFormat, 0);


            for (uint S = 0; S < MeshData.NumberOfSubsets; S++)
            {
                uint SubsetIndex;
                MeshDataPair.Subsets.Get(S, out SubsetIndex);

                // UnmanagedMemory.Get is not working for MarshalAs structs
                Size = Marshal.SizeOf(typeof(Subset));
                var Subset = (Subset)Marshal.PtrToStructure(new IntPtr(SubsetArray.Pointer.ToInt64() + SubsetIndex * Size), typeof(Subset));

                var PrimitiveType = GetPrimitiveType((PrimitiveType)Subset.PrimitiveType);
                if (Adjacent)
                {
                    switch (PrimitiveType)
                    {
                    case PrimitiveTopology.TriangleList:
                        PrimitiveType = PrimitiveTopology.TriangleListAdjacency;
                        break;
                    case PrimitiveTopology.TriangleStrip:
                        PrimitiveType = PrimitiveTopology.TriangleStripAdjacency;
                        break;
                    case PrimitiveTopology.LineList:
                        PrimitiveType = PrimitiveTopology.LineListAdjacency;
                        break;
                    case PrimitiveTopology.LineStrip:
                        PrimitiveType = PrimitiveTopology.LineStripAdjacency;
                        break;
                    }
                }

                Device.IA_SetPrimitiveTopology(PrimitiveType);

                var MaterialPair = MaterialPairArray[Subset.MaterialID];
                if (DiffuseSlot != InvalidSamplerSlot && MaterialPair.DiffuseResourceView != null) Device.PS_SetShaderResources(DiffuseSlot, 1, new[] { MaterialPair.DiffuseResourceView });
                if (NormalSlot != InvalidSamplerSlot && MaterialPair.NormalResourceView != null) Device.PS_SetShaderResources(NormalSlot, 1, new[] { MaterialPair.NormalResourceView });
                if (SpecularSlot != InvalidSamplerSlot && MaterialPair.SpecularResourceView != null) Device.PS_SetShaderResources(SpecularSlot, 1, new[] { MaterialPair.SpecularResourceView });

                var IndexCount = (uint)Subset.IndexCount;
                var IndexStart = (uint)Subset.IndexStart;
                var VertexStart = (int)Subset.VertexStart;
                if (Adjacent)
                {
                    IndexCount *= 2;
                    IndexStart *= 2;
                }
                Device.DrawIndexed(IndexCount, IndexStart, VertexStart);
            }
        }

        void RenderFrame(uint Frame, bool Adjacent, Device Device, EffectTechnique Technique, EffectShaderResourceVariable Diffuse, EffectShaderResourceVariable Normal, EffectShaderResourceVariable Specular, EffectVectorVariable DiffuseVector, EffectVectorVariable SpecularVector)
        {
            if (StaticMeshData == null || FrameArray == null) return;

            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Frame));
            var FrameHeaderData = (Frame)Marshal.PtrToStructure(new IntPtr(FrameArray.Pointer.ToInt64() + Frame * Size), typeof(Frame));

            if (FrameHeaderData.Mesh != InvalidMesh) RenderMesh(FrameHeaderData.Mesh, Adjacent, Device, Technique, Diffuse, Normal, Specular, DiffuseVector, SpecularVector);

            // Render our children
            if (FrameHeaderData.ChildFrame != InvalidFrame) RenderFrame(FrameHeaderData.ChildFrame, Adjacent, Device, Technique, Diffuse, Normal, Specular, DiffuseVector, SpecularVector);

            // Render our siblings
            if (FrameHeaderData.SiblingFrame != InvalidFrame) RenderFrame(FrameHeaderData.SiblingFrame, Adjacent, Device, Technique, Diffuse, Normal, Specular, DiffuseVector, SpecularVector);
        }

        void RenderFrame(uint Frame, bool Adjacent, Device Device, uint DiffuseSlot, uint NormalSlot, uint SpecularSlot)
        {
            if (StaticMeshData == null || FrameArray == null) return;

            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Frame));
            var FrameHeaderData = (Frame)Marshal.PtrToStructure(new IntPtr(FrameArray.Pointer.ToInt64() + Frame * Size), typeof(Frame));

            if (FrameHeaderData.Mesh != InvalidMesh) RenderMesh(FrameHeaderData.Mesh, Adjacent, Device, DiffuseSlot, NormalSlot, SpecularSlot);

            // Render our children
            if (FrameHeaderData.ChildFrame != InvalidFrame) RenderFrame(FrameHeaderData.ChildFrame, Adjacent, Device, DiffuseSlot, NormalSlot, SpecularSlot);

            // Render our siblings
            if (FrameHeaderData.SiblingFrame != InvalidFrame) RenderFrame(FrameHeaderData.SiblingFrame, Adjacent, Device, DiffuseSlot, NormalSlot, SpecularSlot);
        }

        ~SDK_Mesh()
        {
            Delete();
        }

        public int Create(Device Device, string FileName, bool CreateAdjacencyIndices = false, CallbacksStruct[] LoaderCallbacks = null)
        {
            return CreateFromFile(Device, FileName, CreateAdjacencyIndices, LoaderCallbacks);
        }

        public int Create(Device Device, UnmanagedMemory Data, bool CreateAdjacencyIndices = false, bool CopyStatic = false, CallbacksStruct[] LoaderCallbacks = null)
        {
            return CreateFromMemory(Device, Data, CreateAdjacencyIndices, CopyStatic, LoaderCallbacks);
        }

        public int LoadAnimation(string FileName)
        {
            string Path;

            // Find the path for the file
            var Result = Functions.FindSDK_MediaFileCch(out Path, FileName);
            if (Result < 0) return Result;

            // Open the file
            FileStream File;
            try { File = new FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read); }
            catch { return (int)Error.MediaNotFound; }

            try
            {
                /////////////////////////
                // Header

                var AnimationFileHeaderSize = Marshal.SizeOf(typeof(AnimationFileHeader));
                var Buffer = new byte[AnimationFileHeaderSize];
                File.Read(Buffer, 0, AnimationFileHeaderSize);

                var FileHeader = new AnimationFileHeader();
                var FileHeaderHandle = GCHandle.Alloc(FileHeader, GCHandleType.Pinned);
                try { Marshal.Copy(Buffer, 0, FileHeaderHandle.AddrOfPinnedObject(), AnimationFileHeaderSize); }
                finally { FileHeaderHandle.Free(); }

                //allocate
                try { AnimationData = new UnmanagedMemory((uint)AnimationFileHeaderSize + (uint)FileHeader.AnimationDataSize); }
                catch { return (int)Error.OutOfMemory; }

                // read it all in
                Buffer = new byte[AnimationFileHeaderSize + (int)FileHeader.AnimationDataSize];
                File.Seek(0, SeekOrigin.Begin);
                File.Read(Buffer, 0, AnimationFileHeaderSize + (int)FileHeader.AnimationDataSize);
                AnimationData.GetStream().Write(Buffer, 0, AnimationFileHeaderSize + (int)FileHeader.AnimationDataSize);

                // pointer fixup
                AnimationHeader = new UnmanagedMemory<AnimationFileHeader>(AnimationData.Pointer, (uint)AnimationFileHeaderSize);
                AnimationFrameData = new UnmanagedMemory<AnimationFrameData>(new IntPtr(AnimationData.Pointer.ToInt64() + (long)FileHeader.AnimationDataOffset), (uint)(FileHeader.NumberOfFrames * Marshal.SizeOf(typeof(AnimationFrameData))));

                for (uint I = 0; I < FileHeader.NumberOfFrames; I++)
                {
                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    var Size = Marshal.SizeOf(typeof(AnimationFrameData));
                    var AnimationFrame = (AnimationFrameData)Marshal.PtrToStructure(new IntPtr(AnimationFrameData.Pointer.ToInt64() + I * Size), typeof(AnimationFrameData));

                    var FrameIndex = FindFrameIndex(AnimationFrame.FrameName);
                    if (FrameIndex >= 0)
                    {
                        // UnmanagedMemory.Get is not working for MarshalAs structs
                        Size = Marshal.SizeOf(typeof(Frame));
                        var FrameHeaderData = (Frame)Marshal.PtrToStructure(new IntPtr(FrameArray.Pointer.ToInt64() + FrameIndex * Size), typeof(Frame));
                        FrameHeaderData.AnimationDataIndex = I;
                        // UnmanagedMemory.Set is not working for MarshalAs structs
                        Marshal.StructureToPtr(FrameHeaderData, new IntPtr(FrameArray.Pointer.ToInt64() + FrameIndex * Size), false);
                    }
                }

                Result = 0;
            }
            finally
            {
                File.Close();
            }

            return Result;
        }

        public void Delete()
        {
            if (!CheckLoadDone()) return;

            Header Header;
            MeshHeader.Get(out Header);

            if (StaticMeshData != null)
            {
                if (MaterialArray != null)
                {
                    for (var M = 0; M < Header.NumberOfMaterials; M++)
                    {
                        Resource Resource;
                        if (MaterialPairArray[M].DiffuseResourceView != null)
                        {
                            MaterialPairArray[M].DiffuseResourceView.GetResource(out Resource);
                            if (Resource != null)
                            {
                                Resource.Release();
                                Resource.Release(); // do this twice, because GetResource adds a ref
                            }

                            MaterialPairArray[M].DiffuseResourceView.Release();
                        }
                        if (MaterialPairArray[M].NormalResourceView != null)
                        {
                            MaterialPairArray[M].NormalResourceView.GetResource(out Resource);
                            if (Resource != null)
                            {
                                Resource.Release();
                                Resource.Release(); // do this twice, because GetResource adds a ref
                            }

                            MaterialPairArray[M].NormalResourceView.Release();
                        }
                        if (MaterialPairArray[M].SpecularResourceView != null)
                        {
                            MaterialPairArray[M].SpecularResourceView.GetResource(out Resource);
                            if (Resource != null)
                            {
                                Resource.Release();
                                Resource.Release(); // do this twice, because GetResource adds a ref
                            }

                            MaterialPairArray[M].SpecularResourceView.Release();
                        }
                    }
                }

                for (var I = 0; I < Header.NumberOfVertexBuffers; I++)
                {
                    if (VertexBufferPairArray[I].VertexBuffer != null) VertexBufferPairArray[I].VertexBuffer.Release();
                }

                for (var I = 0; I < Header.NumberOfIndexBuffers; I++)
                {
                    if (IndexBufferPairArray[I].IndexBuffer != null) IndexBufferPairArray[I].IndexBuffer.Release();
                }
            }

            if (AdjacencyIndexBufferArray != null)
            {
                for (var I = 0; I < Header.NumberOfIndexBuffers; I++)
                {
                    if (AdjacencyIndexBufferPairArray[I].IndexBuffer != null) AdjacencyIndexBufferPairArray[I].IndexBuffer.Release();
                }
            }

            AdjacencyIndexBufferArray = null;
            AdjacencyIndexBufferPairArray = null;

            HeapData = null;
            StaticMeshData = null;
            AnimationData = null;
            BindPoseFrameMatrices = null;
            TransformedFrameMatrices = null;

            VerticesList = null;
            IndicesList = null;

            MeshHeader = null;
            VertexBufferArray = null;
            VertexBufferPairArray = null;
            IndexBufferArray = null;
            IndexBufferPairArray = null;
            MeshArray = null;
            MeshPairArray = null;
            SubsetArray = null;
            FrameArray = null;
            MaterialArray = null;
            MaterialPairArray = null;

            AnimationHeader = null;
            AnimationFrameData = null;
        }

        // transform bind pose frame using a recursive traversal
        public void TransformBindPoseFrame(uint Frame, ref Matrix ParentWorld)
        {
            if (BindPoseFrameMatrices == null) return;

            // Transform ourselves
            Matrix LocalWorld;

            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Frame));
            var FrameHeaderData = (Frame)Marshal.PtrToStructure(new IntPtr(FrameArray.Pointer.ToInt64() + Frame * Size), typeof(Frame));

            D3DX10Functions.MatrixMultiply(out LocalWorld, ref FrameHeaderData.Matrix, ref ParentWorld);
            BindPoseFrameMatrices[Frame] = LocalWorld;

            // Transform our siblings
            if (FrameHeaderData.SiblingFrame != InvalidFrame) TransformBindPoseFrame(FrameHeaderData.SiblingFrame, ref ParentWorld);

            // Transform our children
            if (FrameHeaderData.ChildFrame != InvalidFrame) TransformBindPoseFrame(FrameHeaderData.ChildFrame, ref LocalWorld);
        }

        // transform the mesh frames according to the animation for time Time
        public void TransformMesh(Matrix World, double Time)
        {
            if (AnimationHeader == null) return;

            Header Header;
            MeshHeader.Get(out Header);

            AnimationFileHeader AnimationFileHeader;
            AnimationHeader.Get(out AnimationFileHeader);
            switch ((FrameTransformType)AnimationFileHeader.FrameTransformType)
            {
            case FrameTransformType.Relative:
                {
                    TransformFrame(0, ref World, Time);

                    // For each frame, move the transform to the bind pose, then
                    // move it to the final position
                    for (uint I = 0; I < Header.NumberOfFrames; I++)
                    {
                        Matrix InvBindPose;
                        D3DX10Functions.MatrixInverse(out InvBindPose, ref BindPoseFrameMatrices[I]);
                        var Final = InvBindPose * TransformedFrameMatrices[I];
                        TransformedFrameMatrices[I] = Final;
                    }
                }
                break;
            case FrameTransformType.Absolute:
                for (uint I = 0; I < Header.NumberOfFrames; I++) TransformFrameAbsolute(I, Time);
                break;
            }
        }

        // Generate an adjacency index buffer for each mesh
        public int CreateAdjacencyIndices(Device Device, float Epsilon, UnmanagedMemory BufferData)
        {
            var Result = 0;

            Header MeshHeaderData;
            MeshHeader.Get(out MeshHeaderData);
            try
            {
                AdjacencyIndexBufferArray = new UnmanagedMemory<IndexBufferHeader>((uint)(MeshHeaderData.NumberOfIndexBuffers * Marshal.SizeOf(typeof(IndexBufferHeader))));
                AdjacencyIndexBufferPairArray = new IndexBufferHeaderPair[MeshHeaderData.NumberOfIndexBuffers];
            }
            catch { return (int)Error.OutOfMemory; }

            IndexBufferHeader IndexBufferHeader;
            for (uint I = 0; I < MeshHeaderData.NumberOfMeshes; I++)
            {
                // UnmanagedMemory.Get is not working for MarshalAs structs
                var Size = Marshal.SizeOf(typeof(Mesh));
                var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + I * Size), typeof(Mesh));

                // ReSharper disable InconsistentNaming
                var VB_Index = MeshData.VertexBuffers[0];
                var IB_Index = MeshData.IndexBuffer;
                // ReSharper restore InconsistentNaming

                // UnmanagedMemory.Get is not working for MarshalAs structs
                Size = Marshal.SizeOf(typeof(VertexBufferHeader));
                var VertexBufferHeader = (VertexBufferHeader)Marshal.PtrToStructure(new IntPtr(VertexBufferArray.Pointer.ToInt64() + VB_Index * Size), typeof(VertexBufferHeader));

                IndexBufferArray.Get(IB_Index, out IndexBufferHeader);
                var VertexData = new UnmanagedMemory(new IntPtr(BufferData.Pointer.ToInt64() + (long)VertexBufferHeader.DataOffset), (uint)VertexBufferHeader.SizeBytes);
                var IndexData = new UnmanagedMemory(new IntPtr(BufferData.Pointer.ToInt64() + (long)IndexBufferHeader.DataOffset), (uint)IndexBufferHeader.SizeBytes);

                var Stride = VertexBufferHeader.StrideBytes;

                var Layout = new[]
                {
                    new InputElementDescription{ SemanticName = "POSITION", Format = Format.R32G32B32_Float, InputSlotClass = InputClassification.InputPerVertexData },
                    new InputElementDescription{ SemanticName = "END", Format = Format.R8_UInt, InputSlotClass = InputClassification.InputPerVertexData },
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

        public void Render(Device Device, uint DiffuseSlot, uint NormalSlot, uint SpecularSlot)
        {
            RenderFrame(0, false, Device, DiffuseSlot, NormalSlot, SpecularSlot);
        }

        public void Render(Device Device, EffectTechnique Technique, EffectShaderResourceVariable Diffuse, EffectShaderResourceVariable Normal, EffectShaderResourceVariable Specular, EffectVectorVariable DiffuseVector, EffectVectorVariable SpecularVector)
        {
            RenderFrame(0, false, Device, Technique, Diffuse, Normal, Specular, DiffuseVector, SpecularVector);
        }

        public void RenderAdjacent(Device Device, EffectTechnique Technique, EffectShaderResourceVariable Diffuse, EffectShaderResourceVariable Normal, EffectShaderResourceVariable Specular, EffectVectorVariable DiffuseVector, EffectVectorVariable SpecularVector)
        {
            RenderFrame(0, true, Device, Technique, Diffuse, Normal, Specular, DiffuseVector, SpecularVector);
        }

        public static PrimitiveTopology GetPrimitiveType(PrimitiveType PrimitiveType)
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
                Result = PrimitiveTopology.TriangleListAdjacency;
                break;
            case PrimitiveType.TriangleStripAdjacency:
                Result = PrimitiveTopology.TriangleStripAdjacency;
                break;
            case PrimitiveType.LineListAdjacency:
                Result = PrimitiveTopology.LineListAdjacency;
                break;
            case PrimitiveType.LineStripAdjacency:
                Result = PrimitiveTopology.LineStripAdjacency;
                break;
            }

            return Result;
        }

        public Format GetIndexBufferFormat(uint Mesh)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            IndexBufferHeader IndexBufferHeader;
            IndexBufferArray.Get(MeshData.IndexBuffer, out IndexBufferHeader);

            switch ((IndexType)IndexBufferHeader.IndexType)
            {
            case IndexType.x16:
                return Format.R16_UInt;
            case IndexType.x32:
                return Format.R32_UInt;
            }
            return Format.R16_UInt;
        }

        public Buffer GetVertexBuffer(uint Mesh, uint VertexBuffer)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            return VertexBufferPairArray[MeshData.VertexBuffers[VertexBuffer]].VertexBuffer;
        }

        public Buffer GetIndexBuffer(uint Mesh)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            return IndexBufferPairArray[MeshData.IndexBuffer].IndexBuffer;
        }

        public Buffer GetAdjacencyIndexBuffer(uint Mesh)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            return AdjacencyIndexBufferPairArray[MeshData.IndexBuffer].IndexBuffer;
        }

        public IndexType GetIndexType(uint Mesh)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            IndexBufferHeader IndexBufferHeader;
            IndexBufferArray.Get(MeshData.IndexBuffer, out IndexBufferHeader);
            return (IndexType)IndexBufferHeader.IndexType;
        }

        public uint GetNumberOfMeshes()
        {
            if (MeshHeader == null) return 0;
            Header Header;
            MeshHeader.Get(out Header);
            return Header.NumberOfMeshes;
        }

        public uint GetNumberOfMaterials()
        {
            if (MeshHeader == null) return 0;
            Header Header;
            MeshHeader.Get(out Header);
            return Header.NumberOfMaterials;
        }

        public uint GetNumberOfVertexBuffers()
        {
            if (MeshHeader == null) return 0;
            Header Header;
            MeshHeader.Get(out Header);
            return Header.NumberOfVertexBuffers;
        }

        public uint GetNumberOfIndexBuffers()
        {
            if (MeshHeader == null) return 0;
            Header Header;
            MeshHeader.Get(out Header);
            return Header.NumberOfIndexBuffers;
        }

        public Buffer GetVertexBufferAt(uint VertexBuffer)
        {
            return VertexBufferPairArray[VertexBuffer].VertexBuffer;
        }

        public Buffer GetIndexBufferAt(uint IndexBuffer)
        {
            return IndexBufferPairArray[IndexBuffer].IndexBuffer;
        }

        public UnmanagedMemory GetRawVerticesAt(uint VertexBuffer)
        {
            return VerticesList[(int)VertexBuffer];
        }

        public UnmanagedMemory GetRawIndicesAt(uint IndexBuffer)
        {
            return IndicesList[(int)IndexBuffer];
        }

        public uint GetNumberOfSubsets(uint Mesh)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            return MeshData.NumberOfSubsets;
        }

        public uint GetVertexStride(uint Mesh, uint VertexBuffer)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            // UnmanagedMemory.Get is not working for MarshalAs structs
            Size = Marshal.SizeOf(typeof(VertexBufferHeader));
            var VertexBufferHeader = (VertexBufferHeader)Marshal.PtrToStructure(new IntPtr(VertexBufferArray.Pointer.ToInt64() + MeshData.VertexBuffers[VertexBuffer] * Size), typeof(VertexBufferHeader));

            return (uint)VertexBufferHeader.StrideBytes;
        }

        public int FindFrameIndex(string Name)
        {
            Header Header;
            MeshHeader.Get(out Header);
            for (uint I = 0; I < Header.NumberOfFrames; I++)
            {
                // UnmanagedMemory.Get is not working for MarshalAs structs
                var Size = Marshal.SizeOf(typeof(Frame));
                var FrameHeaderData = (Frame)Marshal.PtrToStructure(new IntPtr(FrameArray.Pointer.ToInt64() + I * Size), typeof(Frame));

                if (FrameHeaderData.Name.SequenceEqual(Name)) return (int)I;
            }

            return -1;
        }

        public ulong GetNumberOfVertices(uint Mesh, uint VertexBuffer)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));


            // UnmanagedMemory.Get is not working for MarshalAs structs
            Size = Marshal.SizeOf(typeof(VertexBufferHeader));
            var VertexBufferHeader = (VertexBufferHeader)Marshal.PtrToStructure(new IntPtr(VertexBufferArray.Pointer.ToInt64() + MeshData.VertexBuffers[VertexBuffer] * Size), typeof(VertexBufferHeader));

            return VertexBufferHeader.NumberOfVertices;
        }

        public ulong GetNumberOfIndices(uint Mesh)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            IndexBufferHeader IndexBufferHeader;
            IndexBufferArray.Get(MeshData.IndexBuffer, out IndexBufferHeader);

            return IndexBufferHeader.NumberOfIndices;
        }

        public Vector3 GetMeshBBoxCenter(uint Mesh)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            return MeshData.BoundingBoxCenter;
        }

        public Vector3 GetMeshBBoxExtents(uint Mesh)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            return MeshData.BoundingBoxExtents;
        }

        public uint GetOutstandingResources()
        {
            uint OutstandingResources = 0;
            if (MeshHeader == null) return 1;

            Header Header;
            MeshHeader.Get(out Header);
            if (Device != null)
            {
                for (uint I = 0; I < Header.NumberOfMaterials; I++)
                {
                    // UnmanagedMemory.Get is not working for MarshalAs structs
                    var Size = Marshal.SizeOf(typeof(Material));
                    var Material = (Material)Marshal.PtrToStructure(new IntPtr(MaterialArray.Pointer.ToInt64() + I * Size), typeof(Material));

                    if (!string.IsNullOrEmpty(Material.DiffuseTexture))
                    {
                        if (MaterialPairArray[I].DiffuseResourceView == null) OutstandingResources++;
                    }

                    if (!string.IsNullOrEmpty(Material.NormalTexture))
                    {
                        if (MaterialPairArray[I].NormalResourceView == null) OutstandingResources++;
                    }

                    if (!string.IsNullOrEmpty(Material.SpecularTexture))
                    {
                        if (MaterialPairArray[I].SpecularResourceView == null) OutstandingResources++;
                    }
                }
            }

            return OutstandingResources;
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

        public bool IsLoaded()
        {
            return StaticMeshData != null && !Loading;
        }

        public bool HadLoadingError()
        {
            if (MeshHeader != null)
            {
                Header Header;
                MeshHeader.Get(out Header);
                for (uint I = 0; I < Header.NumberOfVertexBuffers; I++)
                {
                    if (VertexBufferPairArray[I].VertexBuffer == null) return true;
                }

                for (uint I = 0; I < Header.NumberOfIndexBuffers; I++)
                {
                    if (IndexBufferPairArray[I].IndexBuffer == null) return true;
                }
            }

            return false;
        }

        public uint GetNumberOfInfluences(uint Mesh)
        {
            // UnmanagedMemory.Get is not working for MarshalAs structs
            var Size = Marshal.SizeOf(typeof(Mesh));
            var MeshData = (Mesh)Marshal.PtrToStructure(new IntPtr(MeshArray.Pointer.ToInt64() + Mesh * Size), typeof(Mesh));

            return MeshData.NumberOfFrameInfluences;
        }

        public uint GetMeshInfluenceFrame(uint Mesh, uint Influence)
        {
            uint Frame;
            MeshPairArray[Mesh].FrameInfluences.Get(Influence, out Frame);
            return Frame;
        }

        public uint GetAnimationKeyFromTime(double Time)
        {
            AnimationFileHeader Header;
            AnimationHeader.Get(out Header);
            var Tick = (uint)(Header.AnimationFPS * Time);

            Tick = Tick % (Header.NumberOfAnimationKeys - 1);
            Tick++;

            return Tick;
        }
    }
}