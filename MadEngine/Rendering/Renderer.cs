using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadEngine.Components;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace MadEngine
{
    public abstract class Renderer : MComponent, IRenderer, IDisposable
    {
        protected ShaderWrapper _shaderWrapper;
        protected Mesh _mesh;

        protected bool _shouldDispose = false;

        public Renderer(ShaderWrapper shader, VertexLayout.Type type)
        {
            _shaderWrapper = shader;
            _mesh = new Mesh(type);
            _shouldDispose = true;
        }

        public Renderer(ShaderWrapper shader, Mesh mesh)
        {
            _shaderWrapper = shader;
            _mesh = mesh;
            _shouldDispose = true;
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_shouldDispose)
            {
                _mesh.Dispose();
            }
        }

        public void Render(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            if (Enabled)
            {
                _mesh.BindMesh();
                SetShader(_shaderWrapper, camera, localMatrix, parentMatrix);
                Draw(camera, localMatrix, parentMatrix);
            }
        }

        protected void SetShader(ShaderWrapper shaderWrapper, Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            shaderWrapper.Shader.Use();
            SetModelMatrix(shaderWrapper, localMatrix, parentMatrix);
        }

        protected abstract void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix);

        private void SetModelMatrix(ShaderWrapper shader, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            var model = localMatrix * parentMatrix;
            shader.SetModelMatrix(model);
        }

        protected abstract void SetBufferData(float[] vertices, uint[] indices);

    }
}
