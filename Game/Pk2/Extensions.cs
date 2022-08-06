using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace Silkroad.Pk2
{
    public partial class Archive
    {
        public Pk2Folder GetFolder3/*ByFullPath*/(string fullPath, Pk2Folder folder = null, StringComparison comparison = StringComparison.InvariantCultureIgnoreCase)
        {
            if (string.IsNullOrEmpty(fullPath))
                return RootFolder;

            if (folder == null)
                folder = RootFolder;

            var found = folder.SubFolders.Find(p => string.Equals(p.FullPath, fullPath, comparison));

            if (found == null)
            {
                foreach (var folderItem in folder.SubFolders)
                {
                    found = GetFolder3/*ByFullPath*/(fullPath, folderItem, comparison);
                    if (found != null)
                        break;
                }
            }

            return found;
        }
        public Pk2Folder GetFolder(string fpath, Pk2Folder pk2Folder = null)
        {
            if (string.IsNullOrEmpty(fpath))
                return RootFolder;

            Pk2Folder tempfolder = null;
            foreach (var item in fpath.Split(new[] { Path.DirectorySeparatorChar }, StringSplitOptions.RemoveEmptyEntries ))
            {
                tempfolder = tempfolder ?? RootFolder;

                if (!tempfolder.IsPopulated)
                    Seek(tempfolder.Entry.Position).Read(tempfolder);

                tempfolder = tempfolder.SubFolders.Find(p => p.Entry.Name.Equals(item, StringComparison.InvariantCultureIgnoreCase));
            }

            return tempfolder;
        }

        public Pk2File GetFile(string path)
        {
            var n = Path.GetDirectoryName(path);
            var folder = GetFolder(n);

            if (!folder.IsPopulated)
                Seek(folder.Entry.Position).Read(folder);

            return folder?
                .Files.Find(p => p.Entry.Name.Equals(Path.GetFileName(path), StringComparison.InvariantCultureIgnoreCase));
        }

        public Stream GetFileStream(string fileName)
        {
            var file = this.GetFile(fileName);
            if (file == null)
                throw new Exception($@"{fileName} not found!");

            return new MemoryStream(file.GetData());
        }

        private IEnumerable<string> ReadAllLines(byte[] buffer)
        {
            using (var reader = new StreamReader(new MemoryStream(buffer)))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                    yield return line;
            }
        }

        public IEnumerable<string> GetFileContext(string filename)
        {
            var file = GetFile(filename);

            return file == null ? null : ReadAllLines(file.GetData());
        }

        public byte[] GetFileBuffer(string path)
        {
            var file = GetFile(path);
            if (file == null)
                return null;

            return file.GetData();
        }
    }
}