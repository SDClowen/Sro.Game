using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Silkroad.Materials;
using System.Collections.Generic;
using System.IO;

namespace Silkroad.Components
{
    public class MapRegion : DrawableGameComponent
    {
        private List<MapObject> mapObjects;
        KeyboardState lastkeyboardState;

        public MapRegion(MainGame game)
            : base(game)
        {
            //_effect = new(game.GraphicsDevice);
            LoadTerrains();
            lastkeyboardState = Keyboard.GetState();
        }

        private void LoadTerrains()
        {
            var xSector = Terrain.XSector;
            var ySector = Terrain.YSector;


            LoadTerrain(xSector, ySector); //CC

            //LoadTerrain(xSector - 1, ySector + 1); //TL
            //LoadTerrain(xSector, ySector + 1); //TC
            //LoadTerrain(xSector + 1, ySector + 1); //TR
            //LoadTerrain(xSector - 1, ySector); //CL
            LoadObjects(xSector, ySector); //CC
            //LoadTerrain(xSector + 1, ySector); //CR
            //LoadTerrain(xSector - 1, ySector - 1); //BL
            //LoadTerrain(xSector, ySector - 1); //BC
            //LoadTerrain(xSector + 1, ySector - 1); //BR
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            if (state.IsKeyUp(Keys.Right) && lastkeyboardState.IsKeyDown(Keys.Right))
                LoadTerrains();

            if (state.IsKeyUp(Keys.Left) && lastkeyboardState.IsKeyDown(Keys.Left))
                LoadTerrains();

            if (state.IsKeyUp(Keys.Up) && lastkeyboardState.IsKeyDown(Keys.Up))
                LoadTerrains();

            if (state.IsKeyUp(Keys.Down) && lastkeyboardState.IsKeyDown(Keys.Down))
                LoadTerrains();

            lastkeyboardState = state;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            var game = Game as MainGame;
            GraphicsDevice.BlendState = BlendState.Opaque;
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            GraphicsDevice.SamplerStates[0] = SamplerState.AnisotropicWrap;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (MapObject obj in mapObjects)
                obj.Draw(game);

            base.Draw(gameTime);
        }

        private bool LoadTerrain(int xsec, int ysec)
        {
            mapObjects = new List<MapObject>();

            var buffer = Program.Map.GetFileBuffer($@"{ysec}\{xsec}.m");
            if (buffer == null)
                return false;

            var mfile = new MFile(buffer);
            for (int y = 0; y < 6; y++)
            {
                for (int x = 0; x < 6; x++)
                {
                    var block = mfile.Blocks[y, x];
                    
                    for (int j = 0; j < 17; j++)
                    {
                        for (int k = 0; k < 17; k++)
                        {
                            var cell = block.Cells[j, k];
                            
                            var objs = Program.Window.Tile2D.Objs;
                            var textureIndex = objs.FindIndex(p => p.Id == cell.Texture);
                            if (textureIndex == -1)
                                throw new System.Exception("Texture not found!");

                            var texturePath = objs[textureIndex].Path;

                            var fileBuffer = Program.Map.GetFileBuffer(Path.Combine("tile2d", texturePath));
                            if (fileBuffer == null)
                                continue;

                            //var texture = DDS.GetTexture(fileBuffer, Program.Window.GraphicsDevice);

                        }
                    }
                }
            }

            return true;
        }

        private bool LoadObjects(int xsec, int ysec)
        {
            mapObjects = new List<MapObject>();
            //var navMesh = new nvm(Path.Combine("navmesh", $"nv_{ysec:X}{xsec:X}.nvm"));


            var ofile = new O2File(xsec, ysec);
            foreach (var obj in ofile.Elements)
            {
                //igrone since .cpd..
                if (obj.Index == 923)
                    continue;
                try
                {

                    mapObjects.Add(new MapObject(obj, xsec, ysec));
                }
                catch
                {
                }
            }

            return true;
        }
    }
}