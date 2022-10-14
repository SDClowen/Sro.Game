using System.IO;

namespace Silkroad.Materials
{
    internal class BmtManager
    {
        public BmtFile[] Entries;
        public string Path;

        public void Parse(byte[] buffer, string path)
        {
            Path = path;
            using (var stream = new BinaryReader(new MemoryStream(buffer)))
            {
                var header = new string(stream.ReadChars(12));
                var entryCount = stream.ReadUInt32();
                Entries = new BmtFile[entryCount];

                for (var i = 0; i < entryCount; i++)
                {
                    var entry = new BmtFile();

                    var nameLen = stream.ReadInt32();
                    entry.Name = new string(stream.ReadChars(nameLen));
                    entry.DiffuseColor = new(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());
                    entry.AmbientColor = new(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());
                    entry.SpecularColor = new(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());
                    entry.EmissiveColor = new(stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle(), stream.ReadSingle());
                    entry.unkFloat16 = stream.ReadSingle();
                    entry.unkUInt1 = stream.ReadInt32();
                    entry.DiffuseMap = new string(stream.ReadChars(stream.ReadInt32()));
                    entry.unkFloat17 = stream.ReadSingle(); //color??
                    entry.unkUShort0 = stream.ReadUInt16();
                    entry.IsNotWithinSameDirectory = stream.ReadBoolean();

                    if ((entry.unkUInt1 & 8192) == 8192)
                    {
                        var normalMapLen = stream.ReadInt32();
                        entry.NormalMap = new string(stream.ReadChars(normalMapLen));
                        entry.unknownForNewSro = stream.ReadInt32();
                    }

                    Entries[i] = entry;
                }
            }
        }
    }
}
