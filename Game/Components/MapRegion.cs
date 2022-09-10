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
        BasicEffect _effect;

        public MapRegion(MainGame game)
            : base(game)
        {
            _effect = new BasicEffect(game.GraphicsDevice);
            LoadTerrain();
            lastkeyboardState = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            if (state.IsKeyUp(Keys.Right) && lastkeyboardState.IsKeyDown(Keys.Right))
                LoadTerrain();

            if (state.IsKeyUp(Keys.Left) && lastkeyboardState.IsKeyDown(Keys.Left))
                LoadTerrain();

            if (state.IsKeyUp(Keys.Up) && lastkeyboardState.IsKeyDown(Keys.Up))
                LoadTerrain();

            if (state.IsKeyUp(Keys.Down) && lastkeyboardState.IsKeyDown(Keys.Down))
                LoadTerrain();

            lastkeyboardState = state;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (MapObject obj in mapObjects)
                obj.Draw(Game as MainGame, _effect);

            base.Draw(gameTime);
        }

        private void LoadTerrain()
        {
            mapObjects = new List<MapObject>();
            //var navMesh = new nvm(Path.Combine("navmesh", $"nv_{ysec:X}{xsec:X}.nvm"));
            var ofile = new ObjectFile(Program.Map.GetFileBuffer(string.Format(@"{0}\{1}.o2", Terrain.YSector, Terrain.XSector)));
            foreach (mObject obj in ofile.objects)
            {
                //igrone since .cpd..
                if (obj.uID == 923)
                    continue;

                mapObjects.Add(new MapObject(obj, Terrain.XSector, Terrain.YSector));
            }
        }
    }
}