using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silkroad.Components;
using Silkroad.Materials;
using System.Collections.Generic;

namespace Silkroad
{
    internal class objMeshes
    {
        public bms Mesh;
        public Texture2D texture;

        public objMeshes(bms bms, Texture2D tex)
        {
            this.Mesh = bms;
            this.texture = tex;
        }
    }

    internal class MapObject
    {
        public int X, Y;
        public Bsr resourceInfo;
        public List<bms> meshes;
        public List<BmtManager> textures;
        private List<objMeshes> meshWithTextures;
        private Texture2D blankTex;
        private O2FileElement obj;

        public MapObject(O2FileElement obj, int x, int y)
        {
            X = x;
            Y = y;
            meshes = new List<bms>();
            textures = new List<BmtManager>();
            meshWithTextures = new List<objMeshes>();

            this.obj = obj;

            string path = Program.Window.ObjIfo.GetPathByID(obj.Index);
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
                meshWithTextures.Add(new objMeshes(b, GetTexture(b.MaterialName)));
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
                        if (texture.IsNotWithinSameDirectory)
                            return DDS.GetTexture(Program.Data.GetFileBuffer(texture.DiffuseMap), Program.Window.GraphicsDevice);
                            
                        string path = bmtTexture.Path.Substring(0, bmtTexture.Path.LastIndexOf('\\') + 1);
                        path += texture.DiffuseMap;

                        return DDS.GetTexture(Program.Data.GetFileBuffer(path), Program.Window.GraphicsDevice);
                    }
                }
            }
            return blankTex;
        }

        public void Draw(MainGame game)
        {
            var effect = game.basicEffect;
            //basicEffect.TextureEnabled = true;
            effect.View = Camera.View;
            effect.Projection = Camera.Projection;
            //basicEffect.LightingEnabled = true;
            //basicEffect.PreferPerPixelLighting = true;
            //basicEffect.EnableDefaultLighting();
            //effect.VertexColorEnabled = false;

            foreach (objMeshes m in meshWithTextures)
            {
                /*var anglePi = new SharpDX.AngleSingle(obj.angle, SharpDX.AngleType.Radian);
                effect.World = Matrix.CreateRotationY(anglePi.Radians) * Matrix.CreateTranslation(obj.Position);*/
                effect.World = Matrix.CreateRotationY(obj.Theta) * Matrix.CreateTranslation(obj.Position);
                effect.Texture = m.texture;

                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    game.GraphicsDevice.DrawUserIndexedPrimitives(
                        PrimitiveType.TriangleList, 
                        m.Mesh.Verticies, 0, 
                        m.Mesh.Verticies.Length,
                        m.Mesh.Indicies, 0,
                        m.Mesh.Indicies.Length / 3);
                }
            }
        }
    }
}