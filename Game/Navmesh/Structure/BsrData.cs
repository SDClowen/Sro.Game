using SharpDX;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Silkroad.Navmesh.Structure
{
    public struct BsrData
    {
        public uint Id;
        public string Filename;

        public BsrPointer Pointer;
        public uint Type;
        public string Name;

        public BoundingBox BoundingBox;
        public OrientedBoundingBox OrientedBoundingBox;
        public BmsData Mesh;
        public bool HasMesh;

        public System.Drawing.RectangleF GetBoundingBox(NavmeshEntry entry, float scale = 1.0f)
        {
            var result = new System.Drawing.RectangleF(entry.Position.X * scale, entry.Position.Y * scale,
                BoundingBox.Minimum.X * scale + BoundingBox.Maximum.X * scale, BoundingBox.Minimum.Z * scale + BoundingBox.Maximum.Z * scale);

            return result;
        }

        /// <summary>
        /// Loads the BSR.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public static BsrData Load(uint id)
        {
            var resourceLink = Manager.Links[id];
            var filePath = resourceLink.Path;

            var bsrData = new BsrData
            {
                Id = id,
                Filename = filePath
            };

            using (var stream = new FileStream(Path.Combine(Manager.Dir, filePath), FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    reader.BaseStream.Position += 12; //magic numbers
                    var pointer = MemoryMarshal.AsRef<BsrPointer>(reader.ReadBytes(32).AsSpan());
                    bsrData.Pointer = pointer;

                    reader.BaseStream.Position += 20; // skip unknown flags

                    bsrData.Type = reader.ReadUInt32();
                    bsrData.Name = reader.ReadStringEx();

                    reader.BaseStream.Position += 48; // skip unknown buffer

                    if (bsrData.Type == 0x20002 || bsrData.Type == 0x20003 || bsrData.Type == 0x20004)
                    {
                        reader.BaseStream.Position = bsrData.Pointer.BoundingBox;
                        var meshPath = reader.ReadStringEx();
                        var hasMesh = string.IsNullOrWhiteSpace(meshPath);

                        if (!hasMesh)
                            bsrData.Mesh = BmsData.Load(meshPath);

                        bsrData.HasMesh = hasMesh;
                        bsrData.BoundingBox = new BoundingBox(reader.ReadVector3DX(), reader.ReadVector3DX());
                        bsrData.OrientedBoundingBox = new OrientedBoundingBox(reader.ReadVector3DX(), reader.ReadVector3DX());
                    }
                }
            }

            return bsrData;
        }

    }
}