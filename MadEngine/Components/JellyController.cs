using MadEngine.Physics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class JellyController : UpdatableMComponent
    {
        private JellyData _jellyData;
        private CubeArray<Vector3d> _velocities;
        private CubeArray<Vector3d> _forces;
        private float _timer = 0.0f;
        private float _frequencyMult = 20.0f;
        private float _amp = 0.075f;

        private double _massOfSinglePoint = 1.0;
        private double _dampening = 1.0;
        private double _springiness = 22.0;

        private double _bounciness = 0.5;

        private List<SpringData> _springs = new List<SpringData>();
        private double _cubeSize = 1.0;

        private JellyControlFrame _controlFrame;
        private JellyBoundingBox _boundingBox;
        private double _eps = 0.001;

        private bool _addSupportSprings = true; //TODO: change

        public JellyController(JellyControlFrame controlFrame)
        {
            _controlFrame = controlFrame;
        }

        public override void Initialize()
        {
            _jellyData = OwnerNode.GetComponent<JellyData>();
            _boundingBox = OwnerNode.GetComponent<JellyBoundingBox>();
            var dp = _jellyData.DataPoints;
            _velocities = new CubeArray<Vector3d>(dp.xSize, dp.ySize, dp.zSize);
            _forces = new CubeArray<Vector3d>(dp.xSize, dp.ySize, dp.zSize);
            ResetCubeArray(_velocities);
            ResetCubeArray(_forces);
            SetupSprings();
            //RandomizeVelocities();
            base.Initialize();
        }

        public void RandomizeVelocities()
        {
            var gen = new Random();
            for (int y = 0; y < _velocities.ySize; y++)
                for (int z = 0; z < _velocities.zSize; z++)
                    for (int x = 0; x < _velocities.xSize; x++)
                    {
                        _velocities[x, y, z] = new Vector3d(gen.NextDouble() * 2.0 - 1.0, gen.NextDouble() * 2.0 - 1.0, gen.NextDouble() * 2.0 - 1.0).Normalized();
                    }
        }

        protected override void OnFixedUpdate(float deltaTime)
        {
            ResetCubeArray(_forces);
            CalculateForces();
            UpdateVelocities(deltaTime);
            UpdatePositions(deltaTime);

            _jellyData.SetDirty();
            base.OnFixedUpdate(deltaTime);
            _timer += deltaTime;
        }

        private void CalculateForces()
        {
            var dps = _jellyData.DataPoints;
            //springiness force per spring
            foreach(var spring in _springs)
            {
                var aPos = spring.APos;
                var bPos = spring.BPos;
                var dir = bPos - aPos;
                var len = dir.Length;
                if (len > _eps)
                {
                    dir = dir.Normalized();
                    var f = spring.c * (len - spring.length);
                    var force = f * dir; //from a to b

                    //apply forces only for datapoints from the jelly
                    if (spring.dataSourceA == _jellyData.DataPoints) _forces[spring.a] += force;
                    if (spring.dataSourceB == _jellyData.DataPoints) _forces[spring.b] -= force; //opposite direction for second end
                }
            }
            //damping per point
            for(int i = 0; i < dps.Length; i++)
            {
                var g = -_dampening * _velocities[i];
                _forces[i] += g;
            }
        }

        private void UpdateVelocities(double deltaTime)
        {
            for (int i = 0; i < _velocities.Length; i++)
            {
                _velocities[i] += (_forces[i] / _massOfSinglePoint) * deltaTime;
            }
        }

        private void UpdatePositions(double deltaTime)
        {
            var dps = _jellyData.DataPoints;
            for (int i = 0; i < _velocities.Length; i++)
            {
                //dps[i] += _velocities[i] * deltaTime;
                var result = HandleCollisions(dps[i], _velocities[i], deltaTime);
                dps[i] = result.newPosition;
                _velocities[i] = result.newVelocity;
            }
        }

        private void ResetCubeArray(CubeArray<Vector3d> arr)
        {
            for (int y = 0; y < _velocities.ySize; y++)
                for (int z = 0; z < _velocities.zSize; z++)
                    for (int x = 0; x < _velocities.xSize; x++)
                    {
                        arr[x, y, z] = new Vector3d(0.0, 0.0, 0.0);
                    }
        }

        private (Vector3d newPosition, Vector3d newVelocity) HandleCollisions(Vector3d position, Vector3d velocity, double deltaTime)
        {
            var halfExtents = _boundingBox.Size * 0.5;

            var curVel = velocity;
            
            var b = position; //setup first iteration - point is currently at starting position
            while (deltaTime > 0.0)
            {
                var a = b;
                b = a + curVel * deltaTime;
                double tx = 1.0, ty = 1.0, tz = 1.0, minT = 1.0;
                if (b.X * Math.Sign(b.X) > halfExtents)
                {
                    tx = (Math.Sign(b.X) * halfExtents - a.X) / (b.X - a.X);
                    minT = tx < minT ? tx : minT;
                }
                if (b.Y * Math.Sign(b.Y) > halfExtents)
                {
                    ty = (Math.Sign(b.Y) * halfExtents - a.Y) / (b.Y - a.Y);
                    minT = ty < minT ? ty : minT;
                }
                if (b.Z * Math.Sign(b.Z) > halfExtents)
                {
                    tz = (Math.Sign(b.Z) * halfExtents - a.Z) / (b.Z - a.Z);
                    minT = tz < minT ? tz : minT;
                }


                b = a + curVel * minT * deltaTime;
                deltaTime *= (1.0 - minT);

                if (minT != 1.0)
                {
                    if (tx == minT) curVel = new Vector3d(-curVel.X * _bounciness, curVel.Y, curVel.Z);
                    else if (ty == minT) curVel = new Vector3d(curVel.X, -curVel.Y * _bounciness, curVel.Z);
                    else if (tz == minT) curVel = new Vector3d(curVel.X, curVel.Y, -curVel.Z * _bounciness);
                }
            }
            return (b, curVel);
        }

        private void SetupSprings()
        {
            var straightLength = (1.0 / 3.0) * _cubeSize;
            var diagonalLength = straightLength * Math.Sqrt(2.0);


            //STRAIGHT SPRINGS
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        _springs.Add(new SpringData(_springiness, straightLength, new Vector3i(x, y, z), new Vector3i(x + 1, y, z), _jellyData.DataPoints));
                    }
                }

                for (int x = 0; x < 4; x++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        _springs.Add(new SpringData(_springiness, straightLength, new Vector3i(x, y, z), new Vector3i(x, y, z + 1), _jellyData.DataPoints));
                    }
                }
            }

            for (int z = 0; z < 4; z++)
            {
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        _springs.Add(new SpringData(_springiness, straightLength, new Vector3i(x, y, z), new Vector3i(x, y + 1, z), _jellyData.DataPoints));
                    }
                }
            }

            //DIAGONAL SPRINGS
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        if (x < 3 && z < 3) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x + 1, y, z + 1), _jellyData.DataPoints));
                        if (x < 3 && z > 0) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x + 1, y, z - 1), _jellyData.DataPoints));
                        if (x > 0 && z > 0) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x - 1, y, z - 1), _jellyData.DataPoints));
                        if (x > 0 && z < 3) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x - 1, y, z + 1), _jellyData.DataPoints));

                        if (x < 3 && y < 3) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x + 1, y + 1, z), _jellyData.DataPoints));
                        if (x < 3 && y > 0) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x + 1, y - 1, z), _jellyData.DataPoints));
                        if (x > 0 && y > 0) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x - 1, y - 1, z), _jellyData.DataPoints));
                        if (x > 0 && y < 3) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x - 1, y + 1, z), _jellyData.DataPoints));

                        if (z < 3 && y < 3) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x, y + 1, z + 1), _jellyData.DataPoints));
                        if (z < 3 && y > 0) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x, y - 1, z + 1), _jellyData.DataPoints));
                        if (z > 0 && y > 0) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x, y - 1, z - 1), _jellyData.DataPoints));
                        if (z > 0 && y < 3) _springs.Add(new SpringData(_springiness, diagonalLength, new Vector3i(x, y, z), new Vector3i(x, y + 1, z - 1), _jellyData.DataPoints));
                    }
                }
            }

            //SUPPORT SPRINGS
            var supportCMult = 0.5;
            if (_addSupportSprings)
            {
                for (int y = 0; y < 4; y++)
                {
                    for (int z = 0; z < 4; z++)
                    {
                        for (int x = 0; x < 2; x++)
                        {
                            _springs.Add(new SpringData(_springiness * supportCMult, straightLength * 2.0, new Vector3i(x, y, z), new Vector3i(x + 2, y, z), _jellyData.DataPoints));
                        }
                    }

                    for (int x = 0; x < 4; x++)
                    {
                        for (int z = 0; z < 2; z++)
                        {
                            _springs.Add(new SpringData(_springiness * supportCMult, straightLength * 2.0, new Vector3i(x, y, z), new Vector3i(x, y, z + 2), _jellyData.DataPoints));
                        }
                    }
                }

                for (int z = 0; z < 4; z++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        for (int y = 0; y < 2; y++)
                        {
                            _springs.Add(new SpringData(_springiness, straightLength * 2.0, new Vector3i(x, y, z), new Vector3i(x, y + 2, z), _jellyData.DataPoints));
                        }
                    }
                }
            }

            //CONTROL FRAME SPRINGS
            var CFCMult = 1.0;

            _springs.Add(new SpringData(_springiness * CFCMult, 0.0, new Vector3i(0, 0, 0), new Vector3i(0, 0, 0), _jellyData.DataPoints, _controlFrame.DataPoints));
            _springs.Add(new SpringData(_springiness * CFCMult, 0.0, new Vector3i(3, 0, 0), new Vector3i(1, 0, 0), _jellyData.DataPoints, _controlFrame.DataPoints));
            _springs.Add(new SpringData(_springiness * CFCMult, 0.0, new Vector3i(0, 3, 0), new Vector3i(0, 1, 0), _jellyData.DataPoints, _controlFrame.DataPoints));
            _springs.Add(new SpringData(_springiness * CFCMult, 0.0, new Vector3i(3, 3, 0), new Vector3i(1, 1, 0), _jellyData.DataPoints, _controlFrame.DataPoints));
            _springs.Add(new SpringData(_springiness * CFCMult, 0.0, new Vector3i(0, 0, 3), new Vector3i(0, 0, 1), _jellyData.DataPoints, _controlFrame.DataPoints));
            _springs.Add(new SpringData(_springiness * CFCMult, 0.0, new Vector3i(3, 0, 3), new Vector3i(1, 0, 1), _jellyData.DataPoints, _controlFrame.DataPoints));
            _springs.Add(new SpringData(_springiness * CFCMult, 0.0, new Vector3i(0, 3, 3), new Vector3i(0, 1, 1), _jellyData.DataPoints, _controlFrame.DataPoints));
            _springs.Add(new SpringData(_springiness * CFCMult, 0.0, new Vector3i(3, 3, 3), new Vector3i(1, 1, 1), _jellyData.DataPoints, _controlFrame.DataPoints));
        }
    }
}
