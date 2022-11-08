using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Silkroad.Components;
using Silkroad.Imgui;
using Silkroad.Materials;
using System;

namespace Silkroad
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class MainGame : Game
    {
        GraphicsDeviceManager graphics;
        SkyDome _skyDome;
        public Camera Camera;
        Statistics _stats;
        Texture2D _cursor;
        Terrain _terrain;
        MapRegion _mapRegion;

        public static string Path = @"E:\Clients\SilkroadOnline_GlobalOfficial_v1_225";
        public AlphaTestEffect basicEffect;
        public ObjIfo ObjIfo;
        public Tile2D Tile2D;
        protected SpriteBatch _spriteBatch;


        private ImGuiRenderer _imGuiRenderer;

        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Window.AllowUserResizing = true;
            Window.Title = "Silkroad World Editor";

            graphics.HardwareModeSwitch = true;
            graphics.PreferMultiSampling = false;
            graphics.PreferredBackBufferWidth = 1600;
            graphics.PreferredBackBufferHeight = 900;
            graphics.GraphicsProfile = GraphicsProfile.HiDef;

            graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 300f);

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
            _imGuiRenderer = new(this);
            ObjIfo = new();
            Tile2D = new();
            basicEffect = new(GraphicsDevice);
            Camera = new(this);
            _skyDome = new(this);
            _stats = new(this, Content);

            // TODO: Add your initialization logic here
            _terrain = new(this);
            _mapRegion = new(this);
           
            Components.Add(Camera);
            Components.Add(_terrain);
            Components.Add(_skyDome);
            Components.Add(_mapRegion);
            Components.Add(_stats);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            _cursor = Content.Load<Texture2D>("cursor");
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            Mouse.PlatformSetCursor(MouseCursor.FromTexture2D(_cursor, 0, 0));
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
            GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.White, 1f, 0);
            base.Draw(gameTime);

            _imGuiRenderer.BeforeLayout(gameTime);
            _imGuiRenderer.InitLayout();
            _imGuiRenderer.AfterLayout();

        }
    }
}
