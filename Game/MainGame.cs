using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SharpDX.Direct3D9;
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
        public Camera Camera;
        Statistics _stats;
        Texture2D _cursor;
        Terrain _terrain;
        MapRegion _mapRegion;

        VertexPositionColor[] vertcies = new VertexPositionColor[8];
        public static string Path = @"D:\Silkroad\Clients\Official\SilkroadOnline_GlobalOfficial_v1_281";
        public BasicEffect basicEffect;
        public objifo objectInfos;
        protected SpriteBatch _spriteBatch;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.Title = "Silkroad World Editör";
            graphics.HardwareModeSwitch = true;
            graphics.PreferMultiSampling = true;
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
            IsMouseVisible = true;
            objectInfos = new objifo();
            objectInfos.Load();

            basicEffect = new(this.GraphicsDevice);

            // TODO: Add your initialization logic here
            _terrain = new(this);
            _mapRegion = new(this);
            Camera = new(this);

            Components.Add(_terrain);
            Components.Add(_mapRegion);
            Components.Add(Camera);

            _stats = new Statistics(this, Content);
            Components.Add(_stats);

            GraphicsDevice.BlendState = BlendState.AlphaBlend;
            GraphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            basicEffect = new BasicEffect(GraphicsDevice);
            _cursor = Content.Load<Texture2D>("cursor");
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            var test = MouseCursor.FromTexture2D(_cursor, 0, 0);
            
            Mouse.PlatformSetCursor(test);
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
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Black, 1.0f, 0);

            base.Draw(gameTime);
        }
    }
}
