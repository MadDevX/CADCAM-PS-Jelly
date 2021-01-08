using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace MadEngine
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IUpdateLoop, IFixedUpdateLoop
    {
        private GLControl _glControl;
        private CompositionRoot _compositionRoot;
        private DispatcherTimer _timer;
        private Stopwatch _deltaStopwatch;

        private float _timeScale = 1.0f;
        public float TimeScale
        {
            get => _timeScale;
            set
            {
                _timeScale = Math.Max(value, 0.0f);
            }
        }

        public event Action<float> OnUpdate;
        public event Action<float> OnFixedUpdate;

        private float _fixedTimer = 0.0f;

        private float _fixedDeltaTime = 0.02f;
        private float _fixedDeltaTimeMin = 0.001f;
        public float FixedDeltaTime
        {
            get => _fixedDeltaTime;
            set
            {
                _fixedDeltaTime = Math.Max(value, _fixedDeltaTimeMin);
            }
        }

        public MainWindow()
        {
            Registry.UpdateLoop.Set(this);
            Registry.FixedUpdateLoop.Set(this);

            InitializeComponent();
            CreateGLControl();
            _compositionRoot = new CompositionRoot(_glControl, this);
            InitLoop();
        }

        private void CreateGLControl()
        {
            _glControl = new GLControl(new OpenTK.Graphics.GraphicsMode(32, 24, 8, 8));
            _glControl.Dock = DockStyle.Fill;
            Host.Child = _glControl;
            _glControl.MakeCurrent();
        }
        private void InitLoop()
        {
            _deltaStopwatch = new Stopwatch();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(16);
            _timer.Tick += SetDirty;
            _timer.Start();

            CompositionTarget.Rendering += OnTick;
            _deltaStopwatch.Start();
        }

        private void SetDirty(object sender, EventArgs e)
        {
            Host.InvalidateVisual();
        }

        private void OnTick(object sender, EventArgs e)
        {
            var deltaTime = (float)_deltaStopwatch.Elapsed.TotalSeconds * TimeScale;
            _deltaStopwatch.Restart();
            _fixedTimer += deltaTime;
            while(_fixedTimer >= FixedDeltaTime)
            {
                _fixedTimer -= FixedDeltaTime;
                OnFixedUpdate?.Invoke(FixedDeltaTime);
            }
            OnUpdate?.Invoke(deltaTime);
            _glControl.Invalidate();
        }

        protected override void OnClosed(EventArgs e)
        {
            CompositionTarget.Rendering -= OnTick;
            _compositionRoot.Dispose();
            _timer.Tick -= SetDirty;
            _timer.Stop();
            base.OnClosed(e);
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.System && e.OriginalSource is System.Windows.Forms.Integration.WindowsFormsHost)
            {
                e.Handled = true;
            }
        }

    }
}
