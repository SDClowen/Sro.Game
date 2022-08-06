using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Silkroad.Pk2
{
    public class Pk2Folder
    {
        public bool IsPopulated;
        public Entry Entry;
        public Pk2Folder Parent { get; set; }
        public List<Pk2File> Files { get; set; }
        public List<Pk2Folder> SubFolders { get; set; }
        public string FullPath => string.IsNullOrEmpty(Entry.Name) ? string.Empty : Path.Combine(Parent.FullPath, Entry.Name);
        
        public void Extract(string filePath, bool extractchilds = true)
        {
            foreach (var item in Files)
                item.Extract(filePath);

            if (extractchilds)
                foreach (var item in SubFolders)
                    item.Extract(filePath);
        }
    }
}