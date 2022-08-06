namespace Silkroad.Materials
{
    public struct Color4
    {
        /// <summary>
        /// The red component of the color.
        /// </summary>
        public float Red;

        /// <summary>
        /// The green component of the color.
        /// </summary>
        public float Green;

        /// <summary>
        /// The blue component of the color.
        /// </summary>
        public float Blue;

        /// <summary>
        /// The alpha component of the color.
        /// </summary>
        public float Alpha;

        public Color4(float r, float g, float b, float a) : this()
        {
            Red = r;
            Green = g;
            Blue = b;
            Alpha = a;
        }

        public override string ToString()
        {
            return $"R:{Red} G:{Green} B:{Blue} A:{Alpha}";
        }
    }
}
