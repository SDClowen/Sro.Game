﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Silkroad.Components
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Statistics : DrawableGameComponent
    {
        protected Dictionary<string, string> m_statistics;
        protected ContentManager m_content;
        protected SpriteBatch m_spriteBatch;
        protected SpriteFont m_font;
        protected Vector2 m_position = new Vector2(20, 15);

        protected int m_frameRate = 0;
        protected int m_frameCounter = 0;
        protected TimeSpan m_elapsedTime = TimeSpan.Zero;
        protected TimeSpan frameElapsed = TimeSpan.Zero;


        public Statistics(Game game, ContentManager content) : base(game)
        {
            m_content = content;
        }


        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // By default, this class only sets the frames per second and memory statistics
            m_statistics = new Dictionary<string, string>();
            m_statistics["FPS"] = "0";
            m_statistics["Memory"] = "0";
            m_statistics["Region"] = "0";
            m_statistics["Mouse"] = "0";
            m_statistics["Camera"] = "0";
            m_statistics["Screen"] = "0";

            base.Initialize();
        }


        /// <summary>
        /// LoadContent will be called once component is started.
        /// </summary>
        protected override void LoadContent()
        {
            m_spriteBatch = new SpriteBatch(GraphicsDevice);
            m_font = m_content.Load<SpriteFont>("Statistics");

            base.LoadContent();
        }


        /// <summary>
        /// UnloadContent will be called when the component is disabled.
        /// </summary>
        protected override void UnloadContent()
        {
            m_content.Unload();

            base.UnloadContent();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            m_elapsedTime += gameTime.ElapsedGameTime;
            frameElapsed += gameTime.ElapsedGameTime;

            if(frameElapsed > TimeSpan.FromSeconds(1))
            {
                frameElapsed -= TimeSpan.FromSeconds(1);

                m_frameRate = m_frameCounter;
                m_frameCounter = 0;
                m_statistics["FPS"] = m_frameRate.ToString();
            }

            if (m_elapsedTime > TimeSpan.FromMilliseconds(50))
            {
                m_elapsedTime -= TimeSpan.FromMilliseconds(50);
                m_statistics["Memory"] = (GC.GetTotalMemory(false) / 1024f / 1024f).ToString("0.0");
                m_statistics["Region"] = $"{Terrain.XSector}x{Terrain.YSector}";
                m_statistics["Mouse"] = $"X:{Mouse.GetState().X:0.0} Y:{Mouse.GetState().Y:0.0}";
                m_statistics["Camera"] = $"X:{Camera.Position.X:0.0} Z:{Camera.Position.Z:0.0} Y:{Camera.Position.Y:0.0}";

                var game = Game as MainGame;
                m_statistics["Screen"] = $"Pitch:{game.Camera.Pitch:0.00} Yaw:{game.Camera.Yaw:0.00}";
            }

            base.Update(gameTime);
        }


        /// <summary>
        /// This is called when the game component should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Draw(GameTime gameTime)
        {
            m_frameCounter++;

            m_spriteBatch.Begin();
            // Enumerate over all statistics and draw them
            int line = 0;
            foreach (KeyValuePair<string, string> stat in m_statistics)
            {
                m_spriteBatch.DrawString(m_font, string.Format("{0}: {1}", stat.Key, stat.Value), new Vector2(m_position.X + 1, m_position.Y + m_font.LineSpacing * line + 1), Color.Black, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                m_spriteBatch.DrawString(m_font, string.Format("{0}: {1}", stat.Key, stat.Value), new Vector2(m_position.X, m_position.Y + m_font.LineSpacing * line), Color.Yellow, 0, Vector2.Zero, 1, SpriteEffects.None, 1);
                line++;
            }
            m_spriteBatch.End();

            base.Draw(gameTime);
        }


        /// <summary>
        /// Access the dictionary used to display statistics.
        /// </summary>
        /// <param name="key">The key of the statistic to display.</param>
        /// <returns>Description and values displayed for the statistic.</returns>
        public string this[string key]
        {
            get { return m_statistics[key]; }
            set { m_statistics[key] = value; }
        }
    }
}