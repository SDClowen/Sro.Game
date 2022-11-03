using Microsoft.Xna.Framework;
using Silkroad.Core.Objects;
using Silkroad.Navmesh.Structure;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Silkroad.Navmesh
{
    public class Manager
    {
        public static readonly string Dir = Path.Combine(Environment.CurrentDirectory, "client", "data");

        public static List<NavmeshData> Navmeshes = new(2500);
        public static List<GameRegionInfo> RegionInfos = new(3000);
        public static NavmeshLinkBsr[] Links { get; private set; }

        public bool IsTown(Region region)
            => RegionInfos.Any(p => p.Type == GameRegionType.TOWN && p.Region == region);

        private static void ParseRegionInfo()
        {
            var lines = File.ReadAllLines(Path.Combine(Dir, "regioninfo.txt"));

            foreach (var line in lines)
            {
                var split = line.Split('\t', StringSplitOptions.RemoveEmptyEntries);

                if (!Enum.TryParse<GameRegionType>(split[0].Remove(0, 1), out var regionType))
                    continue;

                var regionInfo = new GameRegionInfo();

                if (!byte.TryParse(split[2], out regionInfo.Region.X))
                    continue;

                if (!byte.TryParse(split[3], out regionInfo.Region.Y))
                    continue;

                regionInfo.Name = split[4];

                if (regionInfo.Name == "RECT")
                {
                    if (!float.TryParse(split[5], out var rectX))
                        continue;

                    if (!float.TryParse(split[6], out var rectY))
                        continue;

                    if (!float.TryParse(split[7], out var rectWidth))
                        continue;

                    if (!float.TryParse(split[8], out var rectHeight))
                        continue;

                    regionInfo.Rectangle = new SharpDX.RectangleF(rectX, rectY, rectWidth, rectHeight);
                }

                RegionInfos.Add(regionInfo);
            }

            Log.Success($"Loaded {RegionInfos.Count} region informations!");
        }

        public static void LoadResourceLinks()
        {
            using (var stream = new StreamReader(Path.Combine(Dir, "navmesh", "object.ifo")))
            {
                if (stream.ReadLine() != "JMXVOBJI1000")
                {
                    Log.Error(@"object.ifo corrupted!");
                    return;
                }

                var count = Convert.ToInt32(stream.ReadLine());
                Links = new NavmeshLinkBsr[count];

                while (!stream.EndOfStream)
                {
                    var data = stream.ReadLine();

                    var link = new NavmeshLinkBsr();
                    if (!uint.TryParse(data.Substring(0, 5), out link.Id))
                        continue;

                    link.Unknown = data.Substring(6, 10).EndsWith("1");
                    link.Path = data.Substring(18, data.Length - 19);

                    Links[link.Id] = link;
                }

                Log.Success($"Loaded {count} navmesh resource links!");
            }
        }

        public static void Initialize()
        {
            ParseRegionInfo();
            LoadResourceLinks();

            var counter = 0;

            var navFiles = Directory.GetFiles(Path.Combine(Dir, "navmesh"), "*.nvm");
            foreach (var navFile in navFiles)
            //foreach (var regionInfo in RegionInfos)
            {
                var region = ushort.Parse(Path.GetFileNameWithoutExtension(navFile).Replace("nv_", ""), NumberStyles.HexNumber);

                var calc = 100f / navFiles.Length * counter;
                //var calc = counter * 100f / RegionInfos.Count;
                Console.Title = $"Silkroad.Game - Loading: {calc:0.0}%";

                //var navFile = Path.Combine(Dir, "navmesh", $"nv_{(ushort)regionInfo.Region.Id:x}.nvm");

                //if (!File.Exists(navFile))
                //continue;

                if (!NavmeshData.TryLoad(navFile, region, out var navmeshData))
                {
                    Log.Warn($"{Path.GetFileName(navFile)} cant be loaded!");
                    continue;
                }

                counter++;

                Navmeshes.Add(navmeshData);
            }

            Log.Success($"Loaded {counter} navmeshes!");
        }

        public static bool TryGetHeightAt(Region region, Vector3 position, out float value)
        {
            value = 0;
            var navmeshData = Navmeshes.Find(p => p.Region == region);
            if (navmeshData == null)
                return false;

            value = navmeshData.GetHeightAt(position.X, position.Y);
            return true;
        }

        public static bool LineSegmentIntersection(
            Vector2 Line1_A,
            Vector2 Line1_B,
            Vector2 Line2_A,
            Vector2 Line2_B,
            out Vector2 found)
        {
            found = Vector2.Zero;

            // Fail if either line segment is zero-length.
            if (Line1_A.X == Line1_B.X && Line1_A.Y == Line1_B.Y ||
                Line2_A.X == Line2_B.X && Line2_A.Y == Line2_B.Y)
                return false;

            // Translate the system so that point A is on the origin.
            Line1_B.X -= Line1_A.X; Line1_B.Y -= Line1_A.Y;
            Line2_A.X -= Line1_A.X; Line2_A.Y -= Line1_A.Y;
            Line2_B.X -= Line1_A.X; Line2_B.Y -= Line1_A.Y;

            // Discover the length of segment A-B.
            float distAB = (float)Math.Sqrt(Line1_B.X * Line1_B.X + Line1_B.Y * Line1_B.Y);

            // Rotate the system so that point B is on the positive X axis.
            float theCos = Line1_B.X / distAB;
            float theSin = Line1_B.Y / distAB;
            float newX = Line2_A.X * theCos + Line2_A.Y * theSin;
            Line2_A.Y = Line2_A.Y * theCos - Line2_A.X * theSin; Line2_A.X = newX;
            newX = Line2_B.X * theCos + Line2_B.Y * theSin;
            Line2_B.Y = Line2_B.Y * theCos - Line2_B.X * theSin; Line2_B.X = newX;

            // Fail if segment C-D doesn't cross line A-B.
            if (Line2_A.Y < 0.0f && Line2_B.Y < 0.0f || Line2_A.Y >= 0.0f && Line2_B.Y >= 0.0f)
                return false;

            // Discover the position of the intersection point along line A-B.
            float ABpos = Line2_B.X + (Line2_A.X - Line2_B.X) * Line2_B.Y / (Line2_B.Y - Line2_A.Y);

            // Fail if segment C-D crosses line A-B outside of segment A-B.
            if (ABpos < 0.0f || ABpos > distAB) return false;

            found.X = Line1_A.X + ABpos * theCos;
            found.Y = Line1_A.Y + ABpos * theSin;

            return true;
        }

        public static bool IsCollided(short region, Vector3 from, Vector3 to, out Vector3 result)
        {
            var nav = Navmeshes.FirstOrDefault(p => p.Region == region);
            if (nav == null || nav.Entries == null)
            {
                result = from;
                return true;
            }

            bool on_object = false;
            float object_height = 0f;

            foreach (var entry in nav.Entries)
            {
                var box = entry.Resource.BoundingBox;
                if ((box.Minimum.X <= from.X && box.Maximum.X >= from.X && box.Minimum.Y <= from.Y && box.Maximum.Y >= from.Y)
                    || (box.Minimum.X <= to.X && box.Maximum.X >= to.X && box.Minimum.Y <= to.Y && box.Maximum.Y >= to.Y))
                {
                    if (entry.Resource.HasMesh)
                    {
                        //if (entry.Resource.Mesh.BBox.Contains(from) > ContainmentType.Disjoint || entry.Resource.Mesh.BBox.Contains(to) > ContainmentType.Disjoint)
                        {
                            //Console.WriteLine("bbox test ok");
                            foreach (var line in entry.Resource.Mesh.OutLines)
                            {
                                if (from.Y >= Math.Min(entry.Resource.Mesh.Points[line.PointA].Position.Y, entry.Resource.Mesh.Points[line.PointB].Position.Y) - 100 && from.Y <= Math.Max(entry.Resource.Mesh.Points[line.PointA].Position.Y, entry.Resource.Mesh.Points[line.PointB].Position.Y) + 100)
                                {
                                    Vector2 res;
                                    if (LineIntersectsLine(
                                        entry.Resource.Mesh.Points[line.PointA].Position.ToVector2(), entry.Resource.Mesh.Points[line.PointB].Position.ToVector2(),
                                        from.ToVector2(), to.ToVector2(), out res))
                                    {
                                        if (line.Flag != 3) //passable
                                        {
                                            result = res.ToVector3(nav.GetHeightAt(to.X, to.Y));

                                            var triangle = FindTriangle(entry.Resource.Mesh, line.NeighbourA);
                                            if (triangle.Index == line.NeighbourA && TriangleContainsPoint(entry.Resource.Mesh.Points[triangle.PointA].Position, entry.Resource.Mesh.Points[triangle.PointB].Position, entry.Resource.Mesh.Points[triangle.PointC].Position, to))
                                                result = res.ToVector3((entry.Resource.Mesh.Points[triangle.PointA].Position.Y + entry.Resource.Mesh.Points[triangle.PointB].Position.Y + entry.Resource.Mesh.Points[triangle.PointC].Position.Y) / 3f);
                                            else if (line.NeighbourB != 0xFFFF)
                                            {
                                                triangle = FindTriangle(entry.Resource.Mesh, line.NeighbourB);
                                                if (triangle.Index == line.NeighbourB && TriangleContainsPoint(entry.Resource.Mesh.Points[triangle.PointA].Position, entry.Resource.Mesh.Points[triangle.PointB].Position, entry.Resource.Mesh.Points[triangle.PointC].Position, to))
                                                    result = res.ToVector3((entry.Resource.Mesh.Points[triangle.PointA].Position.Y + entry.Resource.Mesh.Points[triangle.PointB].Position.Y + entry.Resource.Mesh.Points[triangle.PointC].Position.Y) / 3f);
                                            }
                                            return false;
                                        }
                                        else
                                        {
                                            result = res.ToVector3(0);
                                            return true;
                                        }
                                    }
                                }
                            }

                            foreach (var line in entry.Resource.Mesh.InLines)
                            {
                                if (from.Y >= Math.Min(entry.Resource.Mesh.Points[line.PointA].Position.Y, entry.Resource.Mesh.Points[line.PointB].Position.Y) - 50 && from.Y <= Math.Max(entry.Resource.Mesh.Points[line.PointA].Position.Y, entry.Resource.Mesh.Points[line.PointB].Position.Y) + 50)
                                {
                                    Vector2 res;
                                    if (LineIntersectsLine(
                                    entry.Resource.Mesh.Points[line.PointA].Position.ToVector2(), entry.Resource.Mesh.Points[line.PointB].Position.ToVector2(),
                                    from.ToVector2(), to.ToVector2(), out res))
                                    {
                                        if (line.Flag != 7) //passable
                                        {
                                            result = res.ToVector3(nav.GetHeightAt(to.X, to.Y));

                                            var triangle = FindTriangle(entry.Resource.Mesh, line.NeighbourA);
                                            if (triangle.Index == line.NeighbourA && TriangleContainsPoint(entry.Resource.Mesh.Points[triangle.PointA].Position, entry.Resource.Mesh.Points[triangle.PointB].Position, entry.Resource.Mesh.Points[triangle.PointC].Position, to))
                                                result = res.ToVector3((entry.Resource.Mesh.Points[triangle.PointA].Position.Y + entry.Resource.Mesh.Points[triangle.PointB].Position.Y + entry.Resource.Mesh.Points[triangle.PointC].Position.Y) / 3f);
                                            else if (line.NeighbourB != 0xFFFF)
                                            {
                                                triangle = FindTriangle(entry.Resource.Mesh, line.NeighbourB);
                                                if (triangle.Index == line.NeighbourB && TriangleContainsPoint(entry.Resource.Mesh.Points[triangle.PointA].Position, entry.Resource.Mesh.Points[triangle.PointB].Position, entry.Resource.Mesh.Points[triangle.PointC].Position, to))
                                                    result = res.ToVector3((entry.Resource.Mesh.Points[triangle.PointA].Position.Y + entry.Resource.Mesh.Points[triangle.PointB].Position.Y + entry.Resource.Mesh.Points[triangle.PointC].Position.Y) / 3f);
                                            }
                                            return false;
                                        }
                                        else
                                        {
                                            result = res.ToVector3(0);
                                            return true;
                                        }
                                    }
                                }
                            }

                            foreach (var tri in entry.Resource.Mesh.Ground)
                            {
                                if (from.Y >= (entry.Resource.Mesh.Points[tri.PointA].Position.Y + entry.Resource.Mesh.Points[tri.PointB].Position.Y + entry.Resource.Mesh.Points[tri.PointC].Position.Y) / 3f - 50 && from.Y <= (entry.Resource.Mesh.Points[tri.PointA].Position.Y + entry.Resource.Mesh.Points[tri.PointB].Position.Y + entry.Resource.Mesh.Points[tri.PointC].Position.Y) / 3f + 50)
                                {
                                    if (TriangleContainsPoint(
                                    entry.Resource.Mesh.Points[tri.PointA].Position.ToVector2(), entry.Resource.Mesh.Points[tri.PointB].Position.ToVector2(),
                                    entry.Resource.Mesh.Points[tri.PointC].Position.ToVector2(), from.ToVector2()))
                                    {
                                        on_object = true;
                                        object_height = (entry.Resource.Mesh.Points[tri.PointA].Position.Y + entry.Resource.Mesh.Points[tri.PointB].Position.Y + entry.Resource.Mesh.Points[tri.PointC].Position.Y) / 3f;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        Vector2 res;
                        var min = entry.Resource.BoundingBox.Minimum.ToVector2();
                        var max = entry.Resource.BoundingBox.Maximum.ToVector2();
                        Vector2 cornerA = new Vector2(min.X, max.Y);
                        Vector2 cornerB = new Vector2(max.X, max.Y);
                        Vector2 cornerC = new Vector2(max.X, min.Y);
                        Vector2 cornerD = new Vector2(min.X, min.Y);

                        if (!LineIntersectsLine(cornerA, cornerB, from.ToVector2(), to.ToVector2(), out res))
                        {
                            if (!LineIntersectsLine(cornerB, cornerC, from.ToVector2(), to.ToVector2(), out res))
                            {
                                if (!LineIntersectsLine(cornerC, cornerD, from.ToVector2(), to.ToVector2(), out res))
                                {
                                    if (!LineIntersectsLine(cornerD, cornerA, from.ToVector2(), to.ToVector2(), out res))
                                    {
                                        result = from;
                                        //Log.Warn("wat ? imposubru should intersect at least 1 line of bbox because one of the points is inside the bbox, check plz");
                                        return true;
                                    }
                                }
                            }
                        }
                        result = res.ToVector3(0);
                        return true;
                    }
                }
            }

            foreach (var zone in nav.CellLinks)
            {
                Vector2 res;
                if (LineIntersectsLine(zone.PointA, zone.PointB, from.ToVector2(), to.ToVector2(), out res))
                {
                    if (zone.CellDestination == 0xFFFF || zone.CellSource == 0xFFFF)
                    {
                        if (!on_object)
                        {
                            result = res.ToVector3(0);
                            return true;
                        }
                    }
                }
            }

            if (on_object)
                result = new Vector3(to.X, object_height, to.Y);
            else
                result = new Vector3(to.X, nav.GetHeightAt(to.X, to.Y), to.Y);

            return false;
        }
        private static MeshTriangle FindTriangle(BmsData mesh, int index)
        {
            for (int i = 0; i < mesh.Ground.Length; i++)
            {
                if (mesh.Ground[i].Index == index)
                    return mesh.Ground[i];
            }
            return default(MeshTriangle);
        }

        private static bool TriangleContainsPoint(Vector3 PointA, Vector3 PointB, Vector3 PointC, Vector3 point)
        {
            return TriangleContainsPoint(PointA.ToVector2(), PointB.ToVector2(), PointC.ToVector2(), point.ToVector2());
        }

        private static bool TriangleContainsPoint(Vector2 PointA, Vector2 PointB, Vector2 PointC, Vector2 point)
        {
            float v0x = PointC.X - PointA.X, v0z = PointC.Y - PointA.Y;
            float v1x = PointB.X - PointA.X, v1z = PointB.Y - PointA.Y;
            float v2x = (int)point.X - PointA.X, v2z = (int)point.Y - PointA.Y;

            float dot00 = dot(v0x, v0z, v0x, v0z);
            float dot01 = dot(v0x, v0z, v1x, v1z);
            float dot02 = dot(v0x, v0z, v2x, v2z);
            float dot11 = dot(v1x, v1z, v1x, v1z);
            float dot12 = dot(v1x, v1z, v2x, v2z);

            float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return u > 0f && v > 0f && u + v < 1f;
        }

        private static float dot(float ax, float az, float bx, float bz)
        {
            return ax * bx + az * bz;
        }

        private static bool LineIntersectsLine(Vector2 start1, Vector2 end1, Vector2 start2, Vector2 end2, out Vector2 pos)
        {
            pos = Vector2.Zero;
            float denom = ((end2.Y - start2.Y) * (end1.X - start1.X)) - ((end2.X - start2.X) * (end1.Y - start1.Y));
            if (denom == 0)
                return false;

            float invDenom = 1f / denom;

            float ua = (((end2.X - start2.X) * (start1.Y - start2.Y)) - ((end2.Y - start2.Y) * (start1.X - start2.X))) * invDenom;
            float ub = (((end1.X - start1.X) * (start1.Y - start2.Y)) - ((end1.Y - start1.Y) * (start1.X - start2.X))) * invDenom;

            if (ua < 0f || ua > 1f || ub < 0f || ub > 1f)
                return false;

            pos = new Vector2(start1.X + ua * (end1.X - start1.X), start1.Y + ua * (end1.Y - start1.Y));
            return true;
        }
    }
}