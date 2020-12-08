using OpenTK;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MadEngine.Components
{
    public class SpringMovement : UpdatableMComponent
    {
        public struct State
        {
            public double pos;
            public double vel;
            public double acc;

            public State(double position, double velocity, double acceleration)
            {
                this.pos = position;
                this.vel = velocity;
                this.acc = acceleration;
            }
        }

        /// <summary>
        /// As a parameter provides current simulation time.
        /// </summary>
        public event Action<double> OnSimulationUpdated;
        public event Action OnSimulationReset;
        public State CurrentState => _x2;

        private LineRenderer _lineRenderer;
        
        private double _timer = 0.0f;
        
        //Configurable
        public double x0 { get; set; } = 1.0f; //Starting position
        public double v0 { get; set; } = 0.0f; //Starting velocity

        public double A { get; set; } = 1.0f; //Amplitude
        public double m { get; set; } = 1.0f; // Mass
        public double c { get; set; } = 1.0f; // Springiness

        public double k { get; set; } = 0.5f; // Damping

        public double omega { get; set; } = 0.0f; //frequency
        public double phi { get; set; } = 0.0f; //phase shift

        public float FixedDeltaTime { get => _fixedLoop.FixedDeltaTime; set { _fixedLoop.FixedDeltaTime = value; InitializeSimulation(false); } }
        public float TimeScale { get => _updateLoop.TimeScale; set => _updateLoop.TimeScale = value; }
        public int CurrentWFunc { get; set; } = 0;
        public int CurrentHFunc { get; set; } = 0;

        public string WFuncDesc { get => _functionDescs[CurrentWFunc]; }
        public string HFuncDesc { get => _functionDescs[CurrentHFunc]; }

        private string[] _functionDescs = {"A", "t<0 ? 0 : A", "sgn(A * sin(ω*t + φ))", "A * sin(ω*t + φ)" };
        
        //Internal

        /// <summary>
        /// Previous state.
        /// </summary>
        private State _x1;
        /// <summary>
        /// Current state.
        /// </summary>
        private State _x2;
        /// <summary>
        /// Next state.
        /// </summary>
        private State _x3;

        Func<double, double>[] _functions;
        private IFixedUpdateLoop _fixedLoop;
        private IUpdateLoop _updateLoop;

        public override void Initialize()
        {
            _fixedLoop = Registry.FixedUpdateLoop.I;
            _updateLoop = Registry.UpdateLoop.I;
            _lineRenderer = OwnerNode.GetComponent<LineRenderer>();
            _functions = new Func<double, double>[] { F1, F2, F3, F4 };

            InitializeSimulation();
            base.Initialize();
        }

        public void InitializeSimulation(bool clearData = true)
        {
            if(clearData)
            {
                _timer = 0.0;
                _x2 = new State(x0, v0, 0.0f);
                _x1 = new State(x0 - (v0 * Registry.FixedUpdateLoop.I.FixedDeltaTime), v0, 0.0f);

                OnSimulationReset?.Invoke();
            }
            else
            {
                _x1 = new State(_x2.pos - (_x2.vel * Registry.FixedUpdateLoop.I.FixedDeltaTime), _x2.vel, _x2.acc);
            }
        }


        protected override void OnFixedUpdate(float deltaTime)
        {
            double dt = deltaTime;
            Transform.Position = Vector3.UnitY * (float)_x2.pos;
            _lineRenderer.SetLine(Vector3.UnitY * (float)(w(_timer) /*+ A*5.0f*/), Transform.Position);

            CalculateNextPosition(_timer, dt); //After this we have x1, x2 and x3, so we can use difference quotient to calculate current velocity and acceleration
            CalculateCurrentVelocity(dt);
            CalculateCurrentAcceleration(dt);
            
            OnSimulationUpdated?.Invoke(_timer);

            _timer += dt;
            _x1 = _x2;
            _x2.pos = _x3.pos;
        }

        /// <summary>
        /// Calculates next position and saves it to _x3.pos variable.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        private void CalculateNextPosition(double t, double deltaTime)
        {
            var d = deltaTime;
            _x3.pos = 2.0f * d * d * (f(t) + h(t)) + d * k * _x1.pos - 2 * m * _x1.pos + 4.0f * m * _x2.pos;
            _x3.pos /= ((deltaTime * k) + (2.0f * m));
        }

        /// <summary>
        /// Calculates current velocity and saves it to _x2.vel variable.
        /// </summary>
        /// <param name="deltaTime"></param>
        private void CalculateCurrentVelocity(double deltaTime)
        {
            _x2.vel = (_x3.pos - _x1.pos) / (2.0f * deltaTime);
        }

        /// <summary>
        /// Calculates current acceleration and saves it to _x2.acc variable.
        /// </summary>
        /// <param name="deltaTime"></param>
        /// <returns></returns>
        private void CalculateCurrentAcceleration(double deltaTime)
        {
            _x2.acc = (_x3.pos - 2.0f * _x2.pos + _x1.pos) / (deltaTime * deltaTime);
        }

        /// <summary>
        /// Springiness force
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double f(double t)
        {
            return c * (w(t) - _x2.pos);
        }

        /// <summary>
        /// Damping force
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double g(double t)
        {
            return -k * _x2.vel;
        }

        /// <summary>
        /// Position of spring's loose end.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double w(double t) => _functions[CurrentWFunc](t);

        /// <summary>
        /// External field force (i.e. gravity)
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public double h(double t) => _functions[CurrentHFunc](t);

        #region Example functions for W and h
        private double F1(double t) => A;

        private double F2(double t) => t < 0 ? 0 : A;

        private double F3(double t) => Math.Sign(A * Math.Sin(omega * t + phi));

        private double F4(double t) => (double)(A * Math.Sin(omega * t + phi));
        #endregion

    }
}
