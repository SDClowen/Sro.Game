﻿using System.Collections.Generic;
using System.IO;
using Accessibility;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Silkroad.Materials
{
    internal class bms
    {
        private Vector3[] verticies;
        private Vector3[] uv;
        private Vector2[] textures;
        public string mesh;
        public string material;
        private VertexPositionNormalTexture[] vert;
        private string modelname;

        public bms(string modelName, byte[] file)
        {
            modelname = modelName;
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
                verticies = new Vector3[vertCount];
                uv = new Vector3[vertCount];
                textures = new Vector2[vertCount];
                vert = new VertexPositionNormalTexture[vertCount];
                for (int i = 0; i < vertCount; i++)
                {
                    verticies[i] = reader.ReadVector3();
                    uv[i] = reader.ReadVector3();
                    textures[i] = reader.ReadVector2();
                    if (lightmapResolution > 0)
                    {
                        Vector2 unk12 = reader.ReadVector2();
                    }

                    vert[i] = new VertexPositionNormalTexture(verticies[i], uv[i], textures[i]);
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
                    reader.BaseStream.Position += vertCount * 6;
                }

                int faceCount = reader.ReadInt32();
                faces = new short[faceCount, 3];
                for (int i = 0; i < faceCount; i++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        faces[i, x] = reader.ReadInt16();
                    }
                }
                reader.Close();

                List<int> tmp = new();
                if (faces != null)
                {
                    for (int i = 0; i < faces.Length / 3; i++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            tmp.Add(faces[i, x]);
                        }
                    }
                }

                _indicies = tmp.ToArray();
            }
        }

        private short[,] faces = null;
        private int[] _indicies = null;

        public VertexPositionNormalTexture[] Verticies => vert;

        public int[] Indicies
        {
            get
            {
                List<int> tmp = new();
                if (faces != null)
                {
                    for (int i = 0; i < faces.Length / 3; i++)
                    {
                        for (int x = 0; x < 3; x++)
                        {
                            tmp.Add(faces[i, x]);
                        }
                    }
                }

                //return tmp.ToArray();
                return _indicies;
            }
        }
    }
}