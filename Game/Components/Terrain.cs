using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.IO;

namespace Silkroad.Components
{
    internal class Terrain : DrawableGameComponent
    {
        public struct terrainObj
        {
            public Matrix world;
            public Vector3 pos;
            public VertexPositionColor[] vertices;
            public int[] indicies;
        }

        public static int XSector = 168;
        public static int YSector = 97;

        BasicEffect effect;
        KeyboardState lastkeyboardState;

        private List<terrainObj> objs;

        public Terrain(Game game) : base(game)
        {
            effect = new BasicEffect(game.GraphicsDevice);
            LoadTerrains();
        }

        public bool LoadTerrain(int xsec, int ysec, out terrainObj obj)
        {
            obj = new terrainObj();

            try
            {
                var nvm = new nvm(Path.Combine("navmesh", $"nv_{ysec:X}{xsec:X}.nvm"));
                obj.vertices = nvm.Vertices;
                obj.indicies = nvm.Indicies;
            }
            catch (System.Exception)
            {
                return false;
            }
            return true;
        }

        private void LoadTerrains()
        {
            objs = new(9);
            var xSector = XSector;
            var ySector = YSector;

            terrainObj obj;
            /*
            LoadTerrain(xSector - 1, ySector + 1, out obj); //TL
            obj.world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            objs.Add(obj);
           
            LoadTerrain(xSector, ySector + 1, out obj); //TC
            obj.world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            objs.Add(obj);

            LoadTerrain(xSector + 1, ySector + 1, out obj); //TR
            obj.world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            objs.Add(obj);
             */
            /*LoadTerrain(xSector - 1, ySector, out obj); //CL
            obj.world = Matrix.CreateWorld(new Vector3(-1, 0, 0), Vector3.Left, Vector3.Up);
            objs.Add(obj);*/

            LoadTerrain(xSector, ySector, out obj); //CC
            obj.world = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            objs.Add(obj);
            /*
            LoadTerrain(xSector + 1, ySector, out obj); //CR
            obj.world = Matrix.CreateWorld(new Vector3(1, 0, 0), Vector3.Forward, Vector3.Up);
            objs.Add(obj);*/
            /*
            LoadTerrain(xSector - 1, ySector - 1, out obj); //BL
            obj.world = Matrix.CreateWorld(new Vector3(-1, -1, 0),Vector3.Left,  Vector3.Up);
            objs.Add(obj);
            
            LoadTerrain(xSector, ySector - 1, out obj); //BC
            obj.world = Matrix.CreateWorld(new Vector3(0, -1, 0), Vector3.Forward, Vector3.Up);
            objs.Add(obj);

            LoadTerrain(xSector + 1, ySector - 1, out obj); //BR
            obj.world = Matrix.CreateWorld(new Vector3(1, -1, 0),Vector3.Right,  Vector3.Up);
            objs.Add(obj); 
            */
        }

        public override void Update(GameTime gameTime)
        {
            if (!Game.IsActive)
                return;

            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyUp(Keys.Right) && lastkeyboardState.IsKeyDown(Keys.Right))
            {
                XSector++;
                LoadTerrains();
            }
            if (state.IsKeyUp(Keys.Left) && lastkeyboardState.IsKeyDown(Keys.Left))
            {
                XSector--;
                LoadTerrains();
            }
            if (state.IsKeyUp(Keys.Up) && lastkeyboardState.IsKeyDown(Keys.Up))
            {
                YSector++;
                LoadTerrains();
            }
            if (state.IsKeyUp(Keys.Down) && lastkeyboardState.IsKeyDown(Keys.Down))
            {
                YSector--;
                LoadTerrains();
            }
            lastkeyboardState = state;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            effect.View = Camera.View;
            effect.Projection = Camera.Projection;

            effect.VertexColorEnabled = true;
            //effect.EnableDefaultLighting();

            var rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = rs;

            foreach (var obj in objs)
            {
                effect.World = obj.world;
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, obj.vertices, 0, obj.vertices.Length, obj.indicies, 0, obj.indicies.Length / 3);
                }
            }
            
            //effect.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);

            base.Draw(gameTime);
        }
    }
}