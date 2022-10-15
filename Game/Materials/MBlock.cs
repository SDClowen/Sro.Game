namespace Silkroad.Materials
{
    public struct MapMeshCell
    {
        /// <summary>
        /// The Cell height
        /// </summary>
        public float Height { get; internal set; }

        /// <summary>
        /// Consists of 10-Bit TextureID & 6-Bit Flags (Blur, Shine, ...)
        /// </summary>
        public byte Texture { get; internal set; }

        /// <summary>
        /// lighting direction indicator?
        /// </summary>
        public ushort Brightness { get; internal set; }
    }

    public enum BlockDensity
    {
        Water,
        Ice, 
        Solid = 0xFF
    }

    public class MapMeshBlock
    {
        /// <summary>
        /// The block name
        /// related to environment.ifo?
        /// </summary>
        public string Name;

        /// <summary>
        /// Map cells
        /// </summary>
        public MapMeshCell[,] Cells;

        /// <summary>
        /// The block density
        /// </summary>
        public BlockDensity Density;

        /// <summary>
        /// Related to Block.Density (see screens)
        /// </summary>
        public byte unkByte0;

        /// <summary>
        /// The Sea Level
        /// </summary>
        public float SeaLevel;

        /// <summary>
        /// This is unknown extra data for each cell, most of them where 0 and 6 or similar.
        /// </summary>
        public (byte CellExtraMin, byte CellExtraMax)[] CellExtra;

        /// <summary>
        /// The HeightMin
        /// </summary>
        public float HeightMin;

        /// <summary>
        /// The HeightMax
        /// </summary>
        public float HeightMax;

        /// <summary>
        /// The unkBuffer0
        /// </summary>
        public char[] unkBuffer0;
    }
}