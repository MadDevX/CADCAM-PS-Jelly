using MadEngine.Physics;
using MadEngine.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class JellyData : MComponent, IDynamicMeshSource
    {
        public event Action OnDataModified;

        public CubeArray<Vector3d> DataPoints = new CubeArray<Vector3d>(4, 4, 4);

        private Vector3d[] _jellyWalls = new Vector3d[16 * 6];

        private float[] _vertices = new float[16 * 6 * 3];
        private uint[] _indices = new uint[16 * 6];

        private bool _isDirty = true;

        public JellyData()
        {
            ResetDataPoints();
            for (int i = 0; i < _indices.Length; i++) _indices[i] = (uint)i;
        }

        public void ResetDataPoints()
        {
            var offset = new Vector3d(-0.5, -0.5, -0.5);
            var third = 1.0f / 3.0f;
            for (int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        DataPoints[x, y, z] = new Vector3d(x, y, z) * third + offset;
                    }
                }
            }
        }

        public void Refresh(Mesh mesh)
        {
            if (_isDirty)
            {
                UpdateJellyWalls();
                for (int i = 0; i < _jellyWalls.Length; i++)
                {
                    VBOUtility.SetVertex(_vertices, _jellyWalls[i].Float(), i);
                }
                mesh.SetBufferData(_vertices, _indices, OpenTK.Graphics.OpenGL.BufferUsageHint.DynamicDraw);
                _isDirty = false;
            }
        }
        
        public int GetIndex(int x, int y, int z)
        {
            return x + z * 4 + y * 16;
        }

        public Vector3d GetPoint(int x, int y, int z)
        {
            return DataPoints[x, y, z];
        }

        public void SetPoint(int x, int y, int z, Vector3d value)
        {
            DataPoints[x, y, z] = value;
            _isDirty = true;
            OnDataModified?.Invoke();
        }

        /// <summary>
        /// Forces to update Mesh data on the next rendered frame
        /// </summary>
        public void SetDirty() { _isDirty = true; OnDataModified?.Invoke(); }
        private void UpdateJellyWalls()
        {
            int idx = 0;
            for (int z = 0; z < 4; z++) for (int x = 0; x < 4; x++)  _jellyWalls[idx++] = DataPoints[x, 0, z]; //bottom
            for (int z = 0; z < 4; z++) for (int x = 0; x < 4; x++)  _jellyWalls[idx++] = DataPoints[x, 3, z]; //top
            for (int y = 0; y < 4; y++) for (int x = 0; x < 4; x++)  _jellyWalls[idx++] = DataPoints[x, y, 0]; //back
            for (int y = 0; y < 4; y++) for (int x = 0; x < 4; x++)  _jellyWalls[idx++] = DataPoints[x, y, 3]; //front
            for (int y = 0; y < 4; y++) for (int z = 0; z < 4; z++)  _jellyWalls[idx++] = DataPoints[0, y, z]; //left
            for (int y = 0; y < 4; y++) for (int z = 0; z < 4; z++)  _jellyWalls[idx++] = DataPoints[3, y, z]; //right
        }
        
    }
}
