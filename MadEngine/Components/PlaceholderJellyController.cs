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
        private Vector3d[] _initialData;
        private Vector3d[] _displacementVectors;
        private double[] _displacementFrequency;
        private float _timer = 0.0f;
        private float _frequencyMult = 20.0f;
        private float _amp = 0.075f;
        public override void Initialize()
        {
            _jellyData = OwnerNode.GetComponent<JellyData>();
            _initialData = _jellyData.DataPoints.ToArray();
            _displacementVectors = new Vector3d[_initialData.Length];
            _displacementFrequency = new double[_initialData.Length];
            var gen = new Random();
            for(int i = 0; i < _displacementVectors.Length; i++)
            {
                _displacementVectors[i] = new Vector3d(gen.NextDouble()*2.0 - 1.0, gen.NextDouble() * 2.0 - 1.0, gen.NextDouble() * 2.0 - 1.0).Normalized();
                _displacementFrequency[i] = gen.NextDouble() * _frequencyMult;
            }
            base.Initialize();
        }

        protected override void OnFixedUpdate(float deltaTime)
        {
            for (int i = 0; i < _initialData.Length; i++)
            {
                _jellyData.DataPoints[i] = _initialData[i] + _displacementVectors[i] * Math.Sin(_timer * _displacementFrequency[i]) * _amp;
            }
            _jellyData.SetDirty();
            base.OnFixedUpdate(deltaTime);
            _timer += deltaTime;
        }
    }
}
