using SharpDX;
using Silkroad.Core.Objects;

namespace Silkroad.Navmesh
{
    public enum GameRegionType
    {
        None,
        FIELD,
        TOWN
    }

    public struct GameRegionInfo
    {

        public GameRegionType Type;
        public Region Region;
        public string Name;
        public RectangleF Rectangle;
    }
}
