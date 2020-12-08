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
    public class Grid : IDisposable
    {
        private Camera _camera;
        private IRenderLoop _renderLoop;

        private LineRenderer _gridRenderer;

        private GridGenerator _gridGenerator;

        public bool Enabled { get; set; } = true;

        public Grid(Camera camera, IRenderLoop renderLoop, LineRenderer gridRenderer)
        {
            _camera = camera;
            _renderLoop = renderLoop;
            _gridRenderer = gridRenderer;

            _gridGenerator = new GridGenerator(200, 1, _camera);
            _gridRenderer.SetData(_gridGenerator.GetVertices(), _gridGenerator.GetIndices());
            _gridRenderer.LineWidth = RenderConstants.GRID_SIZE;
            Initialize();
        }

        private void Initialize()
        {
            _renderLoop.OnRenderLoop += OnRender;
        }

        public void Dispose()
        {
            _renderLoop.OnRenderLoop -= OnRender;
        }

        private void OnRender()
        {
            if (Enabled)
            {
                var pos = _camera.Position;
                GL.DepthMask(false);
                _gridRenderer.Color = new Color4(0.5f, 0.5f, 0.5f, 0.5f * GetPitchMultiplier());
                _gridRenderer.Render(_camera, Matrix4.CreateTranslation(new Vector3((int)pos.X, 0.0f, (int)pos.Z)), Matrix4.Identity);
                GL.DepthMask(true);
            }
        }

        private float GetPitchMultiplier()
        {
            var minVal = 0.01f;
            var heightScale = 1.0f;
            var pitch = _camera.Pitch * _camera.Position.Y < 0.0f ? _camera.Pitch : 0.0f;
            var mult = Math.Min(1.0f, Math.Abs(pitch / ((float)Math.PI * 0.5f)) + 0.1f);
            mult *= Math.Min(1.0f, Math.Abs(_camera.Position.Y * heightScale));
            var corrected = Math.Max(mult - minVal, 0.0f) / (1.0f - minVal);
            return corrected;
        }
    }
}
