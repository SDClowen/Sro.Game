using SharpDX.Text;
using System.IO;

namespace Silkroad.Materials
{
    /// <summary>
    /// JMXVMAPM1000
    /// Map\*.m file
    /// </summary>
    internal class MFile
    {
        public MapMeshBlock[,] Blocks = new MapMeshBlock[6, 6];

        public MFile(byte[] buffer)
        {
            using var reader = new BinaryReader(new MemoryStream(buffer));
            reader.BaseStream.Position += 12; //skip header

            for (int xBlock = 0; xBlock < 6; xBlock++)
            {
                for (int yBlock = 0; yBlock < 6; yBlock++)
                {
                    var block = new MapMeshBlock
                    {
                        Name = Encoding.UTF8.GetString(reader.ReadBytes(6)),
                        Cells = new MapMeshCell[17, 17],
                    };

                    for (int y = 0; y < 17; y++)
                    {
                        for (int x = 0; x < 17; x++)
                        {
                            block.Cells[y, x] = new MapMeshCell
                            {
                                Height = reader.ReadSingle(),
                                Texture = reader.ReadByte(),
                                Brightness = reader.ReadUInt16()
                            };
                        }
                    }

                    block.Density = (BlockDensity)reader.ReadByte();
                    block.unkByte0 = reader.ReadByte();
                    block.SeaLevel = reader.ReadSingle();

                    block.CellExtra = new (byte CellExtraMin, byte CellExtraMax)[256];
                    for (int i = 0; i < 256; i++)
                        block.CellExtra[i] = new(reader.ReadByte(), reader.ReadByte());

                    block.HeightMax = reader.ReadSingle();
                    block.HeightMin = reader.ReadSingle();
                    
                    block.unkBuffer0 = reader.ReadChars(20);

                    Blocks[xBlock, yBlock] = block;
                }
            }
        }
    }
}
