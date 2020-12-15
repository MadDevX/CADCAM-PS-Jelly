using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Rendering
{
    public class DynamicMeshRenderer : MeshRenderer
    {
        private IDynamicMeshSource _source = null;

        public DynamicMeshRenderer(ShaderWrapper shaderWrapper, Mesh mesh, IDynamicMeshSource source = null) : base(shaderWrapper, mesh)
        {
            _source = source;
        }

        public override void Initialize()
        {
            if(_source == null)
            {
                _source = OwnerNode.GetComponent<IDynamicMeshSource>();
            }
            base.Initialize();
        }

        protected override void Draw(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix)
        {
            _source.Refresh(_mesh);
            base.Draw(camera, localMatrix, parentMatrix);
        }
    }
}
