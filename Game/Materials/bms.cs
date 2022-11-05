using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;

namespace Silkroad.Materials
{
    internal class bms
    {
        private Vector3[] _vertices;
        private Vector3[] _normals;
        private Vector2[] _textures;
        public string MeshName;
        public string MaterialName;
        private VertexPositionNormalTexture[] _vertexPositionTexture;
        private string ModelName;

        public short[,] faces = null;
        private int[] _indicies = null;

        public VertexPositionNormalTexture[] Verticies => _vertexPositionTexture;

        public int[] Indicies => _indicies;

        public bms(string modelName, byte[] buffer)
        {
            ModelName = modelName;
            ParseBMS(buffer);
        }

        private void ParseBMS(byte[] buffer)
        {
            using var reader = new BinaryReader(new MemoryStream(buffer));

            var header = reader.ReadStringEx(12);
            if (header != "JMXVBMS 0110")
                return;

            int vertCountAt = reader.ReadInt32();
            int test2 = reader.ReadInt32();
            int test3 = reader.ReadInt32();
            int test4 = reader.ReadInt32();
            int test5 = reader.ReadInt32();
            int test6 = reader.ReadInt32();
            int test7 = reader.ReadInt32();
            int test8 = reader.ReadInt32();
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

            _vertices = new Vector3[vertexCount];
            _normals = new Vector3[vertexCount];
            _textures = new Vector2[vertexCount];
            _vertexPositionTexture = new VertexPositionNormalTexture[vertexCount];

            for (int i = 0; i < vertexCount; i++)
            {
                _vertices[i] = reader.ReadVector3();
                _normals[i] = reader.ReadVector3();
                _textures[i] = reader.ReadVector2();
                _vertexPositionTexture[i] = new(_vertices[i], _normals[i], _textures[i]);
                
                if (lightmapResolution > 0)
                {
                    Vector2 unk12 = reader.ReadVector2();
                }

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

            var indiceCount = reader.ReadInt32();
            _indicies = new int[indiceCount * 3];
            for (int i = 0; i < _indicies.Length; i+=3)
            {
                _indicies[i] = reader.ReadInt16();
                _indicies[i + 1] = reader.ReadInt16();
                _indicies[i + 2] = reader.ReadInt16();
            }
        }
    }
}