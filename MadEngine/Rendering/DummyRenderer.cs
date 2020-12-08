using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace MadEngine
{
    public class DummyRenderer : IRenderer
    {
        public void Dispose() { }
        public void Render(Camera camera, Matrix4 localMatrix, Matrix4 parentMatrix) { }

        public void SetOwnerNode(INode node) { }
    }
}
