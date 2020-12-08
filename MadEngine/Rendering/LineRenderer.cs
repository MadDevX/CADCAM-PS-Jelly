using MadEngine.Utility;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public class LineRenderer : Renderer
    {
        public float LineWidth { get; set; } = 2.0f;
        public Color4 Color { get; set; } = Color4.Blue;

        private float[] _vertices = new float[6] { 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f };
        
        private uint[] _indices = new uint[2] { 0, 1 };

        public bool LineStrip { get; set; } = false;

        public LineRenderer(ShaderWrapper shader) : base(shader, VertexLayout.Type.Position)
        {
            SetBufferData(_vertices, _indices);
        }

        public void SetData(float[] vertices, uint[] indices)
        {
            SetBufferData(vertices, indices);
        }

        public void SetLine(params Vector3[] positions)
        {
            ResizeArrays(positions.Length);

            for(int i = 0; i < positions.Length; i++)
            {
                VBOUtility.SetVertex(_vertices, positions[i], i);
            }
            for(uint i = 0; i < positions.Length; i++)
            {
                _indices[i] = i;
            }

            SetBufferData(_vertices, _indices);
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            GL.LineWidth(LineWidth);
            _shaderWrapper.SetColor(Color);
            if (LineStrip == false)
                GL.DrawElements(PrimitiveType.Lines, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0 * sizeof(uint));
            else
                GL.DrawElements(PrimitiveType.LineStrip, _mesh.IndexCount, DrawElementsType.UnsignedInt, 0 * sizeof(uint));
            _shaderWrapper.SetColor(Color4.White);
            GL.LineWidth(1.0f);
        }

        protected override void SetBufferData(float[] vertices, uint[] indices)
        {
            _mesh.SetBufferData(vertices, indices, BufferUsageHint.DynamicDraw);
        }

        private void ResizeArrays(int elemCount)
        {
            if (_indices.Length != elemCount)
            {
                Array.Resize(ref _vertices, VertexLayout.Stride(_mesh) * elemCount);
                Array.Resize(ref _indices, elemCount);
            }
        }
    }
}
