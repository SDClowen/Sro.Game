using Microsoft.Xna.Framework;

namespace Silkroad.Navmesh.Structure
{
    public struct NavmeshCellLink
    {
        public Vector2 PointA;
        public Vector2 PointB;

        public byte LineFlag;
        public byte LineSource;
        public byte LineDestination;

        public ushort CellSource;
        public ushort CellDestination;
    }
}