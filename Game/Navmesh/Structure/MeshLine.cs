namespace Silkroad.Navmesh.Structure
{
    public struct MeshLine
    {
        public ushort PointA;
        public ushort PointB;
        public ushort NeighbourA;
        public ushort NeighbourB;
        public byte Flag;
        public byte unk00;
    }
}