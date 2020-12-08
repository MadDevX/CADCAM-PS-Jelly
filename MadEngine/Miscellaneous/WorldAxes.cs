using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine.Miscellaneous
{
    public static class WorldAxes
    {
        private static float _scale = 0.1f;
        public static void AddAxesToNode(ShaderProvider provider, INode node)
        {
            var x = new LineRenderer(provider.DefaultWorldShader);
            x.SetLine(Vector3.Zero, Vector3.UnitX * _scale);
            x.Color = Color4.Red;
            var y = new LineRenderer(provider.DefaultWorldShader);
            y.SetLine(Vector3.Zero, Vector3.UnitY * _scale);
            y.Color = Color4.Green;
            var z = new LineRenderer(provider.DefaultWorldShader);
            z.SetLine(Vector3.Zero, Vector3.UnitZ * _scale);
            z.Color = Color4.Blue;
            x.LineWidth = y.LineWidth = z.LineWidth = 1.5f;
            node.AttachComponents(x, y, z);
        }
    }
}
