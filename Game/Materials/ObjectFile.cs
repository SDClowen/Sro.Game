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
            reader.BaseStream.Position += 16; //skip header
            for (int i = 0; i < 142; i++)
            {
                short c = reader.ReadInt16();
                for (int x = 0; x < c; x++)
                {
                    mObject obj = new mObject();
                    obj.group = 0;
                    obj.uID = reader.ReadInt32();
                    obj.x = reader.ReadSingle();
                    obj.y = reader.ReadSingle();
                    obj.z = reader.ReadSingle();
                    reader.ReadBytes(2); // unkown
                    obj.angle = reader.ReadSingle();
                    obj.ID = reader.ReadInt32();
                    reader.ReadBytes(2);
                    obj.xsec = reader.ReadByte();
                    obj.ysec = reader.ReadByte();
                    objects.Add(obj);
                }
            }
        }
    }

    public struct mObject
    {
        public int group;
        public int uID;
        public float x;
        public float y;
        public float z;
        public float angle;
        public int ID;
        public byte xsec;
        public byte ysec;
    }
}