using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silkroad.Components;
using System.Collections.Generic;
using System.IO;

internal class Terrain2 : GameComponent
{
    BasicEffect effect;
    private const string PathPrefix = "Map/tile2d/";
    private const int HeightWidth = 17;
    private const int TileCount = 36;

    private readonly float[][][] heights = new float[TileCount][][];
    private readonly int[][][] tex = new int[TileCount][][];
    private readonly List<Texture2D> textures = new List<Texture2D>();
    private string[] texPaths;

    public Terrain2(GraphicsDevice graphicsDevice, Game game) : base(game)
    {
        effect = new BasicEffect(game.GraphicsDevice);
        LoadTexturePaths();
        LoadTerrainData($@"{Terrain.YSector}\{Terrain.XSector}.m");
        LoadTextures(graphicsDevice);
    }

    private void LoadTerrainData(string path)
    {
        using (var binaryReader = new BinaryReader(File.Open(path, FileMode.Open)))
        {
            binaryReader.BaseStream.Position = 12L;
            for (int i = 0; i < TileCount; i++)
            {
                heights[i] = new float[HeightWidth][];
                tex[i] = new int[HeightWidth][];
                binaryReader.BaseStream.Position += 6L;

                for (int j = 0; j < HeightWidth; j++)
                {
                    heights[i][j] = new float[HeightWidth];
                    tex[i][j] = new int[HeightWidth];
                }

                for (int j = 0; j < HeightWidth; j++)
                {
                    for (int k = 0; k < HeightWidth; k++)
                    {
                        heights[i][j][k] = binaryReader.ReadSingle();
                        tex[i][j][k] = binaryReader.ReadByte();
                        binaryReader.BaseStream.Position += 2L;
                    }
                }
                binaryReader.BaseStream.Position += 546L;
            }
        }
    }

    private void LoadTextures(GraphicsDevice graphicsDevice)
    {
        var uniqueTexIDs = new HashSet<int>();

        for (int i = 0; i < TileCount; i++)
        {
            for (int j = 0; j < HeightWidth; j++)
            {
                for (int k = 0; k < HeightWidth; k++)
                {
                    int texID = tex[i][j][k];
                    if (uniqueTexIDs.Add(texID))
                    {
                        string texturePath = PathPrefix + texPaths[texID];
                        textures.Add(LoadTexture(graphicsDevice, texturePath));
                    }
                }
            }
        }
    }

    private Texture2D LoadTexture(GraphicsDevice graphicsDevice, string texturePath)
    {
        if (!File.Exists(texturePath))
        {
            texturePath = texturePath.Replace(".ddj", ".dds");
        }
        using (var stream = new FileStream(texturePath, FileMode.Open))
        {
            return Texture2D.FromStream(graphicsDevice, stream);
        }
    }

    public void Draw(GraphicsDevice graphicsDevice)
    {
        VertexPositionTexture[] vertices = new VertexPositionTexture[HeightWidth * HeightWidth * TileCount * 4];
        int[] indices = new int[HeightWidth * HeightWidth * TileCount * 6];
        int vertexIndex = 0;
        int indexIndex = 0;

        for (int i = 0; i < TileCount; i++)
        {
            for (int j = 0; j < HeightWidth - 1; j++)
            {
                for (int k = 0; k < HeightWidth - 1; k++)
                {
                    AddTileVertices(i, j, k, ref vertexIndex, vertices);
                    AddTileIndices(ref indexIndex, indices);
                }
            }
        }

        foreach (var texture in textures)
        {
            graphicsDevice.Textures[0] = texture;
            graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertexIndex, indices, 0, indexIndex / 3);
        }
    }

    private void AddTileVertices(int tileIndex, int j, int k, ref int vertexIndex, VertexPositionTexture[] vertices)
    {
        float x = j * 20 + (tileIndex % 6) * 320;
        float y = k * 20 + (tileIndex / 6) * 320;

        vertices[vertexIndex++] = new VertexPositionTexture(new Vector3(x, y, heights[tileIndex][j][k]), new Vector2(0, 0));
        vertices[vertexIndex++] = new VertexPositionTexture(new Vector3(x + 20, y, heights[tileIndex][j + 1][k]), new Vector2(1, 0));
        vertices[vertexIndex++] = new VertexPositionTexture(new Vector3(x, y + 20, heights[tileIndex][j][k + 1]), new Vector2(0, 1));
        vertices[vertexIndex++] = new VertexPositionTexture(new Vector3(x + 20, y + 20, heights[tileIndex][j + 1][k + 1]), new Vector2(1, 1));
    }

    private void AddTileIndices(ref int indexIndex, int[] indices)
    {
        indices[indexIndex++] = indexIndex - 4;
        indices[indexIndex++] = indexIndex - 3;
        indices[indexIndex++] = indexIndex - 2;
        indices[indexIndex++] = indexIndex - 3;
        indices[indexIndex++] = indexIndex - 1;
        indices[indexIndex++] = indexIndex - 2;
    }

    private void LoadTexturePaths()
    {
        if (!File.Exists("Map/tile2d.ifo"))
        {
            throw new FileNotFoundException("Could not find tile2d.ifo. Terminating application.");
        }
        string[] lines = File.ReadAllLines("Map/tile2d.ifo");
        texPaths = new string[lines.Length - 2];
        for (int i = 0; i < texPaths.Length; i++)
        {
            texPaths[i] = lines[i + 2].Split(' ')[3].Trim('"');
        }
    }
}