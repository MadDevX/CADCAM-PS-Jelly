using MadEngine.Physics;
using MadEngine.Utility;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class JellyControlFrame : MComponent
    {
        private double _size = 1.0;
        public double Size
        {
            get => _size;
            set
            {
                _size = value;
                UpdateFrameMesh();
            }
        }

        private LineRenderer _frameRenderer;

        public CubeArray<Vector3d> DataPoints = new CubeArray<Vector3d>(2, 2, 2);
        public CubeArray<Vector3d> _localDataPoints = new CubeArray<Vector3d>(2, 2, 2);

        public JellyControlFrame(LineRenderer frameRenderer, double size = 1.0)
        {
            _frameRenderer = frameRenderer;
            frameRenderer.Color = Color4.Aquamarine;
            _size = size;
            for(int y = 0; y < 2; y++)
                for(int z = 0; z < 2; z++)
                    for(int x = 0; x < 2; x++)
                    {
                        _localDataPoints[x, y, z] = new Vector3d(x - 0.5, y - 0.5, z - 0.5) * size;
                        DataPoints[x, y, z] = _localDataPoints[x, y, z];
                    }
        }

        public override void Initialize()
        {
            UpdateFrameMesh();
            Transform.OnDataChanged += UpdateDataPoints;
            base.Initialize();
        }

        public override void Dispose()
        {
            Transform.OnDataChanged -= UpdateDataPoints;
            base.Dispose();
        }

        private void UpdateDataPoints()
        {
            for (int i = 0; i < _localDataPoints.Length; i++)
            {
                var point = new Vector4(_localDataPoints[i].Float(), 1.0f) * Transform.LocalModelMatrix;
                DataPoints[i] = point.Xyz.Double();
            }
        }

        private void UpdateFrameMesh()
        {
            var data = MeshUtility.WireframeCubeData((float)Size, OwnerNode.Transform.Position);
            _frameRenderer.SetData(data.vertices, data.indices);
        }
    }
}
