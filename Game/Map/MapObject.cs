using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silkroad.Materials;
using System;
using System.Collections.Generic;

namespace Silkroad
{
    internal class objMeshes
    {
        public bms verts;
        public Texture2D texture;

        public objMeshes(bms bms, Texture2D tex)
        {
            this.verts = bms;
            this.texture = tex;
        }
    }

    internal class MapObject
    {
        public int x_r, y_r;
        public Bsr resourceInfo;
        public List<bms> meshes;
        public List<BmtManager> textures;
        private List<objMeshes> meshWithTextures;
        private Vector3 position;
        private Texture2D blankTex;
        private mObject obj;

        public MapObject(mObject obj, int x, int y)
        {
            this.x_r = x;
            this.y_r = y;
            meshes = new List<bms>();
            textures = new List<BmtManager>();
            meshWithTextures = new List<objMeshes>();

            this.obj = obj;
            this.position = new Vector3(obj.x, obj.y, obj.z);

            string path = Program.Window.objectInfos.GetPathByID(obj.uID);
            resourceInfo = Bsr.ReadFromStream(Program.Data.GetFileBuffer(path));
            foreach (var mesh in resourceInfo.Meshs)
            {
                meshes.Add(new bms(resourceInfo.Name, Program.Data.GetFileBuffer(mesh.Path)));
            }

            foreach (var material in resourceInfo.Materials)
            {
                var tex = new BmtManager();
                tex.Parse(Program.Data.GetFileBuffer(material.Path), material.Path);
                textures.Add(tex);
            }
            blankTex = new Texture2D(Program.Window.GraphicsDevice, 1, 1);

            foreach (bms b in meshes)
            {
                meshWithTextures.Add(new objMeshes(b, GetTexture(b.material)));
            }
        }

        public Texture2D GetTexture(string meshPartName)
        {
            foreach (var bmtTexture in textures)
            {
                foreach (var texture in bmtTexture.Entries)
                {
                    if (meshPartName == texture.Name)
                    {
                        if(texture.IsNotWithinSameDirectory)
                            return DDS.GetTexture(Program.Data.GetFileBuffer(texture.DiffuseMap), Program.Window.GraphicsDevice);
                        
                        string path = bmtTexture.Path.Substring(0, bmtTexture.Path.LastIndexOf('\\') + 1);
                        path += texture.DiffuseMap;
                        
                        return DDS.GetTexture(Program.Data.GetFileBuffer(path), Program.Window.GraphicsDevice);
                    }
                }
            }
            return blankTex;
        }

        public void Draw(MainGame game, BasicEffect effect)
        {
            foreach (objMeshes m in meshWithTextures)
            {
                var anglePi = new SharpDX.AngleSingle(obj.angle, SharpDX.AngleType.Radian);
                effect.World = Matrix.CreateRotationY(anglePi.Radians) * Matrix.CreateTranslation(this.position);

                effect.View = game.Camera.View;
                effect.Projection = game.Camera.Projection;
                effect.VertexColorEnabled = false;

                var rasterizerState = new RasterizerState();
                rasterizerState.FillMode = FillMode.Solid;
                rasterizerState.CullMode = CullMode.CullClockwiseFace;
                rasterizerState.DepthClipEnable = true;
                rasterizerState.MultiSampleAntiAlias = true;

                game.GraphicsDevice.RasterizerState = rasterizerState;

                effect.CurrentTechnique.Passes[0].Apply();
                effect.EnableDefaultLighting();
                effect.Texture = m.texture;
                effect.TextureEnabled = true;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    VertexPositionNormalTexture[] verts = m.verts.GetVerticies();
                    var indicies = m.verts.GetIndicies();
                    game.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                        verts, 0, verts.Length, indicies, 0, indicies.Length / 3);
                }
            }
        }
    }
}