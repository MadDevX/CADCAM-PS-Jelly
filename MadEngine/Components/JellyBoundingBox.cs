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
    public class JellyBoundingBox : MComponent
    {
        private double _size = 5.0;
        public double Size 
        {
            get => _size; 
            set
            {
                _size = value;
                UpdateBoundingBoxMesh();
            }
        }

        private LineRenderer _boundingBoxRenderer;

        public JellyBoundingBox(LineRenderer renderer, double size = 5.0)
        {
            _boundingBoxRenderer = renderer;
            _boundingBoxRenderer.Color = Color4.Green;
            Size = size;
        }

        private void UpdateBoundingBoxMesh()
        {
            var data = MeshUtility.WireframeCubeData((float)Size, Vector3.Zero);
            _boundingBoxRenderer.SetData(data.vertices, data.indices);
        }
    }
}
