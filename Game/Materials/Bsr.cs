using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text;

namespace Silkroad.Materials
{
    public class Bsr
    {
        public VertexPositionColor[] m_boudingBox;
        public BoundingBox m_realBoundingBox;

        public byte[] Header { get; set; }

        public uint pointerMaterial { get; set; }
        public uint pointerMesh { get; set; }
        public uint pointerSkeleton { get; set; }
        public uint pointerAnimation { get; set; }
        public uint pointerMeshGroup { get; set; }
        public uint pointerAnimationGroup { get; set; }
        public uint pointerSoundEffect { get; set; }
        public uint pointerBoundingBox { get; set; }

        public uint flagsUnkUInt0 { get; set; }
        public uint flagsUnkUInt1 { get; set; }
        public uint flagsUnkUInt2 { get; set; }
        public uint flagsUnkUInt3 { get; set; }
        public uint flagsUnkUInt4 { get; set; }

        public uint Type { get; set; }
        public string Name { get; set; }

        public byte[] unkBuffer { get; set; }

        public string rootMesh { get; set; }
        public float[] BoundingBox0 { get; set; }
        public float[] BoundingBox1 { get; set; }
        public uint hasExtraBoundingData { get; set; }
        public byte[] extraBoundingData { get; set; }

        public uint materialSetCount { get; set; }
        public Material[] Materials { get; set; }

        public uint meshCount { get; set; }
        public Mesh[] Meshs { get; set; }

        public uint unkUInt5 { get; set; }
        public uint unkUInt6 { get; set; }

        public uint animationCount { get; set; }
        public Animation[] Animations { get; set; }

        public uint skeletonCount { get; set; }
        public Skeleton[] Skeletons { get; set; }

        public uint meshGroupCount { get; set; }
        public MeshGroup[] MeshGroups { get; set; }

        public uint animationGroupCount { get; set; }
        public AnimationGroup[] AnimationGroups { get; set; }

        public class Material
        {
            public uint ID { get; set; }
            public string Path { get; set; }
        }

        public class Mesh
        {
            public uint UnkUInt0 { get; set; }
            public string Path { get; set; }
        }

        public class Animation
        {
            public string Path { get; set; }
        }

        public class Skeleton
        {
            public string Path { get; set; }
            public uint extraByteCount { get; set; }
            public byte[] ExtraBytes { get; set; }
        }

        public class MeshGroup
        {
            public string Name { get; set; }
            public uint[] FileIndex { get; set; }
        }

        public class AnimationGroup
        {
            public uint NameLength { get; set; }
            public string Name { get; set; }

            public uint entryCount { get; set; }
            public Entry[] Entries { get; set; }

            public class Entry
            {
                public uint animationType { get; set; }
                public uint animationFileIndex { get; set; }

                public uint eventCount { get; set; }
                public Event[] Events { get; set; }

                public uint walkGraphPointCount { get; set; }
                public float animationWalkLength { get; set; }

                public GraphPoint[] GraphPoints { get; set; }

                public class Event
                {
                    public uint keyTime { get; set; }
                    public uint type { get; set; }
                    public uint unkValue0 { get; set; }
                    public uint unkValue1 { get; set; }
                }

                public class GraphPoint
                {
                    public float X { get; set; }
                    public float Y { get; set; }
                }
            }
        }

