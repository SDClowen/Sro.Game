using System;
using System.Collections.Generic;
using System.IO;

namespace Silkroad.Materials
{
    public class ObjIfo
    {
        private List<objinfo> _list = new();

        public ObjIfo()
        {
            var file = Program.Map.GetFileBuffer("object.ifo");
            using var reader = new StreamReader(new MemoryStream(file));
            reader.ReadLine();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var obj = new objinfo();
                if (!int.TryParse(line.AsSpan(0, 5), out obj.Id))
                    throw new FileLoadException();

                obj.IsSomething = line[6..16] == "0x00000001";
                obj.Path = line[18..^1];
                _list.Add(obj);
            }
        }

        public string GetPathByID(int id)
        {
            var info = _list.Find(t => t.Id == id);
            if (info != null)
                return info.Path;
            else
                throw new Exception("Unknown objID");
        }

        public class objinfo
        {
            public int Id;
            public bool IsSomething;
            public string Path;
        }
    }
}