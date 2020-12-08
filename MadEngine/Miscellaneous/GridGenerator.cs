using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public class GridGenerator
    {
        public event Action OnParametersChanged;

        public int Size
        {
            get => _size;
            set
            {
                _size = Math.Min(value, _maxSize);
                UpdateData();
            }
        }

        public int Density
        {
            get => _density;
            set
            {
                _density = Math.Min(value, _maxDensity);
                UpdateData();
            }
        }

        private int _size;
        private int _maxSize = 1000;

        //how many lines per unit?
        private int _density;
        private int _maxDensity = 100;

        private Camera _camera;

        private float[] _vertices = new float[0];
        private uint[] _indices = new uint[0];

        public GridGenerator(int size, int density, Camera camera)
        {
            Size = size;
            Density = density;
            _camera = camera;
        }


        public void Dispose()
        {
        }

        public uint[] GetIndices()
        {
            return _indices;
        }

        public float[] GetVertices()
        {
            return _vertices;
        }

        private void UpdateData()
        {
            GenerateData();
            OnParametersChanged?.Invoke();
        }

        private void GenerateData()
        {
            var sideCount = Size * 2 + 1;
            if (_vertices.Length != sideCount * 4 * 3)
            {
                Array.Resize(ref _indices, sideCount * 4);
                Array.Resize(ref _vertices, sideCount * 4 * 3);
            }

            for (int i = 0; i < sideCount; i++)
            {
                VBOUtility.SetVertex(_vertices, new Vector3(-Size + i, 0.0f, -Size), 4 * i); //bottom side
                VBOUtility.SetVertex(_vertices, new Vector3(-Size + i, 0.0f, Size), 4 * i + 1); //top side
                VBOUtility.SetVertex(_vertices, new Vector3(Size, 0.0f, -Size + i), 4 * i + 2); //right side
                VBOUtility.SetVertex(_vertices, new Vector3(-Size, 0.0f, -Size + i), 4 * i + 3); //left side
            }

            for (uint i = 0; i < _indices.Length; i++) _indices[i] = i;
        }

        public void RefreshDataPreRender()
        {
        }
    }
}
