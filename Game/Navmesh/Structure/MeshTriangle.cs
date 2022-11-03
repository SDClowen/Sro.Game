using System.Drawing;

namespace Silkroad.Navmesh.Structure
{
    public struct MeshTriangle
    {
        public int Index;
        public ushort PointA;
        public ushort PointB;
        public ushort PointC;
        public ushort unk00;
        public byte LineIndex;

        /// <summary>
        /// Gets the absolute position.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public PointF[] GetAbsolutePosition(NavmeshEntry entry, float scale)
        {
            var result = new PointF[3];

            var vectorA = entry.Resource.Mesh.Points[PointA].GetAbsolutePosition(entry, scale);
            var vectorB = entry.Resource.Mesh.Points[PointB].GetAbsolutePosition(entry, scale);
            var vectorC = entry.Resource.Mesh.Points[PointC].GetAbsolutePosition(entry, scale);

            result[0] = new PointF
            {
                X = vectorA.X,
                Y = vectorA.Y,
            };

            result[1] = new PointF
            {
                X = vectorB.X,
                Y = vectorB.Y,
            };
            result[2] = new PointF
            {
                X = vectorC.X,
                Y = vectorC.Y,
            };

            return result;
        }
    }
}