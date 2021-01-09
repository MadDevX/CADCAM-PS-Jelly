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
        private JellySimulation _simulation;

        
        public double MassOfControlPoint { get => _simulation.JellyController.MassOfSinglePoint; set => _simulation.JellyController.MassOfSinglePoint = value; }

        public double C1 { get => _simulation.JellyController.Springiness; set => _simulation.JellyController.Springiness = value; }
        public double C2 { get => _simulation.JellyController.SpringinessCF; set => _simulation.JellyController.SpringinessCF = value; }
        public bool SupportSprings { get => _simulation.JellyController.SupportSprings; set { _simulation.JellyController.SupportSprings = value; } }
        public bool ControlFrameSprings { get => _simulation.JellyController.ControlFrameSprings; set { _simulation.JellyController.ControlFrameSprings = value; } }
        public double C3 { get => _simulation.JellyController.SpringinessSupp; set => _simulation.JellyController.SpringinessSupp = value; }
        public double K { get => _simulation.JellyController.Dampening; set => _simulation.JellyController.Dampening = value; }
        public double Bounciness { get => _simulation.JellyController.Bounciness; set => _simulation.JellyController.Bounciness = value; }
        public double RandomVelocityMult { get => _simulation.JellyController.RandomVelocityMult; set => _simulation.JellyController.RandomVelocityMult = value; }
        public double GravityMult { get => _simulation.JellyController.GravityMult; set => _simulation.JellyController.GravityMult = value; }
        public bool Gravity { get => _simulation.JellyController.EnableGravity; set => _simulation.JellyController.EnableGravity = value; }
        public bool WireframeEnabled { get => _simulation.WireframeRenderer.Enabled; set => _simulation.WireframeRenderer.Enabled = value; }
        public bool ControlFrameEnabled { get => _simulation.ControlFrameRenderer.Enabled; set => _simulation.ControlFrameRenderer.Enabled = value; }
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

        public void Initialize(JellySimulation simulation)
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
            _simulation.JellyController.ResetJelly();
        }

        private void btnRandomizeVelocities_Click(object sender, RoutedEventArgs e)
        {
            _simulation.JellyController.RandomizeVelocities();
        }
    }
}
