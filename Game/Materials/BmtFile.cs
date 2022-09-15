namespace Silkroad.Materials
{
    public struct Color4
    {
        public float R;
        public float G;
        public float B;
        public float A;

        public Color4(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }
    }
    class BmtFile
    {
        public string Name;

        public Color4 Diffuse;
        public Color4 Ambient;
        public Color4 Specular;
        public Color4 Emissive;

        public float unkFloat16;
        public int unkUInt1;

        public string DiffuseMap;

        public float unkFloat17;

        public ushort unkUShort0;

        public bool IsNotWithinSameDirectory;

        public string NormalMap;

        public int unknownForNewSro;
    }
}
