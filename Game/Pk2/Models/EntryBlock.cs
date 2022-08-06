using System.Runtime.InteropServices;

namespace Silkroad.Pk2
{
    [StructLayout(LayoutKind.Sequential, Size = 2560)]
    public struct EntryBlock
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public readonly Entry[] Entries;
    }
}