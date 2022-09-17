using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Silkroad.Materials;
using System.Collections.Generic;

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

            //LoadTerrain(xSector - 1, ySector + 1); //TL
            //LoadTerrain(xSector, ySector + 1); //TC
            //LoadTerrain(xSector + 1, ySector + 1); //TR
            //LoadTerrain(xSector - 1, ySector); //CL
            LoadTerrain(xSector, ySector); //CC
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
            GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            GraphicsDevice.RasterizerState = RasterizerState.CullNone;

            foreach (MapObject obj in mapObjects)
                obj.Draw(game);

            base.Draw(gameTime);
        }

        private bool LoadTerrain(int xsec, int ysec)
        {
            mapObjects = new List<MapObject>();
            //var navMesh = new nvm(Path.Combine("navmesh", $"nv_{ysec:X}{xsec:X}.nvm"));

            var buffer = Program.Map.GetFileBuffer($@"{ysec}\{xsec}.o2");
            if (buffer == null)
                return false;

            var ofile = new ObjectFile(buffer);
            foreach (MapObjectElement obj in ofile.Elements)
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