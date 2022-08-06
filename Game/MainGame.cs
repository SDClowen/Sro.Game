using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Silkroad.Components;
using Silkroad.Materials;
using System;
using System.IO;

namespace Silkroad
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        public Camera m_camera;
        Statistics m_stats;
        Skydome m_dome;

        Terrain terrain;
        MapRegion mapRegion;

        VertexPositionColor[] vertcies = new VertexPositionColor[8];
        public static string Path = @"D:\Silkroad\Clients\Official\SilkroadOnline_GlobalOfficial_v1_225";
        public BasicEffect effect;
        public objifo objectInfos;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.Title = "Test Game";

            graphics.PreferredBackBufferWidth = 1440;
            graphics.PreferredBackBufferWidth = 900;
            graphics.PreferMultiSampling = true;
            graphics.SynchronizeWithVerticalRetrace = true;
            graphics.ApplyChanges();
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            objectInfos = new objifo();
            objectInfos.Load();

            effect = new(this.GraphicsDevice);
            // TODO: Add your initialization logic here
            terrain = new(this);
            mapRegion = new(graphics.GraphicsDevice, this);
            m_camera = new(this);
            Components.Add(m_camera);
            Components.Add(mapRegion);
            Components.Add(terrain);

            m_stats = new Statistics(this, Content);
            Components.Add(m_stats);

            m_dome = new Skydome(this, Content);
            Components.Add(m_dome);

            InitOutline();
            base.Initialize();
        }

        private void InitOutline()
        {
            vertcies[0] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue);
            vertcies[1] = new VertexPositionColor(new Vector3(96 * 20, 0, 0), Color.Blue);

            vertcies[2] = new VertexPositionColor(new Vector3(96 * 20, 0, 0), Color.Blue);
            vertcies[3] = new VertexPositionColor(new Vector3(96 * 20, 0, 96 * 20), Color.Blue);

            vertcies[4] = new VertexPositionColor(new Vector3(96 * 20, 0, 96 * 20), Color.Blue);
            vertcies[5] = new VertexPositionColor(new Vector3(0, 0, 96 * 20), Color.Blue);

            vertcies[6] = new VertexPositionColor(new Vector3(0, 0, 96 * 20), Color.Blue);
            vertcies[7] = new VertexPositionColor(new Vector3(0, 0, 0), Color.Blue);
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            effect = new BasicEffect(GraphicsDevice);
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1.0f, 0);

            effect.View = m_camera.View;
            effect.Projection = m_camera.Projection;
            effect.World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
            RasterizerState rs = new RasterizerState();
            rs.CullMode = CullMode.None;
            rs.FillMode = FillMode.Solid;
            GraphicsDevice.RasterizerState = rs;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertcies, 0, 4);
            }

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }
    }
}
