using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Silkroad.Materials
{
    internal class bms
    {
        private Vector3[] uv;
        private Vector2[] textures;
        public VertexPositionTexture[] Verticies;
        public int[] Indicies;

        public string mesh;
        public string material;
        private string _modelname;


        public bms(string modelName, byte[] file)
        {
            _modelname = modelName;

            BinaryReader reader = new BinaryReader(new MemoryStream(file));
            ParseBMS(reader);
            reader.Dispose();
            reader.Close();
        }

        private void ParseBMS(BinaryReader reader)
        {
            string header = new string(reader.ReadChars(12));
            if (header == "JMXVBMS 0110")
            {
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
                mesh = new string(reader.ReadChars(reader.ReadInt32()));
                material = new string(reader.ReadChars(reader.ReadInt32()));
                int unk = reader.ReadInt32();
                int vertCount = reader.ReadInt32();

                uv = new Vector3[vertCount];
                textures = new Vector2[vertCount];
                Verticies = new VertexPositionTexture[vertCount];

                for (int i = 0; i < vertCount; i++)
                {
                    var verticie = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    uv[i] = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
                    textures[i] = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                    if (lightmapResolution > 0)
                    {
                        Vector2 unk12 = new Vector2(reader.ReadSingle(), reader.ReadSingle());
                        //Console.WriteLine($"unk12: {unk12}");
                    }
                    Verticies[i] = new(verticie, /*uv[i],*/ textures[i]);
                    reader.BaseStream.Position += 12;
                }

                if (lightmapResolution > 0)
                {
                    string lightmap = new string(reader.ReadChars(reader.ReadInt32()));
                    //Console.WriteLine($"LightMap: {lightmap}");
                }

                int boneCount = reader.ReadInt32();
                for (int i = 0; i < boneCount; i++)
                {
                    string test = new string(reader.ReadChars(reader.ReadInt32()));
                    //Console.WriteLine($"test: {test}");
                }

                if (boneCount > 0)
                {
                    reader.BaseStream.Position += vertCount * 6;
                }

                var indicies = new List<int>(1000);

                int faceCount = reader.ReadInt32();
                for (int i = 0; i < faceCount; i++)
                    for (int x = 0; x < 3; x++)
                        indicies.Add(reader.ReadInt16());

                Indicies = indicies.ToArray();

                reader.Close();
            }
        }
    }
}