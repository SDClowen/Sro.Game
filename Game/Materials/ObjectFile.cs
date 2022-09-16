using Microsoft.Xna.Framework;
using Silkroad.Components;
using System.Collections.Generic;
using System.IO;

namespace Silkroad.Materials
{
    internal class ObjectFile
    {
        public List<MapObjectElement> Elements = new(128);

        public ObjectFile(byte[] buffer)
        {
            using (var reader = new BinaryReader(new MemoryStream(buffer)))
            {
                reader.BaseStream.Position += 12; //skip header
                for (int i = 0; i < 144; i++)
                {
                    var count = reader.ReadInt16();
                    for (int j = 0; j < count; j++)
                    {
                        var obj = new MapObjectElement
                        {
                            Index = reader.ReadInt32(),
                            Position = new Vector3
                            (
                                reader.ReadSingle(),
                                reader.ReadSingle(),
                                reader.ReadSingle()
                            ),

                            UnknownFlag1 = reader.ReadUInt16(),

                            Theta = reader.ReadSingle(),
                            Id = reader.ReadInt32(),

                            UnknownFlag2 = reader.ReadUInt16(),

                            RegionX = reader.ReadByte(),
                            RegionY = reader.ReadByte()
                        };

                        obj.Position.X += (obj.RegionX - Terrain.XSector) * 1920;
                        obj.Position.Z += (obj.RegionY - Terrain.YSector) * 1920;

                        Elements.Add(obj);
                    }
                }
            }
        }
    }

    public struct MapObjectElement
    {
        public int Index;
        public int Id;
        public byte RegionX;
        public byte RegionY;
        public Vector3 Position;
        public float Theta;
        public ushort UnknownFlag1;
        public ushort UnknownFlag2;
    }
}