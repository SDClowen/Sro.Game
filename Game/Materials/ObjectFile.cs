using Microsoft.Xna.Framework;
using Silkroad.Components;
using System.Collections.Generic;
using System.IO;

namespace Silkroad.Materials
{
    internal class ObjectFile
    {
        private BinaryReader reader;
        public List<mObject> objects;

        public ObjectFile(byte[] file)
        {
            reader = new BinaryReader(new MemoryStream(file));
            objects = new List<mObject>();
            ParseObjectFile(reader);
        }

        private void ParseObjectFile(BinaryReader reader)
        {
            reader.BaseStream.Position += 12; //skip header
            for (int i = 0; i < 144; i++)
            {
                var count = reader.ReadInt16();
                for (int j = 0; j < count; j++)
                {
                    var obj = new mObject();
                    obj.group = i;
                    obj.uID = reader.ReadInt32();
                    obj.Position = new Vector3(
                        reader.ReadSingle(),
                        reader.ReadSingle(),
                        reader.ReadSingle()
                    );
                    reader.ReadBytes(2); // unkown
                    obj.angle = reader.ReadSingle();
                    obj.ID = reader.ReadInt32();
                    reader.ReadBytes(2);
                    obj.xsec = reader.ReadByte();
                    obj.ysec = reader.ReadByte();

                    obj.Position.X += (obj.xsec - Terrain.XSector) * 1920;
                    obj.Position.Z += (obj.ysec - Terrain.YSector) * 1920;

                    objects.Add(obj);
                }
            }

            //objects.Reverse();
        }
    }

    public struct mObject
    {
        public int ID;
        public int group;
        public int uID;
        public byte xsec;
        public byte ysec;
        public Vector3 Position;
        public float angle;
    }
}