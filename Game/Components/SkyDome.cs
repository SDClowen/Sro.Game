using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Silkroad.Components
{
    public class SkyDomeParameters
    {
        #region Private Properties
        private Vector4 lightDirection = new Vector4(100.0f, 100.0f, 100.0f, 1.0f);
        private Vector4 lightColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        private Vector4 lightColorAmbient = new Vector4(0.1f, 0.1f, 0.1f, 1.0f);
        private Vector4 fogColor = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
        private float fDensity = 0.000028f;
        private float sunLightness = 0.2f;
        private float sunRadiusAttenuation = 256.0f;
        private float largeSunLightness = 0.2f;
        private float largeSunRadiusAttenuation = 1.0f;
        private float dayToSunsetSharpness = 1.5f;
        private float hazeTopAltitude = 100.0f;
        #endregion

        public Vector4 LightDirection { get { return lightDirection; } set { lightDirection = value; } }

        public Vector4 LightColor { get { return lightColor; } set { lightColor = value; } }

        public Vector4 LightColorAmbient { get { return lightColorAmbient; } set { lightColorAmbient = value; } }

        public Vector4 FogColor { get { return fogColor; } set { fogColor = value; } }

        public float FogDensity { get { return fDensity; } set { fDensity = value; } }

        public float SunLightness { get { return sunLightness; } set { sunLightness = value; } }

        public float SunRadiusAttenuation { get { return sunRadiusAttenuation; } set { sunRadiusAttenuation = value; } }

        public float LargeSunLightness { get { return largeSunLightness; } set { largeSunLightness = value; } }

        public float LargeSunRadiusAttenuation { get { return largeSunRadiusAttenuation; } set { largeSunRadiusAttenuation = value; } }

        public float DayToSunsetSharpness { get { return dayToSunsetSharpness; } set { dayToSunsetSharpness = value; } }

        public float HazeTopAltitude { get { return hazeTopAltitude; } set { hazeTopAltitude = value; } }
    }

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class SkyDome : DrawableGameComponent
    {

        #region Properties
        private Model domeModel;
        private Effect domeEffect;

        private float fTheta = 0.0f;
        private float fPhi = 0.0f;

        private bool realTime;

        //Camera camera;
        //Game game;

        Texture2D day, sunset, night;

        SkyDomeParameters parameters;
        #endregion

        #region Gets/Sets
        /// <summary>
        /// Gets/Sets Theta value
        /// </summary>
        public float Theta { get { return fTheta; } set { fTheta = value; } }

        /// <summary>
        /// Gets/Sets Phi value
        /// </summary>
        public float Phi { get { return fPhi; } set { fPhi = value; } }

        /// <summary>
        /// Gets/Sets actual time computation
        /// </summary>
        public bool RealTime { get { return realTime; } set { realTime = value; } }

        /// <summary>
        /// Gets/Sets the SkyDome parameters
        /// </summary>
        public SkyDomeParameters Parameters { get { return parameters; } set { parameters = value; } }
        #endregion

        #region Contructor

        public SkyDome(Game game) : base(game)
        {

        }
        #endregion

        protected override void LoadContent()
        {
            base.LoadContent();

            //TODO - move these asset names out
            this.domeModel = Game.Content.Load<Model>("skydome");
            this.domeEffect = Game.Content.Load<Effect>("Sky");

            this.day = Game.Content.Load<Texture2D>("SkyDay");
            this.sunset = Game.Content.Load<Texture2D>("Sunset");
            this.night = Game.Content.Load<Texture2D>("SkyNight");

            domeEffect.CurrentTechnique = domeEffect.Techniques["SkyDome"];

            RemapModel(domeModel, domeEffect);

            realTime = true;

            parameters = new SkyDomeParameters();
            parameters.LightColor = Color.Yellow.ToVector4();
            parameters.SunRadiusAttenuation = 100f;
            parameters.LargeSunRadiusAttenuation = 0f;

        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // KeyboardState keyState = Keyboard.GetState();

            if (realTime)
            {
                int minutes = DateTime.Now.Minute * 60 + DateTime.Now.Second;

                //minutes = DateTime.Now.Minute * 60  + DateTime.Now.Second ;

                // break the day up into <num minutes in a day> equal sections and map to the correct angle based upon the date information
                this.fTheta = (float)minutes * (float)(Math.PI) / 60f / 60f;

            }

            parameters.LightDirection = this.GetDirection();
            parameters.LightDirection.Normalize();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix[] boneTransforms = new Matrix[domeModel.Bones.Count];
            domeModel.CopyAbsoluteBoneTransformsTo(boneTransforms);

            Matrix View = Camera.View;
            Matrix Projection = Camera.Projection;

            //Game.GraphicsDevice.DepthStencilState.DepthBufferEnable = false;
            //Game.GraphicsDevice.DepthStencilState.DepthBufferWriteEnable = false;

            GraphicsDevice graphics = Game.GraphicsDevice;

            graphics.BlendState = BlendState.AlphaBlend;
            graphics.DepthStencilState = DepthStencilState.DepthRead;

            foreach (ModelMesh mesh in domeModel.Meshes)
            {
                //TODO - magic number, parameterize this class
                Matrix World = boneTransforms[mesh.ParentBone.Index] *
                    Matrix.CreateTranslation(Camera.Position.X, -10.0f, Camera.Position.Z);

                Matrix WorldIT = Matrix.Invert(World);
                WorldIT = Matrix.Transpose(WorldIT);

                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["WorldIT"].SetValue(WorldIT);
                    effect.Parameters["WorldViewProj"].SetValue(World * View * Projection);
                    effect.Parameters["ViewInv"].SetValue(Matrix.Invert(View));
                    effect.Parameters["World"].SetValue(World);

                    effect.Parameters["SkyTextureNight"]?.SetValue(night);
                    effect.Parameters["SkyTextureSunset"]?.SetValue(sunset);
                    effect.Parameters["SkyTextureDay"]?.SetValue(day);

                    effect.Parameters["isSkydome"].SetValue(true);

                    effect.Parameters["LightDirection"].SetValue(parameters.LightDirection);
                    effect.Parameters["LightColor"].SetValue(parameters.LightColor);
                    effect.Parameters["LightColorAmbient"].SetValue(parameters.LightColorAmbient);
                    effect.Parameters["FogColor"].SetValue(parameters.FogColor);
                    effect.Parameters["fDensity"].SetValue(parameters.FogDensity);
                    effect.Parameters["SunLightness"].SetValue(parameters.SunLightness);
                    effect.Parameters["sunRadiusAttenuation"].SetValue(parameters.SunRadiusAttenuation);
                    effect.Parameters["largeSunLightness"].SetValue(parameters.LargeSunLightness);
                    effect.Parameters["largeSunRadiusAttenuation"].SetValue(parameters.LargeSunRadiusAttenuation);
                    effect.Parameters["dayToSunsetSharpness"].SetValue(parameters.DayToSunsetSharpness);
                    effect.Parameters["hazeTopAltitude"].SetValue(parameters.HazeTopAltitude);
                    try
                    {

                        mesh.Draw();
                    }
                    catch
                    {
                    }
                }

            }

            //Game.GraphicsDevice.DepthStencilState.DepthBufferEnable = true;
            //Game.GraphicsDevice.DepthStencilState.DepthBufferWriteEnable = true;
        }

        public static void RemapModel(Model model, Effect effect)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
            }
        }

        Vector4 GetDirection()
        {

            float y = (float)Math.Cos((double)this.fTheta);
            float x = (float)(Math.Sin((double)this.fTheta) * Math.Cos(this.fPhi));
            float z = (float)(Math.Sin((double)this.fTheta) * Math.Sin(this.fPhi));
            float w = 1.0f;

            return new Vector4(x, y, z, w);
        }
    }
}