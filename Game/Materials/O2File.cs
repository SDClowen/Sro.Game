using Microsoft.Xna.Framework;
using Silkroad.Components;
using System.Collections.Generic;
using System.IO;

namespace Silkroad.Materials
{
    internal class O2File
    {
        public List<O2FileElement> Elements = new(128);

        public O2File(byte[] buffer)
        {
            using (var reader = new BinaryReader(new MemoryStream(buffer)))
            {
                reader.BaseStream.Position += 12; //skip header
                for (int i = 0; i < 144; i++)
                {
                    var count = reader.ReadInt16();
                    for (int j = 0; j < count; j++)
                    {
                        var obj = new O2FileElement
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
}