using Microsoft.Xna.Framework;
using Silkroad.Core.Objects;

namespace Silkroad.Navmesh.Structure
{
    public struct NavmeshEntry
    {
        public uint Id;
        public Vector3 Position;
        public ushort CollisionFlag; //0x00 = No, 0xFFFF = Yes
        public float Rotation;
        public ushort UniqueId;
        public ushort Scale;
        public ushort EventZoneFlag;
        public Region Region;

        public byte[] MountPointData; //where you can enter the object (bridges etc..)
        public BsrData Resource;
    }
}