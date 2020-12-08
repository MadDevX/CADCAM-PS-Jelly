using Microsoft.Win32;
using OxyPlot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class AmplitudeGatherer : UpdatableMComponent
    {
        private List<(double amp, double phase, double omega)> _amplitudes = new List<(double, double, double)>(100);
        private int _simulationRuns = 200;
        private int _currentRuns = 0;
        private double _omegaIncrement = 0.025;

        private float _timePerSimulation = 100.0f;
        private float _timer = 0.0f;

        private Plotter _plotter;
        private SpringMovement _springMovement;

        public override void Initialize()
        {
            _plotter = OwnerNode.GetComponent<Plotter>();
            _springMovement = OwnerNode.GetComponent<SpringMovement>();
            _springMovement.CurrentWFunc = 3;
            base.Initialize();
        }

        protected override void OnFixedUpdate(float deltaTime)
        {
            _timer += deltaTime;
            if (_timer >= _timePerSimulation && _currentRuns < _simulationRuns)
            {
                _amplitudes.Add((_plotter.Amplitude, _plotter.PhaseDifference, _springMovement.omega));
                RestartSim();
                _currentRuns++;
            }
            if (_currentRuns == _simulationRuns)
            {
                int i = 1;
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "CSV file (*.csv)|*.csv";
                sfd.DefaultExt = "csv";
                sfd.ShowDialog();
                if (sfd.FileName != "")
                {
                    SaveDataAsCSV(_amplitudes, sfd.FileName);
                }
                Enabled = false;
                Registry.UpdateLoop.I.TimeScale = 1.0f;
                RestartSim();
            } 
        }

        private void RestartSim()
        {
            _springMovement.omega += _omegaIncrement;
            _timer = 0.0f;
            _springMovement.InitializeSimulation(true);
        }

        private static void SaveDataAsCSV(List<(double amp, double phase, double omega)> pointList, string fileName)
        {
            using (StreamWriter file = new StreamWriter(fileName))
            {
                file.WriteLine("Omega;Amplitude;Phase");
                foreach (var item in pointList)
                {
                    file.WriteLine(item.omega + ";" + item.amp + ";" + item.phase);
                }
            }
        }
    }
}
