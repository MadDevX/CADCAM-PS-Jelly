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

        public JellyControlFrame(LineRenderer frameRenderer, double size = 1.0)
        {
            _frameRenderer = frameRenderer;
            frameRenderer.Color = Color4.Aquamarine;
            _size = size;
        }

        public override void Initialize()
        {
            UpdateFrameMesh();
            base.Initialize();
        }

        private void UpdateFrameMesh()
        {
            var data = MeshUtility.WireframeCubeData((float)Size, OwnerNode.Transform.Position);
            _frameRenderer.SetData(data.vertices, data.indices);
        }
    }
}
