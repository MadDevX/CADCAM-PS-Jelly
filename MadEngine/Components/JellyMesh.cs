using MadEngine.Miscellaneous;
using MadEngine.Rendering;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Components
{
    public class JellyMesh : MComponent, IDynamicMeshSource
    {
        private readonly JellyData _data;
        private readonly Vector3d[] _UVWs;
        private readonly Vector3[] _normals;
        private float[] _vertices;
        private bool _isDirty = false;

        public JellyMesh(JellyData data, Vector3[] positions, Vector3[] normals)
        {
            _data = data;
            _UVWs = positions.Select((x) => GetVertexUVW(x)).ToArray();
            _normals = normals;
            _vertices = new float[_UVWs.Length * VertexLayout.Stride(VertexLayout.Type.PositionNormal)];
        }

        public override void Initialize()
        {
            _data.OnDataModified += SetDirty;
            SetDirty();
            base.Initialize();
        }

        public override void Dispose()
        {
            _data.OnDataModified -= SetDirty;
            base.Dispose();
        }

        private Vector3d GetVertexUVW(Vector3 position)
        {
            return (position + Vector3.One * 0.5f).Double();
        }

        private Vector3d GetModifiedNormal(Vector3d uvw, Vector3 normal, double offset = 0.005)
        {
            Vector3d offsetUVW = uvw  - offset * normal.Double();
            //Vector3d offsetUVW = (uvw - Vector3d.One * 0.5) * (1.0 - offset) + Vector3d.One * 0.5;
            var basePoint = Bezier.GetPoint(_data.DataPoints, uvw);
            var off = Bezier.GetPoint(_data.DataPoints, offsetUVW);
            return (basePoint - off).Normalized();
        }

        private void SetDirty()
        {
            _isDirty = true;
        }

        public void Refresh(Mesh mesh)
        {
            if (_isDirty)
            {
                Parallel.For(0, _UVWs.Length, (i) =>
                    {
                        VBOUtility.SetVertex(_vertices, Bezier.GetPoint(_data.DataPoints, _UVWs[i]).Float(), GetModifiedNormal(_UVWs[i], _normals[i]).Float(), i);
                    }
                );
                mesh.SetVBOData(_vertices, OpenTK.Graphics.OpenGL.BufferUsageHint.DynamicDraw);
                _isDirty = false;
            }
        }
    }
}
