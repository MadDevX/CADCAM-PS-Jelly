using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class JellyControlPointVisualizer : MComponent
    {
        private LineRenderer _renderer;
        private JellyData _jellyData;

        //24 lines per slice, in 2 directions (vertical and horizontal), 2 indices per line
        private uint[] _indices = new uint[24 * 2 * 2 * 12];
        private float[] _vertices = new float[64 * 3];

        public JellyControlPointVisualizer(LineRenderer renderer)
        {
            _renderer = renderer;
            _renderer.LineStrip = false;
            _renderer.Color = Color4.Orange;
        }

        public override void Initialize()
        {
            _jellyData = OwnerNode.GetComponent<JellyData>();
            _jellyData.OnDataModified += UpdateLines;
            SetupMeshData();
            base.Initialize();
        }

        public override void Dispose()
        {
            _jellyData.OnDataModified -= UpdateLines;
            base.Dispose();
        }

        private void SetupMeshData()
        {
            var idx = 0;
            for(int y = 0; y < 4; y++)
            {
                for (int z = 0; z < 4; z++)
                {
                    for (int x = 0; x < 3; x++)
                    {
                        _indices[idx++] = (uint)_jellyData.GetIndex(x, y, z);
                        _indices[idx++] = (uint)_jellyData.GetIndex(x + 1, y, z);
                    }
                }

                for (int x = 0; x < 4; x++)
                {
                    for (int z = 0; z < 3; z++)
                    {
                        _indices[idx++] = (uint)_jellyData.GetIndex(x, y, z);
                        _indices[idx++] = (uint)_jellyData.GetIndex(x, y, z + 1);
                    }
                }
            }

            for(int z = 0; z < 4; z++)
            {
                for (int x = 0; x < 4; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        _indices[idx++] = (uint)_jellyData.GetIndex(x, y, z);
                        _indices[idx++] = (uint)_jellyData.GetIndex(x, y + 1, z);
                    }
                }
            }
        }

        private void UpdateLines()
        {
            for(int i = 0; i < _jellyData.DataPoints.Length; i++)
            {
                VBOUtility.SetVertex(_vertices, _jellyData.DataPoints[i].Float(), i);
            }
            _renderer.SetData(_vertices, _indices);
        }
    }
}
