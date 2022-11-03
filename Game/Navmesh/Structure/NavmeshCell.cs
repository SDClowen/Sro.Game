using SharpDX;

namespace Silkroad.Navmesh.Structure
{
    public struct NavmeshCell
    {
        //public Vector2 Min;
        //public Vector2 Max;
        public RectangleF Rectangle;

        public ushort[] Entries;
    }
}