using Microsoft.Xna.Framework;
using System;

namespace Silkroad
{
    internal static class GeometryExtensions
    {
        public static Vector2 ToVector2(this Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector2 ToVector2(this SharpDX.Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector3 ToVector3(this Vector2 v)
        {
            return new Vector3(v.X, 0f, v.Y); //TO DO: navmesh height for y position
        }

        public static Vector3 ToVector3(this Vector2 v, float y)
        {
            return new Vector3(v.X, y, v.Y);
        }

        public static Vector2 Rotated(this Vector2 v, float angle)
        {
            var c = Math.Cos(angle);
            var s = Math.Sin(angle);

            return new Vector2((float)(v.X * c - v.Y * s), (float)(v.Y * c + v.X * s));
        }
    }
}
