using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Silkroad.Pk2
{
    public partial class Archive : IDisposable
    {
        #region Fields
        public string Key { get; private set; }
        public FileStream Stream;
        private Blowfish _blowfish;
        private Header _header;
        private byte[] _blowfishKey;
        public Pk2Folder RootFolder;
        private List<Pk2File> FilesCache = new(8192);

        #endregion Fields

        #region Properties
        
        public string FullPath;
        public string FileName => Path.GetFileNameWithoutExtension(FullPath);
        public string DirectoryName => Path.GetDirectoryName(FullPath);
        public long Size { get; private set; }

        #endregion Properties
        
        public Archive(string fullPath, string key = "169841")
        {
            try
            {
                FullPath = fullPath;
                SetBlowfishKey(key);

                OpenStream();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        public void OpenStream()
        {
            Stream = new FileStream(FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite, 4096, FileOptions.Asynchronous);
            Size = Stream.Length;
        }

        public void CloseStream()
        {
            Stream.Dispose();
        }

        public void SetBlowfishKey(string key)
        {
            Key = key;
            _blowfishKey = BlowfishKey.Generate(key);
            _blowfish = new Blowfish();
            _blowfish.Initialize(_blowfishKey);
        }

        public void SetBlowfishKey(byte[] key)
        {
            _blowfishKey = BlowfishKey.Generate(key);
            _blowfish = new Blowfish();
            _blowfish.Initialize(_blowfishKey);
        }

        public bool Load()
        {
            RootFolder = new Pk2Folder
            {
                Files = new List<Pk2File>(16),
                SubFolders = new List<Pk2Folder>(16)
            };

            var headerBytes = new byte[Marshal.SizeOf(typeof(Header))];
            Stream.Read(headerBytes, 0, headerBytes.Length);

            _header = BufferToStruct<Header>(headerBytes);

            Read(RootFolder);

            //Pk2Archive.Globals.View.menuItem10.Text = string.Format("{0} ms Files: {1} Sub: {2}", t.ElapsedMilliseconds, RootFolder.Files.Count, RootFolder.SubFolders.Count);

            GC.Collect();

            if (RootFolder.Files.Count == 0)
                return false;

            return true;
        }

        public Archive Seek(long position)
        {
            Stream.Seek(position, SeekOrigin.Begin);

            return this;
        }
        
        public void Read(Pk2Folder folder)
        {
            var stream = Stream;

            folder.IsPopulated = true;

            while (true)
            {
                var buffer = new byte[Marshal.SizeOf(typeof(EntryBlock))];
                Stream.Read(buffer, 0, buffer.Length); // 2560 = 20 entry

                if (_header.Encrypted == 1)
                    buffer = _blowfish.Decode(buffer);

                var entryBlock = BufferToStruct<EntryBlock>(buffer);

                foreach (var entry in entryBlock.Entries)
                {
                    if (entry.Name == "." || entry.Name == "..") continue;

                    switch (entry.Type)
                    {
                        case EntryType.Folder:
                            var current = new Pk2Folder
                            {
                                Entry = entry,
                                Parent = folder,
                                Files = new List<Pk2File>(16),
                                SubFolders = new List<Pk2Folder>(16)
                            };

                            stream.Position = entry.Position;
                            Read(current);

                            folder.SubFolders.Add(current);
                            break;

                        case EntryType.File:
                        case EntryType.None:
                            var pk2File = new Pk2File(this) { Parent = folder, Entry = entry };
                            FilesCache.Add(pk2File);
                            folder.Files.Add(pk2File);
                            break;

                    }
                }

                var next = entryBlock.Entries[19];
                if (next.NextChain > 0)
                    stream.Position = next.NextChain;
                else
                    break;
            }
        }

        private T BufferToStruct<T>(byte[] data)
        {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            try
            {
                return (T)Convert.ChangeType(Marshal.PtrToStructure(gch.AddrOfPinnedObject(), typeof(T)), typeof(T));
            }
            finally
            {
                gch.Free();
            }
        }

        #region Dispose

        public void Dispose()
        {
            RootFolder = null;
            Stream?.Dispose();
            Stream = null;
            FullPath = null;
            _blowfishKey = null;

            GC.SuppressFinalize(this);
        }

        #endregion Dispose
    }
}