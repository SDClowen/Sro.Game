using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Silkroad.Materials
{
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
