using MadEngine.Utility;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public class Mesh : IDisposable
    {
        private int VAO = 0;
        private int VBO = 0;
        private int EBO = 0;

        public int IndexCount { get; private set; }
        public VertexLayout.Type LayoutType { get; private set; }

        private bool _shouldDispose;

        public Mesh(VertexLayout.Type layoutType)
        {
            LayoutType = layoutType;
            InitializeGLObjects(LayoutType);
        }

        public void Dispose()
        {
            DisposeData();
        }

        public void BindMesh()
        {
            if (MeshUtility.BoundMesh != this)
            {
                GL.BindVertexArray(VAO);
                GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
                MeshUtility.BoundMesh = this;
            }
        }

        public void SetBufferData(float[] vertices, uint[] indices, BufferUsageHint dataType)
        {
            GL.BindVertexArray(VAO);
            IndexCount = indices.Length;
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, dataType);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, dataType);
        }

        public void SetVBOData(float[] vertices, BufferUsageHint dataType)
        {
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, dataType);
        }

        private void InitializeGLObjects(VertexLayout.Type layoutType)
        {
            InitializeVBO();
            InitializeEBO();
            InitializeVAO(layoutType);
            _shouldDispose = true;
        }

        private void InitializeVAO(VertexLayout.Type layoutType)
        {
            VAO = GL.GenVertexArray();
            GL.BindVertexArray(VAO);
            VertexLayout.SetLayout(VAO, layoutType);
        }

        private void InitializeVBO()
        {
            VBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
        }

        private void InitializeEBO()
        {
            EBO = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
        }

        private void DisposeData()
        {
            if (_shouldDispose)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
                GL.BindVertexArray(0);
                GL.DeleteBuffer(VBO);
                GL.DeleteBuffer(EBO);
                GL.DeleteVertexArray(VAO);
                _shouldDispose = false;
            }
        }
    }
}
