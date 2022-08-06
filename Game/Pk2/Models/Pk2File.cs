using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;

namespace Silkroad.Pk2
{
    public class Pk2File
    {
        private Archive _archive;
        public Entry Entry { get; set; }
        public Pk2Folder Parent { get; set; }
        public string Extension => Path.GetExtension(Entry.Name);
        public string FullPath => Path.Combine(Parent.FullPath, Entry.Name);

        public Pk2File(Archive archive)
            => _archive = archive;

        public string Extract(string fullName, bool toTemp = false)
        {
            var path = FullPath;

            if (!toTemp)
            {
                if (path.Length > 0)
                    while (path[0] == Path.DirectorySeparatorChar)
                        path = path.Remove(0, 1);

                path = Path.Combine(fullName, path);
            }
            else
                path = Path.Combine(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()), Entry.Name);

            if (!Directory.Exists(Path.GetDirectoryName(path)))
                Directory.CreateDirectory(Path.GetDirectoryName(path));

            File.WriteAllBytes(path, GetData());

            return path;
        }

        public byte[] GetData()
        {
            if (Entry.Type != EntryType.File)
                throw new InvalidOperationException("It's impossible to read data from a directory or from a deleted file.");

            var buffer = new byte[Entry.Size];

            _archive.Stream.Seek(Entry.Position, SeekOrigin.Begin);
            _archive.Stream.Read(buffer, 0, buffer.Length);

            return buffer;
        }

        public byte[] ToByteArray()
        {
            var returnStruct = this;
            int size = Marshal.SizeOf(returnStruct);
            byte[] arr = new byte[size];

            IntPtr ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(returnStruct, ptr, true);
            Marshal.Copy(ptr, arr, 0, size);
            Marshal.FreeHGlobal(ptr);
            return arr;
        }

        /// <summary>
        /// Reads all text of this file.
        /// </summary>
        /// <returns></returns>
        public string ReadAllText()
        {
            var buffer = GetData();
            if (buffer == null)
                return null;

            using (var reader = new StreamReader(new MemoryStream(buffer)))
            {
                return reader.ReadToEnd();
            }
        }

        public override string ToString()
        {
            return Entry.Name;
        }
    }
}