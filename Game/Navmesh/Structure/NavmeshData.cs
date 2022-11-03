using Silkroad.Core.Objects;
using System.IO;
using System.Linq;

namespace Silkroad.Navmesh.Structure
{
    public class NavmeshData
    {
        /// <summary>
        /// The region
        /// </summary>
        public Region Region { get; set; }

        /// <summary>
        /// Gets or sets the entries.
        /// </summary>
        /// <value>
        /// The entries.
        /// </value>
        public NavmeshEntry[] Entries { get; set; }

        /// <summary>
        /// Gets or sets the cells.
        /// </summary>
        /// <value>
        /// The cells.
        /// </value>
        public NavmeshCell[] Cells { get; set; }

        /// <summary>
        /// Gets or sets the region links.
        /// </summary>
        /// <value>
        /// The region links.
        /// </value>
        public NavmeshRegionLink[] RegionLinks { get; set; }

        /// <summary>
        /// Gets or sets the cell links.
        /// </summary>
        /// <value>
        /// The cell links.
        /// </value>
        public NavmeshCellLink[] CellLinks { get; set; }

        /// <summary>
        /// Gets or sets the height map.
        /// </summary>
        /// <value>
        /// The height map.
        /// </value>
        public float[] HeightMap { get; set; }

        /// <summary>
        /// Get height at with specific x and y
        /// </summary>
        public float GetHeightAt(float x, float y)
        {
            x /= 20;
            y /= 20;

            var id = (int)(y * 97 + x);
            if (id > HeightMap.Length)
            {
                Log.Warn($"NavmeshData Wtf height map idx big than {id}/{HeightMap.Length}");
                return 0;
            }

            return HeightMap[id];
        }


        /// <summary>
        /// Loads the mesh.
        /// </summary>
        public static bool TryLoad(string file, ushort regionId, out NavmeshData navmeshData)
        {
            navmeshData = new NavmeshData
            {
                Region = regionId
            };

            using (var stream = new FileStream(file, FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    reader.ReadChars(12); //magic number (JMXVNVM 1000)

                    navmeshData.LoadEntries(reader);
                    navmeshData.LoadCells(reader); //Zone 1
                    navmeshData.LoadRegionLinks(reader); //Zone 2
                    navmeshData.LoadCellLinks(reader); //Zone 3

                    try
                    {
                        reader.BaseStream.Position += 0x9000;
                        //ReadTextureMap(reader); //Unrelevant data.
                        navmeshData.LoadHeightMap(reader);
                    }
                    catch
                    {
                        return false;
                    }
                    //TranslatePoints(ref navmeshData);

                    return true;
                }
            }
        }

        /// <summary>
        /// Loads the entries.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="navmeshData">The navmesh data.</param>
        private void LoadEntries(BinaryReader reader)
        {
            var entryCount = reader.ReadUInt16();
            Entries = new NavmeshEntry[entryCount];
            for (var i = 0; i < entryCount; i++)
            {
                var entry = new NavmeshEntry
                {
                    Id = reader.ReadUInt32(),
                    Position = reader.ReadVector3(),
                    CollisionFlag = reader.ReadUInt16(),
                    Rotation = reader.ReadSingle(),
                    UniqueId = reader.ReadUInt16(),
                    Scale = reader.ReadUInt16(),
                    EventZoneFlag = reader.ReadUInt16(),
                    Region = reader.ReadUInt16()
                };

                var mountPointCount = reader.ReadUInt16();
                entry.MountPointData = new byte[mountPointCount];
                for (var iM = 0; iM < mountPointCount; iM++)
                    reader.ReadBytes(6);

                //Load the actual model (bsr->bms)
                if (Manager.Links.Length >= entry.Id)
                {
                    var existingEntry = Entries.FirstOrDefault(e => e.Resource.Id == entry.Id);
                    entry.Resource = existingEntry.Resource.Filename == null ? BsrData.Load(entry.Id) : existingEntry.Resource;
                }

                Entries[i] = entry;
            }
        }

        /// <summary>
        /// Loads the cells.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="navmeshData">The navmesh data.</param>
        private void LoadCells(BinaryReader reader)
        {
            var cellCount = reader.ReadUInt32();
            reader.ReadUInt32(); //Cell extra count?!

            Cells = new NavmeshCell[cellCount];
            for (var i = 0; i < cellCount; i++)
            {
                var cell = new NavmeshCell
                {
                    Rectangle = reader.ReadRectangleF()
                    /*Min = reader.ReadVector2(),
                    Max = reader.ReadVector2()*/
                };

                var cellEntryCount = reader.ReadByte();
                cell.Entries = new ushort[cellEntryCount];

                for (var j = 0; j < cellEntryCount; j++)
                    cell.Entries[j] = reader.ReadUInt16();

                Cells[i] = cell;
            }
        }

        /// <summary>
        /// Loads the region links.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="navmeshData">The navmesh data.</param>
        private void LoadRegionLinks(BinaryReader reader)
        {
            var regionLinkCount = reader.ReadUInt32();
            RegionLinks = new NavmeshRegionLink[regionLinkCount];
            for (var i = 0; i < regionLinkCount; i++)
            {
                RegionLinks[i] = new NavmeshRegionLink
                {
                    PointA = reader.ReadVector2(),
                    PointB = reader.ReadVector2(),
                    LineFlag = reader.ReadByte(),
                    LineSource = reader.ReadByte(),
                    LineDestination = reader.ReadByte(),
                    CellSource = reader.ReadUInt16(),
                    CellDestination = reader.ReadUInt16(),
                    RegionSource = reader.ReadInt16(),
                    RegionDestination = reader.ReadInt16(),
                };
            }
        }

        /// <summary>
        /// Loads the cell links.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="navmeshData">The navmesh data.</param>
        private void LoadCellLinks(BinaryReader reader)
        {
            var cellLinkCount = reader.ReadUInt32();
            CellLinks = new NavmeshCellLink[cellLinkCount];
            for (var i = 0; i < cellLinkCount; i++)
            {
                CellLinks[i] = new NavmeshCellLink
                {
                    PointA = reader.ReadVector2(),
                    PointB = reader.ReadVector2(),
                    LineFlag = reader.ReadByte(),
                    LineSource = reader.ReadByte(),
                    LineDestination = reader.ReadByte(),
                    CellSource = reader.ReadUInt16(),
                    CellDestination = reader.ReadUInt16()
                };
            }
        }

        /// <summary>
        /// Loads the height map.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="navmeshData">The navmesh data.</param>
        private void LoadHeightMap(BinaryReader reader)
        {
            HeightMap = new float[9409];
            for (var i = 0; i < 9409; i++)
                HeightMap[i] = reader.ReadSingle();
        }

        /// <summary>
        /// Reads the texture map.
        /// </summary>
        /// <param name="reader">The reader.</param>
        private static void ReadTextureMap(BinaryReader reader)
        {
            //Texture map (useless here)
            for (var x = 0; x < 96; x++)
            {
                for (var y = 0; y < 96; y++)
                {
                    reader.ReadUInt16();
                    reader.ReadUInt16();
                    reader.ReadUInt16();
                    reader.ReadUInt16(); //Texture id...
                }
            }
        }

    }
}