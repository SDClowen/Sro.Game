using System.Runtime.InteropServices;

namespace Silkroad.Pk2
{
    public enum EntryType : byte
    {
        None = 0,
        Folder = 1,
        File = 2
    }

    [StructLayout(LayoutKind.Sequential, Pack = 2, Size = 128)]
    public struct Entry
    {
        public EntryType Type;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 81)]
        public string Name;
        public long AccessTime;
        public long ModifyTime;
        public long CreateTime;  
        public long Position;          
        public int Size;               
        public long NextChain;
        public short Padding;          // So blowfish can be used directly on the structure
    }
}