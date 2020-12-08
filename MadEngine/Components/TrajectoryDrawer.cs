using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class TrajectoryDrawer : UpdatableMComponent
    {
        private List<Vector3> _positions = new List<Vector3>(1001);
        public float MaxTrajectoryLength { get; set; } = 10.0f;
        private double _trajectoryLength = 0.0;

        private LineRenderer _trajectoryRenderer;
        private SpinningTopMovement _spinningTopMovement;

        public TrajectoryDrawer(LineRenderer trajectoryRenderer)
        {
            _trajectoryRenderer = trajectoryRenderer;
            _trajectoryRenderer.LineStrip = true;
            _trajectoryRenderer.Color = Color4.Red;
        }

        public override void Initialize()
        {
            _spinningTopMovement = OwnerNode.GetComponent<SpinningTopMovement>();
            //_spinningTopMovement.OnSimulationUpdated += OnSimulationUpdated;
            _spinningTopMovement.OnSimulationRestarted += ClearTrajectory;
            base.Initialize();
        }

        public override void Dispose()
        {
            //_spinningTopMovement.OnSimulationUpdated -= OnSimulationUpdated;
            _spinningTopMovement.OnSimulationRestarted -= ClearTrajectory;
            base.Dispose();
        }

        protected override void OnUpdate(float deltaTime)
        {
            UpdateTrajectory();
            _trajectoryRenderer.SetLine(_positions.ToArray());
        }

        private void ClearTrajectory()
        {
            _trajectoryLength = 0.0;
            _positions.Clear();
        }

        private void UpdateTrajectory()
        {
            var rotated = new Vector4(Vector3.One * (float)_spinningTopMovement.CubeSize, 1.0f)  * OwnerNode.Transform.LocalModelMatrix;
            var pos = rotated.Xyz;
            _positions.Add(pos);
            if(_positions.Count > 1)
            {
                _trajectoryLength += (_positions[_positions.Count - 1] - _positions[_positions.Count - 2]).Length;
            }
            while (_trajectoryLength > MaxTrajectoryLength || _positions.Count > 1000)
            {
                _trajectoryLength -= (_positions[0] - _positions[1]).Length;
                _positions.RemoveAt(0);
            }
        }
    }
}
