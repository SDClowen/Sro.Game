using System.IO;
using System.Text;

namespace Silkroad
{
    internal static class BinaryReaderExtensions
    {
        public static string ReadFixedSizeString(this BinaryReader reader, int size = 0)
        {
            if (size == 0)
                size = reader.ReadInt32();

            var buffer = reader.ReadBytes(size);
            int idx;
            for (idx = 0; idx < buffer.Length; idx++)
            {
                if (buffer[idx] == 0) 
                    break;
            }

            return Encoding.Default.GetString(buffer, 0, idx);
        }
    }
}
