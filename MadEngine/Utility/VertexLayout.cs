using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public static class VertexLayout
    {
        public enum Type
        {
            Position,
            PositionNormal,
            PositionNormalTexCoords
        }

        public static int Stride(Type type)
        {
            switch (type)
            {
                case Type.Position:
                    return 3;
                case Type.PositionNormal:
                    return 6;
                case Type.PositionNormalTexCoords:
                    return 8;
                default:
                    return -1;
            }
        }

        public static int Stride(Mesh mesh)
        {
            return Stride(mesh.LayoutType);
        }

        public static void SetLayout(int VAO, Type layoutType)
        {
            GL.BindVertexArray(VAO);
            switch(layoutType)
            {
                case Type.Position:
                    PositionLayout();
                    break;
                case Type.PositionNormal:
                    PositionNormalLayout();
                    break;
                case Type.PositionNormalTexCoords:
                    PositionNormalTexCoordLayout();
                    break;
            }
        }

        private static void PositionLayout()
        {
            var stride = Stride(Type.Position);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0 * sizeof(float));
        }

        private static void PositionNormalLayout()
        {
            var stride = Stride(Type.PositionNormal);
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 3 * sizeof(float));
        }

        private static void PositionNormalTexCoordLayout()
        {
            var stride = Stride(Type.PositionNormalTexCoords);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 0 * sizeof(float));
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, stride * sizeof(float), 3 * sizeof(float));
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, stride * sizeof(float), 6 * sizeof(float));
            GL.EnableVertexAttribArray(2);
        }
    }
}
