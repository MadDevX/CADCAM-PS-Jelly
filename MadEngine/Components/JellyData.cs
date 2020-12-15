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
        public Vector3d[] DataPoints = new Vector3d[64];

        private Vector3d[] _jellyWalls = new Vector3d[16 * 6];

        private float[] _vertices = new float[16 * 6 * 3];
        private uint[] _indices = new uint[16 * 6];

        private bool _isDirty = true;

        public JellyData()
        {
            var offset = new Vector3d(-0.5, -0.5, -0.5);
            var third = 1.0f / 3.0f;
            for(int y = 0; y < 4; y++)
            {
                for(int z = 0; z < 4; z++)
                {
                    for(int x = 0; x < 4; x++)
                    {
                        DataPoints[x + z * 4 + y * 16] = new Vector3d(x, y, z) * third + offset;
                    }
                }
            }
            for (int i = 0; i < _indices.Length; i++) _indices[i] = (uint)i;
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
        
        public Vector3d GetPoint(int x, int y, int z)
        {
            return DataPoints[x + z * 4 + y * 16];
        }

        public void SetPoint(int x, int y, int z, Vector3d value)
        {
            DataPoints[x + z * 4 + y * 16] = value;
            _isDirty = true;
        }

        /// <summary>
        /// Forces to update Mesh data on the next rendered frame
        /// </summary>
        public void SetDirty() { _isDirty = true; }
        private void UpdateJellyWalls()
        {
            int idx = 0;
            for (int z = 0; z < 4; z++) for (int x = 0; x < 4; x++)  _jellyWalls[idx++] = GetPoint(x, 0, z); //bottom
            for (int z = 0; z < 4; z++) for (int x = 0; x < 4; x++)  _jellyWalls[idx++] = GetPoint(x, 3, z); //top
            for (int y = 0; y < 4; y++) for (int x = 0; x < 4; x++)  _jellyWalls[idx++] = GetPoint(x, y, 0); //back
            for (int y = 0; y < 4; y++) for (int x = 0; x < 4; x++)  _jellyWalls[idx++] = GetPoint(x, y, 3); //front
            for (int y = 0; y < 4; y++) for (int z = 0; z < 4; z++)  _jellyWalls[idx++] = GetPoint(0, y, z); //left
            for (int y = 0; y < 4; y++) for (int z = 0; z < 4; z++)  _jellyWalls[idx++] = GetPoint(3, y, z); //right
        }
        
    }
}
