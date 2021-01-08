using MadEngine.Physics;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class PlaceholderJellyController : UpdatableMComponent
    {
        private JellyData _jellyData;
        private CubeArray<Vector3d> _initialData;
        private CubeArray<Vector3d> _displacementVectors;
        private CubeArray<double> _displacementFrequency;
        private float _timer = 0.0f;
        private float _frequencyMult = 20.0f;
        private float _amp = 0.075f;

        public override void Initialize()
        {
            _jellyData = OwnerNode.GetComponent<JellyData>();
            _initialData = new CubeArray<Vector3d>(_jellyData.DataPoints);
            _displacementVectors = new CubeArray<Vector3d>(_initialData.xSize, _initialData.ySize, _initialData.zSize);
            _displacementFrequency = new CubeArray<double>(_initialData.xSize, _initialData.ySize, _initialData.zSize);
            var gen = new Random();
            for (int y = 0; y < _initialData.ySize; y++)
                for (int z = 0; z < _initialData.zSize; z++)
                    for (int x = 0; x < _initialData.xSize; x++)
                    {
                        _displacementVectors[x, y, z] = new Vector3d(gen.NextDouble() * 2.0 - 1.0, gen.NextDouble() * 2.0 - 1.0, gen.NextDouble() * 2.0 - 1.0).Normalized();
                        _displacementFrequency[x, y, z] = gen.NextDouble() * _frequencyMult;
                    }
            base.Initialize();
        }

        protected override void OnFixedUpdate(float deltaTime)
        {
            for (int y = 0; y < _initialData.ySize; y++)
                for (int z = 0; z < _initialData.zSize; z++)
                    for (int x = 0; x < _initialData.xSize; x++)
                    {
                        _jellyData.DataPoints[x, y, z] = _initialData[x, y, z] + _displacementVectors[x, y, z] * Math.Sin(_timer * _displacementFrequency[x, y, z]) * _amp /** Math.Max(0.0, 1.0 - _timer/20.0)*/;
                    }
            _jellyData.SetDirty();
            base.OnFixedUpdate(deltaTime);
            _timer += deltaTime;
        }
    }
}
