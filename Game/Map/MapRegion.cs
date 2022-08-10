using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Silkroad.Materials;
using System.Collections.Generic;
using System.IO;

namespace Silkroad
{
    public class MapRegion : DrawableGameComponent
    {
        public int xsec = 168;
        public int ysec = 97;
        private nvm navMesh;
        private MainGame m_game;
        private GraphicsDevice device;
        private BasicEffect effect;
        private List<MapObject> mapObjects;
        KeyboardState lastkeyboardState;

        public MapRegion(GraphicsDevice device, MainGame game) : base(game)
        {
            this.m_game = game;
            this.device = device;
            LoadTerrain();
            effect = new BasicEffect(device);
            lastkeyboardState = Keyboard.GetState();
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyUp(Keys.Right) && lastkeyboardState.IsKeyDown(Keys.Right))
            {
                xsec++;
                LoadTerrain();
            }
            if (state.IsKeyUp(Keys.Left) && lastkeyboardState.IsKeyDown(Keys.Left))
            {
                xsec--;
                LoadTerrain();
            }
            if (state.IsKeyUp(Keys.Up) && lastkeyboardState.IsKeyDown(Keys.Up))
            {
                ysec++;
                LoadTerrain();
            }
            if (state.IsKeyUp(Keys.Down) && lastkeyboardState.IsKeyDown(Keys.Down))
            {
                ysec--;
                LoadTerrain();
            }
            lastkeyboardState = state;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            foreach (MapObject obj in mapObjects)
            {
                obj.Draw();
            }

            base.Draw(gameTime);
        }

        private void LoadTerrain()
        {
            mapObjects = new List<MapObject>();
            navMesh = new nvm(Path.Combine("navmesh", $"nv_{ysec:X}{xsec:X}.nvm"));
            ObjectFile ofile = new ObjectFile(Program.Map.GetFileBuffer(string.Format(@"{0}\{1}.o2", ysec, xsec)));
            foreach (mObject obj in ofile.objects)
            {
                if (obj.uID == 923)
                {
                    //igrone since .cpd..
                }
                else
                {
                    mapObjects.Add(new MapObject(obj, xsec, ysec));
                }
            }
        }
    }
}