using System.Runtime.InteropServices;

namespace Silkroad.Pk2
{
    [StructLayout(LayoutKind.Sequential, Pack = 2, Size = 256)]
    public class Header
    {
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 30)]
        public readonly string Name;
        public readonly int Version;
        public readonly byte Encrypted;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public readonly byte[] Verify;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 205)]
        public readonly byte[] Reserved;
    }
}