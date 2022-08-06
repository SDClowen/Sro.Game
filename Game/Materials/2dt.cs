using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Silkroad.Materials
{
    class TexCoord
    {
        public float U;
        public float V;

        public TexCoord(float u, float v)
        {
            U = u;
            V = v;
        }
    }

    internal class Block
    {
        public string Name;
        public string Texture1;
        public string Texture2;
        public string Text;
        public string Description;
        public string Prototype;
        public int Type;
        public int Id;
        public int ParentId;
        public int SubParentId;
        public int Unk2;
        public int Unk3;
        public Color4 Color;
        public Rectangle Rectangle;
        public TexCoord TexCoordLeftTop;
        public TexCoord TexCoordRightTop;
        public TexCoord TexCoordRightBottom;
        public TexCoord TexCoordLeftBottom;



        public int Unk4;
        public int Unk5;
        public int Unk6;
        public int Unk7;
        public int Unk8;
        public int Unk9;
        public int Unk10;
        public int Unk11;
        public int Unk12;
        public int Unk13;
        public int Unk14;
        public int Unk15;
        public int Unk16;
        public int Unk17;
        public int Unk18;
        public int Unk19;
        public int Unk20;
    }

    internal class _2DT
    {
        internal static void Decode(byte[] buffer)
        {
            using (var stream = new MemoryStream(buffer))
            using (var reader = new BinaryReader(stream))
            {
                const int blockSize = 976;

                var blockCount = reader.ReadInt32();
                if ((blockCount * blockSize) + 4 != stream.Length)
                    throw new InvalidDataException("Invalid size!!!");

                var blocks = new List<Block>(blockCount);

                for (int i = 0; i < blockCount; i++)
                {
                    blocks.Add(new Block
                    {
                        Name = reader.ReadFixedSizeString(64),
                        Texture1 = reader.ReadFixedSizeString(256),
                        Texture2 = reader.ReadFixedSizeString(256),
                        Text = reader.ReadFixedSizeString(128),
                        Description = reader.ReadFixedSizeString(64),
                        Prototype = reader.ReadFixedSizeString(64),
                        Type = reader.ReadInt32(),
                        Id = reader.ReadInt32(),
                        ParentId = reader.ReadInt32(),
                        SubParentId = reader.ReadInt32(),
                        Unk2 = reader.ReadInt32(),
                        Unk3 = reader.ReadInt32(),
                        Color = new Color4
                        {
                            Red = reader.ReadByte(),
                            Green = reader.ReadByte(),
                            Blue = reader.ReadByte(),
                            Alpha = reader.ReadByte()
                        },
                        Rectangle = new Rectangle(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32()),
                        TexCoordLeftTop = new TexCoord(reader.ReadSingle(), reader.ReadSingle()),
                        TexCoordRightTop = new TexCoord(reader.ReadSingle(), reader.ReadSingle()),
                        TexCoordRightBottom = new TexCoord(reader.ReadSingle(), reader.ReadSingle()),
                        TexCoordLeftBottom = new TexCoord(reader.ReadSingle(), reader.ReadSingle()),

                        Unk4 = reader.ReadInt32(),
                        Unk5 = reader.ReadInt32(),
                        Unk6 = reader.ReadInt32(),
                        Unk7 = reader.ReadInt32(),
                        Unk8 = reader.ReadInt32(),
                        Unk9 = reader.ReadInt32(),
                        Unk10 = reader.ReadInt32(),
                        Unk11 = reader.ReadInt32(),
                        Unk12 = reader.ReadInt32(),
                        Unk13 = reader.ReadInt32(),
                        Unk14 = reader.ReadInt32(),
                        Unk15 = reader.ReadInt32(),
                        Unk16 = reader.ReadInt32(),
                        Unk17 = reader.ReadInt32(),
                        Unk18 = reader.ReadInt32(),
                        Unk19 = reader.ReadInt32(),
                        Unk20 = reader.ReadInt32()
                    });
                }
            }
        }
    }
}