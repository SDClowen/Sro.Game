using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Silkroad.Lib;
using Silkroad.Materials;

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
                            return ddjLoader.GetTexture(Program.Data.GetFileBuffer(texture.DiffuseMap), Program.Window.GraphicsDevice);
                        
                        string path = bmtTexture.Path.Substring(0, bmtTexture.Path.LastIndexOf('\\') + 1);
                        path += texture.DiffuseMap;
                        
                        return ddjLoader.GetTexture(Program.Data.GetFileBuffer(path), Program.Window.GraphicsDevice);
                    }
                }
            }
            return blankTex;
        }

        public void Draw()
        {
            foreach (objMeshes m in meshWithTextures)
            {
                Program.Window.effect.World = Matrix.CreateRotationY(MathHelper.ToRadians(this.obj.angle * (180.0f / MathF.PI))) * Matrix.CreateTranslation(this.position);
                Program.Window.effect.View = Program.Window.m_camera.View;
                Program.Window.effect.Projection = Program.Window.m_camera.Projection;
                //Program.Window.effect.VertexColorEnabled = true;

                var rasterizerState = new RasterizerState();
                rasterizerState.FillMode = FillMode.Solid;
                rasterizerState.CullMode = CullMode.CullClockwiseFace;
                rasterizerState.DepthClipEnable = true;
                rasterizerState.MultiSampleAntiAlias = true;

                Program.Window.GraphicsDevice.RasterizerState = rasterizerState;

                Program.Window.effect.CurrentTechnique.Passes[0].Apply();
                Program.Window.effect.EnableDefaultLighting();
                Program.Window.effect.Texture = m.texture;
                Program.Window.effect.TextureEnabled = true;

                foreach (EffectPass pass in Program.Window.effect.CurrentTechnique.Passes)
                {
                    pass.Apply();

                    VertexPositionNormalTexture[] verts = m.verts.GetVerticies();
                    int[] indicies = m.verts.GetIndicies();
                    Program.Window.GraphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
                        verts, 0, verts.Length, indicies, 0, indicies.Length / 3);
                }
            }
        }
    }
}