using System;
using System.Collections.Generic;
using System.IO;

namespace Silkroad.Materials
{
    public class objifo
    {
        private List<objinfo> objinfos = new List<objinfo>();

        public objifo()
        {
            Load();
        }

        public void Load()
        {
            byte[] file = Program.Map.GetFileBuffer("object.ifo");
            StreamReader reader = new StreamReader(new MemoryStream(file));
            reader.ReadLine();
            reader.ReadLine();
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                var info = new objinfo();
                info.id = int.Parse(line.Substring(0, 5));
                info.param = line.Substring(6, 10);
                info.path = line.Substring(18, line.Length - 19);
                objinfos.Add(info);
            }
            reader.Close();
        }

        public string GetPathByID(int id)
        {
            objinfo info = objinfos.Find(t => t.id == id);
            if (info != null)
            {
                return info.path;
            }
            else
            {
                throw new Exception("Unknown objID");
            }
        }

        public class objinfo
        {
            public int id;
            public string param;
            public string path;
        }
    }
}