using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadEngine.Architecture;

namespace MadEngine
{
    public class PatchRenderer : Renderer
    {
        public PatchRenderer(ShaderWrapper shaderWrapper, Mesh mesh) : base(shaderWrapper, mesh)
        {
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            _shaderWrapper.Shader.Use();
            if(_shaderWrapper is TessellationShadedShaderWrapper wrapper)
            {
                wrapper.SetLightPosition(RenderConstants.LIGHT_POS);
                wrapper.SetLightColor(RenderConstants.LIGHT_COL);
                GL.PatchParameter(PatchParameterInt.PatchVertices, wrapper.PatchVertices);

                var mtx = new Matrix3(Matrix4.Transpose(parentMatrix * localMatrix));

                wrapper.SetNormalMatrix(mtx);
                wrapper.SetCameraPosition(camera.Position);

                if (wrapper.OverrideTessLevels)
                {
                    wrapper.SetTessLevelOuter(Registry.TessellationLevels.TessLevelOuter);
                    wrapper.SetTessLevelInner(Registry.TessellationLevels.TessLevelInner);
                }
                if(wrapper.IsTextured)
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
            GL.DrawElements(PrimitiveType.Patches, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0);
        }

        protected override void SetBufferData(float[] vertices, uint[] indices)
        {
            _mesh.SetBufferData(vertices, indices, BufferUsageHint.StaticDraw);
        }
    }
}
