﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.IO;

namespace Silkroad
{
    internal class Terrain : DrawableGameComponent
    {
        public static int XSector = 168;
        public static int YSector = 97;

        MainGame m_game;
        VertexPositionColor[] vertices;
        int[] indicies;
        BasicEffect effect;
        KeyboardState lastkeyboardState;

        public Terrain(MainGame game) : base(game)
        {
            effect = new BasicEffect(game.GraphicsDevice);
            this.m_game = game;
            LoadTerrain();
        }
        public bool LoadTerrain()
        {
            var file = Path.Combine("navmesh", $"nv_{YSector:X}{XSector:X}.nvm");

            var nvm = new nvm(Program.Data.GetFileBuffer(file));
            vertices = nvm.GetVertices();
            indicies = nvm.GetIndicies();
            return true;
        }

        public override void Update(GameTime gameTime)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyUp(Keys.Right) && lastkeyboardState.IsKeyDown(Keys.Right))
            {
                XSector++;
                LoadTerrain();
            }
            if (state.IsKeyUp(Keys.Left) && lastkeyboardState.IsKeyDown(Keys.Left))
            {
                XSector--;
                LoadTerrain();
            }
            if (state.IsKeyUp(Keys.Up) && lastkeyboardState.IsKeyDown(Keys.Up))
            {
                YSector++;
                LoadTerrain();
            }
            if (state.IsKeyUp(Keys.Down) && lastkeyboardState.IsKeyDown(Keys.Down))
            {
                YSector--;
                LoadTerrain();
            }
            lastkeyboardState = state;

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.CullClockwiseFace;
            rs.FillMode = FillMode.WireFrame;
            GraphicsDevice.RasterizerState = rs;


            var d = new DepthStencilState();
            d.DepthBufferEnable = true;
            GraphicsDevice.DepthStencilState = d;

            effect.View = m_game.m_camera.View;
            effect.Projection = m_game.m_camera.Projection;
            effect.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            effect.VertexColorEnabled = false;
            //effect.EnableDefaultLighting();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indicies, 0, indicies.Length / 3);
            }

            base.Draw(gameTime);
        }
    }
}