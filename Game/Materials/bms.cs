using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Silkroad.Materials
{
    internal class bms
    {
        public string MeshName;
        public string MaterialName;
        private VertexPositionNormalTexture[] _vertexPositionTexture;
        private string ModelName;

        private int[] _indicies = null;

        public VertexPositionNormalTexture[] Verticies => _vertexPositionTexture;

        public int[] Indicies => _indicies;

        public bms(string modelName, string path)
        {
            ModelName = modelName;

            var buffer = Program.Data.GetFileBuffer(path);
            using var reader = new BinaryReader(new MemoryStream(buffer));

            var header = reader.ReadStringEx(12);
            //if (header != "JMXVBMS 0110")
            //    return;

            int vertCountAt = reader.ReadInt32();
            int bonesCountAt = reader.ReadInt32();
            int facesCountAt = reader.ReadInt32();
            int unknown1 = reader.ReadInt32();
            int unknown2 = reader.ReadInt32();
            int boundingBoxCountAt = reader.ReadInt32();
            int gatesCountAt = reader.ReadInt32();
            int collisionCountAt = reader.ReadInt32();
            var pointerBoundingBox = reader.ReadInt32();
            int test10 = reader.ReadInt32();
            var pointerHitbox = reader.ReadInt32();
            int test12 = reader.ReadInt32();
            int test13 = reader.ReadInt32();
            int lightmapResolution = reader.ReadInt32();
            int test15 = reader.ReadInt32();

            MeshName = reader.ReadStringEx();
            MaterialName = reader.ReadStringEx();
            int unk = reader.ReadInt32();

            var vertexCount = reader.ReadInt32();
            _vertexPositionTexture = new VertexPositionNormalTexture[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                var vertice = reader.ReadVector3();
                var normal = reader.ReadVector3();
                normal.Normalize();
                _vertexPositionTexture[i] = new(
                    vertice, normal,
                    reader.ReadVector2()
                );

                if (lightmapResolution > 0)
                {
                    reader.ReadVector2();
                }

                //Related to bones, need to look into it.
                reader.BaseStream.Position += 12;
            }

            if (lightmapResolution > 0)
            {
                var lightmap = reader.ReadStringEx();
            }

            int boneCount = reader.ReadInt32();
            for (int i = 0; i < boneCount; i++)
            {
                var test = reader.ReadStringEx();
            }

            if (boneCount > 0)
            {
                reader.BaseStream.Position += vertexCount * 6;
            }

            //reader.BaseStream.Position = facesCountAt;
            var indiceCount = reader.ReadInt32();
            _indicies = new int[indiceCount * 3];
            for (int i = 0; i < _indicies.Length; i += 3)
            {
                _indicies[i] = reader.ReadInt16();
                _indicies[i + 1] = reader.ReadInt16();
                _indicies[i + 2] = reader.ReadInt16();
            }
        }
    }
}