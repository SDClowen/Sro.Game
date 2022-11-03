using Microsoft.Xna.Framework;

namespace Silkroad.Navmesh.Structure
{
    public struct NavmeshRegionLink
    {
        public Vector2 PointA;
        public Vector2 PointB;

        public byte LineFlag;
        public byte LineSource;
        public byte LineDestination;

        public ushort CellSource;
        public ushort CellDestination;

        public short RegionSource;
        public short RegionDestination;
    }
}