using System.Collections.Generic;
using System.IO;
using Accessibility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using static System.Net.WebRequestMethods;

namespace Silkroad.Materials
{
    internal class bms
    {
        private Vector3[] verticies;
        private Vector3[] uv;
        private Vector2[] textures;
        public string mesh;
        public string material;
        private VertexPositionTexture[] vert;
        private string ModelName;

        public short[,] faces = null;
        private int[] _indicies = null;

        public VertexPositionTexture[] Verticies => vert;

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
            mesh = reader.ReadStringEx();
            material = reader.ReadStringEx();
            int unk = reader.ReadInt32();

            var verticieCount = reader.ReadInt32();

            verticies = new Vector3[verticieCount];
            uv = new Vector3[verticieCount];
            textures = new Vector2[verticieCount];
            vert = new VertexPositionTexture[verticieCount];

            for (int i = 0; i < verticieCount; i++)
            {
                verticies[i] = reader.ReadVector3();
                uv[i] = reader.ReadVector3();
                textures[i] = reader.ReadVector2();
                if (lightmapResolution > 0)
                {
                    Vector2 unk12 = reader.ReadVector2();
                }

                vert[i] = new VertexPositionTexture(verticies[i], textures[i]);
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
                reader.BaseStream.Position += verticieCount * 6;
            }

            var indicieCount = reader.ReadInt32();
            _indicies = new int[indicieCount * 3];
            for (int i = 0; i < _indicies.Length; i+=3)
            {
                _indicies[i] = reader.ReadInt16();
                _indicies[i + 1] = reader.ReadInt16();
                _indicies[i + 2] = reader.ReadInt16();
            }
        }
    }
}