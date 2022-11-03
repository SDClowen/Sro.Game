using System.Runtime.InteropServices;

namespace Silkroad.Navmesh.Structure
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BsrPointer
    {
        public uint Material;
        public uint Mesh;
        public uint Skeleton;
        public uint Animation;
        public uint MeshGroup;
        public uint AnimationGroup;
        public uint SoundEffect;
        public uint BoundingBox;
    }
}