        public static Bsr ReadFromStream(byte[] buffer)
        {
            using (var stream = new BinaryReader(new MemoryStream(buffer)))
            {
                var bsr = new Bsr();

                bsr.Header = stream.ReadBytes(12);
                bsr.pointerMaterial = stream.ReadUInt32();
                bsr.pointerMesh = stream.ReadUInt32();
                bsr.pointerSkeleton = stream.ReadUInt32();
                bsr.pointerAnimation = stream.ReadUInt32();
                bsr.pointerMeshGroup = stream.ReadUInt32();
                bsr.pointerAnimationGroup = stream.ReadUInt32();
                bsr.pointerSoundEffect = stream.ReadUInt32();
                bsr.pointerBoundingBox = stream.ReadUInt32();

                bsr.flagsUnkUInt0 = stream.ReadUInt32();
                bsr.flagsUnkUInt1 = stream.ReadUInt32();
                bsr.flagsUnkUInt2 = stream.ReadUInt32();
                bsr.flagsUnkUInt3 = stream.ReadUInt32();
                bsr.flagsUnkUInt4 = stream.ReadUInt32();

                bsr.Type = stream.ReadUInt32();
                bsr.Name = stream.ReadFixedSizeString();

                bsr.unkBuffer = stream.ReadBytes(48);

                bsr.rootMesh = stream.ReadFixedSizeString();

                bsr.BoundingBox0 = new float[6];
                bsr.BoundingBox0[0] = stream.ReadSingle();
                bsr.BoundingBox0[1] = stream.ReadSingle();
                bsr.BoundingBox0[2] = stream.ReadSingle();
                bsr.BoundingBox0[3] = stream.ReadSingle();
                bsr.BoundingBox0[4] = stream.ReadSingle();
                bsr.BoundingBox0[5] = stream.ReadSingle();

                bsr.BoundingBox1 = new float[6];
                bsr.BoundingBox1[0] = stream.ReadSingle();
                bsr.BoundingBox1[1] = stream.ReadSingle();
                bsr.BoundingBox1[2] = stream.ReadSingle();
                bsr.BoundingBox1[3] = stream.ReadSingle();
                bsr.BoundingBox1[4] = stream.ReadSingle();
                bsr.BoundingBox1[5] = stream.ReadSingle();

                bsr.hasExtraBoundingData = stream.ReadUInt32();

                if (bsr.hasExtraBoundingData == 1)
                    bsr.extraBoundingData = stream.ReadBytes(64);

                //Materials
                bsr.materialSetCount = stream.ReadUInt32();
                bsr.Materials = new Material[bsr.materialSetCount];
                for (int i = 0; i < bsr.materialSetCount; i++)
                {
                    bsr.Materials[i] = new Material
                    {
                        ID = stream.ReadUInt32(),
                        Path = stream.ReadFixedSizeString()
                    };
                }

                //Mesh
                bsr.meshCount = stream.ReadUInt32();
                bsr.Meshs = new Mesh[bsr.meshCount];

                for (int i = 0; i < bsr.meshCount; i++)
                {
                    var mesh = new Mesh();
                    mesh.Path = stream.ReadFixedSizeString();

                    if (bsr.flagsUnkUInt0 == 1)
                        mesh.UnkUInt0 = stream.ReadUInt32();

                    bsr.Meshs[i] = mesh;
                }

                //Animation
                bsr.unkUInt5 = stream.ReadUInt32();
                bsr.unkUInt6 = stream.ReadUInt32();

                bsr.animationCount = stream.ReadUInt32();
                bsr.Animations = new Animation[bsr.animationCount];
                for (int i = 0; i < bsr.animationCount; i++)
                {
                    var animation = new Animation();
                    animation.Path = stream.ReadFixedSizeString();

                    bsr.Animations[i] = animation;
                }

                //Skeleton
                bsr.skeletonCount = stream.ReadUInt32();
                bsr.Skeletons = new Skeleton[bsr.skeletonCount];
                for (int i = 0; i < bsr.skeletonCount; i++)
                {
                    var skeleton = new Skeleton();
                    skeleton.Path = stream.ReadFixedSizeString();
                    skeleton.extraByteCount = stream.ReadUInt32();
                    skeleton.ExtraBytes = stream.ReadBytes((int)skeleton.extraByteCount);

                    bsr.Skeletons[i] = skeleton;
                }

                //Mesh Group
                bsr.meshGroupCount = stream.ReadUInt32();
                bsr.MeshGroups = new MeshGroup[bsr.meshGroupCount];
                for (int i = 0; i < bsr.meshGroupCount; i++)
                {
                    var meshGroup = new MeshGroup();
                    meshGroup.Name = stream.ReadFixedSizeString();
                    
                    var fileCount = stream.ReadUInt32();
                    meshGroup.FileIndex = new uint[fileCount];

                    for (int j = 0; j < fileCount; j++)
                        meshGroup.FileIndex[j] = stream.ReadUInt32();

                    bsr.MeshGroups[i] = meshGroup;
                }
                /*
                //Animation Group
                bsr.animationGroupCount = reader.ReadUInt32();
                bsr.AnimationGroups = new AnimationGroup[bsr.animationGroupCount];

                for (int i = 0; i < bsr.animationGroupCount; i++)
                {
                    var animationGroup = new AnimationGroup();

                    animationGroup.NameLength = reader.ReadUInt32();
                    animationGroup.Name = Encoding.ASCII.GetString(reader.ReadBytes((int)animationGroup.NameLength));

                    animationGroup.entryCount = reader.ReadUInt32();
                    animationGroup.Entries = new AnimationGroup.Entry[animationGroup.entryCount];

                    for (int j = 0; j < animationGroup.entryCount; j++)
                    {
                        var entry = new AnimationGroup.Entry();

                        entry.animationType = reader.ReadUInt32();
                        entry.animationFileIndex = reader.ReadUInt32();

                        entry.eventCount = reader.ReadUInt32();
                        entry.Events = new AnimationGroup.Entry.Event[entry.eventCount];

                        for (int k = 0; k < entry.eventCount; k++)
                        {
                            var entryEvent = new AnimationGroup.Entry.Event();

                            entryEvent.keyTime = reader.ReadUInt32();
                            entryEvent.type = reader.ReadUInt32();
                            entryEvent.unkValue0 = reader.ReadUInt32();
                            entryEvent.unkValue1 = reader.ReadUInt32();

                            entry.Events[k] = entryEvent;
                        }

                        entry.walkGraphPointCount = reader.ReadUInt32();
                        entry.animationWalkLength = reader.ReadSingle();

                        entry.GraphPoints = new AnimationGroup.Entry.GraphPoint[entry.eventCount];
                        for (int l = 0; l < entry.walkGraphPointCount; l++)
                        {
                            var graphPoint = new AnimationGroup.Entry.GraphPoint();

                            graphPoint.X = reader.ReadSingle();
                            graphPoint.Y = reader.ReadSingle();

                            entry.GraphPoints[l] = graphPoint;
                        }

                        animationGroup.Entries[j] = entry;
                    }

                    bsr.AnimationGroups[i] = animationGroup;
                }
                */
                //
                // etc...
                //

                stream.Close();
                return bsr;
            }
        }

