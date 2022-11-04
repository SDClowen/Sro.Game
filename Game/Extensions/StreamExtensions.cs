using Microsoft.Xna.Framework;
using System.IO;
using System.Text;

namespace Silkroad
{
    internal static class StreamExtensions
    {
        /// <summary>
        /// Reads the joymax string.
        /// korean codepage: 949
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static string ReadStringEx(this BinaryReader reader, int size = 0)
        {
            if (size == 0)
                size = reader.ReadInt32();

            return Encoding.UTF8.GetString(reader.ReadBytes(size));
        }

        /// <summary>
        /// Reads the vector3.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static Vector3 ReadVector3(this BinaryReader reader)
        {
            return new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Reads the vector3.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static SharpDX.Vector3 ReadVector3DX(this BinaryReader reader)
        {
            return new SharpDX.Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Reads the vector3.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static Vector2 ReadVector2(this BinaryReader reader)
        {
            return new Vector2(reader.ReadSingle(), reader.ReadSingle());
        }

        /// <summary>
        /// Reads the rectangle f.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns></returns>
        public static SharpDX.RectangleF ReadRectangleF(this BinaryReader reader)
        {
            var x1 = reader.ReadSingle();
            var y1 = reader.ReadSingle();
            var x2 = reader.ReadSingle();
            var y2 = reader.ReadSingle();
            return new SharpDX.RectangleF(x1, y1, x2 - x1, y2 - y1);
        }
    }
}
