using MadEngine.Utility;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MadEngine.Architecture;
using MadEngine.Miscellaneous;

namespace MadEngine
{
    public class CompositionRoot : IDisposable
    {
        private MainWindow _window;
        private GLControl _control;

        private BackgroundManager _backgroundManager;
        private ScreenBufferManager _screenBufferManager;
        private ViewportManager _viewportManager;

        private TextureProvider _textureProvider;
        private ShaderProvider _shaderProvider;
        private SceneManager _sceneManager;

        private Camera _camera;
        private CameraMovement _camMovement;
        private RenderLoop _renderLoop;

        private Grid _grid;
        private JellySimulation _simulation;

        public CompositionRoot(GLControl control, MainWindow window)
        {
            _window = window;
            _control = control;
            Initialize();
        }

        private void Initialize()
        {
            //var backgroundColorStandard = new Color4(0.7f, 0.7f, 0.7f, 1.0f);
            var backgroundColorStandard = new Color4(0.157f, 0.157f, 0.157f, 1.0f);

            _backgroundManager = new BackgroundManager(backgroundColorStandard);
            _screenBufferManager = new ScreenBufferManager(_backgroundManager);
            _viewportManager = new ViewportManager(_control);

            _textureProvider = new TextureProvider();
            _shaderProvider = new ShaderProvider();
            _sceneManager = new SceneManager(new Scene("Main"));
            WorldAxes.AddAxesToNode(_shaderProvider, _sceneManager.CurrentScene);

            _camera = new Camera(_viewportManager);
            _camMovement = new CameraMovement(_camera, _control);
            _renderLoop = new RenderLoop(_control, _screenBufferManager, _sceneManager, _camera, _shaderProvider);

            _grid = new Grid(_camera, _renderLoop, new LineRenderer(_shaderProvider.DefaultShader));
            _simulation = new JellySimulation(_sceneManager, _shaderProvider);


            _window.tessellationParametersView.DataContext = Registry.TessellationLevels;
            _window.meshSelector.Initialize(_simulation);
            _window.polygonFillView.Initialize();

        }

        public void Dispose()
        {
            _simulation.Dispose();
            _grid.Dispose();
            _renderLoop.Dispose();
            _camMovement.Dispose();
            _camera.Dispose();
            _sceneManager.Dispose();
            _shaderProvider.Dispose();
            _viewportManager.Dispose();
        }
    }
}
