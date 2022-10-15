using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace Silkroad.Materials
{
    public class Tile2D
    {
        public List<Tile2DObj> Objs = new();

        public struct Tile2DObj
        {
            public int Id;
            public string Param;
            public string Area;
            public string Path;
            public Vector2[] Points;
        }

        public Tile2D()
        {
            var file = Program.Map.GetFileBuffer("tile2d.ifo");
            using var reader = new StreamReader(new MemoryStream(file));
            reader.ReadLine();
            reader.ReadLine();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                
                var obj = new Tile2DObj();
                if (!int.TryParse(line.AsSpan(0, 5), out obj.Id))
                    throw new FileLoadException();
                
                obj.Param = line[6..16];
                var split = line[18..^1].Split('"', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                
                obj.Area = split[0];
                obj.Path = split[1];
                if(split.Length > 2)
                {
                    var points = split[2].Replace(" ", "").Replace("{", "").Split('}', StringSplitOptions.TrimEntries);
                    obj.Points = new Vector2[points.Length];

                    for (int i = 0; i < points.Length; i++)
                    {
                        var p = points[i].Split(',');

                        if (p.Length <= 0)
                            throw new FileLoadException("Couldn't parse the point tab on tile2d.ifo");

                        if (!float.TryParse(p[0], out var x))
                            throw new FileLoadException();

                        if (!float.TryParse(p[1], out var y))
                            throw new FileLoadException();

                        obj.Points[i] = new Vector2(x, y);
                    }
                }

                Objs.Add(obj);
            }
        }
    }
}
