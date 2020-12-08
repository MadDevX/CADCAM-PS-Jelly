using MadEngine.Physics;
using MadEngine.Utility;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class SpinningTopMovement : UpdatableMComponent
    {
        public event Action OnSimulationUpdated;
        public event Action OnSimulationRestarted;

        private double _cubeSize;
        public double CubeSize 
        { 
            get => _cubeSize;
            set
            {
                _cubeSize = value;
                UpdateInertiaTensor();
                UpdateCubeMesh();
            }
        }

        private double _cubeDensity;
        public double CubeDensity
        {
            get => _cubeDensity;
            set
            {
                _cubeDensity = value;
                UpdateInertiaTensor();
            }
        }

        public double Omega { get; set; } = 30.0;
        public bool GravityEnabled { get; set; } = true;
        public double GravityMult { get; set; } = 1.0;
        public Vector3d GravityField => GravityEnabled ? new Vector3d(0.0, -9.81 * GravityMult, 0.0) : Vector3d.Zero;

        public InertiaData InertiaData { get; }
        public Quaterniond _Q;
        public Vector3d _W = Vector3d.Zero;
        private Vector3d _e = Vector3d.One.Normalized();
        private float _time = 0.0f;
        private LineRenderer _cubeRenderer;
        private LineRenderer _diagonalRenderer;

        public SpinningTopMovement(double size, double density, LineRenderer cubeRenderer, LineRenderer diagonalRenderer):base()
        {
            _cubeRenderer = cubeRenderer;
            _diagonalRenderer = diagonalRenderer;
            InertiaData = new InertiaData(size, density);
            CubeSize = size;
            CubeDensity = density;
            _W = _e * Omega;
        }

        public void RestartSimulation()
        {
            _Q = OwnerNode.Transform.Rotation.Double().Normalized();
            _W = _e * Omega;
            OnSimulationRestarted?.Invoke();
        }

        public override void Initialize()
        {
            RestartSimulation();
            base.Initialize();
        }

        private void UpdateCubeMesh()
        {
            var hSize = 0.5f * (float)_cubeSize;
            var offset = new Vector3(hSize, hSize, hSize);
            var data = MeshUtility.WireframeCubeData((float)_cubeSize, offset);
            _cubeRenderer.SetData(data.vertices, data.indices);
            _diagonalRenderer.SetLine(Vector3.One * hSize + offset, -Vector3.One * hSize + offset);
        }

        protected override void OnFixedUpdate(float deltaTime)
        {
            _W = RungeKutta.AdvanceByTimeStep(_W, ODE1Wrapper, _time, deltaTime);
            _Q = RungeKutta.AdvanceByTimeStep(_Q, ODE2Wrapper, _time, deltaTime).Normalized();
            OwnerNode.Transform.Rotation = _Q.Float();
            _time += deltaTime;
            OnSimulationUpdated?.Invoke();
        }

        private void UpdateInertiaTensor()
        {
            InertiaData.Recalculate(CubeSize, CubeDensity);
        }

        public Vector3d ODE1Wrapper(Vector3d W, double t)
        {
            return ODE1(W, InertiaData.Mass, _Q);
        }

        public Quaterniond ODE2Wrapper(Quaterniond Q, double t)
        {
            return ODE2(Q, _W);
        }

        public Vector3d ODE1(Vector3d W, double mass, Quaterniond rotation)
        {
            return InertiaData.InverseTensor.Multiply(N(mass, rotation) + Vector3d.Cross(InertiaData.Tensor.Multiply(W), W));
        }

        public Quaterniond ODE2(Quaterniond q, Vector3d w)
        {
            return q * new Quaterniond(w, 0.0) * 0.5;
        }

        public Vector3d N(double mass, Quaterniond rotation)
        {
            return GyrationInBody() + GravityTorqueInBody(mass, rotation);
        }

        private Vector3d GyrationInBody()
        {
            var e = _e;
            var Ie = InertiaData.Tensor.Multiply(e);
            var cross = Vector3d.Cross(Ie, e);
            return -Omega * Omega * cross;
        }

        private Vector3d GravityTorqueInBody(double mass, Quaterniond q)
        {
            var CoM = InertiaData.CenterOfMass;
            var cQ = Quaterniond.Conjugate(q);
            var gInBody = cQ.RotateVector(GravityForce(mass));
            return Vector3d.Cross(CoM, gInBody);
        }

        private Vector3d GravityForce(double mass)
        {
            return mass * GravityField;
        }

    }
}
