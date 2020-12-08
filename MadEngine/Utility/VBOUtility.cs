using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace MadEngine
{
    public static class VBOUtility
    {
        public static void SetVertex(float[] vertexArray, Vector3 position, int vertexIndex)
        {
            var stride = VertexLayout.Stride(VertexLayout.Type.Position);
            vertexArray[stride * vertexIndex + 0] = position.X;
            vertexArray[stride * vertexIndex + 1] = position.Y;
            vertexArray[stride * vertexIndex + 2] = position.Z;
        }


        public static void SetVertex(float[] vertexArray, Vector3 position, Vector3 normal, int vertexIndex)
        {
            var stride = VertexLayout.Stride(VertexLayout.Type.PositionNormal);
            vertexArray[stride * vertexIndex + 0] = position.X;
            vertexArray[stride * vertexIndex + 1] = position.Y;
            vertexArray[stride * vertexIndex + 2] = position.Z;
            vertexArray[stride * vertexIndex + 3] = normal.X;
            vertexArray[stride * vertexIndex + 4] = normal.Y;
            vertexArray[stride * vertexIndex + 5] = normal.Z;
        }


        public static void SetVertex(float[] vertexArray, Vector3 position, Vector3 normal, Vector2 texCoords, int vertexIndex)
        {
            var stride = VertexLayout.Stride(VertexLayout.Type.PositionNormalTexCoords);
            vertexArray[stride * vertexIndex + 0] = position.X;
            vertexArray[stride * vertexIndex + 1] = position.Y;
            vertexArray[stride * vertexIndex + 2] = position.Z;
            vertexArray[stride * vertexIndex + 3] = normal.X;
            vertexArray[stride * vertexIndex + 4] = normal.Y;
            vertexArray[stride * vertexIndex + 5] = normal.Z;
            vertexArray[stride * vertexIndex + 6] = texCoords.X;
            vertexArray[stride * vertexIndex + 7] = texCoords.Y;
        }
    }
}
