using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MadEngine.Utility;
using MadEngine.Components;
using MadEngine.Miscellaneous;
using OpenTK.Graphics;

namespace MadEngine
{
    /// <summary>
    /// Interaction logic for MeshSelector.xaml
    /// </summary>
    public partial class SimulationProperties : UserControl
    {
        private SpinningTopSimulation _simulation;

        public float CubeSize { get => _simulation.CubeSize; set => _simulation.CubeSize = value; }
        public float CubeDensity { get => _simulation.CubeDensity; set => _simulation.CubeDensity = value; }
        public bool CubeEnabled { get => _simulation.CubeRenderer.Enabled; set => _simulation.CubeRenderer.Enabled = value; }
        public bool DiagonalEnabled { get => _simulation.DiagonalRenderer.Enabled; set => _simulation.DiagonalRenderer.Enabled = value; }
        public bool TrajectoryEnabled { get => _simulation.TrajectoryRenderer.Enabled; set => _simulation.TrajectoryRenderer.Enabled = value; }
        public bool GravityEnabled { get => _simulation.SpinningTopMovement.GravityEnabled; set => _simulation.SpinningTopMovement.GravityEnabled = value; }
        public float Gravity { get => (float)_simulation.SpinningTopMovement.GravityMult; set => _simulation.SpinningTopMovement.GravityMult = value; }
        public float Omega { get => _simulation.Omega; set => _simulation.Omega = value; }
        public float DiagonalInclination { get => _simulation.DiagonalInclination; set => _simulation.DiagonalInclination = value; }
        public float TrajectoryLength { get => _simulation.TrajectoryDrawer.MaxTrajectoryLength; set => _simulation.TrajectoryDrawer.MaxTrajectoryLength = value; }
        public float TimeScale { get => Registry.UpdateLoop.I.TimeScale; set => Registry.UpdateLoop.I.TimeScale = value; }
        public float FixedDeltaTime { get => Registry.FixedUpdateLoop.I.FixedDeltaTime; set => Registry.FixedUpdateLoop.I.FixedDeltaTime = value; }


        private List<TextBox> _tbs = new List<TextBox>();

        public bool InputOk
        {
            get
            {
                var val = true;
                foreach(var tb in _tbs)
                {
                    val = val && !Validation.GetHasError(tb);
                }
                return val;
            }
        }

        public SimulationProperties()
        {
            InitializeComponent();
            //_tbs.Add(tbX0);
            //_tbs.Add(tbV0);
            //_tbs.Add(tbA);
            //_tbs.Add(tbM);
            //_tbs.Add(tbC);
            //_tbs.Add(tbK);
            //_tbs.Add(tbOmega);
            //_tbs.Add(tbPhi);
            //_tbs.Add(tbFDT);
        }

        public void Initialize(SpinningTopSimulation simulation)
        {
            _simulation = simulation;
            DataContext = this;
        }

        private void UpdateValidation(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).GetBindingExpression(TextBox.TextProperty).UpdateSource();
            //btnRestartSim.IsEnabled = InputOk;
        }

        private void btnRestartSim_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
