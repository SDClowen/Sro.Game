using Microsoft.Xna.Framework;
using System.IO;

namespace Silkroad.Navmesh.Structure
{
    public struct BmsData
    {
        public string Path;
        public string Filename;
        public BoundingBox BoundingBox;
        public MeshPoint[] Points;
        public MeshTriangle[] Ground;
        public MeshLine[] OutLines;
        public MeshLine[] InLines;
        public string[] Events;

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public static BmsData Load(string path)
        {
            using (var stream = new FileStream(System.IO.Path.Combine(Manager.Dir, path), FileMode.Open))
            {
                using (var reader = new BinaryReader(stream))
                {
                    var bmsData = new BmsData
                    {
                        Filename = path,
                        Path = path
                    };

                    var header = reader.ReadStringEx(12);

                    var verticesPointer = reader.ReadInt32();
                    var bonesPointer = reader.ReadInt32();
                    var facesPointer = reader.ReadInt32();
                    var unknown1Pointer = reader.ReadInt32();
                    var unknown2Pointer = reader.ReadInt32();

                    var pointerBoundingBox = reader.ReadUInt32();
                    var pointerGates = reader.ReadUInt32();
                    var pointerCollision = reader.ReadUInt32();//0 when not used (characters, items, etc..)
                    var pointerUnknown0 = reader.ReadUInt32();
                    var pointerUnknown1 = reader.ReadUInt32();

                    var flagsUnkUInt0 = reader.ReadUInt32();
                    var flagsUnkUInt1 = reader.ReadUInt32();
                    var flagsUnkUInt2 = reader.ReadUInt32();
                    var flagsLightMap = reader.ReadUInt32();
                    var flagsUnkUInt3 = reader.ReadUInt32();

                    var name = reader.ReadStringEx();
                    var material = reader.ReadStringEx();
                    var unkUInt4 = reader.ReadInt32();

                    reader.BaseStream.Position = pointerBoundingBox;
                    bmsData.BoundingBox = new BoundingBox(reader.ReadVector3(), reader.ReadVector3());

                    if (pointerCollision <= 0)
                        return bmsData;

                    reader.BaseStream.Position = pointerCollision;

                    var pointCount = reader.ReadUInt32();
                    bmsData.Points = new MeshPoint[pointCount];

                    for (var i = 0; i < pointCount; i++)
                    {
                        bmsData.Points[i] = new MeshPoint
                        {
                            Position = reader.ReadVector3() / 10,  // / 10 from the keinplan navmesh
                            Flag = reader.ReadByte()
                        };
                    }

                    var collisionCellCount = reader.ReadUInt32();
                    bmsData.Ground = new MeshTriangle[collisionCellCount];
                    for (var i = 0; i < collisionCellCount; i++)
                    {
                        var collisionTriangle = new MeshTriangle
                        {
                            Index = i,
                            PointA = reader.ReadUInt16(),
                            PointB = reader.ReadUInt16(),
                            PointC = reader.ReadUInt16(),
                            unk00 = reader.ReadUInt16()
                        };

                        if (flagsUnkUInt1 == 6 || flagsUnkUInt1 == 7 || flagsUnkUInt1 == 14)
                            collisionTriangle.LineIndex = reader.ReadByte();

                        bmsData.Ground[i] = collisionTriangle;
                    }

                    var outlineCount = reader.ReadUInt32();
                    bmsData.OutLines = new MeshLine[outlineCount];
                    for (var i = 0; i < outlineCount; i++)
                    {
                        var outline = new MeshLine
                        {
                            PointA = reader.ReadUInt16(),
                            PointB = reader.ReadUInt16(),
                            NeighbourA = reader.ReadUInt16(),
                            NeighbourB = reader.ReadUInt16(),
                            Flag = reader.ReadByte()
                        };

                        if (flagsUnkUInt1 == 5 || flagsUnkUInt1 == 7)
                            outline.unk00 = reader.ReadByte();

                        bmsData.OutLines[i] = outline;
                    }

                    var inlineLinkCount = reader.ReadUInt32();
                    bmsData.InLines = new MeshLine[inlineLinkCount];
                    for (var i = 0; i < inlineLinkCount; i++)
                    {
                        var inline = new MeshLine
                        {
                            PointA = reader.ReadUInt16(),
                            PointB = reader.ReadUInt16(),
                            NeighbourA = reader.ReadUInt16(),
                            NeighbourB = reader.ReadUInt16(),
                            Flag = reader.ReadByte()
                        };

                        if (flagsUnkUInt1 == 5 || flagsUnkUInt1 == 7)
                            inline.unk00 = reader.ReadByte();

                        bmsData.InLines[i] = inline;
                    }

                    if (flagsUnkUInt1 >= 4 && flagsUnkUInt1 <= 7 || flagsUnkUInt1 == 14)
                    {
                        var eventCount = reader.ReadUInt32();
                        bmsData.Events = new string[eventCount];
                        for (var i = 0; i < eventCount; i++)
                            bmsData.Events[i] = reader.ReadStringEx();
                    }

                    return bmsData;
                }
            }
        }

    }
}