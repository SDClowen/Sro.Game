using Microsoft.Xna.Framework;
using System;

namespace Silkroad.Navmesh.Structure
{
    public struct MeshPoint
    {
        #region Fields

        public Vector3 Position;
        public byte Flag;

        #endregion Fields

        #region Methods

        private Vector2 RotateRadians(Vector2 v, float angle)
        {
            var sinCos = MathF.SinCos(angle);

            return new Vector2(sinCos.Cos * v.X - sinCos.Sin * v.Y, sinCos.Sin * v.X + sinCos.Cos * v.Y);
        }

        /// <summary>
        /// Gets the absolute position.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <returns></returns>
        public Vector2 GetAbsolutePosition(NavmeshEntry entry, float scale = 1, bool flip = false)
        {
            var vector2 = new Vector2(Position.X, Position.Y);

            if (!flip)
                vector2 = RotateRadians(vector2, entry.Rotation);
            else
                vector2 = RotateRadians(vector2, -1 * entry.Rotation);

            vector2.X += entry.Position.X;
            vector2.Y += entry.Position.Z;

            vector2.X *= scale;
            vector2.Y *= scale;

            return vector2;
        }

        /// <summary>
        /// To the position.
        /// </summary>
        /// <param name="entry">The entry.</param>
        /// <param name="scale">The scale.</param>
        /// <returns></returns>
        public Vector4 ToGamePosition(NavmeshEntry entry, float scale)
        {
            var offsetVector = GetAbsolutePosition(entry, scale);

            var position = new Vector4
            {
                W = entry.Region,
                X = offsetVector.X,
                Y = offsetVector.Y,
                Z = 0,
            };

            return position;
        }

        #endregion Methods
    }
}