        private void CreateBoundingBox(float[,] idk)
        {
            m_boudingBox = new VertexPositionColor[24];
            float width = idk[1, 3] - idk[1, 0];
            float height = idk[1, 4] - idk[1, 1];
            float lenght = idk[1, 5] - idk[1, 2];

            m_realBoundingBox = new BoundingBox(new Vector3(-width / 2f, 0, -lenght / 2f), new Vector3(width / 2f, height, lenght / 2f));

            m_boudingBox[0] = new VertexPositionColor(new Vector3(-width / 2.0f, 0, lenght / 2f), Color.Red);
            m_boudingBox[1] = new VertexPositionColor(new Vector3(width / 2.0f, 0, lenght / 2f), Color.Red);

            m_boudingBox[2] = new VertexPositionColor(new Vector3(-width / 2.0f, 0, -lenght / 2f), Color.Red);
            m_boudingBox[3] = new VertexPositionColor(new Vector3(width / 2.0f, 0, -lenght / 2f), Color.Red);

            m_boudingBox[4] = new VertexPositionColor(new Vector3(-width / 2f, 0, -lenght / 2f), Color.Red);
            m_boudingBox[5] = new VertexPositionColor(new Vector3(-width / 2f, 0, lenght / 2f), Color.Red);

            m_boudingBox[6] = new VertexPositionColor(new Vector3(width / 2f, 0, -lenght / 2f), Color.Red);
            m_boudingBox[7] = new VertexPositionColor(new Vector3(width / 2f, 0, lenght / 2f), Color.Red);

            m_boudingBox[8] = new VertexPositionColor(new Vector3(width / 2f, 0, lenght / 2f), Color.Red);
            m_boudingBox[9] = new VertexPositionColor(new Vector3(width / 2f, height, lenght / 2f), Color.Red);

            m_boudingBox[10] = new VertexPositionColor(new Vector3(width / 2f, 0, -lenght / 2f), Color.Red);
            m_boudingBox[11] = new VertexPositionColor(new Vector3(width / 2f, height, -lenght / 2f), Color.Red);

            m_boudingBox[12] = new VertexPositionColor(new Vector3(-width / 2f, 0, -lenght / 2f), Color.Red);
            m_boudingBox[13] = new VertexPositionColor(new Vector3(-width / 2f, height, -lenght / 2f), Color.Red);

            m_boudingBox[14] = new VertexPositionColor(new Vector3(-width / 2f, 0, lenght / 2f), Color.Red);
            m_boudingBox[15] = new VertexPositionColor(new Vector3(-width / 2f, height, lenght / 2f), Color.Red);

            m_boudingBox[16] = new VertexPositionColor(new Vector3(width / 2f, height, lenght / 2f), Color.Red);
            m_boudingBox[17] = new VertexPositionColor(new Vector3(width / 2f, height, -lenght / 2f), Color.Red);

            m_boudingBox[18] = new VertexPositionColor(new Vector3(-width / 2f, height, lenght / 2f), Color.Red);
            m_boudingBox[19] = new VertexPositionColor(new Vector3(-width / 2f, height, -lenght / 2f), Color.Red);

            m_boudingBox[20] = new VertexPositionColor(new Vector3(-width / 2.0f, height, -lenght / 2f), Color.Red);
            m_boudingBox[21] = new VertexPositionColor(new Vector3(width / 2.0f, height, -lenght / 2f), Color.Red);

            m_boudingBox[22] = new VertexPositionColor(new Vector3(-width / 2.0f, height, lenght / 2f), Color.Red);
            m_boudingBox[23] = new VertexPositionColor(new Vector3(width / 2.0f, height, lenght / 2f), Color.Red);
        }
    }
}