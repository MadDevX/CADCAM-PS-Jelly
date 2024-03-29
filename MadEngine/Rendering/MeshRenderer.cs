﻿using MadEngine.Architecture;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Rendering
{
    public class MeshRenderer : Renderer
    {
        public MeshRenderer(ShaderWrapper shader, Mesh mesh) : base(shader, mesh)
        {
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            if(_shaderWrapper is ShadedShaderWrapper wrapper)
            {
                wrapper.SetLightPosition(RenderConstants.LIGHT_POS);
                wrapper.SetLightColor(RenderConstants.LIGHT_COL);

                var mtx = new Matrix3(Matrix4.Transpose(parentMatrix * localMatrix));

                wrapper.SetNormalMatrix(mtx);
                wrapper.SetCameraPosition(camera.Position);

                if (wrapper.IsTextured)
                {
                    wrapper.SetColor(Color4.White);
                    TextureProvider.Instance.DiffuseMap.Use(TextureUnit.Texture0);
                    TextureProvider.Instance.NormalMap.Use(TextureUnit.Texture1);
                    TextureProvider.Instance.HeightMap.Use(TextureUnit.Texture2);
                    wrapper.SetDiffuseMap(0);
                    wrapper.SetNormalMap(1);
                    wrapper.SetHeightMap(2);
                }
                else
                {
                    wrapper.SetColor(Color4.Gray);
                }
            }

            GL.DrawElements(PrimitiveType.Triangles, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        protected override void SetBufferData(float[] vertices, uint[] indices)
        {
            throw new NotImplementedException();
        }
    }
}
