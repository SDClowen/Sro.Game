using Microsoft.Xna.Framework;

namespace Silkroad.Materials
{
    class BmtFile
    {
        public string Name;

        public Vector4 DiffuseColor;
        public Vector4 AmbientColor;
        public Vector4 SpecularColor;
        public Vector4 EmissiveColor;

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
