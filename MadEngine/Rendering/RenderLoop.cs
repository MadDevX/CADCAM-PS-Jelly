using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public class RenderLoop : IDisposable, IRenderLoop
    {
        public event Action OnRenderLoop;

        private GLControl _glControl;
        private ScreenBufferManager _screenBufferManager;
        private SceneManager _sceneManager;
        private Camera _camera;
        private ShaderProvider _shaderProvider;

        public RenderLoop(GLControl glControl, ScreenBufferManager screenBufferManager, SceneManager sceneManager, Camera camera, ShaderProvider shaderProvider)
        {
            _glControl = glControl;
            _screenBufferManager = screenBufferManager;
            _sceneManager = sceneManager;
            _camera = camera;
            _shaderProvider = shaderProvider;

            Initialize();
        }

        private void Initialize()
        {
            _glControl.Paint += GLControlOnPaint;
        }

        public void Dispose()
        {
            _glControl.Paint -= GLControlOnPaint;
        }

        private void GLControlOnPaint(object sender, System.Windows.Forms.PaintEventArgs e)
        {
            #region Timer
            //_stopwatch.Start();
            #endregion

            //Reset
            _glControl.MakeCurrent();
            _screenBufferManager.ResetScreenBuffer();
            //Render
            _shaderProvider.UpdateShadersCameraMatrices(_camera);
            OnRenderLoop?.Invoke();
            _sceneManager.CurrentScene.Render(_camera);
            //Replace
            GL.Finish();
            _glControl.SwapBuffers();

            #region Timer
            //_frames++;
            //if (_stopwatch.ElapsedTicks >= Stopwatch.Frequency)
            //{
            //    this.Title = $"{_frames} FPS";
            //    _frames = 0;
            //    _stopwatch.Reset();
            //}
            #endregion
        }
    }
}
